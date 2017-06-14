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
using BodyReport.Models;
using BodyReport.Resources;
using BodyReport.ViewModels.Admin;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using BodyReport.ServiceLayers.Interfaces;

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
        /// <summary>
        /// ServiceLayer BodyExercisesService
        /// </summary>
        IBodyExercisesService _bodyExercisesService = null;
        /// <summary>
        /// ServiceLayer MusclesService
        /// </summary>
        IMusclesService _musclesService = null;

        public BodyExerciseController(IHostingEnvironment env,
                                      UserManager<ApplicationUser> userManager,
                                      IBodyExercisesService bodyExercisesService,
                                      IMusclesService musclesService) : base(userManager)
        {
            _env = env;
            _bodyExercisesService = bodyExercisesService;
            _musclesService = musclesService;
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
        public IActionResult Index()
        {
            var result = new List<BodyExerciseViewModel>();
            var muscles = _musclesService.FindMuscles();
            var bodyExercises = _bodyExercisesService.FindBodyExercises();
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
                                ExerciseCategoryType = (int)bodyExercise.ExerciseCategoryType,
                                ExerciseUnitType = (int)bodyExercise.ExerciseUnitType,
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
            ViewBag.Muscles = ControllerUtils.CreateSelectMuscleItemList(_musclesService.FindMuscles(), 0);
            ViewBag.ExerciseCategories = ControllerUtils.CreateSelectExerciseCategoryTypeItemList(0);
            ViewBag.ExerciseUnitTypes = ControllerUtils.CreateSelectExerciseUnitTypeItemList(0);
            return View(new BodyExerciseViewModel());
        }

        // Create Body Exercise
        // POST: /Admin/Create
        [HttpPost]
        public IActionResult Create(BodyExerciseViewModel bodyExerciseViewModel, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var bodyExercise = new BodyExercise()
                {
                    Name = bodyExerciseViewModel.Name,
                    MuscleId = bodyExerciseViewModel.MuscleId,
                    ExerciseCategoryType = Utils.IntToEnum<TExerciseCategoryType>(bodyExerciseViewModel.ExerciseCategoryType),
                    ExerciseUnitType = Utils.IntToEnum<TExerciseUnitType>(bodyExerciseViewModel.ExerciseUnitType)
                };
                bodyExercise = _bodyExercisesService.CreateBodyExercise(bodyExercise);
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
            
            ViewBag.Muscles = ControllerUtils.CreateSelectMuscleItemList(_musclesService.FindMuscles(), 0);
            ViewBag.ExerciseCategories = ControllerUtils.CreateSelectExerciseCategoryTypeItemList(0);
            ViewBag.ExerciseUnitTypes = ControllerUtils.CreateSelectExerciseUnitTypeItemList(0);

            return View(bodyExerciseViewModel);
        }

        // Edit body exercise
        // GET: /Admin/BodyExercise/Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id > 0)
            {
                var key = new BodyExerciseKey() { Id = id };
                var bodyExercise = _bodyExercisesService.GetBodyExercise(key);
                if (bodyExercise != null)
                {
                    var bodyExerciseViewModel = new BodyExerciseViewModel();
                    bodyExerciseViewModel.Id = bodyExercise.Id;
                    bodyExerciseViewModel.Name = bodyExercise.Name;
                    bodyExerciseViewModel.MuscleId = bodyExercise.MuscleId;
                    bodyExerciseViewModel.MuscleName = Translation.GetInDB(MuscleTransformer.GetTranslationKey(bodyExercise.MuscleId));
                    bodyExerciseViewModel.ExerciseCategoryType = (int)bodyExercise.ExerciseCategoryType;
                    bodyExerciseViewModel.ExerciseUnitType = (int)bodyExercise.ExerciseUnitType;
                    bodyExerciseViewModel.ImageUrl = ImageUtils.GetImageUrl(_env.WebRootPath, "bodyexercises", bodyExercise.ImageName);
                    
                    ViewBag.Muscles = ControllerUtils.CreateSelectMuscleItemList(_musclesService.FindMuscles(), bodyExercise.MuscleId);
                    ViewBag.ExerciseCategories = ControllerUtils.CreateSelectExerciseCategoryTypeItemList((int)bodyExercise.ExerciseCategoryType);
                    ViewBag.ExerciseUnitTypes = ControllerUtils.CreateSelectExerciseUnitTypeItemList((int)bodyExercise.ExerciseUnitType);

                    return View(bodyExerciseViewModel);
                }
            }
            return RedirectToAction("Index");
        }

        // Edit an Body Exercise
        // POST: /Admin/BodyExercise/Edit
        [HttpPost]
        public IActionResult Edit(BodyExerciseViewModel bodyExerciseViewModel, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // Verify not exist on bodyExercise name
                var key = new BodyExerciseKey() { Id = bodyExerciseViewModel.Id };
                var bodyExercise = _bodyExercisesService.GetBodyExercise(key);
                if (bodyExercise != null)
                {
                    string oldImageName = bodyExercise.ImageName;
                    bodyExercise.Name = bodyExerciseViewModel.Name;
                    bodyExercise.MuscleId = bodyExerciseViewModel.MuscleId;
                    bodyExercise.ExerciseCategoryType = Utils.IntToEnum<TExerciseCategoryType>(bodyExerciseViewModel.ExerciseCategoryType);
                    bodyExercise.ExerciseUnitType = Utils.IntToEnum<TExerciseUnitType>(bodyExerciseViewModel.ExerciseUnitType);
                    bodyExercise = _bodyExercisesService.UpdateBodyExercise(bodyExercise);
                    //Save a new Image if it's correct
                    if (ImageUtils.CheckUploadedImageIsCorrect(imageFile, "png"))
                    {
                        ImageUtils.SaveImage(imageFile, Path.Combine(_env.WebRootPath, "images", "bodyexercises"), bodyExercise.ImageName);
                    }

                    return RedirectToAction("Index");
                }
            }

            ViewBag.Muscles = ControllerUtils.CreateSelectMuscleItemList(_musclesService.FindMuscles(), bodyExerciseViewModel?.MuscleId ?? 0);
            ViewBag.ExerciseCategories = ControllerUtils.CreateSelectExerciseCategoryTypeItemList(bodyExerciseViewModel?.ExerciseCategoryType ?? 0);
            ViewBag.ExerciseUnitTypes = ControllerUtils.CreateSelectExerciseUnitTypeItemList(bodyExerciseViewModel?.ExerciseUnitType ?? 0);

            return View(bodyExerciseViewModel);
        }

        //Delete Body Exercise
        // GET: /Admin/BodyExercise/Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id > 0)
            {
                var key = new BodyExerciseKey() { Id = id };
                var bodyExercise = _bodyExercisesService.GetBodyExercise(key);
                if (bodyExercise != null)
                {
                    _bodyExercisesService.DeleteBodyExercise(key);
                    DeleteImage(bodyExercise.ImageName);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
