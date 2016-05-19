using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BodyReport.Framework;
using BodyReport.Models;
using BodyReport.Data;

namespace BodyReport.Areas.User.Controllers
{
    [Authorize(Roles = "User,Admin")]
    [Area("User")]
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

        public HomeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //
        // GET: /User/Home/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
