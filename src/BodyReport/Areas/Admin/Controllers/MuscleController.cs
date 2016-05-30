using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using BodyReport.Message;
using BodyReport.Crud.Transformer;
using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Resources;
using BodyReport.ViewModels.Admin;
using BodyReport.Data;

namespace BodyReport.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class MuscleController : Controller
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(MuscleController));
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;

        public MuscleController(ApplicationDbContext dbContext, IHostingEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        // manage muscles
        // GET: /Admin/Muscle/Index
        [HttpGet]
        public IActionResult Index()
        {
            MuscleViewModel muscleViewModel;
            var musculeViewModels = new List<MuscleViewModel>();

            var manager = new MuscleManager(_dbContext);
            var muscles = manager.FindMuscles();
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
            var muscularGroupManager = new MuscularGroupManager(_dbContext);
            ViewBag.MuscularGroups = ControllerUtils.CreateSelectMuscularGroupItemList(muscularGroupManager.FindMuscularGroups(), 0);

            return View(new MuscleViewModel());
        }

        // Create
        // POST: /Admin/Muscle/Create
        [HttpPost]
        public IActionResult Create(MuscleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var manager = new MuscleManager(_dbContext);
                var muscle = new Muscle() { Name = viewModel.Name, MuscularGroupId = viewModel.MuscularGroupId };
                muscle = manager.CreateMuscle(muscle);
                if (muscle == null || muscle.Id == 0)
                {
                    _logger.LogError("Create new muscle fail");
                }

                return RedirectToAction("Index");
            }

            var muscularGroupManager = new MuscularGroupManager(_dbContext);
            ViewBag.MuscularGroups = ControllerUtils.CreateSelectMuscularGroupItemList(muscularGroupManager.FindMuscularGroups(), 0);

            return View(viewModel);
        }

        // Edit
        // GET: /Admin/Muscle/Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id != 0)
            {
                var manager = new MuscleManager(_dbContext);
                var key = new MuscleKey() { Id = id };
                var muscle = manager.GetMuscle(key);
                if (muscle != null)
                {
                    var viewModel = new MuscleViewModel();
                    viewModel.Id = muscle.Id;
                    viewModel.Name = muscle.Name;
                    viewModel.MuscularGroupId = muscle.MuscularGroupId;

                    var muscularGroupManager = new MuscularGroupManager(_dbContext);
                    ViewBag.MuscularGroups = ControllerUtils.CreateSelectMuscularGroupItemList(muscularGroupManager.FindMuscularGroups(), viewModel.MuscularGroupId);

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
                var manager = new MuscleManager(_dbContext);
                var key = new MuscleKey() { Id = viewModel.Id };
                var muscle = manager.GetMuscle(key);
                if (muscle != null)
                {
                    muscle.Name = viewModel.Name;
                    muscle.MuscularGroupId = viewModel.MuscularGroupId;
                    muscle = manager.UpdateMuscle(muscle);
                    return RedirectToAction("Index");
                }
            }

            int muscularGroupId = 0;
            if (viewModel != null)
                muscularGroupId = viewModel.MuscularGroupId;

            var muscularGroupManager = new MuscularGroupManager(_dbContext);
            ViewBag.MuscularGroups = ControllerUtils.CreateSelectMuscularGroupItemList(muscularGroupManager.FindMuscularGroups(), muscularGroupId);

            return View(viewModel);
        }

        //Delete
        // GET: /Admin/Muscle/Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id != 0)
            {
                var manager = new MuscleManager(_dbContext);
                var key = new MuscleKey() { Id = id };
                var muscle = manager.GetMuscle(key);
                if (muscle != null)
                {
                    manager.DeleteMuscle(muscle);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
