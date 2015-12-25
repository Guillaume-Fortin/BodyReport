using BodyReport.Crud.Transformer;
using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Resources;
using BodyReport.ViewModels.Admin;
using Message;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

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

