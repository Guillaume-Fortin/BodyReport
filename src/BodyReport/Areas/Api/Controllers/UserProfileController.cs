using BodyReport.Models;
using Message.WebApi;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using BodyReport.Framework;
using System.IO;
using Microsoft.AspNet.Hosting;
using BodyReport.Manager;
using Message;
using BodyReport.Services;

namespace BodyReport.Areas.Api.Controllers
{
    [Area("Api")]
    public class UserProfileController : Controller
    {
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;

        public UserProfileController(IHostingEnvironment env, ApplicationDbContext dbContext)
        {
            _env = env;
            _dbContext = dbContext;
        }

        //
        // GET: /UserProfile/GetUserProfileImageRelativeUrl
        [HttpGet]
        public IActionResult GetUserProfileImageRelativeUrl(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return HttpBadRequest();

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
                string userId = User.GetUserId();
                if (string.IsNullOrWhiteSpace(userId))
                    return HttpBadRequest();
                else if (!ImageUtils.CheckUploadedImageIsCorrect(imageFile))
                {
                   return HttpBadRequest();
                }
                string ext = ImageUtils.GetImageExtension(imageFile);
                if (string.IsNullOrWhiteSpace(ext))
                    return HttpBadRequest();
                ImageUtils.SaveImage(imageFile, Path.Combine(_env.WebRootPath, "images", "userprofil"), userId + ext);
                string imageRelativeUrl = string.Format("images/userprofil/{0}.png", userId);
                return new HttpOkObjectResult(imageRelativeUrl);
            }
            catch (Exception exception)
            {
                return HttpBadRequest(new WebApiException("Error", exception));
            }
        }
    }
}
