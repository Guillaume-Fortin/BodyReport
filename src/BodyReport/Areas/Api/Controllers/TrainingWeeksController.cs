using System;
using Microsoft.AspNet.Mvc;
using BodyReport.Models;
using BodyReport.Manager;
using Message;
using System.Collections.Generic;
using System.Security.Claims;

namespace BodyReport.Areas.Api.Controllers
{
	//[Authorize(Roles = "Admin")]
	//[AllowAnonymous]
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

		// Get api/Translations/Find
		[HttpGet]
		public List<TrainingWeek> Find()
		{
			string userId = User.GetUserId ();
			var searchCriteria = new TrainingWeekCriteria() { UserId = new StringCriteria() { EqualList = new List<string>() { userId } } };
			return _manager.FindTrainingWeek (searchCriteria, false);
		}
	}
}

