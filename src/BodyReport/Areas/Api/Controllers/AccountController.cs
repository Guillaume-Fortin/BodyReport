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

namespace BodyReport.Areas1.Api.Controllers
{
	[Authorize]
    [Area("Api")]
	public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
		ApplicationDbContext _dbContext = null;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory,
			ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
			if(string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
				return new HttpStatusCodeResult(403);
			
            //Verify email validate
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
				return new HttpStatusCodeResult(403);
            }
            //Add this to check if the email was confirmed.
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
				return new HttpStatusCodeResult(403);
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
		public UserInfo GetUserInfo(string userId)
		{
			UserInfo userInfo = null;
			if(userId == null)
				userId = User.GetUserId();
			var userManager = new UserManager(_dbContext);
			var user = userManager.GetUser(new UserKey() { Id = userId });

			if (user != null)
			{
				var userInfoManager = new UserInfoManager(_dbContext);
				userInfo = userInfoManager.GetUserInfo(new UserInfoKey() { UserId = userId });
			}
			return userInfo;
		}
    }
}

