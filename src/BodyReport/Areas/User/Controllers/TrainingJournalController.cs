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

            if (trainingWeekList != null)
            {
                foreach (var trainingWeek in trainingWeekList)
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

            ViewBag.UserUnit = userInfo.Unit;
            return View(TransformTrainingWeekToViewModel(trainingWeek));
        }

        // Create a training journal week
        // GET: /User/TrainingJournal/Create
        [HttpPost]
        public IActionResult Create(TrainingWeekViewModel viewModel)
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

                    return RedirectToAction("View", new { userId = trainingWeek.UserId, year = trainingWeek.Year, weekOfYear = trainingWeek.WeekOfYear });
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
                if (string.IsNullOrWhiteSpace(viewModel.UserId) || viewModel.Year == 0 || viewModel.WeekOfYear == 0 || User.GetUserId() != viewModel.UserId)
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

        // Delete a training journals
        // GET: /User/TrainingJournal/Delete
        [HttpGet]
        public IActionResult Delete(string userId, int year, int weekOfYear, bool confirmation = false)
        {
            if (confirmation)
            {
                var actionResult = RedirectToAction("View", "TrainingJournal", new { Area = "User" });
                if (string.IsNullOrWhiteSpace(userId) || year == 0 || weekOfYear == 0 || User.GetUserId() != userId)
                    return actionResult;

                var trainingWeekManager = new TrainingWeekManager(_dbContext);
                var key = new TrainingWeekKey()
                {
                    UserId = userId,
                    Year = year,
                    WeekOfYear = weekOfYear
                };
                var trainingWeek = trainingWeekManager.GetTrainingWeek(key, true);
                if (trainingWeek == null)
                    return actionResult;

                trainingWeekManager.DeleteTrainingWeek(trainingWeek);
                return actionResult;
            }
            else
            {
                string title = Translation.TRAINING_WEEK;
                string message = Translation.ARE_YOU_SURE_YOU_WANNA_DELETE_THIS_ELEMENT_PI;
                string returnUrlYes = Url.Action("Delete", "TrainingJournal", new { Area = "User", userId = userId, year = year, weekOfYear = weekOfYear, confirmation = true });
                string returnUrlNo = Url.Action("View", "TrainingJournal", new { Area = "User" });
                return RedirectToAction("Confirm", "Message", new { Area = "Site", title = title, message = message, returnUrlYes = returnUrlYes, returnUrlNo = returnUrlNo });
            }
        }

        //
        // GET: /User/TrainingJournal/View
        [HttpGet]
        public IActionResult View(string userId, int year, int weekOfYear, int? dayOfWeekSelected)
        {
            DayOfWeek currentDayOfWeek = GetCurrentDayOfWeek(dayOfWeekSelected);
            if (!dayOfWeekSelected.HasValue)
                dayOfWeekSelected = (int)currentDayOfWeek;

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

            var bodyExerciseManager = new BodyExerciseManager(_dbContext);
            var trainingWeekViewModel = TransformTrainingWeekToViewModel(trainingWeek);
            List<TrainingDayViewModel> trainingDayViewModels = null;
            List<TrainingExerciseViewModel> trainingExerciseViewModels = null;
            if (trainingWeek != null && trainingWeek.TrainingDays != null && trainingWeek.TrainingDays.Count > 0)
            {
                trainingDayViewModels = new List<TrainingDayViewModel>();
                foreach (var trainingDay in trainingWeek.TrainingDays)
                {
                    trainingDayViewModels.Add(TransformTrainingDayToViewModel(trainingDay));

                    if (dayOfWeekSelected.HasValue && trainingDay.DayOfWeek == dayOfWeekSelected)
                    { // Get only curretn
                        var trainingExercises = FindTrainingExercise(trainingDay);
                        if(trainingExercises != null)
                        {
                            foreach (var trainingExercise in trainingExercises)
                            {
                                if (trainingExerciseViewModels == null)
                                    trainingExerciseViewModels = new List<TrainingExerciseViewModel>();
                                trainingExerciseViewModels.Add(TransformTrainingExerciseToViewModel(bodyExerciseManager, trainingExercise));
                            }
                        }
                    }
                }
            }

            ViewBag.CurrentDayOfWeek = currentDayOfWeek;
            return View(new Tuple<TrainingWeekViewModel, List<TrainingDayViewModel>, List<TrainingExerciseViewModel>>(trainingWeekViewModel, trainingDayViewModels, trainingExerciseViewModels));
        }

        private List<TrainingExercise> FindTrainingExercise(TrainingDay trainingDay)
        {
            if (trainingDay == null)
                return null;

            var criteria = new TrainingExerciseCriteria()
            {
                Year = new IntegerCriteria() { EqualList = new List<int>() { trainingDay.Year } },
                WeekOfYear = new IntegerCriteria() { EqualList = new List<int>() { trainingDay.WeekOfYear } },
                DayOfWeek = new IntegerCriteria() { EqualList = new List<int>() { trainingDay.DayOfWeek } },
                TrainingDayId = new IntegerCriteria() { EqualList = new List<int>() { trainingDay.TrainingDayId } },
                UserId = new StringCriteria() { EqualList = new List<string>() { trainingDay.UserId } }
            };

            var trainingExerciseManager = new TrainingExerciseManager(_dbContext);
            return trainingExerciseManager.FindTrainingExercise(criteria);
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

        private TrainingWeekViewModel TransformTrainingWeekToViewModel(TrainingWeek trainingWeek)
        {
            TrainingWeekViewModel trainingJournalVM = new TrainingWeekViewModel();

            trainingJournalVM.UserId = trainingWeek.UserId;
            trainingJournalVM.Year = trainingWeek.Year;
            trainingJournalVM.WeekOfYear = trainingWeek.WeekOfYear;
            trainingJournalVM.UserHeight = trainingWeek.UserHeight;
            trainingJournalVM.UserWeight = trainingWeek.UserWeight;

            var userManager = new UserManager(_dbContext);
            var user = userManager.GetUser(new UserKey() { Id = trainingWeek.UserId });
            if (user != null)
                trainingJournalVM.UserName = user.Name;

            return trainingJournalVM;
        }

        private TrainingDayViewModel TransformTrainingDayToViewModel(TrainingDay trainingDay)
        {
            return new TrainingDayViewModel()
            {
                UserId = trainingDay.UserId,
                Year = trainingDay.Year,
                WeekOfYear = trainingDay.WeekOfYear,
                DayOfWeek = trainingDay.DayOfWeek,
                TrainingDayId = trainingDay.TrainingDayId,
                BeginHour = trainingDay.BeginHour,
                EndHour = trainingDay.EndHour
            };
        }

        private TrainingExerciseViewModel TransformTrainingExerciseToViewModel(BodyExerciseManager bodyExerciseManager, TrainingExercise trainingExercise)
        {
            var bodyExercise = bodyExerciseManager.GetBodyExercise(new BodyExerciseKey() { Id = trainingExercise.BodyExerciseId });

            var viewModel = new TrainingExerciseViewModel()
            {
                UserId = trainingExercise.UserId,
                Year = trainingExercise.Year,
                WeekOfYear = trainingExercise.WeekOfYear,
                DayOfWeek = trainingExercise.DayOfWeek,
                TrainingDayId = trainingExercise.TrainingDayId,
                BodyExerciseId = trainingExercise.BodyExerciseId,
                RestTime = trainingExercise.RestTime,
                BodyExerciseName = bodyExercise != null && !string.IsNullOrWhiteSpace(bodyExercise.Name) ? bodyExercise.Name : string.Empty,
                BodyExerciseImage = string.Format("/images/bodyexercises/{0}.png", trainingExercise.BodyExerciseId)
            };

            viewModel.TupleSetReps = new List<Tuple<int, int, int>>();
            if (trainingExercise.TrainingExerciseSets != null)
            {
                foreach (var set in trainingExercise.TrainingExerciseSets)
                    viewModel.TupleSetReps.Add(new Tuple<int, int, int>(set.NumberOfSets, set.NumberOfReps, set.Weight));
            }

            return viewModel;
        }

        // Create a training day
        // GET: /User/TrainingJournal/CreateTrainingDay
        [HttpGet]
        public IActionResult CreateTrainingDay(string userId, int year, int weekOfYear, int dayOfWeek)
        {
            if (string.IsNullOrWhiteSpace(userId) || year == 0 || weekOfYear == 0 || dayOfWeek < 0 || dayOfWeek > 6 || User.GetUserId() != userId)
                return RedirectToAction("Index");

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

        private TrainingDay TransformViewModelToTrainingDay(TrainingDayViewModel viewModel)
        {
            TrainingDay trainingDay = new TrainingDay();

            trainingDay.UserId = viewModel.UserId;
            trainingDay.Year = viewModel.Year;
            trainingDay.WeekOfYear = viewModel.WeekOfYear;
            trainingDay.DayOfWeek = viewModel.DayOfWeek;
            trainingDay.TrainingDayId = viewModel.TrainingDayId;
            trainingDay.BeginHour = viewModel.BeginHour;
            trainingDay.EndHour = viewModel.EndHour;

            return trainingDay;
        }

        // Create a training day
        // GET: /User/TrainingJournal/CreateTrainingDay
        [HttpPost]
        public IActionResult CreateTrainingDay(TrainingDayViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(viewModel.UserId) || viewModel.Year == 0 || viewModel.WeekOfYear == 0 ||
                    viewModel.DayOfWeek < 0 || viewModel.DayOfWeek > 6 || User.GetUserId() != viewModel.UserId)
                    return View(viewModel);

                //Verify trainingWeek exist
                var trainingWeekManager = new TrainingWeekManager(_dbContext);

                var trainingWeekKey = new TrainingWeekKey()
                {
                    UserId = viewModel.UserId,
                    Year = viewModel.Year,
                    WeekOfYear = viewModel.WeekOfYear
                };
                var trainingWeek = trainingWeekManager.GetTrainingWeek(trainingWeekKey, true);

                if (trainingWeek == null)
                {
                    ModelState.AddModelError(string.Empty, string.Format(Translation.P0_NOT_EXIST, Translation.TRAINING_WEEK));
                    return View(viewModel);
                }

                //Verify valide week of year
                if (viewModel.WeekOfYear > 0 && viewModel.WeekOfYear <= 52)
                {
                    var trainingDayManager = new TrainingDayManager(_dbContext);
                    var trainingDay = TransformViewModelToTrainingDay(viewModel);

                    var trainingDayCriteria = new TrainingDayCriteria()
                    {
                        UserId = new StringCriteria() { EqualList = new List<string>() { viewModel.UserId } },
                        Year = new IntegerCriteria() { EqualList = new List<int>() { viewModel.Year } },
                        WeekOfYear = new IntegerCriteria() { EqualList = new List<int>() { viewModel.WeekOfYear } },
                        DayOfWeek = new IntegerCriteria() { EqualList = new List<int>() { viewModel.DayOfWeek } },
                    };

                    var trainingDayList = trainingDayManager.FindTrainingDay(trainingDayCriteria, false);
                    int trainingDayId = 1;
                    if (trainingDayList != null && trainingDayList.Count > 0)
                    {
                        trainingDayId = trainingDayList.Max(td => td.TrainingDayId) + 1;
                    }

                    trainingDay.TrainingDayId = trainingDayId;

                    trainingDay = trainingDayManager.CreateTrainingDay(trainingDay);
                    if (trainingDay != null)
                    {
                        return RedirectToAction("View", new { userId = trainingDay.UserId, year = trainingDay.Year, weekOfYear = trainingDay.WeekOfYear, dayOfWeekSelected = trainingDay.DayOfWeek });
                    }
                }
            }

            return View(viewModel);
        }

        // Edit a training day
        // GET: /User/TrainingJournal/EditTrainingDay
        [HttpGet]
        public IActionResult EditTrainingDay(string userId, int year, int weekOfYear, int dayOfWeek, int trainingDayId)
        {
            if (string.IsNullOrWhiteSpace(userId) || year == 0 || weekOfYear == 0 || dayOfWeek < 0 || dayOfWeek > 6 || trainingDayId == 0 || User.GetUserId() != userId)
                return RedirectToAction("Index");

            var trainingDayManager = new TrainingDayManager(_dbContext);
            var key = new TrainingDayKey()
            {
                UserId = userId,
                Year = year,
                WeekOfYear = weekOfYear,
                DayOfWeek = dayOfWeek,
                TrainingDayId = trainingDayId
            };
            var trainingDay = trainingDayManager.GetTrainingDay(key, true);
            if (trainingDay == null) // no data found
                return RedirectToAction("View", new { userId = userId, year = year, weekOfYear = weekOfYear, dayOfWeek = dayOfWeek });

            return View(TransformTrainingDayToViewModel(trainingDay));
        }

        // Edit a training day
        // GET: /User/TrainingJournal/EditTrainingDay
        [HttpPost]
        public IActionResult EditTrainingDay(TrainingDayViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(viewModel.UserId) || viewModel.Year == 0 || viewModel.WeekOfYear == 0 ||
                    viewModel.DayOfWeek < 0 || viewModel.DayOfWeek > 6 || viewModel.TrainingDayId == 0 || User.GetUserId() != viewModel.UserId)
                    return View(viewModel);

                //Verify valide week of year
                if (viewModel.WeekOfYear > 0 && viewModel.WeekOfYear <= 52)
                {
                    var trainingDay = TransformViewModelToTrainingDay(viewModel);

                    var trainingDayManager = new TrainingDayManager(_dbContext);
                    var key = new TrainingDayKey()
                    {
                        UserId = trainingDay.UserId,
                        Year = trainingDay.Year,
                        WeekOfYear = trainingDay.WeekOfYear,
                        DayOfWeek = trainingDay.DayOfWeek,
                        TrainingDayId = trainingDay.TrainingDayId
                    };
                    var foundTrainingDay = trainingDayManager.GetTrainingDay(key, true);
                    if (foundTrainingDay == null) // no data found
                    {
                        ModelState.AddModelError(string.Empty, string.Format(Translation.P0_NOT_EXIST, Translation.TRAINING_DAY));
                        return View(viewModel);
                    }

                    trainingDay = trainingDayManager.UpdateTrainingDay(trainingDay, true);
                    if (trainingDay != null)
                    {
                        return RedirectToAction("View", new { userId = trainingDay.UserId, year = trainingDay.Year, weekOfYear = trainingDay.WeekOfYear, dayOfWeekSelected = trainingDay.DayOfWeek });
                    }
                }
            }

            return View(viewModel);
        }

        // Delete a training journals
        // GET: /User/TrainingJournal/DeleteTrainingDay
        [HttpGet]
        public IActionResult DeleteTrainingDay(string userId, int year, int weekOfYear, int dayOfWeek, int trainingDayId, bool confirmation = false)
        {
            if (string.IsNullOrWhiteSpace(userId) || year == 0 || weekOfYear == 0 || dayOfWeek < 0 || dayOfWeek > 6 || trainingDayId == 0 || User.GetUserId() != userId)
                return RedirectToAction("Index");

            if (confirmation)
            {
                var actionResult = RedirectToAction("View", "TrainingJournal", new { Area = "User", userId = userId, year = year, weekOfYear = weekOfYear, dayOfWeekSelected = dayOfWeek });
                var trainingDayManager = new TrainingDayManager(_dbContext);
                var key = new TrainingDayKey()
                {
                    UserId = userId,
                    Year = year,
                    WeekOfYear = weekOfYear,
                    DayOfWeek = dayOfWeek,
                    TrainingDayId = trainingDayId
                };
                var trainingDay = trainingDayManager.GetTrainingDay(key, true);
                if (trainingDay == null)
                    return actionResult;

                trainingDayManager.DeleteTrainingDay(trainingDay);
                return actionResult;
            }
            else
            {
                string title = Translation.TRAINING_DAY;
                string message = Translation.ARE_YOU_SURE_YOU_WANNA_DELETE_THIS_ELEMENT_PI;
                string returnUrlYes = Url.Action("DeleteTrainingDay", "TrainingJournal", new { Area = "User", userId = userId, year = year, weekOfYear = weekOfYear, dayOfWeek = dayOfWeek, trainingDayId = trainingDayId, confirmation = true });
                string returnUrlNo = Url.Action("View", "TrainingJournal", new { Area = "User", userId = userId, year = year, weekOfYear = weekOfYear, dayOfWeekSelected = dayOfWeek });
                return RedirectToAction("Confirm", "Message", new { Area = "Site", title = title, message = message, returnUrlYes = returnUrlYes, returnUrlNo = returnUrlNo });
            }
        }

        private bool IncorrectHttpData(string userId, int year, int weekOfYear, int dayOfWeek, int trainingDayId)
        {
            return string.IsNullOrWhiteSpace(userId) || User.GetUserId() != userId || year == 0 || weekOfYear == 0 || dayOfWeek < 0 || dayOfWeek > 6 || trainingDayId == 0;
        }

        private IActionResult GetViewActionResult(string userId, int year, int weekOfYear, int dayOfWeek)
        {
            return RedirectToAction("View", "TrainingJournal", new { Area = "User", userId = userId, year = year, weekOfYear = weekOfYear, dayOfWeekSelected = dayOfWeek });
        }

        // Add a training exercises
        // GET: /User/TrainingJournal/AddTrainingExercises
        [HttpGet]
        public IActionResult AddTrainingExercises(string userId, int year, int weekOfYear, int dayOfWeek, int trainingDayId)
        {
            if (IncorrectHttpData(userId, year, weekOfYear, dayOfWeek, trainingDayId))
                return RedirectToAction("Index");

            List<BodyExercise> bodyExerciseList;
            InsertViewBagOnEditTrainingExercise(out bodyExerciseList);

            var viewModel = new TrainingExercisesViewModel();
            CopyViewBagBodyExerciseToViewModel(viewModel, bodyExerciseList);
            return View(viewModel);
        }

        // Add a training exercises
        // POST: /User/TrainingJournal/AddTrainingExercise
        [HttpPost]
        public IActionResult AddTrainingExercises(TrainingExercisesViewModel viewModel, string buttonType)
        {
            if (IncorrectHttpData(viewModel.UserId, viewModel.Year, viewModel.WeekOfYear, viewModel.DayOfWeek, viewModel.TrainingDayId))
                return View(viewModel);

            bool displayMessage = buttonType == "submit";

            int currentMuscularGroupId = 0, currentMuscleId = 0;
            if (viewModel != null)
            {
                currentMuscularGroupId = viewModel.MuscularGroupId;
                currentMuscleId = viewModel.MuscleId;
            }

            List<BodyExercise> bodyExerciseList;
            InsertViewBagOnEditTrainingExercise(out bodyExerciseList, currentMuscularGroupId, currentMuscleId);
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                if(viewModel.BodyExerciseList != null)
                { // filter selected data with existing exercise
                    if (bodyExerciseList == null)
                        viewModel.BodyExerciseList = null;
                    else
                    {
                        var deleteList = new List<SelectBodyExercise>();
                        foreach(var selectBodyExercise in viewModel.BodyExerciseList)
                        {
                            if(bodyExerciseList.FirstOrDefault(c => c.Id == selectBodyExercise.Id) == null)
                            {
                                deleteList.Add(selectBodyExercise);
                            }
                        }

                        foreach (var selectBodyExercise in deleteList)
                        {
                            viewModel.BodyExerciseList.Remove(selectBodyExercise);
                        }
                    }
                }

                if (viewModel.BodyExerciseList == null || viewModel.BodyExerciseList.Count(b => b.Selected == true) == 0)
                {
                    CopyViewBagBodyExerciseToViewModel(viewModel, bodyExerciseList);
                    if (displayMessage)
                        ModelState.AddModelError(string.Empty, string.Format(Translation.THE_P0_FIELD_IS_REQUIRED, Translation.BODY_EXERCISES));
                    return View(viewModel);
                }

                var trainingDayManager = new TrainingDayManager(_dbContext);
                var trainingDayKey = new TrainingDayKey() { UserId = viewModel.UserId, Year = viewModel.Year, WeekOfYear = viewModel.WeekOfYear, DayOfWeek = viewModel.DayOfWeek, TrainingDayId = viewModel.TrainingDayId };
                var trainingDay = trainingDayManager.GetTrainingDay(trainingDayKey, true);

                if(trainingDay == null)
                {
                    CopyViewBagBodyExerciseToViewModel(viewModel, bodyExerciseList);
                    ModelState.AddModelError(string.Empty, string.Format(Translation.P0_NOT_EXIST, Translation.TRAINING_DAY));
                    return View(viewModel);
                }

                if (trainingDay.TrainingExercises == null)
                    trainingDay.TrainingExercises = new List<TrainingExercise>();

                int bodyExerciseCount = trainingDay.TrainingExercises.Count;
                TrainingExercise trainingExercise;
                var bodyExerciseSelectedList = viewModel.BodyExerciseList.Where(b => b.Selected == true);
                foreach (var bodyExercise in bodyExerciseSelectedList)
                {
                    if(trainingDay.TrainingExercises.Count(e => e.BodyExerciseId == bodyExercise.Id) == 0)
                    { //Only manage add in this page
                        trainingExercise = new TrainingExercise() { UserId = viewModel.UserId, Year = viewModel.Year, WeekOfYear = viewModel.WeekOfYear, DayOfWeek = viewModel.DayOfWeek, TrainingDayId = viewModel.TrainingDayId, BodyExerciseId = bodyExercise.Id };
                        trainingDay.TrainingExercises.Add(trainingExercise);
                    }
                }
                if(bodyExerciseCount != trainingDay.TrainingExercises.Count)
                { //data changed
                    trainingDayManager.UpdateTrainingDay(trainingDay);
                }

                return GetViewActionResult(viewModel.UserId, viewModel.Year, viewModel.WeekOfYear, viewModel.DayOfWeek);
            }

            CopyViewBagBodyExerciseToViewModel(viewModel, bodyExerciseList);
            return View(viewModel);
        }

        private void CopyViewBagBodyExerciseToViewModel(TrainingExercisesViewModel viewModel, List<BodyExercise> bodyExerciseList)
        {
            viewModel.BodyExerciseList = new List<SelectBodyExercise>();
            if (bodyExerciseList != null)
            {   
                foreach (BodyExercise bodyExercise in bodyExerciseList)
                {
                    viewModel.BodyExerciseList.Add(new SelectBodyExercise() { Id = bodyExercise.Id, Name = bodyExercise.Name });
                }
            }
        }

        private void InsertViewBagOnEditTrainingExercise(out List<BodyExercise> bodyExerciseList, int currentMuscularGroupId = 0, int currentMuscleId = 0, int currentBodyExerciseId = 0)
        {
            bodyExerciseList = null;
            var muscularGroupManager = new MuscularGroupManager(_dbContext);
            ViewBag.MuscularGroups = ControllerUtils.CreateSelectMuscularGroupItemList(muscularGroupManager.FindMuscularGroups(), currentMuscularGroupId, true);

            var bodyExerciseManager = new BodyExerciseManager(_dbContext);
            if (currentMuscularGroupId == 0)
            { // All exercises
                bodyExerciseList = bodyExerciseManager.FindBodyExercises();
            }
            else
            {
                var muscleManager = new MuscleManager(_dbContext);
                var muscleCriteria = new MuscleCriteria()
                {
                    MuscularGroupId = new IntegerCriteria() { EqualList = new List<int>() { currentMuscularGroupId } }
                };
                var muscleList = muscleManager.FindMuscles(muscleCriteria);
                ViewBag.Muscles = ControllerUtils.CreateSelectMuscleItemList(muscleList, currentMuscleId, true);

                if (currentMuscleId == 0)
                {
                    if (muscleList != null)
                    {
                        var mucleIdList = new List<int>();
                        foreach (var muscle in muscleList)
                        {
                            mucleIdList.Add(muscle.Id);
                        }
                        var bodyExerciseCriteria = new BodyExerciseCriteria()
                        {
                            MuscleId = new IntegerCriteria() { EqualList = mucleIdList }
                        };
                        bodyExerciseList = bodyExerciseManager.FindBodyExercises(bodyExerciseCriteria);
                    }
                    else //Security
                        bodyExerciseList = bodyExerciseManager.FindBodyExercises();
                }
                else
                {
                    var bodyExerciseCriteria = new BodyExerciseCriteria()
                    {
                        MuscleId = new IntegerCriteria() { EqualList = new List<int>() { currentMuscleId } }
                    };
                    bodyExerciseList = bodyExerciseManager.FindBodyExercises(bodyExerciseCriteria);
                }
            }
        }

        // Add a training exercise
        // GET: /User/TrainingJournal/AddTrainingExercise
        [HttpGet]
        public IActionResult EditTrainingExercise(string userId, int year, int weekOfYear, int dayOfWeek, int trainingDayId, int bodyExerciseId)
        {
            if (IncorrectHttpData(userId, year, weekOfYear, dayOfWeek, trainingDayId))
                return RedirectToAction("Index");

            var actionResult = GetViewActionResult(userId, year, weekOfYear, dayOfWeek);

            var trainingExerciseManager = new TrainingExerciseManager(_dbContext);

            var key = new TrainingExerciseKey()
            {
                UserId = userId,
                Year = year,
                WeekOfYear = weekOfYear,
                DayOfWeek = dayOfWeek,
                TrainingDayId = trainingDayId,
                BodyExerciseId = bodyExerciseId
            };
            var trainingExercise = trainingExerciseManager.GetTrainingExercise(key);
            if (trainingExercise == null)
                return actionResult;

            var bodyExerciseManager = new BodyExerciseManager(_dbContext);

            var bodyExercise = bodyExerciseManager.GetBodyExercise(new BodyExerciseKey() { Id = trainingExercise.BodyExerciseId });

            var viewModel = new TrainingExerciseViewModel();
            viewModel.UserId = userId;
            viewModel.Year = year;
            viewModel.WeekOfYear = weekOfYear;
            viewModel.DayOfWeek = dayOfWeek;
            viewModel.TrainingDayId = trainingDayId;
            viewModel.BodyExerciseId = bodyExerciseId;
            viewModel.BodyExerciseName = bodyExercise.Name;
            viewModel.BodyExerciseImage = bodyExercise.ImageName;
            viewModel.RestTime = trainingExercise.RestTime;
            
            if(trainingExercise.TrainingExerciseSets != null)
            { 
                foreach(var trainingExerciseSet in trainingExercise.TrainingExerciseSets)
                {
                    for(int i=0; i < trainingExerciseSet.NumberOfSets; i++)
                    {
                        viewModel.Reps.Add(trainingExerciseSet.NumberOfReps);
                        viewModel.Weights.Add(trainingExerciseSet.Weight);
                    }
                }
            }

            if(viewModel.Reps == null || viewModel.Reps.Count == 0)
                viewModel.Reps = new List<int?>() { 8, 8, 8, 8 };
            if (viewModel.Weights == null || viewModel.Weights.Count == 0)
                viewModel.Weights = new List<int?>() { 0, 0, 0, 0 };
            return View(viewModel);
        }

        // Add a training exercise
        // POST: /User/TrainingJournal/AddTrainingExercise
        [HttpPost]
        public IActionResult EditTrainingExercise(TrainingExerciseViewModel viewModel, string buttonType)
        {
            const int MAX_REPS = 10;
            if(viewModel == null)
                return RedirectToAction("Index");

            if (viewModel.Reps == null || viewModel.Reps.Count == 0) //Security
                viewModel.Reps = new List<int?>() { 8, 8, 8, 8 };

            if (viewModel.Weights == null)
                viewModel.Weights = new List<int?>();

            while (viewModel.Reps.Count > MAX_REPS)
            {
                viewModel.Reps.RemoveAt(viewModel.Reps.Count - 1);
            }

            while (viewModel.Weights.Count > MAX_REPS)
            {
                viewModel.Weights.RemoveAt(viewModel.Weights.Count - 1);
            }

            while (viewModel.Weights.Count < viewModel.Reps.Count)
            {
                viewModel.Weights.Add(0);
            }

            for (int i = 0; i < viewModel.Reps.Count; i++)
            {
                if(viewModel.Reps[i] == null)
                    viewModel.Reps[i] = 0;
                if (viewModel.Weights[i] == null)
                    viewModel.Weights[i] = 0;
            }

            if ("addRep" == buttonType)
            {
                if (viewModel.Reps.Count < MAX_REPS)
                {
                    int newRepValue = 8;
                    if (viewModel.Reps.Count > 0)
                        newRepValue = viewModel.Reps[viewModel.Reps.Count - 1].Value;
                    viewModel.Reps.Add(newRepValue);

                    int newWeightValue = 8;
                    if (viewModel.Weights.Count > 0)
                        newWeightValue = viewModel.Weights[viewModel.Weights.Count - 1].Value;
                    viewModel.Weights.Add(newWeightValue);
                }
                return View(viewModel);
            }
            else if ("delete" == buttonType)
            {
                if (viewModel.Reps.Count > 1)
                    viewModel.Reps.RemoveAt(viewModel.Reps.Count - 1);
                if (viewModel.Weights.Count > 1)
                    viewModel.Weights.RemoveAt(viewModel.Weights.Count - 1);
            }
            else if ("submit" == buttonType && ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(viewModel.UserId) || User.GetUserId() != viewModel.UserId || viewModel.Year == 0 || viewModel.WeekOfYear == 0 ||
                    viewModel.DayOfWeek < 0 || viewModel.DayOfWeek > 6 || viewModel.TrainingDayId == 0 || viewModel.BodyExerciseId == 0)
                    return View(viewModel);

                var trainingExerciseManager = new TrainingExerciseManager(_dbContext);

                var key = new TrainingExerciseKey()
                {
                    UserId = viewModel.UserId,
                    Year = viewModel.Year,
                    WeekOfYear = viewModel.WeekOfYear,
                    DayOfWeek = viewModel.DayOfWeek,
                    TrainingDayId = viewModel.TrainingDayId,
                    BodyExerciseId = viewModel.BodyExerciseId
                };
                var trainingExercise = trainingExerciseManager.GetTrainingExercise(key);
                if (trainingExercise == null)
                {
                    ModelState.AddModelError(string.Empty, string.Format("{0} {1}", Translation.INVALID_INPUT_2P, Translation.TRAINING_EXERCISE));
                    return View(viewModel);
                }

                trainingExercise.RestTime = viewModel.RestTime;

                if (viewModel.Reps != null && viewModel.Reps.Count > 0)
                {
                    //Regroup Reps with Set
                    int nbSet = 0, currentRepValue = 0, currentWeightValue = 0;
                    var tupleSetRepList = new List<Tuple<int, int, int>>();
                    int repValue, weightValue;
                    //foreach (var repValue in viewModel.Reps)
                    for (int i=0; i < viewModel.Reps.Count; i++)
                    {
                        repValue = viewModel.Reps[i].Value;
                        weightValue = viewModel.Weights[i].Value;
                        if (repValue == 0)
                            continue;

                        if (weightValue == currentWeightValue && repValue == currentRepValue)
                            nbSet++;
                        else
                        {
                            if (nbSet != 0)
                                tupleSetRepList.Add(new Tuple<int, int, int>(nbSet, currentRepValue, currentWeightValue));
                            currentRepValue = repValue;
                            currentWeightValue = weightValue;
                            nbSet = 1;
                        }
                    }

                    //last data
                    if (nbSet != 0)
                        tupleSetRepList.Add(new Tuple<int, int, int>(nbSet, currentRepValue, currentWeightValue));

                    trainingExercise.TrainingExerciseSets = new List<TrainingExerciseSet>();
                    int id = 1;
                    foreach (Tuple<int, int, int> tupleSetRep in tupleSetRepList)
                    {
                        trainingExercise.TrainingExerciseSets.Add(new TrainingExerciseSet()
                        {
                            UserId = viewModel.UserId,
                            Year = viewModel.Year,
                            WeekOfYear = viewModel.WeekOfYear,
                            DayOfWeek = viewModel.DayOfWeek,
                            TrainingDayId = viewModel.TrainingDayId,
                            BodyExerciseId = viewModel.BodyExerciseId,
                            Id = id,
                            NumberOfSets = tupleSetRep.Item1,
                            NumberOfReps = tupleSetRep.Item2,
                            Weight = tupleSetRep.Item3
                        });
                        id++;
                    }
                    
                    trainingExerciseManager.UpdateTrainingExercise(trainingExercise, true);

                    return RedirectToAction("View", "TrainingJournal", new { Area = "User", userId = viewModel.UserId, year = viewModel.Year, weekOfYear = viewModel.WeekOfYear, dayOfWeekSelected = viewModel.DayOfWeek });

                }
            }
            return View(viewModel);
        }
    }
}
