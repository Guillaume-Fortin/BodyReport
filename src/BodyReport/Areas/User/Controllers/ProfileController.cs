using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using BodyReport.Message;
using BodyReport.Framework;
using BodyReport.Areas.User.ViewModels;
using BodyReport.Services;
using BodyReport.Framework;
using BodyReport.Models;
using BodyReport.Manager;
using BodyReport.Resources;
using BodyReport.Data;
using System;
using System.Collections.Generic;

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
        /// SignInManager Identity
        /// </summary>
        private readonly SignInManager<ApplicationUser> _signInManager;
        /// <summary>
        /// UserManager Identity
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;

        public ProfileController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            IHostingEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
            _env = env;
        }

        //
        // GET: /User/Profile/Index
        [HttpGet]
        public IActionResult Index(string userId)
        {
            string userIdViewer = _userManager.GetUserId(User);
            if(userId == null)
                userId = _userManager.GetUserId(User);
            var userManager = new UserManager(_dbContext);
            var user = userManager.GetUser(new UserKey() { Id = userId });

            var viewModel = new UserProfilViewModel();
            viewModel.UserId = user.Id;

            if (user != null)
            {
                viewModel.Name = user.Name;
                viewModel.Email = user.Email;

                var userInfoManager = new UserInfoManager(_dbContext);
                var userInfoViewer = userInfoManager.GetUserInfo(new UserInfoKey() { UserId = userIdViewer });
                if (userInfoViewer == null)
                    userInfoViewer = new UserInfo();
                var userInfo = userInfoManager.GetUserInfo(new UserInfoKey() { UserId = userId });
                if (userInfo != null)
                {
                    viewModel.SexId = (int)userInfo.Sex;
                    viewModel.Unit = (int)userInfoViewer.Unit; //On viewer Mode, it's viewer unit which display
                    viewModel.Height = Utils.TransformLengthToUnitSytem(userInfo.Unit, userInfoViewer.Unit, userInfo.Height);
                    viewModel.Weight = Utils.TransformWeightToUnitSytem(userInfo.Unit, userInfoViewer.Unit, userInfo.Weight);
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

                    ViewBag.TimeZoneName = userInfo.TimeZoneName;

                    var userProfileService = new UserProfileService(_dbContext, _env);
                    viewModel.ImageUrl = userProfileService.GetImageUserProfileRelativeURL(userInfo.UserId);
                }
            }

            ViewBag.Editable = userIdViewer == userId;
            ViewBag.IsMobileBrowser = Request.IsMobileBrowser();
            return View(viewModel);
        }
        
        //
        // GET: /User/Profile/Edit
        [HttpGet]
        public IActionResult Edit()
        {
            var userManager = new UserManager(_dbContext);
            var user = userManager.GetUser(new UserKey() { Id = _userManager.GetUserId(User) });

            if (user != null)
            {
                var userInfoManager = new UserInfoManager(_dbContext);
                var userInfo = userInfoManager.GetUserInfo(new UserInfoKey() { UserId = _userManager.GetUserId(User) });

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
                    viewModel.TimeZoneName = userInfo.TimeZoneName;

                    var userProfileService = new UserProfileService(_dbContext, _env);
                    viewModel.ImageUrl = userProfileService.GetImageUserProfileRelativeURL(userInfo.UserId);
                }

                ViewBag.Sex = ControllerUtils.CreateSelectSexItemList(viewModel.SexId);

                var countryManager = new CountryManager(_dbContext);
                ViewBag.Countries = ControllerUtils.CreateSelectCountryItemList(countryManager.FindCountries(), viewModel.CountryId);

                ViewBag.Units = ControllerUtils.CreateSelectUnitItemList(viewModel.Unit);

                ViewBag.TimeZones = ControllerUtils.CreateSelectTimeZoneItemList(viewModel.TimeZoneName);

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
            if (ModelState.IsValid && _signInManager.IsSignedIn(User) && viewModel != null)
            {
                if (viewModel.UserId == _userManager.GetUserId(User))
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
                            Sex = (TSexType)viewModel.SexId,
                            TimeZoneName = viewModel.TimeZoneName,
                        };

                        userInfo = userInfoManager.UpdateUserInfo(userInfo);

                        if (!string.IsNullOrWhiteSpace(userInfo.UserId) && ImageUtils.CheckUploadedImageIsCorrect(imageFile))
                        {
                            string ext = ImageUtils.GetImageExtension(imageFile);
                            if (string.IsNullOrWhiteSpace(ext))
                                return BadRequest();
                            ImageUtils.SaveImage(imageFile, Path.Combine(_env.WebRootPath, "images", "userprofil"), userInfo.UserId + ext);
                        }

                        return RedirectToAction("Index");
                    }
                }
            }

            ViewBag.Sex = ControllerUtils.CreateSelectSexItemList(sexId);
            ViewBag.Countries = ControllerUtils.CreateSelectCountryItemList(countryManager.FindCountries(), countryId);
            ViewBag.Units = ControllerUtils.CreateSelectUnitItemList(viewModel.Unit);
            ViewBag.TimeZones = ControllerUtils.CreateSelectTimeZoneItemList(viewModel.TimeZoneName);
            return View(viewModel);
        }
    }
}
