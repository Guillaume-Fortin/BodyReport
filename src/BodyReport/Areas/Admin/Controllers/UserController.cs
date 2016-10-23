using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using BodyReport.Message;
using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.ViewModels.Admin;
using BodyReport.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using BodyReport.Services;
using System;
using System.Linq;

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
        /// <summary>
        /// IdentityUserManager
        /// </summary>
        private readonly UserManager<ApplicationUser> _identityUserManager;
        /// <summary>
        /// Email sender
        /// </summary>
        private readonly IEmailSender _emailSender;

        public UserController(ApplicationDbContext dbContext, IHostingEnvironment env, UserManager<ApplicationUser> identityUserManager, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _env = env;
            _identityUserManager = identityUserManager;
            _emailSender = emailSender;
        }

        private void ApplyUserSort(ref UserCriteria userCriteria, string sortOrder)
        {
            if (userCriteria == null)
                userCriteria = new UserCriteria();

            userCriteria.FieldSortList = new List<FieldSort>();

            var sortFieldDirection = ControllerUtils.GetSortFieldDirection(sortOrder);
            if (sortFieldDirection != null && sortFieldDirection.Item2 != TFieldSort.None) // need sort value
            {
                userCriteria.FieldSortList.Add(new FieldSort() { Name = sortFieldDirection.Item1, Sort = sortFieldDirection.Item2 });
            }
        }
        
        // manage users
        // GET: /Admin/User/Index
        [HttpGet]
        public async Task<IActionResult> Index(SearchUserViewModel searchUserViewModel = null, int currentPage = 1, string sortOrder = null)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SortPossibilities = new Dictionary<string, string>()
            {
                {"id", "asc"},
                {"username", "asc"},
                {"email", "asc"},
                {"registrationdate", "asc"},
                {"lastlogindate", "asc"}
            };
            ControllerUtils.ManageSortingPossibilities(ViewBag.SortPossibilities, sortOrder);

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
            if (!string.IsNullOrWhiteSpace(searchUserViewModel.UserId))
            {
                if(userCriteria == null)
                    userCriteria = new UserCriteria();
                userCriteria.Id = new StringCriteria();
                userCriteria.Id.StartsWithList = new List<string>() { searchUserViewModel.UserId };
            }
            if (!string.IsNullOrWhiteSpace(searchUserViewModel.UserName))
            {
                if (userCriteria == null)
                    userCriteria = new UserCriteria();
                userCriteria.UserName = new StringCriteria();
                userCriteria.UserName.StartsWithList = new List<string>() { searchUserViewModel.UserName };
            }

            ApplyUserSort(ref userCriteria, sortOrder);

            int totalRecords;
            var users = manager.FindUsers(out totalRecords, userCriteria, true, currentRecordIndex, pageSize);
            if (users != null)
            {
                ApplicationUser appUser;
                bool emailConfirmed;
                foreach (var user in users)
                {
                    appUser = await _identityUserManager.FindByIdAsync(user.Id);
                    emailConfirmed = appUser != null && appUser.EmailConfirmed;

                    userViewModel = new UserViewModel()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        RegistrationDate = user.RegistrationDate,
                        LastLoginDate = user.LastLoginDate,
                        Suspended = user.Suspended,
                        EmailConfirmed = emailConfirmed
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

        //
        // GET: /Admin/User/ConfirmUserEmail
        [HttpGet]
        public async Task<IActionResult> ConfirmUserEmail(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var appUser = await _identityUserManager.FindByIdAsync(id);
                if (appUser != null && !appUser.EmailConfirmed)
                {
                    appUser.EmailConfirmed = true;
                    await _identityUserManager.UpdateAsync(appUser);

                    //Add user role
                    var manager = new UserManager(_dbContext);
                    var userKey = new UserKey() { Id = id };
                    var user = manager.GetUser(userKey);
                    if (user != null)
                    {
                        //Verify role exist
                        var roleKey = new RoleKey();
                        roleKey.Id = "1"; //User
                        var role = manager.GetRole(roleKey);
                        if (role != null)
                        {
                            user.Role = role;
                            user = manager.UpdateUser(user);
                        }
                    }

                    //Add empty user profil (for correct connect error on mobile application)
                    var userInfoManager = new UserInfoManager(_dbContext);
                    var userInfoKey = new UserInfoKey() { UserId = id };
                    var userInfo = userInfoManager.GetUserInfo(userInfoKey);
                    if (userInfo == null)
                    {
                        userInfo = new UserInfo()
                        {
                            UserId = id,
                            Unit = TUnitType.Metric
                        };
                        userInfoManager.UpdateUserInfo(userInfo);
                    }

                    try
                    {
                        await _emailSender.SendEmailAsync(appUser.Email, "Account validated",
                            "Your account validated by admin");
                    }
                    catch(Exception except)
                    {
                        _logger.LogError(0, except, "can't send email");
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}
