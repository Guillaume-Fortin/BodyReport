using BodyReport.Data;
using BodyReport.Framework;
using BodyReport.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BodyReport.Controllers
{
    [Authorize(Roles="Admin")]
    [Area("Admin")]
    public class HomeController : Controller
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(HomeController));
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;

        public HomeController(ApplicationDbContext dbContext, IHostingEnvironment env)
        {
            _dbContext = dbContext;
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

