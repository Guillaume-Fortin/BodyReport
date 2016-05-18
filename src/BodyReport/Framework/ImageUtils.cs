using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;

namespace BodyReport.Framework
{
    public static class ImageUtils
    {
        public static bool CheckUploadedImageIsCorrect(IFormFile imageFile, string forceExtName=null)
        {
            if (imageFile != null)
            {
                // Treat upload image
                double fileSizeKo = imageFile.Length / (double)1024;
                if (fileSizeKo <= 2000)
                { // Accept little file image <= 2Mo
                    if (string.IsNullOrWhiteSpace(forceExtName))
                    {
                        switch (imageFile.ContentType)// Accept only png, bmp, jpeg, jpg file
                        {
                            case "image/png":
                            case "image/bmp":
                            case "image/jpeg":
                                return true;
                        }
                    }
                    else
                    {
                        if (imageFile.ContentType == "image/png" && forceExtName == "png")
                            return true;
                        else if (imageFile.ContentType == "image/bmp" && forceExtName == "bmp")
                            return true;
                        else if (imageFile.ContentType == "image/jpeg" && (forceExtName == "jpeg" || forceExtName == "jpg"))
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

                DeleteImagesWithDifferentExtension(rootPath, imageName);

                var filePath = Path.Combine(rootPath, imageName);
                imageFile.SaveAsAsync(filePath).Wait();
            }
        }

        public static string GetImageUrl(string rootPath, string module, string imageName)
        {
            if (!System.IO.File.Exists(Path.Combine(rootPath, "images", module, imageName)))
            {
                return null;
            }
            else
            {
                return string.Format("/images/{0}/{1}", module, imageName);
            }
        }

        public static void DeleteImagesWithDifferentExtension(string path, string imageName)
        {
            string[] files = Directory.GetFiles(path, Path.GetFileNameWithoutExtension(imageName) + ".*");
            if(files != null && files.Length > 0)
            {
                foreach (var file in files)
                    File.Delete(file);
            }
        }

        public static string GetImageExtension(IFormFile imageFile)
        {
            switch (imageFile.ContentType)// Accept only png, bmp, jpeg, jpg file
            {
                case "image/png":
                    return ".png";
                case "image/bmp":
                    return ".bmp";
                case "image/jpeg":
                    return ".jpg";
            }
            return null;
        }
    }
}
