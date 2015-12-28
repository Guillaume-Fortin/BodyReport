﻿using BodyReport.Crud.Transformer;
using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Resources;
using BodyReport.ViewModels.Admin;
using Message;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
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
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;

        public BodyExerciseController(ApplicationDbContext dbContext, IHostingEnvironment env)
        {
            _dbContext = dbContext;
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
        public IActionResult Index()
        {
            var result = new List<BodyExerciseViewModel>();
            var manager = new BodyExerciseManager(_dbContext);
            var bodyExercises = manager.FindBodyExercises();
            if (bodyExercises != null)
            {
                foreach (var bodyExercise in bodyExercises)
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
        public IActionResult Create(BodyExerciseViewModel bodyExerciseViewModel, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var manager = new BodyExerciseManager(_dbContext);
                var bodyExercise = new BodyExercise() { Name = bodyExerciseViewModel.Name, MuscleId = bodyExerciseViewModel.MuscleId };
                bodyExercise = manager.CreateBodyExercise(bodyExercise);
                if (bodyExercise == null || bodyExercise.Id == 0)
                {
                    _logger.LogError("Create new Body Exercise fail");
                }
                else if (ImageUtils.CheckUploadedImageIsCorrect(imageFile))
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
                    bodyExercise.MuscleId = bodyExerciseViewModel.MuscleId;
                    bodyExercise = manager.UpdateBodyExercise(bodyExercise);
                    //Save a new Image if it's correct
                    if (ImageUtils.CheckUploadedImageIsCorrect(imageFile))
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
        public IActionResult Delete(int id)
        {
            if (id > 0)
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