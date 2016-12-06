using BodyReport.Framework;
using BodyReport.Message;
using BodyReport.Message.Web;
using BodyReport.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.Api.Controllers
{
    [Authorize]
    [Area("Api")]
    public class ReportController : MvcController
    {
        public ReportController(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        // POST api/BodyExercises/Create
        [HttpPost]
        public IActionResult TrainingDayReport([FromBody]TrainingDayReport trainingDayReport)
        {
            try
            {
                string outputFileName = string.Format("TrainingDayReport_{0}_{1}_{2}_{3}.pdf", trainingDayReport.Year, trainingDayReport.WeekOfYear,
                                                      trainingDayReport.DayOfWeek,
                                                      trainingDayReport.TrainingDayId.HasValue ? trainingDayReport.TrainingDayId.Value.ToString() : "all");
                string reportPath = System.IO.Path.Combine("trainingDay", trainingDayReport.UserId);
                string reportUrl = string.Format("http://localhost:5000/Report/TrainingDayReport/Index?userId={0}&year={1}&weekOfYear={2}&dayOfWeek={3}&trainingDayId{4}&displayImages={5}&culture={6}&userIdViewer={7}",
                                                 trainingDayReport.UserId, trainingDayReport.Year, trainingDayReport.WeekOfYear,
                                                 trainingDayReport.DayOfWeek, 
                                                 trainingDayReport.TrainingDayId.HasValue ? trainingDayReport.TrainingDayId.Value.ToString() : "null", trainingDayReport.DisplayImages, 
                                                 CultureInfo.CurrentUICulture.ToString(), SessionUserId);
                return RedirectToAction("Pdf", "Report", new { area = "Report", reportPath = reportPath, reportUrl = reportUrl, outputFileName = outputFileName });

                //return new OkObjectResult(result); // BodyExercise
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }
    }
}
