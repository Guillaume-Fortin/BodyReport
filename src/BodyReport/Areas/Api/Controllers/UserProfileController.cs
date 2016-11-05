using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using BodyReport.Message.Web;
using BodyReport.Framework;
using BodyReport.Models;
using BodyReport.Services;
using Microsoft.AspNetCore.Authorization;
using BodyReport.ServiceLayers.Interfaces;

namespace BodyReport.Areas.Api.Controllers
{
    [Area("Api")]
    [Authorize]
    public class UserProfileController : MvcController
    {
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;

        public UserProfileController(UserManager<ApplicationUser> userManager,
                                     IHostingEnvironment env) : base(userManager)
        {
            _env = env;
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
                string userId = SessionUserId;
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
