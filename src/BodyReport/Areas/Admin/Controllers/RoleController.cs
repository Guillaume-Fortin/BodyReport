using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Message;
using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.ViewModels.Admin;
using Microsoft.AspNetCore.Hosting;
using BodyReport.Data;

namespace BodyReport.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class RoleController : Controller
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(RoleController));
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;

        public RoleController(ApplicationDbContext dbContext, IHostingEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        //
        // GET: /Admin/Role/Index
        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            var result = new List<RoleViewModel>();

            var manager = new UserManager(_dbContext);
            var roles = manager.FindRoles();
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    result.Add(new RoleViewModel() { Id = role.Id, Name = role.Name, NormalizedName = role.NormalizedName });
                }
            }
            return View(result);
        }

        // Create new role
        // GET: /Admin/Role/Create
        [HttpGet]
        public IActionResult Create(string returnUrl = null)
        {
            return View(new RoleViewModel());

        }

        // Create new role
        // POST: /Admin/Role/Create
        [HttpPost]
        public IActionResult Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var manager = new UserManager(_dbContext);
                var role = new Role() { Name = roleViewModel.Name, NormalizedName = roleViewModel.NormalizedName };
                role = manager.CreateRole(role);
                if (role == null || string.IsNullOrWhiteSpace(role.Id))
                {
                    _logger.LogError("Create new role fail");
                }

                return RedirectToAction("Index");
            }

            return View(roleViewModel);
        }

        // Edit a role
        // GET: /Admin/Role/Edit
        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var manager = new UserManager(_dbContext);
                var key = new RoleKey() { Id = id };
                var role = manager.GetRole(key);
                if (role != null)
                {
                    var viewModel = new RoleViewModel();
                    viewModel.Id = role.Id;
                    viewModel.Name = role.Name;
                    viewModel.NormalizedName = role.NormalizedName;
                    return View(viewModel);
                }
            }
            return RedirectToAction("Index");
        }

        // Edit a role
        // POST: /Admin/Role/Edit
        [HttpPost]
        public IActionResult Edit(RoleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Verify not exist on id
                var manager = new UserManager(_dbContext);
                var key = new RoleKey() { Id = viewModel.Id };
                var role = manager.GetRole(key);
                if (role != null)
                {
                    role.Name = viewModel.Name;
                    role.NormalizedName = viewModel.NormalizedName;
                    role = manager.UpdateRole(role);
                    return RedirectToAction("Index");
                }
            }

            return View(viewModel);
        }

        //
        // GET: /Admin/Role/Delete
        [HttpGet]
        public IActionResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var manager = new UserManager(_dbContext);
                var key = new RoleKey() { Id = id };
                var role = manager.GetRole(key);
                if (role != null)
                {
                    manager.DeleteRole(key);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
