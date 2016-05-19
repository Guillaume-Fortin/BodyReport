using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Message.WebApi;
using BodyReport.Framework;
using BodyReport.Models;
using BodyReport.Services;
using BodyReport.Data;

namespace BodyReport.Areas.Api.Controllers
{
    [Area("Api")]
    public class UserProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;

        public UserProfileController(UserManager<ApplicationUser> userManager, IHostingEnvironment env, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _env = env;
            _dbContext = dbContext;
        }

        //
        // GET: /UserProfile/GetUserProfileImageRelativeUrl
        [HttpGet]
        public IActionResult GetUserProfileImageRelativeUrl(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            var userProfileService = new UserProfileService(_dbContext, _env);
            string imageUrl = userProfileService.GetImageUserProfileRelativeURL(userId);
            return new JsonResult(imageUrl);
        }

        //
        // POST: /UserProfile/UploadProfileImage
        [HttpPost]
        public IActionResult UploadProfileImage(IFormFile imageFile)
        {
            try
            {
                string userId = _userManager.GetUserId(User);
                if (string.IsNullOrWhiteSpace(userId))
                    return BadRequest();
                else if (!ImageUtils.CheckUploadedImageIsCorrect(imageFile))
                {
                   return BadRequest();
                }
                string ext = ImageUtils.GetImageExtension(imageFile);
                if (string.IsNullOrWhiteSpace(ext))
                    return BadRequest();
                ImageUtils.SaveImage(imageFile, Path.Combine(_env.WebRootPath, "images", "userprofil"), userId + ext);
                string imageRelativeUrl = string.Format("images/userprofil/{0}.png", userId);
                return new OkObjectResult(imageRelativeUrl);
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }
    }
}
