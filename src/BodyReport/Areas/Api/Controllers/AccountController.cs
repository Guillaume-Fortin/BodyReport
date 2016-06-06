using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BodyReport.Message;
using BodyReport.Message.WebApi;
using BodyReport.Areas.Api.ViewModels;
using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Resources;
using BodyReport.Services;
using BodyReport.Data;

namespace BodyReport.Areas.Api.Controllers
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
            return new OkResult();
        }

        //
        // POST: /Api/Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string userName, string password)
        {
            string culture = CultureInfo.CurrentCulture.Name;
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                return new StatusCodeResult(403);

            //Verify email validate
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return BadRequest(new WebApiException(Translation.INVALID_LOGIN_ATTEMPT));
            }
            //Add this to check if the email was confirmed.
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest(new WebApiException(Translation.YOU_NEED_TO_CONFIRM_YOUR_EMAIL));
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(userName, password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                user.LastLoginDate = DateTime.Now;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation(1, Translation.USER_LOGGED_IN);
                return new OkResult();
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(2, Translation.USER_ACCOUNT_LOCKED_OUT);
                return new StatusCodeResult(403);
            }
            else
            {
                return new StatusCodeResult(403);
            }
        }

        //
        // GET: /Api/Account/GetUser
        [HttpGet]
        public IActionResult GetUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                userId = _userManager.GetUserId(User);
            var userManager = new UserManager(_dbContext);
            var user = userManager.GetUser(new UserKey() { Id = userId });
            if (user != null)
                return new OkObjectResult(user);
            return new NotFoundResult();
        }

        //
        // GET: /Api/Account/GetUserInfo
        [HttpGet]
        public IActionResult GetUserInfo(string userId)
        {
            try
            {
                UserInfo userInfo = null;
                if (string.IsNullOrWhiteSpace(userId))
                    userId = _userManager.GetUserId(User);
                var userManager = new UserManager(_dbContext);
                var user = userManager.GetUser(new UserKey() { Id = userId });

                if (user != null)
                {
                    var userInfoManager = new UserInfoManager(_dbContext);
                    userInfo = userInfoManager.GetUserInfo(new UserInfoKey() { UserId = userId });
                    return new OkObjectResult(userInfo);
                }
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        //
        // POST: /Api/Account/UpdateUserInfo
        [HttpPost]
        public IActionResult UpdateUserInfo([FromBody]UserInfo userInfo)
        {
            try
            {
                if (userInfo == null || string.IsNullOrWhiteSpace(userInfo.UserId) ||
                    _userManager.GetUserId(User) != userInfo.UserId)
                    return BadRequest();

                var userInfoManager = new UserInfoManager(_dbContext);
                userInfo = userInfoManager.UpdateUserInfo(userInfo);
                return new OkObjectResult(userInfo);
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        //
        // POST: /Api/Account/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegisterAccountViewModel registerAccount)
        {
            if (registerAccount == null || string.IsNullOrWhiteSpace(registerAccount.UserName) ||
                string.IsNullOrWhiteSpace(registerAccount.Email) || string.IsNullOrWhiteSpace(registerAccount.Password))
                return BadRequest();

            if (ModelState.IsValid)
            {
                var mailUser = await _userManager.FindByEmailAsync(registerAccount.Email);
                if (mailUser != null)
                    return BadRequest(new WebApiException("Email already exist"));

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
                    return new OkObjectResult(true);
                }
                StringBuilder errorMessage = new StringBuilder(); ;
                foreach (var error in result.Errors)
                {
                    errorMessage.AppendLine(error.Description);
                }
                return BadRequest(new WebApiException(errorMessage.ToString()));
            }
            else
                return BadRequest(new WebApiException(ControllerUtils.GetModelStateError(ModelState)));
        }
    
    }
}

