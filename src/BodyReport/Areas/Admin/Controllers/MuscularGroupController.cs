using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using BodyReport.Message;
using BodyReport.Framework;
using BodyReport.ViewModels.Admin;
using BodyReport.Data;
using BodyReport.ServiceLayers.Interfaces;
using BodyReport.Models;
using Microsoft.AspNetCore.Identity;

namespace BodyReport.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class MuscularGroupController : MvcController
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(MuscularGroupController));
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;
        /// <summary>
        /// Service layer
        /// </summary>
        private readonly IMuscularGroupsService _muscularGroupsService;

        public MuscularGroupController(UserManager<ApplicationUser> userManager,
                                       IMuscularGroupsService muscularGroupsService,
                                       IHostingEnvironment env) : base(userManager)
        {
            _muscularGroupsService = muscularGroupsService;
            _env = env;
        }

        // manage muscular group
        // GET: /Admin/MuscularGroup/Index
        [HttpGet]
        public IActionResult Index()
        {
            MuscularGroupViewModel muscularGroupViewModel;
            var muscularGroupsViewModels = new List<MuscularGroupViewModel>();
            var muscularGroups = _muscularGroupsService.FindMuscularGroups();
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
                var muscularGroup = new MuscularGroup() { Name = muscularGroupViewModel.Name };
                muscularGroup = _muscularGroupsService.CreateMuscularGroup(muscularGroup);
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
                var key = new MuscularGroupKey() { Id = id };
                var muscularGroup = _muscularGroupsService.GetMuscularGroup(key);
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
                var key = new MuscularGroupKey() { Id = viewModel.Id };
                var muscularGroup = _muscularGroupsService.GetMuscularGroup(key);
                if (muscularGroup != null)
                {
                    muscularGroup.Name = viewModel.Name;
                    muscularGroup = _muscularGroupsService.UpdateMuscularGroup(muscularGroup);
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
                var key = new MuscularGroupKey() { Id = id };
                var muscularGroup = _muscularGroupsService.GetMuscularGroup(key);
                if (muscularGroup != null)
                {
                    _muscularGroupsService.DeleteMuscularGroup(muscularGroup);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
