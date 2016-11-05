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
    public class HomeController : MvcController
    {
        public HomeController() : base()
        {
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
