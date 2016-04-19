using System;
using Microsoft.AspNet.Mvc;
using BodyReport.Models;
using BodyReport.Manager;
using Message;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNet.Authorization;
using System.Net;
using Microsoft.Data.Entity;

namespace BodyReport.Areas.Api.Controllers
{
	//[Authorize(Roles = "Admin")]
	[AllowAnonymous]
	[Area("Api")]
	public class TrainingWeeksController : Controller
	{
		/// <summary>
		/// Database db context
		/// </summary>
		ApplicationDbContext _dbContext = null;
		TrainingWeekManager _manager = null;

		public TrainingWeeksController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
			_manager = new TrainingWeekManager(_dbContext);
		}

		// Get api/TrainingWeeks/Find
		[HttpGet]
		public List<TrainingWeek> Find()
		{
			string userId = User.GetUserId ();
			var searchCriteria = new TrainingWeekCriteria() { UserId = new StringCriteria() { EqualList = new List<string>() { userId } } };
			return _manager.FindTrainingWeek (searchCriteria, false);
		}

        // Post api/TrainingWeeks/Create
        [HttpPost]
        public IActionResult Create([FromBody]TrainingWeek trainingWeek)
        {
            if(trainingWeek == null && trainingWeek.UserId != User.GetUserId())
                return HttpBadRequest("Oups!");
            
            var result = _manager.CreateTrainingWeek(trainingWeek);
            return new HttpOkObjectResult(result);
        }

        // Post api/TrainingWeeks/Update
        [HttpPost]
        public IActionResult Update([FromBody]TrainingWeek trainingWeek)
		{
           // return HttpBadRequest("Oups!");
            TrainingWeek result = new TrainingWeek();
			if (trainingWeek.UserId == User.GetUserId ())
			{
				result = _manager.UpdateTrainingWeek(trainingWeek);
			}
			return new HttpOkObjectResult(result);
		}

        // Post api/TrainingWeeks/DeleteByKey
        [HttpPost]
        public IActionResult DeleteByKey([FromBody]TrainingWeekKey trainingWeekKey)
        {
            if (trainingWeekKey == null)
                return HttpBadRequest("Oups!");

            if (trainingWeekKey.UserId != User.GetUserId() || string.IsNullOrWhiteSpace(trainingWeekKey.UserId) || trainingWeekKey.Year == 0 || trainingWeekKey.WeekOfYear == 0)
                return HttpBadRequest("Oups!");
            
            var trainingWeekManager = new TrainingWeekManager(_dbContext);
            var trainingWeek = trainingWeekManager.GetTrainingWeek(trainingWeekKey, false);
            if (trainingWeek == null)
                return HttpBadRequest("Oups!");

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
            return new HttpOkResult();
        }
    }
}

