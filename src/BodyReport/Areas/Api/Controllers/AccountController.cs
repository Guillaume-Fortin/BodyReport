using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BodyReport.Message;
using BodyReport.Message.Web;
using BodyReport.Areas.Api.ViewModels;
using BodyReport.Framework;
using BodyReport.Models;
using BodyReport.Resources;
using BodyReport.Services;
using BodyReport.Data;
using BodyReport.ServiceLayers.Interfaces;

namespace BodyReport.Areas.Api.Controllers
{
    [Authorize]
    [Area("Api")]
    public class AccountController : MvcController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        /// <summary>
        /// Service layer users
        /// </summary>
        private readonly IUsersService _usersService;
        /// <summary>
        /// Service layer userInfos
        /// </summary>
        private readonly IUserInfosService _userInfosService;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 ApplicationDbContext dbContext,
                                 SignInManager<ApplicationUser> signInManager,
                                 IUsersService usersService,
                                 IUserInfosService userInfosService,
                                 IEmailSender emailSender, 
                                 ILoggerFactory loggerFactory) : base(userManager, dbContext)
        {
            _usersService = usersService;
            _userInfosService = userInfosService;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
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
            var user = await _identityUserManager.FindByNameAsync(userName);
            if (user == null)
            {
                return BadRequest(new WebApiException(Translation.INVALID_LOGIN_ATTEMPT));
            }
            //Add this to check if the email was confirmed.
            if (!await _identityUserManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest(new WebApiException(Translation.YOU_NEED_TO_CONFIRM_YOUR_EMAIL));
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(userName, password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                user.LastLoginDate = DateTime.Now;
                await _identityUserManager.UpdateAsync(user);
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
                userId = SessionUserId;
            var user = _usersService.GetUser(new UserKey() { Id = userId });
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
                    userId = SessionUserId;
                var user = _usersService.GetUser(new UserKey() { Id = userId });

                if (user != null)
                {
                    userInfo = _userInfosService.GetUserInfo(new UserInfoKey() { UserId = userId });
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
                    SessionUserId != userInfo.UserId)
                    return BadRequest();
                
                userInfo = _userInfosService.UpdateUserInfo(userInfo);
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
                var mailUser = await _identityUserManager.FindByEmailAsync(registerAccount.Email);
                if (mailUser != null)
                    return BadRequest(new WebApiException("Email already exist"));

                var user = new ApplicationUser { UserName = registerAccount.UserName, Email = registerAccount.Email };
                var result = await _identityUserManager.CreateAsync(user, registerAccount.Password);
                if (result.Succeeded)
                {
                    user.RegistrationDate = DateTime.Now;
                    await _identityUserManager.UpdateAsync(user);
                    _logger.LogInformation(3, "User created a new account with password.");
                    try
                    {
                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                        // Send an email with this link
                        var code = await _identityUserManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { area = "Site", userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                        await _emailSender.SendEmailAsync(registerAccount.Email, "Confirm your account",
                        "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                        //await _signInManager.SignInAsync(user, isPersistent: false);
                    }
                    catch (Exception except)
                    {
                        _logger.LogError(3, except, "can't send email ");
                    }
                    //SendEmail to admin
                    ControllerUtils.SendEmailToAdmin(_dbContext, _usersService, _emailSender, "BodyReport : New mobile user", "New user register with mobile");
                    return new OkObjectResult(true);
                }
                StringBuilder errorMessage = new StringBuilder();
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

