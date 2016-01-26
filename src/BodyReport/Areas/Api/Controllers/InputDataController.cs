using BodyReport.Areas.Api.ViewModels;
using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Models;
using Message;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BodyReport.Areas.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Api")]
    public class InputDataController : Controller
    {
        public InputDataController(ApplicationDbContext dbContext)
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
