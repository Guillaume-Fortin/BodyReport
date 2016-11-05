using BodyReport.Data;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Message;
using BodyReport.Message.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;
using BodyReport.Framework;
using BodyReport.ServiceLayers.Interfaces;

namespace BodyReport.Areas.Api.Controllers
{
    [Area("Api")]
    [Authorize]
    public class TrainingExercisesController : MvcController
    {
        // <summary>
        /// ServiceLayer
        /// </summary>
        ITrainingExercisesService _trainingExercisesService;

        public TrainingExercisesController(UserManager<ApplicationUser> userManager,
                                           ITrainingExercisesService trainingExercisesService) : base(userManager)
        {
            _trainingExercisesService = trainingExercisesService;
        }

        // Post api/TrainingExercises/Delete
        [HttpPost]
        public IActionResult Delete([FromBody]TrainingExerciseKey trainingExerciseKey)
        {
            try
            {
                if (trainingExerciseKey == null)
                    return BadRequest();

                if (trainingExerciseKey.UserId != SessionUserId ||
                    string.IsNullOrWhiteSpace(trainingExerciseKey.UserId) ||
                    trainingExerciseKey.Year == 0 || trainingExerciseKey.WeekOfYear == 0 ||
                    trainingExerciseKey.DayOfWeek < 0 || trainingExerciseKey.DayOfWeek > 6 || trainingExerciseKey.TrainingDayId == 0 ||
                    trainingExerciseKey.Id == 0)
                    return BadRequest();

                _trainingExercisesService.DeleteTrainingExercise(trainingExerciseKey);
                return new OkObjectResult(true); // bool
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }
    }
}
