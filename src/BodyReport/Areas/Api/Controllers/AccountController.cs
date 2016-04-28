using BodyReport.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BodyReport.Manager;
using Message;
using System.Security.Claims;
using Message.WebApi;
using BodyReport.Services;
using System.Text;
using BodyReport.Areas.Site.ViewModels.Account;
using BodyReport.Areas.Api.ViewModels;
using BodyReport.Framework;

namespace BodyReport.Areas1.Api.Controllers
{
    [Authorize]
    [Area("Api")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        ApplicationDbContext _dbContext = null;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender, 
            ILoggerFactory loggerFactory,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _dbContext = dbContext;
        }

        //
        // POST: /Api/Account/LogOff
        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return new HttpOkResult();
        }

        //
        // POST: /Api/Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                return new HttpStatusCodeResult(403);

            //Verify email validate
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return HttpBadRequest(new WebApiException("Invalid login attempt."));
            }
            //Add this to check if the email was confirmed.
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return HttpBadRequest(new WebApiException("You need to confirm your email."));
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(userName, password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                user.LastLoginDate = DateTime.Now;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation(1, "User logged in.");
                return new HttpOkResult();
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(2, "User account locked out.");
                return new HttpStatusCodeResult(403);
            }
            else
            {
                return new HttpStatusCodeResult(403);
            }
        }

        //
        // GET: /Api/Account/GetUserInfo
        [HttpGet]
        public IActionResult GetUserInfo(string userId)
        {
            UserInfo userInfo = null;
            if (string.IsNullOrWhiteSpace(userId))
                userId = User.GetUserId();
            var userManager = new UserManager(_dbContext);
            var user = userManager.GetUser(new UserKey() { Id = userId });

            if (user != null)
            {
                var userInfoManager = new UserInfoManager(_dbContext);
                userInfo = userInfoManager.GetUserInfo(new UserInfoKey() { UserId = userId });
                return new HttpOkObjectResult(userInfo);
            }
            return new HttpNotFoundObjectResult("Oups");
        }

        //
        // POST: /Api/Account/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegisterAccountViewModel registerAccount)
        {
            if (registerAccount == null || string.IsNullOrWhiteSpace(registerAccount.UserName) ||
                string.IsNullOrWhiteSpace(registerAccount.Email) || string.IsNullOrWhiteSpace(registerAccount.Password))
                return HttpBadRequest();

            if (ModelState.IsValid)
            {
                var mailUser = await _userManager.FindByEmailAsync(registerAccount.Email);
                if (mailUser != null)
                    return HttpBadRequest(new WebApiException("Email already exist"));

                var user = new ApplicationUser { UserName = registerAccount.UserName, Email = registerAccount.Email };
                var result = await _userManager.CreateAsync(user, registerAccount.Password);
                if (result.Succeeded)
                {
                    user.RegistrationDate = DateTime.Now;
                    await _userManager.UpdateAsync(user);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(registerAccount.Email, "Confirm your account",
                        "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User created a new account with password.");
                    return new HttpOkObjectResult(true);
                }
                StringBuilder errorMessage = new StringBuilder(); ;
                foreach (var error in result.Errors)
                {
                    errorMessage.AppendLine(error.Description);
                }
                return HttpBadRequest(new WebApiException(errorMessage.ToString()));
            }
            else
                return HttpBadRequest(new WebApiException(ControllerUtils.GetModelStateError(ModelState)));
        }
    
    }
}

