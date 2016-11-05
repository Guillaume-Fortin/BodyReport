using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using BodyReport.Message;
using BodyReport.Framework;
using BodyReport.Models;
using BodyReport.ViewModels.Admin;
using Microsoft.AspNetCore.Hosting;
using BodyReport.ServiceLayers.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BodyReport.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class RoleController : MvcController
    {
        /// <summary>
        /// Service layer users
        /// </summary>
        private readonly IUsersService _usersService;
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(RoleController));
        /// <summary>
        /// Hosting Environement
        /// </summary>
        private readonly IHostingEnvironment _env = null;

        public RoleController(IHostingEnvironment env, 
                              UserManager<ApplicationUser> userManager,
                              IUsersService usersService) : base(userManager)
        {
            _env = env;
            _usersService = usersService;
        }

        //
        // GET: /Admin/Role/Index
        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            var result = new List<RoleViewModel>();
            var roles = _usersService.FindRoles();
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
                var role = new Role() { Name = roleViewModel.Name, NormalizedName = roleViewModel.NormalizedName };
                role = _usersService.CreateRole(role);
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
                var key = new RoleKey() { Id = id };
                var role = _usersService.GetRole(key);
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
                var key = new RoleKey() { Id = viewModel.Id };
                var role = _usersService.GetRole(key);
                if (role != null)
                {
                    role.Name = viewModel.Name;
                    role.NormalizedName = viewModel.NormalizedName;
                    role = _usersService.UpdateRole(role);
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
                var key = new RoleKey() { Id = id };
                var role = _usersService.GetRole(key);
                if (role != null)
                {
                    _usersService.DeleteRole(key);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
