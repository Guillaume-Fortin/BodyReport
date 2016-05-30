using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BodyReport.Message;
using BodyReport.Message.WebApi;
using BodyReport.Message.WebApi.MultipleParameters;
using BodyReport.Framework.Exceptions;
using BodyReport.Models;
using BodyReport.Manager;
using BodyReport.Services;
using BodyReport.Data;

namespace BodyReport.Areas.Api.Controllers
{
	[Area("Api")]
	public class TrainingWeeksController : Controller
	{
        private readonly UserManager<ApplicationUser> _userManager;
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
		TrainingWeekManager _manager = null;

		public TrainingWeeksController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
		{
            _userManager = userManager;
			_dbContext = dbContext;
			_manager = new TrainingWeekManager(_dbContext);
		}

        // Get api/TrainingWeeks/Get
        [HttpGet]
        public IActionResult Get(TrainingWeekKey trainingWeekKey, Boolean manageDay = false)
        {
            try
            {
                if (trainingWeekKey == null)
                    return BadRequest();
                var trainingWeekScenario = new TrainingWeekScenario() { ManageTrainingDay = manageDay };
                if (manageDay)
                {
                    trainingWeekScenario.TrainingDayScenario = new TrainingDayScenario() { ManageExercise = true };
                }
                var trainingWeek = _manager.GetTrainingWeek(trainingWeekKey, trainingWeekScenario);
                return new OkObjectResult(trainingWeek);
            }
            catch(Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // Post api/TrainingWeeks/Find
        [HttpPost]
        public IActionResult Find([FromBody]TrainingWeekFinder trainingWeekFinder)
		{
            try
            {
                if (trainingWeekFinder == null)
                    return BadRequest();

                var trainingWeekCriteriaList = trainingWeekFinder.TrainingWeekCriteriaList;
                var trainingWeekScenario = trainingWeekFinder.TrainingWeekScenario;

                if (trainingWeekCriteriaList == null)
                    return BadRequest();
                return new OkObjectResult(_manager.FindTrainingWeek (trainingWeekCriteriaList, trainingWeekScenario));
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // Post api/TrainingWeeks/Create
        [HttpPost]
        public IActionResult Create([FromBody]TrainingWeek trainingWeek)
        {
            try
            {
                if(trainingWeek == null || trainingWeek.UserId != _userManager.GetUserId(User))
                    return BadRequest();
            
                var result = _manager.CreateTrainingWeek(trainingWeek);
                return new OkObjectResult(result);
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // Post api/TrainingWeeks/Update
        [HttpPost]
        public IActionResult Update([FromBody]TrainingWeekWithScenario trainingWeekWithScenario)
		{
            try
            {
                if (trainingWeekWithScenario == null || trainingWeekWithScenario.TrainingWeek == null ||
                    trainingWeekWithScenario.TrainingWeekScenario == null)
                    return BadRequest();

                var trainingWeek = trainingWeekWithScenario.TrainingWeek;
                var trainingWeekScenario = trainingWeekWithScenario.TrainingWeekScenario;

                if(trainingWeek.UserId != _userManager.GetUserId(User))
			        return BadRequest();

                var result = _manager.UpdateTrainingWeek(trainingWeek, trainingWeekScenario);
			    return new OkObjectResult(result);
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // Post api/TrainingWeeks/DeleteByKey
        [HttpPost]
        public IActionResult DeleteByKey([FromBody]TrainingWeekKey trainingWeekKey)
        {
            try
            {
                if (trainingWeekKey == null )
                    return BadRequest();
            
                if (trainingWeekKey.UserId != _userManager.GetUserId(User) || string.IsNullOrWhiteSpace(trainingWeekKey.UserId) || trainingWeekKey.Year == 0 || trainingWeekKey.WeekOfYear == 0)
                    return BadRequest();
            
                var trainingWeekManager = new TrainingWeekManager(_dbContext);
                var trainingWeekScenario = new TrainingWeekScenario() { ManageTrainingDay = false };
                var trainingWeek = trainingWeekManager.GetTrainingWeek(trainingWeekKey, trainingWeekScenario);
                if (trainingWeek == null)
                    return NotFound();

                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        trainingWeekManager.DeleteTrainingWeek(trainingWeek);
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

        // Post api/TrainingWeeks/Copy
        [HttpPost]
        public IActionResult Copy([FromBody]CopyTrainingWeek copyTrainingWeek)
        {
            if (copyTrainingWeek == null)
                return BadRequest();

            try
            {
                var service = new TrainingWeekService(_dbContext);
                TrainingWeek trainingWeek;
                if (!service.CopyTrainingWeek(_userManager.GetUserId(User), copyTrainingWeek, out trainingWeek))
                    return new OkResult();

                return new OkObjectResult(trainingWeek);
            }
            catch (Exception exception) when (exception is ErrorException || exception is Exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }
    }
}

