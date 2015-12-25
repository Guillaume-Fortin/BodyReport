using BodyReport.Framework;
using BodyReport.Models;
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
using Microsoft.AspNet.Http;
using System.IO;
using BodyReport.Areas.User.ViewModels;

namespace BodyReport.Areas.User.Controllers
{
    [Authorize(Roles = "User,Admin")]
    [Area("User")]
    public class ProfileController : Controller
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(ProfileController));
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;

        public ProfileController(ApplicationDbContext dbContext, IHostingEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        //
        // GET: /User/Profile/Index
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
                    viewModel.CountryId = userInfo.CountryId;

                    if (userInfo.CountryId == 0)
                        ViewBag.City = Translation.NOT_SPECIFIED;
                    else
                    {
                        if (string.IsNullOrEmpty(userInfo.ZipCode))
                        {
                            ViewBag.City = Translation.NOT_SPECIFIED;
                        }
                        else
                        {
                            var city = (new CityManager(_dbContext)).GetCity(new CityKey() { CountryId = userInfo.CountryId, ZipCode = userInfo.ZipCode });
                            ViewBag.City = city == null ? Translation.NOT_SPECIFIED : city.Name;
                        }
                    }

                    var countryManager = new CountryManager(_dbContext);
                    var country = countryManager.GetCountry(new CountryKey() { Id = userInfo.CountryId });
                    ViewBag.Country = country == null ? Translation.NOT_SPECIFIED : country.Name;
                    viewModel.ImageUrl = ImageUtils.GetImageUrl(_env.WebRootPath, "userprofil", viewModel.UserId + ".png");
                }
            }

            return View(viewModel);
        }
        
        //
        // GET: /User/Profile/Edit
        [HttpGet]
        public IActionResult Edit()
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
                    viewModel.CountryId = userInfo.CountryId;
                    viewModel.ImageUrl = ImageUtils.GetImageUrl(_env.WebRootPath, "userprofil", userInfo.UserId + ".png");
                }

                ViewBag.Sex = ControllerUtils.CreateSelectSexItemList(viewModel.SexId);

                var countryManager = new CountryManager(_dbContext);
                ViewBag.Countries = ControllerUtils.CreateSelectCountryItemList(countryManager.FindCountries(), viewModel.CountryId);

                ViewBag.Units = ControllerUtils.CreateSelectUnitItemList(viewModel.Unit);
                return View(viewModel);
            }
            
            return RedirectToAction("Index");
        }

        //
        // POST: /User/Profile/Edit
        [HttpPost]
        public IActionResult Edit(UserProfilViewModel viewModel, IFormFile imageFile)
        {
            var countryManager = new CountryManager(_dbContext);
            int sexId = 0, countryId = 0;
            if (ModelState.IsValid && User.IsSignedIn() && viewModel != null)
            {
                if (viewModel.UserId == User.GetUserId())
                {
                    if (viewModel.CountryId == 0) // not specified
                    {
                        viewModel.ZipCode = string.Empty;
                    }
                    bool continute = true;
                    if (sexId < 0 && sexId > 1)
                    {
                        ModelState.AddModelError(string.Empty, string.Format("{0} {1}", Translation.INVALID_INPUT_2P, Translation.SEX));
                        viewModel.SexId = 0;
                        continute = false;
                    }

                    if (continute && viewModel.CountryId != 0)
                    {
                        var country = countryManager.GetCountry(new CountryKey() { Id = viewModel.CountryId });
                        if (country == null)
                        {
                            ModelState.AddModelError(string.Empty, string.Format("{0} {1}", Translation.INVALID_INPUT_2P, Translation.COUNTRY));
                            viewModel.CountryId = 0;
                            continute = false;
                        }
                    }

                    if (continute && !string.IsNullOrEmpty(viewModel.ZipCode))
                    { // ZipCode not Required
                        var cityManager = new CityManager(_dbContext);
                        var city = cityManager.GetCity(new CityKey() { CountryId = viewModel.CountryId, ZipCode = viewModel.ZipCode });
                        if (city == null)
                        {
                            ModelState.AddModelError(string.Empty, string.Format("{0} {1}", Translation.INVALID_INPUT_2P, Translation.ZIP_CODE));
                            continute = false;
                        }
                    }

                    sexId = viewModel.SexId;
                    countryId = viewModel.CountryId;

                    if (continute)
                    {
                        var userInfoManager = new UserInfoManager(_dbContext);
                        var userInfo = new UserInfo()
                        {
                            UserId = viewModel.UserId,
                            Unit = (TUnitType)viewModel.Unit,
                            Height = viewModel.Height,
                            Weight = viewModel.Weight,
                            ZipCode = viewModel.ZipCode,
                            CountryId = viewModel.CountryId,
                            Sex = (TSexType)viewModel.SexId
                        };

                        userInfo = userInfoManager.UpdateUserInfo(userInfo);

                        if (!string.IsNullOrWhiteSpace(userInfo.UserId) && ImageUtils.CheckUploadedImageIsCorrect(imageFile))
                        {
                            ImageUtils.SaveImage(imageFile, Path.Combine(_env.WebRootPath, "images", "userprofil"), userInfo.UserId + ".png");
                        }

                        return RedirectToAction("Index");
                    }
                }
            }

            ViewBag.Sex = ControllerUtils.CreateSelectSexItemList(sexId);
            ViewBag.Countries = ControllerUtils.CreateSelectCountryItemList(countryManager.FindCountries(), countryId);
            ViewBag.Units = ControllerUtils.CreateSelectUnitItemList(viewModel.Unit);
            return View(viewModel);
        }
    }
}
