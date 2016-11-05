using BodyReport.Data;
using BodyReport.Framework;
using BodyReport.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BodyReport.Controllers
{
    [Authorize(Roles="Admin")]
    [Area("Admin")]
    public class HomeController : MvcController
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(HomeController));
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;

        public HomeController(UserManager<ApplicationUser> userManager, IHostingEnvironment env) : base(userManager)
        {
            _env = env;
        }

        //
        // GET: /Admin/Home/Index
        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            return View();
        }
    }
}

