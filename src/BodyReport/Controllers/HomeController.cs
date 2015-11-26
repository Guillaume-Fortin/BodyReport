using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Localization;
using System.Globalization;
using Microsoft.Extensions.Localization;
using BodyReport.Resources;
using BodyReport.Framework;

namespace BodyReport.Controllers
{
    public class HomeController : Controller
    {
        private IStringLocalizer _htmlLocalizer;

        public HomeController(/*IStringLocalizer<Translation> localizer*/)
        {
            //CultureInfo.CurrentCulture = new CultureInfo("fr");
            //CultureInfo.CurrentUICulture = new CultureInfo("fr");

            StringLocalizerFactory sl = new StringLocalizerFactory();
            _htmlLocalizer = sl.Create("Translation", "Resources");
           // _htmlLocalizer = localizer;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            //ViewData["Message"] = "My Sport Report WebSite.";

            ViewData["Message"] = _htmlLocalizer["HELLO"];
            ViewData["CurrentCulture"] = CultureInfo.CurrentCulture.ToString();
            ViewData["CurrentUICulture"] = CultureInfo.CurrentUICulture.ToString();

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }


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
