using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Message;
using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.ViewModels.Admin;
using BodyReport.Data;

namespace BodyReport.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class MuscularGroupController : Controller
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(MuscularGroupController));
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;

        public MuscularGroupController(ApplicationDbContext dbContext, IHostingEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        // manage muscular group
        // GET: /Admin/MuscularGroup/Index
        [HttpGet]
        public IActionResult Index()
        {
            MuscularGroupViewModel muscularGroupViewModel;
            var muscularGroupsViewModels = new List<MuscularGroupViewModel>();
            var manager = new MuscularGroupManager(_dbContext);

            var muscularGroups = manager.FindMuscularGroups();
            if (muscularGroups != null)
            {
                foreach (var muscularGroup in muscularGroups)
                {
                    muscularGroupViewModel = new MuscularGroupViewModel() { Id = muscularGroup.Id, Name = muscularGroup.Name };
                    muscularGroupsViewModels.Add(muscularGroupViewModel);
                }
            }

            ViewBag.MuscularGroups = muscularGroupsViewModels;

            return View();
        }

        // Create new muscular group
        // GET: /Admin/MuscularGroup/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new MuscularGroupViewModel());
        }

        // Create new muscular group
        // POST: /Admin/MuscularGroup/Create
        [HttpPost]
        public IActionResult Create(MuscularGroupViewModel muscularGroupViewModel)
        {
            if (ModelState.IsValid)
            {
                var manager = new MuscularGroupManager(_dbContext);
                var muscularGroup = new MuscularGroup() { Name = muscularGroupViewModel.Name };
                muscularGroup = manager.CreateMuscularGroup(muscularGroup);
                if (muscularGroup == null || muscularGroup.Id == 0)
                {
                    _logger.LogError("Create new muscular group fail");
                }

                return RedirectToAction("Index");
            }

            return View(muscularGroupViewModel);
        }

        // Edit a Muscular Group
        // GET: /Admin/MuscularGroup/Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id != 0)
            {
                var manager = new MuscularGroupManager(_dbContext);
                var key = new MuscularGroupKey() { Id = id };
                var muscularGroup = manager.GetMuscularGroup(key);
                if (muscularGroup != null)
                {
                    var viewModel = new MuscularGroupViewModel();
                    viewModel.Id = muscularGroup.Id;
                    viewModel.Name = muscularGroup.Name;
                    return View(viewModel);
                }
            }

            return RedirectToAction("Index");
        }

        // Edit a muscular group
        // POST: /Admin/MuscularGroup/Edit
        [HttpPost]
        public IActionResult Edit(MuscularGroupViewModel viewModel)
        {
            if (ModelState.IsValid && viewModel.Id > 0)
            {
                // Verify not exist on id
                var manager = new MuscularGroupManager(_dbContext);
                var key = new MuscularGroupKey() { Id = viewModel.Id };
                var muscularGroup = manager.GetMuscularGroup(key);
                if (muscularGroup != null)
                {
                    muscularGroup.Name = viewModel.Name;
                    muscularGroup = manager.UpdateMuscularGroup(muscularGroup);
                    return RedirectToAction("Index");
                }
            }

            return View(viewModel);
        }

        // Delete muscular group
        // GET: /Admin/MuscularGroup/Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id != 0)
            {
                var manager = new MuscularGroupManager(_dbContext);
                var key = new MuscularGroupKey() { Id = id };
                var muscularGroup = manager.GetMuscularGroup(key);
                if (muscularGroup != null)
                {
                    manager.DeleteMuscularGroup(muscularGroup);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
