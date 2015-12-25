using BodyReport.Framework;
using BodyReport.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;

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

        public HomeController(ApplicationDbContext dbContext, IHostingEnvironment env)
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
