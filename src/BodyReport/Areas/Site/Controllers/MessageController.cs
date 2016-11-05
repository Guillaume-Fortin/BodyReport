using BodyReport.Framework;
using Microsoft.AspNetCore.Mvc;

namespace BodyReport.Areas.Site.Controllers
{
    [Area("Site")]
    public class MessageController : MvcController
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
