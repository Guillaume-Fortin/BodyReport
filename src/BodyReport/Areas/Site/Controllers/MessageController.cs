using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.Site.Controllers
{
    [Area("Site")]
    public class MessageController : Controller
    {
        //
        // GET: /Site/Message/Index
        public IActionResult Confirm(string title, string message, string returnUrlYes, string returnUrlNo)
        {
            ViewBag.Title = title;
            ViewBag.Message = message;
            ViewBag.ReturnUrlYes = returnUrlYes;
            ViewBag.ReturnUrlNo = returnUrlNo;
            return View();
        }
    }
}
