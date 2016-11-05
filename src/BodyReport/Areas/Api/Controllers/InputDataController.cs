using System.Net.Http;
using System.Text;
using BodyReport.Areas.Api.ViewModels;
using BodyReport.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BodyReport.Data;
using BodyReport.Framework;
using Microsoft.AspNetCore.Identity;

namespace BodyReport.Areas.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Api")]
    public class InputDataController : MvcController
    {
        public InputDataController(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        // Get api/InputData/Index
        [HttpGet]
        public ActionResult Index()
        {
            return View(new InputDataViewModel());
        }

        // Get api/InputData/Index
        [HttpPost]
        public ActionResult Index(InputDataViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var content = new StringContent(viewModel.Data, Encoding.UTF8, "application/json");

                    string baseUrl;
                    if (viewModel.Url.StartsWith("http://"))
                        baseUrl = viewModel.Url;
                    else
                        baseUrl = "http://" + Request.Host.Value + viewModel.Url;
                    var result = httpClient.PostAsync(baseUrl, content).Result;
                    string resultContent = result.Content.ReadAsStringAsync().Result;
                    return Content(resultContent);
                }
            }
            return View(viewModel);
        }
    }
}
