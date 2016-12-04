using BodyReport.Framework;
using BodyReport.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.Report.Controllers
{
    [Area("Report")]
    [Authorize]
    public class ReportController : MvcController
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(ReportController));
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;

        public ReportController(UserManager<ApplicationUser> userManager, IHostingEnvironment env) : base(userManager)
        {
            _env = env;
        }

        //
        // GET: /Report/Report/Pdf
        public IActionResult Pdf(string reportPath, string reportUrl, string outputFileName)
        {
            string virtualPath = "report";
            string reportRootPath = Path.Combine(_env.WebRootPath, "report");
            if (!Directory.Exists(reportRootPath))
                Directory.CreateDirectory(reportRootPath);

            string path;
            if (string.IsNullOrWhiteSpace(reportPath))
                path = reportRootPath;
            else
            {
                path = Path.Combine(reportRootPath, reportPath);
                virtualPath = Path.Combine(virtualPath, reportPath);
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string[] files = Directory.GetFiles(reportRootPath, "phantomjs.*");
            if (files == null || files.Length == 0)
            {
                _logger.LogCritical("phantomjs not found at path : " + reportRootPath);
                return NotFound();
            }
            
            if (!System.IO.File.Exists(Path.Combine(reportRootPath, "rasterize.js")))
            {
                _logger.LogCritical("rasterize.js not found at path : " + reportRootPath);
                return NotFound();
            }

            string phantomJSPath = Path.Combine(reportRootPath, "phantomjs");
            string fileName = Guid.NewGuid().ToString() + ".pdf";
            var filepath = Path.Combine(path, fileName);
            ProcessStartInfo psi = new ProcessStartInfo()
            {
                WorkingDirectory = reportRootPath,
                FileName = phantomJSPath,
                Arguments = string.Format("rasterize.js \"{0}\" \"{1}\" A4", reportUrl, filepath)
            };
            var process = Process.Start(psi);
            if (process != null)
            {
                process.WaitForExit();
                
                if(System.IO.File.Exists(filepath))
                {
                    return File(Path.Combine(virtualPath, fileName), "application/pdf", outputFileName);
                }
            }
            return NotFound();
        }
    }
}
