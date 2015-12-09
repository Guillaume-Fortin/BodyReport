using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.ViewModels.Admin;
using Message;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(AdminController));

        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;

        public AdminController(ApplicationDbContext dbContext, IHostingEnvironment env)
        {
            _dbContext = dbContext;
        }

        //
        // GET: /Admin/Index
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string returnUrl = null)
        {
            return View();
        }

        private List<SelectListItem> CreateSelectRoleItemList(List<Role> roleList, string currentUserId)
        {
            var result = new List<SelectListItem>();

            foreach (Role role in roleList)
            {
                result.Add(new SelectListItem { Text = role.Name, Value = role.Id , Selected = currentUserId == role.Id});
            }

            return result;
        }

        // manage users
        // GET: /Admin/ManageUsers
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ManageUsers(string returnUrl = null)
        {
            UserViewModel userViewModel;
            var result = new List<UserViewModel>();
            var manager = new UserManager(_dbContext);
            var users = manager.FindUsers();
            if (users != null)
            {
                foreach (var user in users)
                {
                    userViewModel = new UserViewModel() { Id = user.Id, Name = user.Name, Email = user.Email };
                    if (user.Role != null)
                    {
                        userViewModel.RoleId = user.Role.Id;
                        userViewModel.RoleName = user.Role.Name;
                    }
                    result.Add(userViewModel);
                }
            }

            return View(result);
        }

        // Edit a user
        // GET: /Admin/EditUser
        [HttpGet]
        [AllowAnonymous]
        public IActionResult EditUser(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var manager = new UserManager(_dbContext);
                var key = new UserKey() { Id = id };
                var user = manager.GetUser(key);
                if (user != null)
                {
                    var viewModel = new UserViewModel();
                    viewModel.Id = user.Id;
                    viewModel.Name = user.Name;
                    viewModel.Email = user.Email;
                    if(user.Role != null)
                        viewModel.RoleId = user.Role.Id;

                    // Populate SelectListItem of roles
                    ViewBag.Roles = CreateSelectRoleItemList(manager.FindRoles(), user.Id);
                    return View(viewModel);
                }
            }

            return RedirectToAction("ManageUsers");
        }

        // Edit a role
        // POST: /Admin/EditRole
        [HttpPost]
        [AllowAnonymous]
        public IActionResult EditUser(UserViewModel viewModel, IFormFile imageFile)
        {
            var manager = new UserManager(_dbContext);
            if (ModelState.IsValid)
            {
                // Verify not exist on id
                var key = new UserKey() { Id = viewModel.Id };
                var user = manager.GetUser(key);
                if (user != null)
                {
                    user.Id = viewModel.Id;
                    user.Name = viewModel.Name;
                    user.Email = viewModel.Email;

                    //Verify role exist
                    var roleKey = new RoleKey();
                    roleKey.Id = viewModel.RoleId;
                    var role = manager.GetRole(roleKey);
                    if (role != null)
                    {
                        user.Role = role;
                        user = manager.UpdateUser(user);
                        return RedirectToAction("ManageUsers");
                    }
                }
            }
            
            // Populate SelectListItem of roles
            ViewBag.Roles = CreateSelectRoleItemList(manager.FindRoles(), viewModel.Id);
            return View(viewModel);
        }

        #region manage roles
        //
        // GET: /Admin/ManageRoles
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ManageRoles(string returnUrl = null)
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
        // GET: /Admin/CreateRole
        [HttpGet]
        [AllowAnonymous]
        public IActionResult CreateRole(string returnUrl = null)
        {
            return View(new RoleViewModel());

        }

        // Create new role
        // POST: /Admin/CreateRole
        [HttpPost]
        [AllowAnonymous]
        public IActionResult CreateRole(RoleViewModel roleViewModel)
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

                return RedirectToAction("ManageRoles");
            }

            return View(roleViewModel);
        }

        // Edit a role
        // GET: /Admin/EditRole
        [HttpGet]
        [AllowAnonymous]
        public IActionResult EditRole(string id)
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
            return RedirectToAction("ManageRoles");
        }

        // Edit a role
        // POST: /Admin/EditRole
        [HttpPost]
        [AllowAnonymous]
        public IActionResult EditRole(RoleViewModel viewModel, IFormFile imageFile)
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
                    return RedirectToAction("ManageRoles");
                }
            }

            return View(viewModel);
        }

        //
        // GET: /Admin/DeleteRole
        [HttpGet]
        [AllowAnonymous]
        public IActionResult DeleteRole(string id)
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
            return RedirectToAction("ManageRoles");
        }
        #endregion
    }
}
