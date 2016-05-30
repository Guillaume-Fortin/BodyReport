using BodyReport.Data;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Message;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace BodyReport.Services
{
    public class UserProfileService
    {
        ApplicationDbContext _dbContext = null;
        IHostingEnvironment _env = null;

        public UserProfileService(ApplicationDbContext dbContext, IHostingEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        public string GetImageUserProfileRelativeURL(string userId)
        {
            var userManager = new UserManager(_dbContext);
            var user = userManager.GetUser(new UserKey() { Id = userId });

            if (user != null)
            {
                string imagePath = Path.Combine(_env.WebRootPath, "images", "userprofil", userId);
                string[] imageExts = new string[] { ".png", ".jpg", ".bmp" };
                foreach (string imageExt in imageExts)
                {
                    if (System.IO.File.Exists(imagePath + imageExt))
                    {
                        return string.Format("/images/userprofil/{0}{1}", userId, imageExt);
                    }
                }
            }
            return null;
        }
    }
}
