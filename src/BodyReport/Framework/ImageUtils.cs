using Microsoft.AspNet.Http;
using Microsoft.Net.Http.Headers;
using System.IO;

namespace BodyReport.Framework
{
    public static class ImageUtils
    {
        public static bool CheckUploadedImageIsCorrect(IFormFile imageFile)
        {
            if (imageFile != null)
            {
                // Treat upload image
                double fileSizeKo = imageFile.Length / (double)1024;
                if (fileSizeKo <= 500)
                { // Accept little file image <= 500ko
                    var fileName = ContentDispositionHeaderValue.Parse(imageFile.ContentDisposition).FileName.Trim('"');
                    if (fileName.EndsWith(".png"))// Accept only png file
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void SaveImage(IFormFile imageFile, string rootPath, string imageName)
        {
            if (imageFile != null)
            {
                if (!Directory.Exists(rootPath))
                    Directory.CreateDirectory(rootPath);
                
                var filePath = Path.Combine(rootPath, imageName);
                if (File.Exists(filePath))
                    File.Delete(filePath);
                imageFile.SaveAsAsync(filePath).Wait();
            }
        }

        public static string GetImageUrl(string rootPath, string module, string imageName)
        {
            if (!System.IO.File.Exists(Path.Combine(rootPath, "images", module, imageName)))
            {
                return "/images/unknown.png";
            }
            else
            {
                return string.Format("/images/{0}/{1}", module, imageName);
            }
        }
    }
}
