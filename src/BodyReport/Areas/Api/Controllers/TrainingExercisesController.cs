using BodyReport.Data;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Message;
using BodyReport.Message.WebApi;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BodyReport.Areas.Api.Controllers
{
    [Area("Api")]
    [Authorize]
    public class TrainingExercisesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        /// <summary>
		/// Database db context
		/// </summary>
		ApplicationDbContext _dbContext = null;
        TrainingExerciseManager _manager = null;

        public TrainingExercisesController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _manager = new TrainingExerciseManager(_dbContext);
        }

        // Post api/TrainingWeeks/DeleteByKey
        [HttpPost]
        public IActionResult DeleteByKey([FromBody]TrainingExerciseKey trainingExerciseKey)
        {
            try
            {
                if (trainingExerciseKey == null)
                    return BadRequest();

                if (trainingExerciseKey.UserId != _userManager.GetUserId(User) || 
                    string.IsNullOrWhiteSpace(trainingExerciseKey.UserId) || 
                    trainingExerciseKey.Year == 0 || trainingExerciseKey.WeekOfYear == 0 ||
                    trainingExerciseKey.DayOfWeek < 0 || trainingExerciseKey.DayOfWeek > 6 || trainingExerciseKey.TrainingDayId == 0 || 
                    trainingExerciseKey.Id == 0)
                    return BadRequest();

                var trainingExercise = _manager.GetTrainingExercise(trainingExerciseKey);
                if (trainingExercise == null)
                    return NotFound();

                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        _manager.DeleteTrainingExercise(trainingExercise);
                        transaction.Commit();
                    }
                    catch (Exception exception)
                    {
                        //_logger.LogCritical("Unable to delete training week", exception);
                        transaction.Rollback();
                        throw exception;
                    }
                }
                return new OkResult();
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }
    }
}
