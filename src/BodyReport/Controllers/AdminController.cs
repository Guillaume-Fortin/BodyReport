using BodyReport.Crud.Transformer;
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
    [Authorize(Roles="Admin")]
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
        public IActionResult ManageUsers(SearchUserViewModel searchUserViewModel=null, int currentPage=1)
        {
            if(searchUserViewModel == null)
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
                    userViewModel = new UserViewModel() { Id = user.Id, Name = user.Name, Email = user.Email, Suspended = user.Suspended };
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
        // GET: /Admin/EditUser
        [HttpGet]
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
                    viewModel.Suspended = user.Suspended;
                    if (user.Role != null)
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
                    user.Suspended = viewModel.Suspended;

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

        //
        // GET: /Admin/SuspendUser
        [HttpGet]
        public IActionResult SuspendUser(string id)
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
            return RedirectToAction("ManageUsers");
        }

        //
        // GET: /Admin/ActivateUser
        [HttpGet]
        public IActionResult ActivateUser(string id)
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
            return RedirectToAction("ManageUsers");
        }

        #region manage roles
        //
        // GET: /Admin/ManageRoles
        [HttpGet]
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
        public IActionResult CreateRole(string returnUrl = null)
        {
            return View(new RoleViewModel());

        }

        // Create new role
        // POST: /Admin/CreateRole
        [HttpPost]
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

        #region ManageMuscularGroups

        // manage users
        // GET: /Admin/ManageMuscularGroups
        [HttpGet]
        public IActionResult ManageMuscularGroups()
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
        
        // Create new role
        // GET: /Admin/CreateMuscularGroup
        [HttpGet]
        public IActionResult CreateMuscularGroup()
        {
            return View(new MuscularGroupViewModel());
        }

        // Create new role
        // POST: /Admin/CreateMuscularGroup
        [HttpPost]
        public IActionResult CreateMuscularGroup(MuscularGroupViewModel muscularGroupViewModel)
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

                return RedirectToAction("ManageMuscularGroups");
            }

            return View(muscularGroupViewModel);
        }

        // Edit a Muscular Group
        // GET: /Admin/EditMuscularGroup
        [HttpGet]
        public IActionResult EditMuscularGroup(int id)
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

            return RedirectToAction("ManageMuscularGroups");
        }

        // Edit a role
        // POST: /Admin/EditMuscularGroup
        [HttpPost]
        public IActionResult EditMuscularGroup(MuscularGroupViewModel viewModel)
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
                    return RedirectToAction("ManageMuscularGroups");
                }
            }

            return View(viewModel);
        }

        //
        // GET: /Admin/DeleteMuscularGroup
        [HttpGet]
        public IActionResult DeleteMuscularGroup(int id)
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
            return RedirectToAction("ManageMuscularGroups");
        }
        #endregion

        #region ManageMuscles

        // manage muscles
        // GET: /Admin/ManageMuscles
        [HttpGet]
        public IActionResult ManageMuscles()
        {
            MuscleViewModel muscleViewModel;
            var musculeViewModels = new List<MuscleViewModel>();

            var manager = new MuscleManager(_dbContext);
            var muscles = manager.FindMuscles();
            if (muscles != null)
            {
                foreach (var muscle in muscles)
                {
                    muscleViewModel = new MuscleViewModel() {
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

        private List<SelectListItem> CreateSelectMuscularGroupItemList(List<MuscularGroup> muscularGroupList, int currentId)
        {
            var result = new List<SelectListItem>();

            foreach (MuscularGroup muscularGroup in muscularGroupList)
            {
                result.Add(new SelectListItem { Text = muscularGroup.Name, Value = muscularGroup.Id.ToString(), Selected = currentId == muscularGroup.Id });
            }

            return result;
        }

        // Create
        // GET: /Admin/CreateMuscle
        [HttpGet]
        public IActionResult CreateMuscle()
        {
            var muscularGroupManager = new MuscularGroupManager(_dbContext);
            ViewBag.MuscularGroups = CreateSelectMuscularGroupItemList(muscularGroupManager.FindMuscularGroups(), 0);

            return View(new MuscleViewModel());
        }

        // Create
        // POST: /Admin/CreateMuscle
        [HttpPost]
        public IActionResult CreateMuscle(MuscleViewModel viewModel)
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

                return RedirectToAction("ManageMuscles");
            }

            var muscularGroupManager = new MuscularGroupManager(_dbContext);
            ViewBag.MuscularGroups = CreateSelectMuscularGroupItemList(muscularGroupManager.FindMuscularGroups(), 0);

            return View(viewModel);
        }

        // Edit
        // GET: /Admin/EditMuscle
        [HttpGet]
        public IActionResult EditMuscle(int id)
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
                    ViewBag.MuscularGroups = CreateSelectMuscularGroupItemList(muscularGroupManager.FindMuscularGroups(), viewModel.MuscularGroupId);

                    return View(viewModel);
                }
            }

            return RedirectToAction("ManageMuscles");
        }

        // Edit
        // POST: /Admin/EditMuscle
        [HttpPost]
        public IActionResult EditMuscle(MuscleViewModel viewModel)
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
                    return RedirectToAction("ManageMuscles");
                }
            }

            int muscularGroupId = 0;
            if (viewModel != null)
                muscularGroupId = viewModel.MuscularGroupId;

            var muscularGroupManager = new MuscularGroupManager(_dbContext);
            ViewBag.MuscularGroups = CreateSelectMuscularGroupItemList(muscularGroupManager.FindMuscularGroups(), muscularGroupId);

            return View(viewModel);
        }

        //Delete
        // GET: /Admin/DeleteMuscle
        [HttpGet]
        public IActionResult DeleteMuscle(int id)
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
            return RedirectToAction("ManageMuscles");
        }
        #endregion
    }
}

