using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using BodyReport.Framework;
using Microsoft.AspNetCore.Identity;
using BodyReport.Models;

namespace BodyReport.Areas.Site.Controllers
{
    [Area("Site")]
    public class HomeController : MvcController
    {
        private IStringLocalizer _htmlLocalizer;

        public HomeController() : base()
        {
            StringLocalizerFactory sl = new StringLocalizerFactory();
            _htmlLocalizer = sl.Create("Translation", "Resources");
        }

        //
        // GET: /Site/Home/Index
        public IActionResult Index()
        {
            return View();
        }

        //
        // GET: /Site/Home/About
        public IActionResult About()
        {
            //ViewData["Message"] = "My Sport Report WebSite.";

            ViewData["Message"] = _htmlLocalizer["HELLO"];
            ViewData["CurrentCulture"] = CultureInfo.CurrentCulture.ToString();
            ViewData["CurrentUICulture"] = CultureInfo.CurrentUICulture.ToString();

            return View();
        }

        //
        // GET: /Site/Home/Contact
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        //
        // GET: /Site/Home/Error
        public IActionResult Error()
        {
            return View();
        }
        
        //
        // GET: /Site/Home/Json
        public IActionResult Json()
        {
            List<string> dataList = new List<string>();
            for(int i=0; i < 50; i++)
            {
                dataList.Add("test " + i);
            }
            return Json(dataList);
        }
    }
}
