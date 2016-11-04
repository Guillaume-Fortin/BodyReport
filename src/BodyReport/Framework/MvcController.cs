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
        protected readonly UserManager<ApplicationUser> _userManager;
        /// <summary>
        /// User Id (calculated)
        /// </summary>
        private string _userId = null;
        /// <summary>
        /// UserIdentityCookie (calculated)
        /// </summary>
        private Cookie _userIdentityCookie = null;

        public MvcController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string UserId
        {
            get
            {
                if(_userId == null)
                    _userId = _userManager.GetUserId(HttpContext.User);
                return _userId;
            }
        }
        
        public Cookie UserIdentityCookie
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
