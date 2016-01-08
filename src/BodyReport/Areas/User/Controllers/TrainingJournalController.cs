using BodyReport.Areas.User.ViewModels;
using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Models;
using Message;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Globalization;
using BodyReport.Resources;

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

        private DayOfWeek GetCurrentDayOfWeek(int? dayOfWeekSelected)
        {
            DayOfWeek currentDayOfWeek = DateTime.Now.DayOfWeek; // TODO Manage Time with world position of user

            if (dayOfWeekSelected.HasValue && dayOfWeekSelected >= 0 && dayOfWeekSelected <= 6)
                currentDayOfWeek = (DayOfWeek)dayOfWeekSelected.Value; // TODO Manage Time with world position of user

            return currentDayOfWeek;
        }

        //
        // GET: /User/TrainingJournal/Index
        [HttpGet]
        public IActionResult Index(string userId, int year, int weekOfYear, int? dayOfWeekSelected)
        {
            var viewModel = new List<TrainingWeekViewModel>();
            var trainingWeekManager = new TrainingWeekManager(_dbContext);
            
            var searchCriteria = new TrainingWeekCriteria() { UserId = new StringCriteria() { EqualList = new List<string>() { User.GetUserId() } } };
            var trainingWeekList = trainingWeekManager.FindTrainingWeek(searchCriteria, false);

            if(trainingWeekList != null)
            {
                foreach(var trainingWeek in trainingWeekList)
                {
                    viewModel.Add(TransformTrainingWeekToViewModel(trainingWeek));
                }
            }
            
            return View(viewModel);
        }

        
        // Create a training journal
        // GET: /User/TrainingJournal/Create
        [HttpGet]
        public IActionResult Create()
        {
            var userInfoManager = new UserInfoManager(_dbContext);
            var userInfo = userInfoManager.GetUserInfo(new UserInfoKey() { UserId = User.GetUserId() });
            if (userInfo == null)
                return RedirectToAction("Index");
            
            DateTime dateTime = DateTime.Now; // TODO Manage Time with world position of user

            var trainingWeek = new TrainingWeek();
            trainingWeek.UserId = User.GetUserId();
            trainingWeek.Year = dateTime.Year;
            trainingWeek.WeekOfYear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            trainingWeek.UserHeight = userInfo.Height;
            trainingWeek.UserWeight = userInfo.Weight;
            
            ViewBag.UserUnit =  userInfo.Unit;
            return View(TransformTrainingWeekToViewModel(trainingWeek));
        }

        // Create a training journal week
        // GET: /User/TrainingJournal/Create
        [HttpPost]
        public IActionResult Create(TrainingWeekViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(viewModel.UserId) || viewModel.Year == 0 || viewModel.Year == 0 || User.GetUserId() != viewModel.UserId)
                    return View(viewModel);

                //Verify valide week of year
                if (viewModel.WeekOfYear > 0 && viewModel.WeekOfYear <= 52)
                {
                    var trainingWeekManager = new TrainingWeekManager(_dbContext);
                    var trainingWeek = TransformViewModelToTrainingJournal(viewModel);

                    var trainingWeekKey = new TrainingWeekKey() { UserId = trainingWeek.UserId, Year = trainingWeek.Year, WeekOfYear = trainingWeek.WeekOfYear };
                    var existTrainingWeek = trainingWeekManager.GetTrainingWeek(trainingWeekKey, false);

                    if(existTrainingWeek != null)
                    {
                        ModelState.AddModelError(string.Empty, string.Format(Translation.P0_ALREADY_EXIST, Translation.TRAINING_WEEK));
                        return View(viewModel);
                    }

                    //Create data in database
                    trainingWeek = trainingWeekManager.CreateTrainingWeek(trainingWeek);

                    if (trainingWeek == null)
                    {
                        ModelState.AddModelError(string.Empty, Translation.IMPOSSIBLE_TO_CREATE_NEW_TRAINING_JOURNAL);
                        return View(viewModel);
                    }

                    return RedirectToAction("Index");
                }
            }

            return View(viewModel);
        }


        // Edit a training journals
        // GET: /User/TrainingJournal/Edit
        [HttpGet]
        public IActionResult Edit(string userId, int year, int weekOfYear)
        {
            if (string.IsNullOrWhiteSpace(userId) || year == 0 || weekOfYear == 0 || User.GetUserId() != userId)
                return RedirectToAction("Index");

            var trainingJournalManager = new TrainingWeekManager(_dbContext);
            var key = new TrainingWeekKey()
            {
                UserId = userId,
                Year = year,
                WeekOfYear = weekOfYear
            };
            var trainingJournal = trainingJournalManager.GetTrainingWeek(key, true);
            if (trainingJournal == null) // no data found
                return RedirectToAction("Index");

            return View(TransformTrainingWeekToViewModel(trainingJournal));
        }

        // Edit a training journal week
        // GET: /User/TrainingJournal/Create
        [HttpPost]
        public IActionResult Edit(TrainingWeekViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(viewModel.UserId) || viewModel.Year == 0 || viewModel.Year == 0 || User.GetUserId() != viewModel.UserId)
                    return View(viewModel);

                //Verify valide week of year
                if (viewModel.WeekOfYear > 0 && viewModel.WeekOfYear <= 52)
                {
                    var trainingWeekManager = new TrainingWeekManager(_dbContext);
                    var trainingWeek = TransformViewModelToTrainingJournal(viewModel);

                    var trainingWeekKey = new TrainingWeekKey() { UserId = trainingWeek.UserId, Year = trainingWeek.Year, WeekOfYear = trainingWeek.WeekOfYear };
                    var existTrainingWeek = trainingWeekManager.GetTrainingWeek(trainingWeekKey, false);
                    
                    if (existTrainingWeek == null)
                    {
                        ModelState.AddModelError(string.Empty, string.Format(Translation.P0_NOT_EXIST, Translation.TRAINING_WEEK));
                        return View(viewModel);
                    }

                    //Create data in database
                    trainingWeek = trainingWeekManager.UpdateTrainingWeek(trainingWeek);

                    if (trainingWeek == null)
                    {
                        ModelState.AddModelError(string.Empty, string.Format(Translation.IMPOSSIBLE_TO_UPDATE_P0, Translation.TRAINING_JOURNAL));
                        return View(viewModel);
                    }

                    return RedirectToAction("View", new { userId = trainingWeek.UserId, year = trainingWeek.Year, weekOfYear = trainingWeek.WeekOfYear });
                }
            }

            return View(viewModel);
        }

        //
        // GET: /User/TrainingJournal/View
        [HttpGet]
        public IActionResult View(string userId, int year, int weekOfYear, int? dayOfWeekSelected)
        {
            DayOfWeek currentDayOfWeek = GetCurrentDayOfWeek(dayOfWeekSelected);

            var trainingWeekManager = new TrainingWeekManager(_dbContext);

            var trainingWeekKey = new TrainingWeekKey()
            {
                UserId = userId,
                Year = year,
                WeekOfYear = weekOfYear
            };
            var trainingWeek = trainingWeekManager.GetTrainingWeek(trainingWeekKey, true);

            if (trainingWeek == null)
                return RedirectToAction("Index");

            var trainingWeekViewModel = TransformTrainingWeekToViewModel(trainingWeek);
            List<TrainingDayViewModel> trainingDayViewModels = null;

            ViewBag.CurrentDayOfWeek = currentDayOfWeek;
            return View(new Tuple<TrainingWeekViewModel, List<TrainingDayViewModel>>(trainingWeekViewModel, trainingDayViewModels));
        }

        private TrainingWeek TransformViewModelToTrainingJournal(TrainingWeekViewModel viewModel)
        {
            TrainingWeek trainingJournal = new TrainingWeek();

            trainingJournal.UserId = viewModel.UserId;
            trainingJournal.Year = viewModel.Year;
            trainingJournal.WeekOfYear = viewModel.WeekOfYear;
            trainingJournal.UserHeight = viewModel.UserHeight;
            trainingJournal.UserWeight = viewModel.UserWeight;
           
            return trainingJournal;
        }

        private TrainingWeekViewModel TransformTrainingWeekToViewModel(TrainingWeek trainingJournal)
        {
            TrainingWeekViewModel trainingJournalVM = new TrainingWeekViewModel();

            trainingJournalVM.UserId = trainingJournal.UserId;
            trainingJournalVM.Year = trainingJournal.Year;
            trainingJournalVM.WeekOfYear = trainingJournal.WeekOfYear;
            trainingJournalVM.UserHeight = trainingJournal.UserHeight;
            trainingJournalVM.UserWeight = trainingJournal.UserWeight;

            var userManager = new UserManager(_dbContext);
            var user = userManager.GetUser(new UserKey() { Id = trainingJournal.UserId });
            if (user != null)
                trainingJournalVM.UserName = user.Name;
            /*
            trainingJournalVM.TrainingJournalDays = new List<TrainingJournalDayViewModel>();
            if(trainingJournal != null)
            {
                foreach(var trainingJournalDay in trainingJournal.TrainingJournalDays)
                {
                    trainingJournalVM.
                }
            }*/

            return trainingJournalVM;
        }

        private TrainingDayViewModel TransformTrainingDayToViewModel(TrainingDay trainingDay)
        {
            TrainingDayViewModel trainingDayVM = new TrainingDayViewModel()
            {
                UserId = trainingDay.UserId,
                Year = trainingDay.Year,
                WeekOfYear = trainingDay.WeekOfYear,
                DayOfWeek = trainingDay.DayOfWeek,
                TrainingDayId = trainingDay.TrainingDayId,
                BeginHour = trainingDay.BeginHour,
                EndHour = trainingDay.EndHour
            };
            
            return trainingDayVM;
        }

        // Create a training day
        // GET: /User/TrainingJournal/CreateTrainingDay
        [HttpGet]
        public IActionResult CreateTrainingDay(string userId, int year, int weekOfYear, int dayOfWeek)
        {
            var userInfoManager = new UserInfoManager(_dbContext);
            var userInfo = userInfoManager.GetUserInfo(new UserInfoKey() { UserId = User.GetUserId() });
            if (userInfo == null)
                return RedirectToAction("View", new { userId = userId, year = year, weekOfYear = weekOfYear, dayOfWeek = dayOfWeek });
            
            var trainingDayManager = new TrainingDayManager(_dbContext);
            var trainingDayCriteria = new TrainingDayCriteria()
            {
                UserId = new StringCriteria() { EqualList = new List<string>() { userId } },
                Year = new IntegerCriteria() { EqualList = new List<int>() { year } },
                WeekOfYear = new IntegerCriteria() { EqualList = new List<int>() { weekOfYear } }
            };
            var trainingDayList = trainingDayManager.FindTrainingDay(trainingDayCriteria, false);

            int trainingDayId = 0;
            if (trainingDayList != null && trainingDayList.Count > 0)
                trainingDayId = trainingDayList.Max(td => td.TrainingDayId) + 1;

            var trainingDay = new TrainingDay()
            {
                UserId = userId,
                Year = year,
                WeekOfYear = weekOfYear,
                DayOfWeek = dayOfWeek,
                TrainingDayId = trainingDayId
            };

            ViewBag.UserUnit = userInfo.Unit;
            return View(TransformTrainingDayToViewModel(trainingDay));
        }

        // Create a training day
        // GET: /User/TrainingJournal/CreateTrainingDay
        [HttpPost]
        public IActionResult CreateTrainingDay(TrainingDayViewModel viewModel)
        {
           /* if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(viewModel.UserId) || viewModel.Year == 0 || viewModel.Year == 0 || User.GetUserId() != viewModel.UserId)
                    return View(viewModel);

                //Verify valide wekk of year
                if (viewModel.WeekOfYear > 0 && viewModel.WeekOfYear <= 52)
                {
                    var trainingWeekManager = new TrainingWeekManager(_dbContext);
                    var trainingWeek = TransformViewModelToTrainingJournal(viewModel);

                    var trainingWeekKey = new TrainingWeekKey() { UserId = trainingWeek.UserId, Year = trainingWeek.Year, WeekOfYear = trainingWeek.WeekOfYear };
                    var existTrainingWeek = trainingWeekManager.GetTrainingWeek(trainingWeekKey, false);

                    if (existTrainingWeek != null)
                    {
                        ModelState.AddModelError(string.Empty, string.Format(Translation.P0_ALREADY_EXIST, Translation.TRAINING_WEEK));
                        return View(viewModel);
                    }

                    //Create data in database
                    trainingWeek = trainingWeekManager.CreateTrainingWeek(trainingWeek);

                    if (trainingWeek == null)
                    {
                        ModelState.AddModelError(string.Empty, Translation.IMPOSSIBLE_TO_CREATE_NEW_TRAINING_JOURNAL);
                        return View(viewModel);
                    }

                    return RedirectToAction("Index");
                }
            }*/

            return View(viewModel);
        }
    }
}
