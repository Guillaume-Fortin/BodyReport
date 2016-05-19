using Microsoft.AspNetCore.Mvc;

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
