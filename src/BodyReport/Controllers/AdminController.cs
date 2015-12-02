using BodyReport.ViewModels.Admin;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/BodyExercises
        [HttpGet]
        [AllowAnonymous]
        public IActionResult BodyExercises(string returnUrl = null)
        {
            // ViewData["ReturnUrl"] = returnUrl;
            var result = new List<BodyExerciseViewModel>();
            for (int i = 0; i < 30; i++)
                result.Add(new BodyExerciseViewModel() { Id = i, Name="name "+ i, ImageUrl= "http://thumbs.dreamstime.com/z/gros-exercice-d-homme-28195227.jpg" });
            return View(result);
        }

        //
        // GET: /Admin/CreateBodyExercise
        [HttpGet]
        [AllowAnonymous]
        public IActionResult CreateBodyExercise(string returnUrl = null)
        {
            return View(new BodyExerciseViewModel());
        }

        //
        // POST: /Admin/CreateBodyExercise
        [HttpPost]
        [AllowAnonymous]
        public IActionResult CreateBodyExercise(BodyExerciseViewModel bodyExerciseViewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("BodyExercises");
            }

            return View(bodyExerciseViewModel);
        }
    }
}
