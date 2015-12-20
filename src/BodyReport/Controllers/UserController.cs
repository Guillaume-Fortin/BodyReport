using BodyReport.Framework;
using BodyReport.Models;
using BodyReport.ViewModels.User;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using BodyReport.Manager;
using Message;
using Microsoft.AspNet.Mvc.Rendering;
using BodyReport.Resources;

namespace BodyReport.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class UserController : Controller
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(AdminController));
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

        //
        // GET: /User/Index
        [HttpGet]
        public IActionResult Index()
        {
            var userManager = new UserManager(_dbContext);
            var user = userManager.GetUser(new UserKey() { Id = User.GetUserId() });

            var viewModel = new UserProfilViewModel();
            viewModel.UserId = user.Id;

            if (user != null)
            {
                viewModel.Name = user.Name;
                viewModel.Email = user.Email;

                var userInfoManager = new UserInfoManager(_dbContext);
                var userInfo = userInfoManager.GetUserInfo(new UserInfoKey() { UserId = User.GetUserId() });
                if (userInfo != null)
                {
                    viewModel.SexId = (int)userInfo.Sex;
                    viewModel.Unit = (int)userInfo.Unit;
                    viewModel.Height = userInfo.Height;
                    viewModel.Weight = userInfo.Weight;
                    viewModel.ZipCode = userInfo.ZipCode;
                    viewModel.CityId = userInfo.CityId;

                    var city = GetTemporaryCities().Where(c => c.Id == userInfo.CityId).FirstOrDefault();
                    viewModel.City = city == null ? "" : city.Name;
                }
            }

            return View(viewModel);
        }

        private List<SelectListItem> CreateSelectSexItemList(int sexId)
        {
            var result = new List<SelectListItem>();
            result.Add(new SelectListItem { Text = Translation.MAN, Value = ((int)TSexType.MAN).ToString(), Selected = sexId == (int)TSexType.MAN });
            result.Add(new SelectListItem { Text = Translation.WOMAN, Value = ((int)TSexType.WOMAN).ToString(), Selected = sexId == (int)TSexType.WOMAN });
            return result;
        }

        private List<SelectListItem> CreateSelectUnitItemList(int unitId)
        {
            var result = new List<SelectListItem>();
            result.Add(new SelectListItem { Text = Translation.IMPERIAL, Value = ((int)TUnitType.Imperial).ToString(), Selected = unitId == (int)TUnitType.Imperial });
            result.Add(new SelectListItem { Text = Translation.METRIC, Value = ((int)TUnitType.Metric).ToString(), Selected = unitId == (int)TUnitType.Metric });
            return result;
        }

        private List<SelectListItem> CreateSelectCityItemList(List<City> cityList, int userCityId)
        {
            var result = new List<SelectListItem>();

            foreach (City city in cityList)
            {
                result.Add(new SelectListItem { Text = city.Name, Value = city.Id.ToString(), Selected = userCityId == city.Id });
            }

            return result;
        }
        
        //
        // GET: /User/EditUserProfil
        [HttpGet]
        public IActionResult EditUserProfil()
        {
            var userManager = new UserManager(_dbContext);
            var user = userManager.GetUser(new UserKey() { Id = User.GetUserId() });

            if (user != null)
            {
                var userInfoManager = new UserInfoManager(_dbContext);
                var userInfo = userInfoManager.GetUserInfo(new UserInfoKey() { UserId = User.GetUserId() });

                var viewModel = new UserProfilViewModel();
                viewModel.UserId = user.Id;
                viewModel.Name = user.Name;
                viewModel.Email = user.Email;
                if (userInfo != null)
                {
                    viewModel.SexId = (int)userInfo.Sex;
                    viewModel.Unit = (int)userInfo.Unit;
                    viewModel.Height = userInfo.Height;
                    viewModel.Weight = userInfo.Weight;
                    viewModel.ZipCode = userInfo.ZipCode;
                    viewModel.CityId = userInfo.CityId;
                }
                ViewBag.Sex = CreateSelectSexItemList(viewModel.SexId);
                ViewBag.Cities = CreateSelectCityItemList(GetTemporaryCities(), viewModel.CityId);
                ViewBag.Units = CreateSelectUnitItemList(viewModel.Unit);
                return View(viewModel);
            }
            
            return RedirectToAction("Index");
        }

        private List<City> GetTemporaryCities()
        {
            return new List<City>()
            {
                new City() {Id=0, Name="Undefined" },
                new City() {Id=1, Name="France" },
                new City() {Id=2, Name="USA" },
                new City() {Id=3, Name="England" }
            };
        }

        //
        // GET: /User/EditUserProfil
        [HttpPost]
        public IActionResult EditUserProfil(UserProfilViewModel viewModel)
        {
            int sexId = 0, cityId = 0;
            if (ModelState.IsValid && User.IsSignedIn() && viewModel != null)
            {
                if (viewModel.UserId == User.GetUserId())
                {
                    sexId = viewModel.SexId;
                    cityId = viewModel.CityId;
                    var userInfoManager = new UserInfoManager(_dbContext);
                    var userInfo = new UserInfo()
                    {
                        UserId = viewModel.UserId,
                        Unit = (TUnitType)viewModel.Unit,
                        Height = viewModel.Height,
                        Weight = viewModel.Weight,
                        ZipCode = viewModel.ZipCode,
                        CityId = viewModel.CityId,
                        Sex = (TSexType)viewModel.SexId
                    };

                    userInfoManager.UpdateUserInfo(userInfo);
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Sex = CreateSelectSexItemList(sexId);
            ViewBag.Cities = CreateSelectCityItemList(GetTemporaryCities(), cityId);
            ViewBag.Units = CreateSelectUnitItemList(viewModel.Unit);
            return View(viewModel);
        }
    }
}
