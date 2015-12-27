using BodyReport.Areas.User.ViewModels;
using BodyReport.Framework;
using BodyReport.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.User.Controllers
{
    [Authorize(Roles = "User,Admin")]
    [Area("User")]
    public class TrainingJournalController : Controller
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(TrainingJournalController));
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;

        public TrainingJournalController(ApplicationDbContext dbContext, IUrlHelper urlHelper)
        {
            _dbContext = dbContext;
        }

        //
        // GET: /User/TrainingJournal/Index
        [HttpGet]
        public IActionResult Index(int? dayOfWeekSelected)
        {
            DayOfWeek currentDayOfWeek = DateTime.Now.DayOfWeek; // TODO Manage Time with world position of user

            if (dayOfWeekSelected.HasValue && dayOfWeekSelected >= 0 && dayOfWeekSelected <= 6)
                currentDayOfWeek = (DayOfWeek)dayOfWeekSelected.Value; // TODO Manage Time with world position of user

            var trainingJournalViewModel = new TrainingJournalViewModel();
            trainingJournalViewModel.Year = 2016;
            trainingJournalViewModel.WeekOfYear = 1;
            trainingJournalViewModel.UserId = "123123123123123";
            trainingJournalViewModel.TrainingJournalDays = new List<TrainingJournalDayViewModel>()
            {
                new TrainingJournalDayViewModel() { UserId = "123123123123123", DayOfWeek = (int)DayOfWeek.Monday},
                new TrainingJournalDayViewModel() { UserId = "123123123123123", DayOfWeek = (int)DayOfWeek.Tuesday},
                new TrainingJournalDayViewModel() { UserId = "123123123123123", DayOfWeek = (int)DayOfWeek.Thursday },
                new TrainingJournalDayViewModel() { UserId = "123123123123123", DayOfWeek = (int)DayOfWeek.Friday },
                new TrainingJournalDayViewModel() { UserId = "123123123123123", DayOfWeek = (int)DayOfWeek.Sunday }
            };

            ViewBag.CurrentDayOfWeek = currentDayOfWeek;
            return View(trainingJournalViewModel);
        }
    }
}
