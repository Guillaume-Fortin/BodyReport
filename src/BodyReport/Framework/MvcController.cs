using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BodyReport.Models;
using BodyReport.WebApiServices;
using BodyReport.Data;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BodyReport.Framework
{
    public class MvcController : Controller
    {
        /// <summary>
        /// user manager
        /// </summary>
        protected readonly UserManager<ApplicationUser> _identityUserManager;
        /// <summary>
        /// Application db context
        /// </summary>
        protected readonly ApplicationDbContext _dbContext = null;
        /// <summary>
        /// User Id (calculated)
        /// </summary>
        private string _sessionUserId = null;
        /// <summary>
        /// UserIdentityCookie (calculated)
        /// </summary>
        private Cookie _userIdentityCookie = null;

        public MvcController()
        {
        }
        public MvcController(UserManager<ApplicationUser> userManager)
        {
            _identityUserManager = userManager;
        }
        public MvcController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _identityUserManager = userManager;
            _dbContext = dbContext;
        }

        protected string SessionUserId
        {
            get
            {
                if(_sessionUserId == null)
                    _sessionUserId = _identityUserManager.GetUserId(HttpContext.User);
                return _sessionUserId;
            }
        }
        
        protected Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _identityUserManager.GetUserAsync(HttpContext.User);
        }

        protected Cookie UserIdentityCookie
        {
            get
            {
                if (_userIdentityCookie == null)
                    _userIdentityCookie = ControllerUtils.GetIdentityUserCookie(HttpContext);
                return _userIdentityCookie;
            }
        }
    }
}
