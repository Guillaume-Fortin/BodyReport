using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BodyReport.Message;
using BodyReport.Crud.Transformer;
using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Resources;
using BodyReport.ViewModels.Admin;
using BodyReport.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using BodyReport.WebApiServices;

namespace BodyReport.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class BodyExerciseController : MvcController
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(BodyExerciseController));
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;

        public BodyExerciseController(ApplicationDbContext dbContext, IHostingEnvironment env,
                                      UserManager<ApplicationUser> userManager,
                                      WebApiWrapper webApiWrapper) : base(dbContext, userManager, webApiWrapper)
        {
            _env = env;
        }

        private void DeleteImage(string imageName)
        {
            var filePath = Path.Combine(_env.WebRootPath, "images", "bodyexercises", imageName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        //Manage body exercise
        // GET: /Admin/BodyExercise/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = new List<BodyExerciseViewModel>();
            var muscles = await MusclesWS.Find(_webApiWrapper, UserId, UserIdentityCookie);
            var bodyExercises = await BodyExercisesWS.Find(_webApiWrapper, UserId, UserIdentityCookie);
            if (bodyExercises != null && muscles != null)
            {
                foreach (var muscle in muscles.OrderBy(t=>t.Name))
                {
                    foreach (var bodyExercise in bodyExercises)
                    {
                        if (muscle.Id == bodyExercise.MuscleId)
                        {
                            result.Add(new BodyExerciseViewModel()
                            {
                                Id = bodyExercise.Id,
                                Name = bodyExercise.Name,
                                ImageUrl = ImageUtils.GetImageUrl(_env.WebRootPath, "bodyexercises", bodyExercise.ImageName),
                                MuscleId = bodyExercise.MuscleId,
                                MuscleName = Resources.Translation.GetInDB(MuscleTransformer.GetTranslationKey(bodyExercise.MuscleId))
                            });
                        }
                    }
                }
            }
            return View(result);
        }

        // Create Body Exercise
        // GET: /Admin/BodyExercise/Create
        [HttpGet]
        public IActionResult Create(string returnUrl = null)
        {
            var muscleManager = new MuscleManager(_dbContext);
            ViewBag.Muscles = ControllerUtils.CreateSelectMuscleItemList(muscleManager.FindMuscles(), 0);

            return View(new BodyExerciseViewModel());
        }

        // Create Body Exercise
        // POST: /Admin/Create
        [HttpPost]
        public async Task<IActionResult> Create(BodyExerciseViewModel bodyExerciseViewModel, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var bodyExercise = new BodyExercise() { Name = bodyExerciseViewModel.Name, MuscleId = bodyExerciseViewModel.MuscleId };
                bodyExercise = await BodyExercisesWS.Create(_webApiWrapper, UserId, UserIdentityCookie, bodyExercise);
                if (bodyExercise == null || bodyExercise.Id == 0)
                {
                    _logger.LogError("Create new Body Exercise fail");
                }
                else if (ImageUtils.CheckUploadedImageIsCorrect(imageFile, "png"))
                {
                    ImageUtils.SaveImage(imageFile, Path.Combine(_env.WebRootPath, "images", "bodyexercises"), bodyExercise.ImageName);
                }

                return RedirectToAction("Index");
            }

            var muscleManager = new MuscleManager(_dbContext);
            ViewBag.Muscles = ControllerUtils.CreateSelectMuscleItemList(muscleManager.FindMuscles(), 0);

            return View(bodyExerciseViewModel);
        }

        // Edit body exercise
        // GET: /Admin/BodyExercise/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id > 0)
            {
                var key = new BodyExerciseKey() { Id = id };
                var bodyExercise = await BodyExercisesWS.Get(_webApiWrapper, UserId, UserIdentityCookie, key);
                if (bodyExercise != null)
                {
                    var bodyExerciseViewModel = new BodyExerciseViewModel();
                    bodyExerciseViewModel.Id = bodyExercise.Id;
                    bodyExerciseViewModel.Name = bodyExercise.Name;
                    bodyExerciseViewModel.MuscleId = bodyExercise.MuscleId;
                    bodyExerciseViewModel.MuscleName = Translation.GetInDB(MuscleTransformer.GetTranslationKey(bodyExercise.MuscleId));
                    bodyExerciseViewModel.ImageUrl = ImageUtils.GetImageUrl(_env.WebRootPath, "bodyexercises", bodyExercise.ImageName);

                    var muscleManager = new MuscleManager(_dbContext);
                    ViewBag.Muscles = ControllerUtils.CreateSelectMuscleItemList(muscleManager.FindMuscles(), bodyExercise.MuscleId);

                    return View(bodyExerciseViewModel);
                }
            }
            return RedirectToAction("Index");
        }

        // Edit an Body Exercise
        // POST: /Admin/BodyExercise/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(BodyExerciseViewModel bodyExerciseViewModel, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // Verify not exist on bodyExercise name
                var key = new BodyExerciseKey() { Id = bodyExerciseViewModel.Id };
                var bodyExercise = await BodyExercisesWS.Get(_webApiWrapper, UserId, UserIdentityCookie, key);
                if (bodyExercise != null)
                {
                    string oldImageName = bodyExercise.ImageName;
                    bodyExercise.Name = bodyExerciseViewModel.Name;
                    bodyExercise.MuscleId = bodyExerciseViewModel.MuscleId;
                    bodyExercise = await BodyExercisesWS.Update(_webApiWrapper, UserId, UserIdentityCookie, bodyExercise);
                    //Save a new Image if it's correct
                    if (ImageUtils.CheckUploadedImageIsCorrect(imageFile, "png"))
                    {
                        ImageUtils.SaveImage(imageFile, Path.Combine(_env.WebRootPath, "images", "bodyexercises"), bodyExercise.ImageName);
                    }

                    return RedirectToAction("Index");
                }
            }

            int muscleId = 0;
            if (bodyExerciseViewModel != null)
                muscleId = bodyExerciseViewModel.MuscleId;

            var muscleManager = new MuscleManager(_dbContext);
            ViewBag.Muscles = ControllerUtils.CreateSelectMuscleItemList(muscleManager.FindMuscles(), muscleId);

            return View(bodyExerciseViewModel);
        }

        //Delete Body Exercise
        // GET: /Admin/BodyExercise/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id > 0)
            {
                var key = new BodyExerciseKey() { Id = id };
                var bodyExercise = await BodyExercisesWS.Get(_webApiWrapper, UserId, UserIdentityCookie, key);
                if (bodyExercise != null)
                {
                    await BodyExercisesWS.Delete(_webApiWrapper, UserId, UserIdentityCookie, key);
                    DeleteImage(bodyExercise.ImageName);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
