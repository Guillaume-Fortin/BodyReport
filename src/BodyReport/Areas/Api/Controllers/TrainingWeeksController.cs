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
using Message.WebApi;
using BodyReport.Services;
using BodyReport.Framework.Exceptions;
using BodyReport.Resources;

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

        // Get api/TrainingWeeks/Get
        [HttpGet]
        public IActionResult Get(TrainingWeekKey trainingWeekKey)
        {
            try
            {
                if (trainingWeekKey == null)
                    return HttpBadRequest();
                var trainingWeek = _manager.GetTrainingWeek(trainingWeekKey, false);
                return new HttpOkObjectResult(trainingWeek);
            }
            catch(Exception exception)
            {
                return HttpBadRequest(new WebApiException("Error", exception));
            }
        }

        // Get api/TrainingWeeks/Find
        [HttpGet]
		public IActionResult Find()
		{
            try
            {
			    string userId = User.GetUserId ();
			    var searchCriteria = new TrainingWeekCriteria() { UserId = new StringCriteria() { EqualList = new List<string>() { userId } } };
			    return new HttpOkObjectResult(_manager.FindTrainingWeek (searchCriteria, false));
            }
            catch (Exception exception)
            {
                return HttpBadRequest(new WebApiException("Error", exception));
            }
        }

        // Post api/TrainingWeeks/Create
        [HttpPost]
        public IActionResult Create([FromBody]TrainingWeek trainingWeek)
        {
            try
            {
                if(trainingWeek == null && trainingWeek.UserId != User.GetUserId())
                    return HttpBadRequest();
            
                var result = _manager.CreateTrainingWeek(trainingWeek);
                return new HttpOkObjectResult(result);
            }
            catch (Exception exception)
            {
                return HttpBadRequest(new WebApiException("Error", exception));
            }
        }

        // Post api/TrainingWeeks/Update
        [HttpPost]
        public IActionResult Update([FromBody]TrainingWeek trainingWeek)
		{
            try
            {
			    if (trainingWeek == null || trainingWeek.UserId != User.GetUserId ())
			        return HttpBadRequest();
                var result = _manager.UpdateTrainingWeek(trainingWeek);
			    return new HttpOkObjectResult(result);
            }
            catch (Exception exception)
            {
                return HttpBadRequest(new WebApiException("Error", exception));
            }
        }

        // Post api/TrainingWeeks/DeleteByKey
        [HttpPost]
        public IActionResult DeleteByKey([FromBody]TrainingWeekKey trainingWeekKey)
        {
            try
            {
                if (trainingWeekKey == null )
                    return HttpBadRequest();
            
                if (trainingWeekKey.UserId != User.GetUserId() || string.IsNullOrWhiteSpace(trainingWeekKey.UserId) || trainingWeekKey.Year == 0 || trainingWeekKey.WeekOfYear == 0)
                    return HttpBadRequest();
            
                var trainingWeekManager = new TrainingWeekManager(_dbContext);
                var trainingWeek = trainingWeekManager.GetTrainingWeek(trainingWeekKey, false);
                if (trainingWeek == null)
                    return HttpNotFound();

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
            catch (Exception exception)
            {
                return HttpBadRequest(new WebApiException("Error", exception));
            }
        }

        // Post api/TrainingWeeks/Copy
        [HttpPost]
        public IActionResult Copy([FromBody]CopyTrainingWeek copyTrainingWeek)
        {
            if (copyTrainingWeek == null)
                return HttpBadRequest();

            try
            {
                var service = new TrainingWeekService(_dbContext);
                TrainingWeek trainingWeek;
                if (!service.CopyTrainingWeek(User.GetUserId(), copyTrainingWeek, out trainingWeek))
                    return new HttpOkResult();

                return new HttpOkObjectResult(trainingWeek);
            }
            catch (Exception exception) when (exception is ErrorException || exception is Exception)
            {
                return HttpBadRequest(new WebApiException("Error", exception));
            }
        }
    }
}

