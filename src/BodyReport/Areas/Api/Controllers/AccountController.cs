using BodyReport.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.Api.Controllers
{
    [Authorize]
    [Area("Api")]
    public class AccountController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
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
            //Verify email validate
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return new HttpUnauthorizedResult();
            }
            //Add this to check if the email was confirmed.
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new HttpUnauthorizedResult();
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
                return new HttpUnauthorizedResult();
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }
    }
}

