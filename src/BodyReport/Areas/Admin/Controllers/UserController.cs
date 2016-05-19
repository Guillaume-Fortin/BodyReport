using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
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
    public class UserController : Controller
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(UserController));
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;

        public UserController(ApplicationDbContext dbContext, IHostingEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        // manage users
        // GET: /Admin/User/Index
        [HttpGet]
        public IActionResult Index(SearchUserViewModel searchUserViewModel = null, int currentPage = 1)
        {
            if (searchUserViewModel == null)
                searchUserViewModel = new SearchUserViewModel();

            if (currentPage < 1 || currentPage > 999)
            {
                return View(searchUserViewModel);
            }
            UserViewModel userViewModel;
            const int pageSize = 10;
            int currentRecordIndex = (currentPage - 1) * pageSize;
            var userViewModels = new List<UserViewModel>();
            var manager = new UserManager(_dbContext);

            UserCriteria userCriteria = null;
            if (!string.IsNullOrWhiteSpace(searchUserViewModel.UserName))
            {
                userCriteria = new UserCriteria();
                userCriteria.UserName = new StringCriteria();
                userCriteria.UserName.StartsWithList = new List<string>() { searchUserViewModel.UserName };
            }

            int totalRecords;
            var users = manager.FindUsers(out totalRecords, userCriteria, true, currentRecordIndex, pageSize);
            if (users != null)
            {
                foreach (var user in users)
                {
                    userViewModel = new UserViewModel()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        RegistrationDate = user.RegistrationDate,
                        LastLoginDate = user.LastLoginDate,
                        Suspended = user.Suspended
                    };
                    if (user.Role != null)
                    {
                        userViewModel.RoleId = user.Role.Id;
                        userViewModel.RoleName = user.Role.Name;
                    }
                    userViewModels.Add(userViewModel);
                }
            }

            ViewBag.Users = userViewModels;
            ViewBag.CurrentPage = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = totalRecords;

            //Security
            if (currentRecordIndex > totalRecords)
            {
                ViewBag.CurrentPage = 1;
            }

            return View(searchUserViewModel);
        }

        // Edit a user
        // GET: /Admin/User/Edit
        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var manager = new UserManager(_dbContext);
                var key = new UserKey() { Id = id };
                var user = manager.GetUser(key);
                if (user != null)
                {
                    var viewModel = new UserViewModel()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        RegistrationDate = user.RegistrationDate,
                        LastLoginDate = user.LastLoginDate,
                        Suspended = user.Suspended
                    };
                    if (user.Role != null)
                        viewModel.RoleId = user.Role.Id;

                    // Populate SelectListItem of roles
                    ViewBag.Roles = ControllerUtils.CreateSelectRoleItemList(manager.FindRoles(), user.Id);
                    return View(viewModel);
                }
            }

            return RedirectToAction("Index");
        }

        // Edit a role
        // POST: /Admin/User/Edit
        [HttpPost]
        public IActionResult Edit(UserViewModel viewModel)
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
                    user.Suspended = viewModel.Suspended;

                    //Verify role exist
                    var roleKey = new RoleKey();
                    roleKey.Id = viewModel.RoleId;
                    var role = manager.GetRole(roleKey);
                    if (role != null)
                    {
                        user.Role = role;
                        user = manager.UpdateUser(user);
                        return RedirectToAction("Index");
                    }
                }
            }

            // Populate SelectListItem of roles
            ViewBag.Roles = ControllerUtils.CreateSelectRoleItemList(manager.FindRoles(), viewModel.Id);
            return View(viewModel);
        }

        //
        // GET: /Admin/User/Suspend
        [HttpGet]
        public IActionResult Suspend(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var manager = new UserManager(_dbContext);
                var key = new UserKey() { Id = id };
                var user = manager.GetUser(key);
                if (user != null)
                {
                    user.Suspended = true;
                    manager.UpdateUser(user);
                }
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /Admin/User/Activate
        [HttpGet]
        public IActionResult Activate(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var manager = new UserManager(_dbContext);
                var key = new UserKey() { Id = id };
                var user = manager.GetUser(key);
                if (user != null)
                {
                    user.Suspended = false;
                    manager.UpdateUser(user);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
