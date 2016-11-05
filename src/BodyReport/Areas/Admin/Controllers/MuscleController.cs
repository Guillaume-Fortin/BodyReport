using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using BodyReport.Message;
using BodyReport.Crud.Transformer;
using BodyReport.Framework;
using BodyReport.Models;
using BodyReport.Resources;
using BodyReport.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using BodyReport.ServiceLayers.Interfaces;

namespace BodyReport.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class MuscleController : MvcController
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(MuscleController));
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;
        /// <summary>
        /// Service layer
        /// </summary>
        private readonly IMusclesService _musclesService;
        /// <summary>
        /// Service layer
        /// </summary>
        private readonly IMuscularGroupsService _muscularGroupsService;

        public MuscleController(UserManager<ApplicationUser> userManager,
                                IMusclesService musclesService,
                                IMuscularGroupsService muscularGroupsService,
                                IHostingEnvironment env) : base(userManager)
        {
            _env = env;
            _musclesService = musclesService;
            _muscularGroupsService = muscularGroupsService;
        }

        // manage muscles
        // GET: /Admin/Muscle/Index
        [HttpGet]
        public IActionResult Index()
        {
            MuscleViewModel muscleViewModel;
            var musculeViewModels = new List<MuscleViewModel>();
            
            var muscles = _musclesService.FindMuscles();
            if (muscles != null)
            {
                foreach (var muscle in muscles)
                {
                    muscleViewModel = new MuscleViewModel()
                    {
                        Id = muscle.Id,
                        Name = muscle.Name,
                        MuscularGroupId = muscle.MuscularGroupId,
                        MuscularGroupName = Translation.GetInDB(MuscularGroupTransformer.GetTranslationKey(muscle.MuscularGroupId))
                    };
                    musculeViewModels.Add(muscleViewModel);
                }
            }

            ViewBag.Muscles = musculeViewModels;

            return View();
        }

        // Create
        // GET: /Admin/Muscle/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.MuscularGroups = ControllerUtils.CreateSelectMuscularGroupItemList(_muscularGroupsService.FindMuscularGroups(), 0);

            return View(new MuscleViewModel());
        }

        // Create
        // POST: /Admin/Muscle/Create
        [HttpPost]
        public IActionResult Create(MuscleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var muscle = new Muscle() { Name = viewModel.Name, MuscularGroupId = viewModel.MuscularGroupId };
                muscle = _musclesService.CreateMuscle(muscle);
                if (muscle == null || muscle.Id == 0)
                {
                    _logger.LogError("Create new muscle fail");
                }

                return RedirectToAction("Index");
            }
            
            ViewBag.MuscularGroups = ControllerUtils.CreateSelectMuscularGroupItemList(_muscularGroupsService.FindMuscularGroups(), 0);

            return View(viewModel);
        }

        // Edit
        // GET: /Admin/Muscle/Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id != 0)
            {
                var key = new MuscleKey() { Id = id };
                var muscle = _musclesService.GetMuscle(key);
                if (muscle != null)
                {
                    var viewModel = new MuscleViewModel();
                    viewModel.Id = muscle.Id;
                    viewModel.Name = muscle.Name;
                    viewModel.MuscularGroupId = muscle.MuscularGroupId;
                    
                    ViewBag.MuscularGroups = ControllerUtils.CreateSelectMuscularGroupItemList(_muscularGroupsService.FindMuscularGroups(), viewModel.MuscularGroupId);

                    return View(viewModel);
                }
            }

            return RedirectToAction("Index");
        }

        // Edit
        // POST: /Admin/Muscle/Edit
        [HttpPost]
        public IActionResult Edit(MuscleViewModel viewModel)
        {
            if (ModelState.IsValid && viewModel.Id > 0)
            {
                // Verify not exist on id
                var key = new MuscleKey() { Id = viewModel.Id };
                var muscle = _musclesService.GetMuscle(key);
                if (muscle != null)
                {
                    muscle.Name = viewModel.Name;
                    muscle.MuscularGroupId = viewModel.MuscularGroupId;
                    muscle = _musclesService.UpdateMuscle(muscle);
                    return RedirectToAction("Index");
                }
            }

            int muscularGroupId = 0;
            if (viewModel != null)
                muscularGroupId = viewModel.MuscularGroupId;
            
            ViewBag.MuscularGroups = ControllerUtils.CreateSelectMuscularGroupItemList(_muscularGroupsService.FindMuscularGroups(), muscularGroupId);

            return View(viewModel);
        }

        //Delete
        // GET: /Admin/Muscle/Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id != 0)
            {
                var key = new MuscleKey() { Id = id };
                var muscle = _musclesService.GetMuscle(key);
                if (muscle != null)
                {
                    _musclesService.DeleteMuscle(muscle);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
