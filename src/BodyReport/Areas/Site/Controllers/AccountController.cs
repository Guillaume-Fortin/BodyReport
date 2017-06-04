using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using BodyReport.Areas.Site.ViewModels.Account;
using BodyReport.Models;
using BodyReport.Resources;
using BodyReport.Services;
using BodyReport.Message;
using BodyReport.Framework;
using Microsoft.AspNetCore.Localization;
using BodyReport.Data;
using BodyReport.ServiceLayers.Interfaces;

namespace BodyReport.Areas.Site.Controllers
{
    [Authorize]
    [Area("Site")]
    public class AccountController : MvcController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        /// <summary>
        /// Service layer users
        /// </summary>
        private readonly IUsersService _usersService;
        /// <summary>
        /// Service layer users
        /// </summary>
        private readonly IUserInfosService _userInfosService;
        /// <summary>
        /// Service layer roles
        /// </summary>
        private readonly IRolesService _rolesService;
        /// <summary>
        /// Report service
        /// </summary>
        private readonly IReportService _reportService;

        public AccountController(ApplicationDbContext dbContext,
                                 UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 IUsersService usersService,
                                 IUserInfosService userInfosService,
                                 IRolesService rolesService,
                                 IEmailSender emailSender,
                                 ISmsSender smsSender,
                                 ILoggerFactory loggerFactory,
                                 IReportService reportService) : base(userManager, dbContext)
        {
            _signInManager = signInManager;
            _usersService = usersService;
            _userInfosService = userInfosService;
            _rolesService = rolesService;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _reportService = reportService;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        //
        // GET: /Site/Account/AccessDenied
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Site/Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Site/Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                //Verify email validate
                var user = await _identityUserManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    ModelState.AddModelError("", Translation.INVALID_LOGIN_ATTEMPT);
                    return View(model);
                }
                //Add this to check if the email was confirmed after 30 days.
                if (!await _identityUserManager.IsEmailConfirmedAsync(user) &&
                    (user.RegistrationDate == null || (DateTime.Now.Date - user.RegistrationDate.Date).TotalDays >= 30))
                {
                    ModelState.AddModelError("", Translation.YOU_NEED_TO_CONFIRM_YOUR_EMAIL);
                    return View(model);
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    user.LastLoginDate = DateTime.Now;
                    await _identityUserManager.UpdateAsync(user);
                    _logger.LogInformation(1, Translation.USER_LOGGED_IN);
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, Translation.USER_ACCOUNT_LOCKED_OUT);
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translation.INVALID_LOGIN_ATTEMPT);
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Site/Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        //
        // POST: /Site/Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var mailUser = await _identityUserManager.FindByEmailAsync(model.Email);
                if(mailUser != null)
                {
                    ModelState.AddModelError("", "Email already exist");
                    return View(model);
                }
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await _identityUserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    string reportData;
                    user.RegistrationDate = DateTime.Now;
                    await _identityUserManager.UpdateAsync(user);
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User created a new account with password. USerName : " + user.UserName);
                    try
                    {
                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                        // Send an email with this link
                        var code = await _identityUserManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                        reportData = await _reportService.CreateReportForConfirmUserAccountAsync(this.ControllerContext, user.Id, callbackUrl);
                        await _emailSender.SendEmailAsync(model.Email, Translation.CONFIRM_USER_ACCOUNT, reportData);
                    }
                    catch (Exception except)
                    {
                        _logger.LogError(0, except, "Can't send email");
                    }
                    //SendEmail to admin
                    reportData = await _reportService.CreateReportForAdminNewUserAccountCreatedAsync(this.ControllerContext, user.Id);
                    await ControllerUtils.SendEmailToAdminAsync(_usersService, _emailSender, "Nouvel utilisateur site", reportData);
                    return RedirectToAction(nameof(AccountController.Login), "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Site/Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Site" });
        }

        //
        // POST: /Site/Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        //
        // GET: /Site/Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
        }

        //
        // POST: /Site/Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction(nameof(ManageController.Index), "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _identityUserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _identityUserManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        // GET: /Site/Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var applicationUser = await _identityUserManager.FindByIdAsync(userId);
            if (applicationUser == null)
            {
                return View("Error");
            }
            var result = await _identityUserManager.ConfirmEmailAsync(applicationUser, code);

            if(result.Succeeded)
            {
                // add role to user
                //await _userManager.AddToRoleAsync(user, "user"); //don't work on rc2???
                // Verify not exist on id
                var key = new UserKey() { Id = applicationUser.Id };
                var user = _usersService.GetUser(key);
                if (user != null)
                {
                    //Verify role exist
                    var roleKey = new RoleKey();
                    roleKey.Id = "1"; //User
                    var role = _rolesService.GetRole(roleKey);
                    if (role != null)
                    {
                        user.Role = role;
                        user = _usersService.UpdateUser(user);
                    }

                    if (user != null)
                    {
                        //Add empty user profil (for correct connect error on mobile application)
                        var userInfo = new UserInfo()
                        {
                            UserId = user.Id,
                            Unit = TUnitType.Metric
                        };
                        _userInfosService.UpdateUserInfo(userInfo);
                    }

                    return RedirectToAction("Index", "Home", new { area = "Site" });
                }
            }
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Site/Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Site/Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _identityUserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _identityUserManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                try
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    var code = await _identityUserManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { area = "Site", userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    string reportData = await _reportService.CreateReportForResetUserPasswordAsync(this.ControllerContext, user.Id, callbackUrl);
                    await _emailSender.SendEmailAsync(model.Email, Translation.FORGOT_PASSWORD, reportData);
                }
                catch (Exception except)
                {
                    _logger.LogError(0, except, "can't send email");
                }
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Site/Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Site/Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string userId, string code)
        {
            ResetPasswordViewModel model = null;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var user = await _identityUserManager.FindByIdAsync(userId);
                if (user != null)
                {
                    model = new ResetPasswordViewModel() { Code = code, Email = user.Email };
                }
            }
            return View(model);
        }

        //
        // POST: /Site/Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _identityUserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _identityUserManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Site/Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Site/Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _identityUserManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Site/Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _identityUserManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                try
                {
                    await _emailSender.SendEmailAsync(await _identityUserManager.GetEmailAsync(user), "Security Code", message);
                }
                catch (Exception except)
                {
                    _logger.LogError(0, except, "can't send email");
                }
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _smsSender.SendSmsAsync(await _identityUserManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Site/Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Site/Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError("", "Invalid code.");
                return View(model);
            }
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Site" });
            }
        }

        #endregion

        [AllowAnonymous]
        [HttpGet]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return Redirect(returnUrl);
        }
    }
}
