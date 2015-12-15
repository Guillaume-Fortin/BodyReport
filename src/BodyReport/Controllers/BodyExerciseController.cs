using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.ViewModels.Admin;
using Message;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BodyExerciseController : Controller
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(BodyExerciseController));

        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
        IHostingEnvironment _env = null;

        public BodyExerciseController(ApplicationDbContext dbContext, IHostingEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        private string GetImageUrl(string imageName)
        {
            if (!System.IO.File.Exists(Path.Combine(_env.WebRootPath, "images", "bodyexercises", imageName)))
            {
                return "/images/unknown.png";
            }
            else
            {
                return string.Format("/images/bodyexercises/{0}", imageName);
            }
        }

        //
        // GET: /BodyExercise/Index
        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            var result = new List<BodyExerciseViewModel>();
            var manager = new BodyExerciseManager(_dbContext);
            var bodyExercises = manager.FindBodyExercises();
            if(bodyExercises != null)
            {
                foreach(var bodyExercise in bodyExercises)
                {
                    result.Add(new BodyExerciseViewModel() { Id = bodyExercise.Id, Name = bodyExercise.Name, ImageUrl = GetImageUrl(bodyExercise.ImageName) });
                }
            }
            return View(result);
        }

        //
        // GET: /BodyExercise/Create
        [HttpGet]
        public IActionResult Create(string returnUrl = null)
        {
            return View(new BodyExerciseViewModel());
           
        }

        // Create new Body Exercise
        // POST: /Admin/Create
        [HttpPost]
        public IActionResult Create(BodyExerciseViewModel bodyExerciseViewModel, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var manager = new BodyExerciseManager(_dbContext);
                var bodyExercise = new BodyExercise() { Name = bodyExerciseViewModel.Name };
                bodyExercise = manager.CreateBodyExercise(bodyExercise);
                if(bodyExercise == null || bodyExercise.Id == 0)
                {
                    _logger.LogError("Create new Body Exercise fail");
                }
                else if (CheckUploadedImageIsCorrect(imageFile))
                {
                    SaveImage(imageFile, bodyExercise.ImageName);
                }

                return RedirectToAction("Index");
            }

            return View(bodyExerciseViewModel);
        }

        //
        // GET: /BodyExercise/Create
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id > 0)
            {
                var manager = new BodyExerciseManager(_dbContext);
                var key = new BodyExerciseKey() { Id = id };
                var bodyExercise = manager.GetBodyExercise(key);
                if (bodyExercise != null)
                {
                    var bodyExerciseViewModel = new BodyExerciseViewModel();
                    bodyExerciseViewModel.Id = bodyExercise.Id;
                    bodyExerciseViewModel.Name = bodyExercise.Name;
                    bodyExerciseViewModel.ImageUrl = GetImageUrl(bodyExercise.ImageName);
                    return View(bodyExerciseViewModel);
                }
            }
            return RedirectToAction("Index");
        }

        // Edit an Body Exercise
        // POST: /Admin/Create
        [HttpPost]
        public IActionResult Edit(BodyExerciseViewModel bodyExerciseViewModel, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // Verify not exist on bodyExercise name
                var manager = new BodyExerciseManager(_dbContext);
                var key = new BodyExerciseKey() { Id = bodyExerciseViewModel.Id };
                var bodyExercise = manager.GetBodyExercise(key);
                if (bodyExercise != null)
                {
                    string oldImageName = bodyExercise.ImageName;
                    bodyExercise.Name = bodyExerciseViewModel.Name;
                    bodyExercise = manager.UpdateBodyExercise(bodyExercise);
                    //Save a new Image if it's correct
                    if (CheckUploadedImageIsCorrect(imageFile))
                    {
                        SaveImage(imageFile, bodyExercise.ImageName);
                    }

                    return RedirectToAction("Index");
                }
            }

            return View(bodyExerciseViewModel);
        }

        private void DeleteImage(string imageName)
        {
            var filePath = Path.Combine(_env.WebRootPath, "images", "bodyexercises", imageName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        
        private bool CheckUploadedImageIsCorrect(IFormFile imageFile)
        {
            if (imageFile != null)
            {
                // Treat upload image
                double fileSizeKo = imageFile.Length / (double)1024;
                if (fileSizeKo <= 500)
                { // Accept little file image <= 500ko
                    var fileName = ContentDispositionHeaderValue.Parse(imageFile.ContentDisposition).FileName.Trim('"');
                    if (fileName.EndsWith(".png"))// Accept only png file
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void SaveImage(IFormFile imageFile, string imageName)
        {
            if (imageFile != null)
            {
                var fileName = ContentDispositionHeaderValue.Parse(imageFile.ContentDisposition).FileName.Trim('"');
                var filePath = Path.Combine(_env.WebRootPath, "images", "bodyexercises", imageName);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                imageFile.SaveAsAsync(filePath).Wait();
            }
        }

        //
        // GET: /BodyExercise/Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if(id >0)
            {
                var manager = new BodyExerciseManager(_dbContext);
                var key = new BodyExerciseKey() { Id = id };
                var bodyExercise = manager.GetBodyExercise(key);
                if (bodyExercise != null)
                {
                    manager.DeleteBodyExercise(key);
                    DeleteImage(bodyExercise.ImageName);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
