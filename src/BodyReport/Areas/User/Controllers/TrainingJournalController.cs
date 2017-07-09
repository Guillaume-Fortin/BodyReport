using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BodyReport.Message;
using BodyReport.Message.Web;
using BodyReport.Framework;
using BodyReport.Areas.User.ViewModels;
using BodyReport.Framework.Exceptions;
using BodyReport.Models;
using BodyReport.Resources;
using BodyReport.ServiceLayers.Interfaces;
using BodyReport.Areas.User.ViewModels.Transformer;

namespace BodyReport.Areas.User.Controllers
{
    [Authorize(Roles = "User,Admin")]
    [Area("User")]
    public class TrainingJournalController : MvcController
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(TrainingJournalController));
        /// <summary>
        /// Service layer users
        /// </summary>
        private readonly IUsersService _usersService;
        /// <summary>
        /// Service layer userInfos
        /// </summary>
        private readonly IUserInfosService _userInfosService;
        /// <summary>
        /// Service layer TrainingWeeks
        /// </summary>
        private readonly ITrainingWeeksService _trainingWeeksService;
        /// <summary>
        /// Service layer TrainingDays
        /// </summary>
        private readonly ITrainingDaysService _trainingDaysService;
        /// <summary>
        /// Service layer ITrainingExercises
        /// </summary>
        private readonly ITrainingExercisesService _trainingExercisesService;
        /// <summary>
        /// Service layer BodyExercises
        /// </summary>
        private readonly IBodyExercisesService _bodyExercisesService;
        /// <summary>
        /// Service layer muscles
        /// </summary>
        private readonly IMusclesService _musclesService;
        /// <summary>
        /// Service layer muscularGroups
        /// </summary>
        private readonly IMuscularGroupsService _muscularGroupsService;

        public TrainingJournalController(UserManager<ApplicationUser> userManager,
                                         IUsersService usersService,
                                         IUserInfosService userInfosService,
                                         ITrainingWeeksService trainingWeeksService,
                                         ITrainingDaysService trainingDaysService,
                                         ITrainingExercisesService trainingExercisesService,
                                         IBodyExercisesService bodyExercisesService,
                                         IMusclesService musclesService,
                                         IMuscularGroupsService muscularGroupsService) : base(userManager)
        {
            _usersService = usersService;
            _userInfosService = userInfosService;
            _trainingWeeksService = trainingWeeksService;
            _trainingDaysService = trainingDaysService;
            _trainingExercisesService = trainingExercisesService;
            _bodyExercisesService = bodyExercisesService;
            _musclesService = musclesService;
            _muscularGroupsService = muscularGroupsService;
        }

        private DayOfWeek GetCurrentDayOfWeek(int? dayOfWeekSelected, TimeZoneInfo timeZoneInfo)
        {
            if (dayOfWeekSelected.HasValue && dayOfWeekSelected >= 0 && dayOfWeekSelected <= 6)
                return (DayOfWeek)dayOfWeekSelected.Value; // TODO Manage Time with world position of user
            else
                return TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo).DayOfWeek;
        }

        //
        // GET: /User/TrainingJournal/Index
        [HttpGet]
        public IActionResult Index(string userId, int year, int weekOfYear, int? dayOfWeekSelected)
        {
            var viewModel = new List<TrainingWeekViewModel>();

            var searchCriteria = new TrainingWeekCriteria() { UserId = new StringCriteria() { Equal = SessionUserId } };
            var scenario = new TrainingWeekScenario() { ManageTrainingDay = false };
            var trainingWeekList = _trainingWeeksService.FindTrainingWeek(searchCriteria, scenario);

            if (trainingWeekList != null)
            {
                foreach (var trainingWeek in trainingWeekList)
                {
                    viewModel.Add(TrainingViewModelTransformer.TrainingWeekToViewModel(trainingWeek, _usersService));
                }
            }

            return View(viewModel);
        }


        // Create a training journal
        // GET: /User/TrainingJournal/Create
        [HttpGet]
        public IActionResult Create()
        {
            var userInfo = _userInfosService.GetUserInfo(new UserInfoKey() { UserId = SessionUserId });
            if (userInfo == null)
                userInfo = new UserInfo();

            var timeZoneInfo = TimeZoneMapper.GetTimeZoneByOlsonName(userInfo.TimeZoneName);
            if (timeZoneInfo == null)
                timeZoneInfo = TimeZoneInfo.Local;
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);

            var trainingWeek = new TrainingWeek();
            trainingWeek.UserId = SessionUserId;
            trainingWeek.Year = dateTime.Year;
            trainingWeek.WeekOfYear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            trainingWeek.UserHeight = userInfo.Height;
            trainingWeek.UserWeight = userInfo.Weight;
            trainingWeek.Unit = userInfo.Unit;

            ViewBag.UserUnit = userInfo.Unit;
            return View(TrainingViewModelTransformer.TrainingWeekToViewModel(trainingWeek, _usersService));
        }

        // Create a training journal week
        // GET: /User/TrainingJournal/Create
        [HttpPost]
        public IActionResult Create(TrainingWeekViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(viewModel.UserId) || viewModel.Year == 0 || viewModel.Year == 0 || SessionUserId != viewModel.UserId)
                    return View(viewModel);

                //Verify valide week of year
                if (viewModel.WeekOfYear > 0 && viewModel.WeekOfYear <= 52 && 
                    (viewModel.Unit == (int)TUnitType.Imperial || viewModel.Unit == (int)TUnitType.Metric))
                {
                    var trainingWeek = TransformViewModelToTrainingWeek(viewModel);
                    var trainingWeekKey = new TrainingWeekKey() { UserId = trainingWeek.UserId, Year = trainingWeek.Year, WeekOfYear = trainingWeek.WeekOfYear };
                    var trainingWeekScenario = new TrainingWeekScenario() { ManageTrainingDay = false };
                    var existTrainingWeek = _trainingWeeksService.GetTrainingWeek(trainingWeekKey, trainingWeekScenario);

                    if (existTrainingWeek != null)
                    {
                        ModelState.AddModelError(string.Empty, string.Format(Translation.P0_ALREADY_EXIST, Translation.TRAINING_WEEK));
                        return View(viewModel);
                    }

                    trainingWeek = _trainingWeeksService.CreateTrainingWeek(trainingWeek);
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
            if (string.IsNullOrWhiteSpace(userId) || year == 0 || weekOfYear == 0 || SessionUserId != userId)
                return RedirectToAction("Index");

            ViewBag.UserUnit = AppUtils.GetUserUnit(_userInfosService, userId);
            var key = new TrainingWeekKey()
            {
                UserId = userId,
                Year = year,
                WeekOfYear = weekOfYear
            };
            var trainingWeekScenario = new TrainingWeekScenario()
            {
                ManageTrainingDay = true,
                TrainingDayScenario = new TrainingDayScenario() { ManageExercise = true }
            };
            var trainingJournal = _trainingWeeksService.GetTrainingWeek(key, trainingWeekScenario);
            if (trainingJournal == null) // no data found
                return RedirectToAction("Index");

            return View(TrainingViewModelTransformer.TrainingWeekToViewModel(trainingJournal, _usersService));
        }

        // Edit a training journal week
        // GET: /User/TrainingJournal/Create
        [HttpPost]
        public IActionResult Edit(TrainingWeekViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                ViewBag.UserUnit = AppUtils.GetUserUnit(_userInfosService, viewModel.UserId);
                if (string.IsNullOrWhiteSpace(viewModel.UserId) || viewModel.Year == 0 || viewModel.WeekOfYear == 0 || SessionUserId != viewModel.UserId)
                    return View(viewModel);

                //Verify valide week of year
                if (viewModel.WeekOfYear > 0 && viewModel.WeekOfYear <= 52 &&
                    (viewModel.Unit == (int)TUnitType.Imperial || viewModel.Unit == (int)TUnitType.Metric))
                {
                    var trainingWeek = TransformViewModelToTrainingWeek(viewModel);
                    var trainingWeekKey = new TrainingWeekKey() { UserId = trainingWeek.UserId, Year = trainingWeek.Year, WeekOfYear = trainingWeek.WeekOfYear };
                    var trainingWeekScenario = new TrainingWeekScenario() { ManageTrainingDay = false };
                    var existTrainingWeek = _trainingWeeksService.GetTrainingWeek(trainingWeekKey, trainingWeekScenario);

                    if (existTrainingWeek == null)
                    {
                        ModelState.AddModelError(string.Empty, string.Format(Translation.P0_NOT_EXIST, Translation.TRAINING_WEEK));
                        return View(viewModel);
                    }

                    //Create data in database. No need transaction, only header
                    trainingWeekScenario = new TrainingWeekScenario() { ManageTrainingDay = false };
                    trainingWeek = _trainingWeeksService.UpdateTrainingWeek(trainingWeek, trainingWeekScenario);

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

        // Copy a training week
        // GET: /User/TrainingJournal/Copy
        [HttpGet]
        public IActionResult Copy(string userId, int year, int weekOfYear)
        {
            if (string.IsNullOrWhiteSpace(userId) || year == 0 || weekOfYear == 0 || SessionUserId != userId)
                return RedirectToAction("Index");

            ViewBag.UserUnit = AppUtils.GetUserUnit(_userInfosService, userId);
            var key = new TrainingWeekKey()
            {
                UserId = userId,
                Year = year,
                WeekOfYear = weekOfYear
            };
            var trainingWeekScenario = new TrainingWeekScenario()
            {
                ManageTrainingDay = true,
                TrainingDayScenario = new TrainingDayScenario() { ManageExercise = true }
            };
            var trainingWeek = _trainingWeeksService.GetTrainingWeek(key, trainingWeekScenario);
            if (trainingWeek == null) // no data found
                return RedirectToAction("Index");
            
            var userInfo = _userInfosService.GetUserInfo(new UserInfoKey() { UserId = SessionUserId });
            if (userInfo == null)
                userInfo = new UserInfo();
            var timeZoneInfo = TimeZoneMapper.GetTimeZoneByOlsonName(userInfo.TimeZoneName);
            if (timeZoneInfo == null)
                timeZoneInfo = TimeZoneInfo.Local;
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            int nextWeekOfYear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

            if (dateTime.Year == year && nextWeekOfYear == weekOfYear
                && nextWeekOfYear < 52)
            {
                nextWeekOfYear++;
            }

            var viewModel = new CopyTrainingWeekViewModel()
            {
                UserId = userId,
                OriginWeekOfYear = weekOfYear,
                OriginYear = year,
                Year = dateTime.Year,
                WeekOfYear = nextWeekOfYear
            };

            //Need for refresh WeekOfYear in CopyTrainingWeekViewModel. Why? i don't understand on this page.
            ModelState.Clear();
            return View(viewModel);
        }

        // Edit a training journal week
        // GET: /User/TrainingJournal/Copy
        [HttpPost]
        public IActionResult Copy(CopyTrainingWeekViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                ViewBag.UserUnit = AppUtils.GetUserUnit(_userInfosService, viewModel.UserId);
                if(viewModel == null)
                    return View(viewModel);

                try
                {
                    var copyTrainingWeek = new CopyTrainingWeek()
                    {
                        UserId = viewModel.UserId,
                        OriginYear = viewModel.OriginYear,
                        OriginWeekOfYear = viewModel.OriginWeekOfYear,
                        Year = viewModel.Year,
                        WeekOfYear = viewModel.WeekOfYear
                    };
                    TrainingWeek trainingWeek;
                    if (!_trainingWeeksService.CopyTrainingWeek(SessionUserId, copyTrainingWeek, out trainingWeek))
                        return View(viewModel);

                    return RedirectToAction("View", new { userId = trainingWeek.UserId, year = trainingWeek.Year, weekOfYear = trainingWeek.WeekOfYear });
                }
                catch (ErrorException error)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                    return View(viewModel);
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
                if (string.IsNullOrWhiteSpace(userId) || year == 0 || weekOfYear == 0 || SessionUserId != userId)
                    return actionResult;
                
                var key = new TrainingWeekKey()
                {
                    UserId = userId,
                    Year = year,
                    WeekOfYear = weekOfYear
                };
                var trainingWeekScenario = new TrainingWeekScenario() { ManageTrainingDay = false };
                var trainingWeek = _trainingWeeksService.GetTrainingWeek(key, trainingWeekScenario);
                if (trainingWeek == null)
                    return actionResult;

                _trainingWeeksService.DeleteTrainingWeek(trainingWeek);
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
            var userInfo = _userInfosService.GetUserInfo(new UserInfoKey() { UserId = userId });
            if (userInfo == null)
                userInfo = new UserInfo();

            var timeZoneInfo = TimeZoneMapper.GetTimeZoneByOlsonName(userInfo.TimeZoneName);
            if (timeZoneInfo == null)
                timeZoneInfo = TimeZoneInfo.Local;

            DayOfWeek currentDayOfWeek = GetCurrentDayOfWeek(dayOfWeekSelected, timeZoneInfo);
            if (!dayOfWeekSelected.HasValue)
                dayOfWeekSelected = (int)currentDayOfWeek;

            var trainingWeekKey = new TrainingWeekKey()
            {
                UserId = userId,
                Year = year,
                WeekOfYear = weekOfYear
            };

            var trainingWeekScenario = new TrainingWeekScenario()
            {
                ManageTrainingDay = true,
                TrainingDayScenario = new TrainingDayScenario() { ManageExercise = true }
            };
            var trainingWeek = _trainingWeeksService.GetTrainingWeek(trainingWeekKey, trainingWeekScenario);

            if (trainingWeek == null)
                return RedirectToAction("Index");

            //Unit viewer convertion
            string userIdViewer = SessionUserId;
            var viewerUnit = AppUtils.GetUserUnit(_userInfosService, userIdViewer);
            var userUnit = AppUtils.GetUserUnit(_userInfosService, userId);
            trainingWeek.UserHeight = Utils.TransformLengthToUnitSytem(userUnit, viewerUnit, trainingWeek.UserHeight);
            trainingWeek.UserWeight = Utils.TransformWeightToUnitSytem(userUnit, viewerUnit, trainingWeek.UserWeight);
            
            var trainingWeekViewModel = TrainingViewModelTransformer.TrainingWeekToViewModel(trainingWeek, _usersService);
            List<TrainingDayViewModel> trainingDayViewModels = null;
            List<TrainingExerciseViewModel> trainingExerciseViewModels = null;
            if (trainingWeek != null && trainingWeek.TrainingDays != null && trainingWeek.TrainingDays.Count > 0)
            {
                trainingDayViewModels = new List<TrainingDayViewModel>();
                foreach (var trainingDay in trainingWeek.TrainingDays)
                {
                    trainingDayViewModels.Add(TrainingViewModelTransformer.TrainingDayToViewModel(trainingDay, userInfo));

                    if (dayOfWeekSelected.HasValue && trainingDay.DayOfWeek == dayOfWeekSelected)
                    { // Get only current
                        var trainingExercises = FindTrainingExercise(trainingDay);
                        if(trainingExercises != null)
                        {
                            foreach (var trainingExercise in trainingExercises)
                            {
                                //Convert user Unit to viewer unit
                                if (trainingExercise.TrainingExerciseSets != null)
                                {
                                    foreach (var set in trainingExercise.TrainingExerciseSets)
                                        set.Weight = Utils.TransformWeightToUnitSytem(userUnit, viewerUnit, set.Weight);
                                }

                                if (trainingExerciseViewModels == null)
                                    trainingExerciseViewModels = new List<TrainingExerciseViewModel>();
                                trainingExerciseViewModels.Add(TrainingViewModelTransformer.TrainingExerciseToViewModel(trainingExercise, _bodyExercisesService));
                            }
                        }
                    }
                }
            }

            ViewBag.UserIdViewer = userIdViewer;
            ViewBag.CurrentDayOfWeek = currentDayOfWeek;
            ViewBag.ViewerUnit = viewerUnit;
            ViewBag.Editable = userId == userIdViewer;
            return View(new Tuple<TrainingWeekViewModel, List<TrainingDayViewModel>, List<TrainingExerciseViewModel>>(trainingWeekViewModel, trainingDayViewModels, trainingExerciseViewModels));
        }

        private List<TrainingExercise> FindTrainingExercise(TrainingDay trainingDay)
        {
            if (trainingDay == null)
                return null;

            var criteria = new TrainingExerciseCriteria()
            {
                Year = new IntegerCriteria() { Equal = trainingDay.Year },
                WeekOfYear = new IntegerCriteria() { Equal = trainingDay.WeekOfYear },
                DayOfWeek = new IntegerCriteria() { Equal = trainingDay.DayOfWeek },
                TrainingDayId = new IntegerCriteria() { Equal = trainingDay.TrainingDayId },
                UserId = new StringCriteria() { Equal = trainingDay.UserId }
            };
            
            return _trainingExercisesService.FindTrainingExercise(criteria);
        }

        private TrainingWeek TransformViewModelToTrainingWeek(TrainingWeekViewModel viewModel)
        {
            TrainingWeek trainingJournal = new TrainingWeek();

            trainingJournal.UserId = viewModel.UserId;
            trainingJournal.Year = viewModel.Year;
            trainingJournal.WeekOfYear = viewModel.WeekOfYear;
            trainingJournal.UserHeight = viewModel.UserHeight;
            trainingJournal.UserWeight = viewModel.UserWeight;
            trainingJournal.Unit = Utils.IntToEnum<TUnitType>(viewModel.Unit);

            return trainingJournal;
        }

        // Create a training day
        // GET: /User/TrainingJournal/CreateTrainingDay
        [HttpGet]
        public IActionResult CreateTrainingDay(string userId, int year, int weekOfYear, int dayOfWeek)
        {
            if (string.IsNullOrWhiteSpace(userId) || year == 0 || weekOfYear == 0 || dayOfWeek < 0 || dayOfWeek > 6 || SessionUserId != userId)
                return RedirectToAction("Index");
            
            var userInfo = _userInfosService.GetUserInfo(new UserInfoKey() { UserId = SessionUserId });
            if (userInfo == null)
                return RedirectToAction("View", new { userId = userId, year = year, weekOfYear = weekOfYear, dayOfWeek = dayOfWeek });
            
            var trainingDayCriteria = new TrainingDayCriteria()
            {
                UserId = new StringCriteria() { Equal = userId },
                Year = new IntegerCriteria() { Equal = year },
                WeekOfYear = new IntegerCriteria() { Equal = weekOfYear }
            };
            var trainingDayScenario = new TrainingDayScenario()
            {
                ManageExercise = false
            };
            var trainingDayList = _trainingDaysService.FindTrainingDay(AppUtils.GetUserUnit(_userInfosService, userId), trainingDayCriteria, trainingDayScenario);

            int trainingDayId = 0;
            if (trainingDayList != null && trainingDayList.Count > 0)
                trainingDayId = trainingDayList.Max(td => td.TrainingDayId) + 1;

            var trainingDay = new TrainingDay()
            {
                UserId = userId,
                Year = year,
                WeekOfYear = weekOfYear,
                DayOfWeek = dayOfWeek,
                TrainingDayId = trainingDayId,
                Unit = userInfo.Unit
            };
            
            ViewBag.Units = ControllerUtils.CreateSelectUnitItemList((int)trainingDay.Unit);
            return View(TrainingViewModelTransformer.TrainingDayToViewModel(trainingDay, userInfo));
        }
        
        // Create a training day
        // GET: /User/TrainingJournal/CreateTrainingDay
        [HttpPost]
        public IActionResult CreateTrainingDay(TrainingDayViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userInfo = _userInfosService.GetUserInfo(new UserInfoKey() { UserId = SessionUserId });
                if (userInfo != null)
                { // default ViewBag value
                    ViewBag.Units = ControllerUtils.CreateSelectUnitItemList((int)userInfo.Unit);
                }
                if (string.IsNullOrWhiteSpace(viewModel.UserId) || viewModel.Year == 0 || viewModel.WeekOfYear == 0 ||
                    viewModel.DayOfWeek < 0 || viewModel.DayOfWeek > 6 || SessionUserId != viewModel.UserId)
                    return View(viewModel);

                // viewModel ViewBag value
                ViewBag.Units = ControllerUtils.CreateSelectUnitItemList(viewModel.Unit);

                //Verify trainingWeek exist
                var trainingWeekKey = new TrainingWeekKey()
                {
                    UserId = viewModel.UserId,
                    Year = viewModel.Year,
                    WeekOfYear = viewModel.WeekOfYear
                };

                var trainingWeekScenario = new TrainingWeekScenario() { ManageTrainingDay = false };
                var trainingWeek = _trainingWeeksService.GetTrainingWeek(trainingWeekKey, trainingWeekScenario);

                if (trainingWeek == null)
                {
                    ModelState.AddModelError(string.Empty, string.Format(Translation.P0_NOT_EXIST, Translation.TRAINING_WEEK));
                    return View(viewModel);
                }

                //Verify valid week of year
                if (viewModel.WeekOfYear > 0 && viewModel.WeekOfYear <= 52)
                {
                    var trainingDay = ControllerUtils.TransformViewModelToTrainingDay(viewModel);
                    trainingDay = _trainingDaysService.CreateTrainingDay(trainingDay);
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
            if (string.IsNullOrWhiteSpace(userId) || year == 0 || weekOfYear == 0 || dayOfWeek < 0 || dayOfWeek > 6 || trainingDayId == 0 || SessionUserId != userId)
                return RedirectToAction("Index");
            
            var key = new TrainingDayKey()
            {
                UserId = userId,
                Year = year,
                WeekOfYear = weekOfYear,
                DayOfWeek = dayOfWeek,
                TrainingDayId = trainingDayId
            };
            var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };
            var trainingDay = _trainingDaysService.GetTrainingDay(key, trainingDayScenario);
            if (trainingDay == null) // no data found
                return RedirectToAction("View", new { userId = userId, year = year, weekOfYear = weekOfYear, dayOfWeek = dayOfWeek });
            
            var userInfo = _userInfosService.GetUserInfo(new UserInfoKey() { UserId = SessionUserId });
            if (userInfo == null)
                userInfo = new UserInfo();
            ViewBag.Units = ControllerUtils.CreateSelectUnitItemList((int)userInfo.Unit);
            return View(TrainingViewModelTransformer.TrainingDayToViewModel(trainingDay, userInfo));
        }

        // Edit a training day
        // GET: /User/TrainingJournal/EditTrainingDay
        [HttpPost]
        public IActionResult EditTrainingDay(TrainingDayViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(viewModel.UserId) || viewModel.Year == 0 || viewModel.WeekOfYear == 0 ||
                    viewModel.DayOfWeek < 0 || viewModel.DayOfWeek > 6 || viewModel.TrainingDayId == 0 || SessionUserId != viewModel.UserId)
                    return View(viewModel);

                ViewBag.Units = ControllerUtils.CreateSelectUnitItemList((int)viewModel.Unit);
                //Verify valide week of year
                if (viewModel.WeekOfYear > 0 && viewModel.WeekOfYear <= 52)
                {
                    var trainingDay = ControllerUtils.TransformViewModelToTrainingDay(viewModel);

                    var key = new TrainingDayKey()
                    {
                        UserId = trainingDay.UserId,
                        Year = trainingDay.Year,
                        WeekOfYear = trainingDay.WeekOfYear,
                        DayOfWeek = trainingDay.DayOfWeek,
                        TrainingDayId = trainingDay.TrainingDayId
                    };
                    // Warning, here need reload with exercise if AutomaticalUnitConversion activated
                    var trainingDayScenario = new TrainingDayScenario() { ManageExercise = viewModel.AutomaticalUnitConversion };
                    var databaseTrainingDay = _trainingDaysService.GetTrainingDay(key, trainingDayScenario);
                    if (databaseTrainingDay == null) // no data found
                    {
                        ModelState.AddModelError(string.Empty, string.Format(Translation.P0_NOT_EXIST, Translation.TRAINING_DAY));
                        return View(viewModel);
                    }
                    
                    if(viewModel.AutomaticalUnitConversion && databaseTrainingDay.Unit != trainingDay.Unit && databaseTrainingDay.TrainingExercises != null)
                    { // Copy exercises references
                        trainingDay.TrainingExercises = databaseTrainingDay.TrainingExercises;
                        _trainingDaysService.ChangeUnitForTrainingExercises(trainingDay, databaseTrainingDay.Unit);
                    }
                    //update trainingDay
                    trainingDay = _trainingDaysService.UpdateTrainingDay(trainingDay, trainingDayScenario);

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
            if (string.IsNullOrWhiteSpace(userId) || year == 0 || weekOfYear == 0 || dayOfWeek < 0 || dayOfWeek > 6 || trainingDayId == 0 || SessionUserId != userId)
                return RedirectToAction("Index");

            if (confirmation)
            {
                var actionResult = RedirectToAction("View", "TrainingJournal", new { Area = "User", userId = userId, year = year, weekOfYear = weekOfYear, dayOfWeekSelected = dayOfWeek });
                var key = new TrainingDayKey()
                {
                    UserId = userId,
                    Year = year,
                    WeekOfYear = weekOfYear,
                    DayOfWeek = dayOfWeek,
                    TrainingDayId = trainingDayId
                };
                var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };
                var trainingDay = _trainingDaysService.GetTrainingDay(key, trainingDayScenario);
                if (trainingDay == null)
                    return actionResult;

                _trainingDaysService.DeleteTrainingDay(trainingDay);
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

        private bool IncorrectHttpData(string userId, int year, int weekOfYear, int dayOfWeek, int trainingDayId, int? trainingExerciseId = null)
        {
            return string.IsNullOrWhiteSpace(userId) || SessionUserId != userId || year == 0 || weekOfYear == 0 ||
                dayOfWeek < 0 || dayOfWeek > 6 || trainingDayId == 0 || (trainingExerciseId != null && !trainingExerciseId.HasValue);
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
                
                var trainingDayKey = new TrainingDayKey() { UserId = viewModel.UserId, Year = viewModel.Year, WeekOfYear = viewModel.WeekOfYear, DayOfWeek = viewModel.DayOfWeek, TrainingDayId = viewModel.TrainingDayId };
                var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };
                var trainingDay = _trainingDaysService.GetTrainingDay(trainingDayKey, trainingDayScenario);

                if(trainingDay == null)
                {
                    CopyViewBagBodyExerciseToViewModel(viewModel, bodyExerciseList);
                    ModelState.AddModelError(string.Empty, string.Format(Translation.P0_NOT_EXIST, Translation.TRAINING_DAY));
                    return View(viewModel);
                }

                if (trainingDay.TrainingExercises == null)
                    trainingDay.TrainingExercises = new List<TrainingExercise>();

                int bodyExerciseCount = trainingDay.TrainingExercises.Count;
                int maxId = 1;
                if (bodyExerciseCount > 0)
                    maxId = trainingDay.TrainingExercises.Max(t => t.Id) + 1;
                TrainingExercise trainingExercise;
                var bodyExerciseSelectedList = viewModel.BodyExerciseList.Where(b => b.Selected == true);
                BodyExercise bodyExercise;
                foreach (var bodyExerciseSelected in bodyExerciseSelectedList)
                {
                    bodyExercise = _bodyExercisesService.GetBodyExercise(new BodyExerciseKey() { Id = bodyExerciseSelected.Id });
                    //Only manage add in this page
                    trainingExercise = new TrainingExercise()
                    {
                        UserId = viewModel.UserId,
                        Year = viewModel.Year,
                        WeekOfYear = viewModel.WeekOfYear,
                        DayOfWeek = viewModel.DayOfWeek,
                        TrainingDayId = viewModel.TrainingDayId,
                        Id = maxId,
                        BodyExerciseId = bodyExerciseSelected.Id,
                        RestTime = 0,
                        EccentricContractionTempo = 1,
                        StretchPositionTempo = 0,
                        ConcentricContractionTempo = 1,
                        ContractedPositionTempo = 0,
                        ExerciseUnitType = bodyExercise?.ExerciseUnitType ?? TExerciseUnitType.RepetitionNumber
                    };
                    trainingDay.TrainingExercises.Add(trainingExercise);
                    maxId++;
                }
                if(bodyExerciseCount != trainingDay.TrainingExercises.Count)
                { //data changed
                    _trainingDaysService.UpdateTrainingDay(trainingDay, trainingDayScenario);
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
            ViewBag.MuscularGroups = ControllerUtils.CreateSelectMuscularGroupItemList(_muscularGroupsService.FindMuscularGroups(), currentMuscularGroupId, true);
            
            if (currentMuscularGroupId == 0)
            { // All exercises
                bodyExerciseList = _bodyExercisesService.FindBodyExercises();
            }
            else
            {
                var muscleCriteria = new MuscleCriteria()
                {
                    MuscularGroupId = new IntegerCriteria() { Equal = currentMuscularGroupId }
                };
                var muscleList = _musclesService.FindMuscles(muscleCriteria);
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
                        bodyExerciseList = _bodyExercisesService.FindBodyExercises(bodyExerciseCriteria);
                    }
                    else //Security
                        bodyExerciseList = _bodyExercisesService.FindBodyExercises();
                }
                else
                {
                    var bodyExerciseCriteria = new BodyExerciseCriteria()
                    {
                        MuscleId = new IntegerCriteria() { Equal = currentMuscleId }
                    };
                    bodyExerciseList = _bodyExercisesService.FindBodyExercises(bodyExerciseCriteria);
                }
            }
        }

        private void OrderTrainingExercices(List<TrainingExercise> trainingExercises, int index, bool upward)
        {
            if (upward && index == 0)
                return;
            if (!upward && index == (trainingExercises.Count -1))
                return;

            var trainingExercise = trainingExercises[index];
            trainingExercises.RemoveAt(index);
            if (upward)
                trainingExercises.Insert(index - 1, trainingExercise);
            else
                trainingExercises.Insert(index + 1, trainingExercise);

            //Parse and change index of exercise
            for (int i = 0; i < trainingExercises.Count; i++)
            {
                index = i + 1;
                trainingExercise = trainingExercises[i];
                trainingExercise.Id = index;

                if (trainingExercise.TrainingExerciseSets != null)
                {
                    foreach (var set in trainingExercise.TrainingExerciseSets)
                    {
                        set.TrainingExerciseId = index;
                    }
                }
            }
        }

        // Add a training exercise
        // GET: /User/TrainingJournal/EditTrainingExercise
        [HttpGet]
        public IActionResult EditTrainingExercise(string userId, int year, int weekOfYear, int dayOfWeek, int trainingDayId,
            int trainingExerciseId, bool upward=false, bool downward=false)
        {
            if (IncorrectHttpData(userId, year, weekOfYear, dayOfWeek, trainingDayId, trainingExerciseId))
                return RedirectToAction("Index");

            var actionResult = GetViewActionResult(userId, year, weekOfYear, dayOfWeek);

            if (upward || downward)
            {
                var findcriteria = new TrainingExerciseCriteria()
                {
                    UserId = new StringCriteria() { Equal = userId },
                    Year = new IntegerCriteria() { Equal = year },
                    WeekOfYear = new IntegerCriteria() { Equal = weekOfYear },
                    DayOfWeek = new IntegerCriteria() { Equal = dayOfWeek },
                    TrainingDayId = new IntegerCriteria() { Equal = trainingDayId }
                };
                var trainingExercises = _trainingExercisesService.FindTrainingExercise(findcriteria);
                if (trainingExercises == null || trainingExercises.Count == 0)
                    return actionResult;
                
                trainingExercises = trainingExercises.OrderBy(t => t.Id).ToList();
                int indexOfCurrentExercice = trainingExercises.FindIndex(t => t.Id == trainingExerciseId);
                if (indexOfCurrentExercice == -1)
                    return actionResult;

                foreach (var trainingExerciseTmp in trainingExercises)
                    _trainingExercisesService.DeleteTrainingExercise(trainingExerciseTmp);

                OrderTrainingExercices(trainingExercises, indexOfCurrentExercice, upward == true);

                foreach (var trainingExerciseTmp in trainingExercises)
                    _trainingExercisesService.CreateTrainingExercise(trainingExerciseTmp);

                return actionResult;
            }

            var trainingDayScenario = new TrainingDayScenario()
            {
                ManageExercise = false
            };
            var trainingDayKey = new TrainingDayKey()
            {
                UserId = userId,
                Year = year,
                WeekOfYear = weekOfYear,
                DayOfWeek = dayOfWeek,
                TrainingDayId = trainingDayId
            };
            var trainingDay = _trainingDaysService.GetTrainingDay(trainingDayKey, trainingDayScenario);
            if (trainingDay == null)
                return actionResult;

            var key = new TrainingExerciseKey()
            {
                UserId = userId,
                Year = year,
                WeekOfYear = weekOfYear,
                DayOfWeek = dayOfWeek,
                TrainingDayId = trainingDayId,
                Id = trainingExerciseId
            };
            var trainingExercise = _trainingExercisesService.GetTrainingExercise(key);
            if (trainingExercise == null)
                return actionResult;

            var bodyExercise = _bodyExercisesService.GetBodyExercise(new BodyExerciseKey() { Id = trainingExercise.BodyExerciseId });

            var viewModel = new TrainingExerciseViewModel()
            {
                UserId = userId,
                Year = year,
                WeekOfYear = weekOfYear,
                DayOfWeek = dayOfWeek,
                TrainingDayId = trainingDayId,
                TrainingExerciseId = trainingExerciseId,
                BodyExerciseId = bodyExercise.Id,
                BodyExerciseName = bodyExercise.Name,
                BodyExerciseImage = bodyExercise.ImageName,
                ExerciseUnitType = (int)trainingExercise.ExerciseUnitType,
                RestTime = trainingExercise.RestTime,
                EccentricContractionTempo = trainingExercise.EccentricContractionTempo,
                StretchPositionTempo = trainingExercise.StretchPositionTempo,
                ConcentricContractionTempo = trainingExercise.ConcentricContractionTempo,
                ContractedPositionTempo = trainingExercise.ContractedPositionTempo,
                Unit = trainingDay.Unit
            };

            if (trainingExercise.TrainingExerciseSets != null)
            {
                foreach (var trainingExerciseSet in trainingExercise.TrainingExerciseSets)
                {
                    for(int i=0; i < trainingExerciseSet.NumberOfSets; i++)
                    {
                        viewModel.Reps.Add(trainingExerciseSet.NumberOfReps);
                        viewModel.ExecutionTimes.Add(trainingExerciseSet.ExecutionTime);
                        viewModel.Weights.Add(trainingExerciseSet.Weight);
                    }
                }
            }

            if (viewModel.ExerciseUnitType == (int)TExerciseUnitType.RepetitionNumber)
            {
                viewModel.ExecutionTimes = null; // Security
                if (viewModel.Reps == null || viewModel.Reps.Count == 0) // Security
                    viewModel.Reps = new List<int?>() { 8 };
            }
            else
            {
                viewModel.Reps = null; // Security
                if (viewModel.ExecutionTimes == null || viewModel.ExecutionTimes.Count == 0) //Security
                    viewModel.ExecutionTimes = new List<int?>() { 30 };
            }

            if (viewModel.Weights == null || viewModel.Weights.Count == 0)
                viewModel.Weights = new List<double?>() { 0 };
            
            ViewBag.ExerciseUnitTypes = ControllerUtils.CreateSelectExerciseUnitTypeItemList(viewModel.ExerciseUnitType);

            return View(viewModel);
        }

        // Add a training exercise
        // POST: /User/TrainingJournal/EditTrainingExercise
        [HttpPost]
        public IActionResult EditTrainingExercise(TrainingExerciseViewModel viewModel, string buttonType)
        {
            const int MAX_LINES = 10;
            if(viewModel == null)
                return RedirectToAction("Index");
            
            ViewBag.ExerciseUnitTypes = ControllerUtils.CreateSelectExerciseUnitTypeItemList((int)viewModel.ExerciseUnitType);

            bool reinitWeights = false;
            List<int?> repsOrExecutionTimeList;
            if (viewModel.ExerciseUnitType == (int)TExerciseUnitType.RepetitionNumber)
            {
                viewModel.ExecutionTimes = null; // Security
                if (viewModel.Reps == null || viewModel.Reps.Count == 0) //Security
                {
                    viewModel.Reps = new List<int?>() { 8, 8, 8, 8 };
                    reinitWeights = true;
                }

                repsOrExecutionTimeList = viewModel.Reps;
            }
            else
            {
                viewModel.Reps = null; // Security
                if (viewModel.ExecutionTimes == null || viewModel.ExecutionTimes.Count == 0) //Security
                {
                    viewModel.ExecutionTimes = new List<int?>() { 30 }; // 1 X 30 second
                    reinitWeights = true;
                }

                repsOrExecutionTimeList = viewModel.ExecutionTimes;
            }
            
            while (repsOrExecutionTimeList.Count > MAX_LINES)
            {
                repsOrExecutionTimeList.RemoveAt(repsOrExecutionTimeList.Count - 1);
            }

            if (viewModel.Weights == null || reinitWeights)
                viewModel.Weights = new List<double?>();
            
            while (viewModel.Weights.Count > MAX_LINES)
            {
                viewModel.Weights.RemoveAt(viewModel.Weights.Count - 1);
            }

            while (viewModel.Weights.Count < repsOrExecutionTimeList.Count)
            {
                viewModel.Weights.Add(0);
            }

            // initialize values
            for (int i = 0; i < repsOrExecutionTimeList.Count; i++)
            {
                if(repsOrExecutionTimeList[i] == null)
                    repsOrExecutionTimeList[i] = 0;
                if (viewModel.Weights[i] == null)
                    viewModel.Weights[i] = 0;
            }

            if ("addRep" == buttonType)
            {
                if (repsOrExecutionTimeList.Count < MAX_LINES)
                {
                    //Add previous value inside added value
                    int newAddedValue = (viewModel.ExerciseUnitType == (int)TExerciseUnitType.RepetitionNumber) ? 8 : 30; // default value (8 rep or 30 sec)
                    if (repsOrExecutionTimeList.Count > 0)
                        newAddedValue = repsOrExecutionTimeList[repsOrExecutionTimeList.Count - 1].Value;
                    repsOrExecutionTimeList.Add(newAddedValue);

                    double newWeightValue = 8;
                    if (viewModel.Weights.Count > 0)
                        newWeightValue = viewModel.Weights[viewModel.Weights.Count - 1].Value;
                    viewModel.Weights.Add(newWeightValue);
                }
                return View(viewModel);
            }
            else if ("delete" == buttonType)
            {
                if (repsOrExecutionTimeList.Count > 1)
                    repsOrExecutionTimeList.RemoveAt(repsOrExecutionTimeList.Count - 1);
                if (viewModel.Weights.Count > 1)
                    viewModel.Weights.RemoveAt(viewModel.Weights.Count - 1);
            }
            else if ("submit" == buttonType && ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(viewModel.UserId) || SessionUserId != viewModel.UserId || viewModel.Year == 0 || viewModel.WeekOfYear == 0 ||
                    viewModel.DayOfWeek < 0 || viewModel.DayOfWeek > 6 || viewModel.TrainingDayId == 0 || viewModel.TrainingExerciseId == 0 ||
                    viewModel.BodyExerciseId == 0)
                    return View(viewModel);
                
                var key = new TrainingExerciseKey()
                {
                    UserId = viewModel.UserId,
                    Year = viewModel.Year,
                    WeekOfYear = viewModel.WeekOfYear,
                    DayOfWeek = viewModel.DayOfWeek,
                    TrainingDayId = viewModel.TrainingDayId,
                    Id = viewModel.TrainingExerciseId
                };
                var trainingExercise = _trainingExercisesService.GetTrainingExercise(key);
                if (trainingExercise == null)
                {
                    ModelState.AddModelError(string.Empty, string.Format("{0} {1}", Translation.INVALID_INPUT_2P, Translation.TRAINING_EXERCISE));
                    return View(viewModel);
                }

                trainingExercise.BodyExerciseId = viewModel.BodyExerciseId;
                trainingExercise.RestTime = viewModel.RestTime;
                trainingExercise.ExerciseUnitType = Utils.IntToEnum<TExerciseUnitType>(viewModel.ExerciseUnitType);

                //Tempos
                trainingExercise.EccentricContractionTempo = viewModel.EccentricContractionTempo;
                trainingExercise.StretchPositionTempo = viewModel.StretchPositionTempo;
                trainingExercise.ConcentricContractionTempo = viewModel.ConcentricContractionTempo;
                trainingExercise.ContractedPositionTempo = viewModel.ContractedPositionTempo;

                if (repsOrExecutionTimeList != null && repsOrExecutionTimeList.Count > 0)
                {
                    //Regroup Reps with Set
                    int nbSet = 0, currentRepOrExecTimeValue = 0;
                    var tupleRegroupList = new List<Tuple<int, int, double>>(); // NumberOfSet, NumberOfRepetition or Execution time, Weight
                    int repOrExecTimeValue;
                    double weightValue, currentWeightValue = 0;
                    for (int i=0; i < repsOrExecutionTimeList.Count; i++)
                    {
                        repOrExecTimeValue = repsOrExecutionTimeList[i].Value;
                        weightValue = viewModel.Weights[i].Value;
                        if (repOrExecTimeValue == 0)
                            continue;

                        if (weightValue == currentWeightValue && repOrExecTimeValue == currentRepOrExecTimeValue)
                            nbSet++;
                        else
                        {
                            if (nbSet != 0)
                                tupleRegroupList.Add(new Tuple<int, int, double>(nbSet, currentRepOrExecTimeValue, currentWeightValue));
                            currentRepOrExecTimeValue = repOrExecTimeValue;
                            currentWeightValue = weightValue;
                            nbSet = 1;
                        }
                    }

                    //last data
                    if (nbSet != 0)
                        tupleRegroupList.Add(new Tuple<int, int, double>(nbSet, currentRepOrExecTimeValue, currentWeightValue));

                    trainingExercise.TrainingExerciseSets = new List<TrainingExerciseSet>();
                    int id = 1;
                    foreach (Tuple<int, int, double> tupleSetRep in tupleRegroupList)
                    {
                        trainingExercise.TrainingExerciseSets.Add(new TrainingExerciseSet()
                        {
                            UserId = viewModel.UserId,
                            Year = viewModel.Year,
                            WeekOfYear = viewModel.WeekOfYear,
                            DayOfWeek = viewModel.DayOfWeek,
                            TrainingDayId = viewModel.TrainingDayId,
                            TrainingExerciseId = viewModel.TrainingExerciseId,
                            Id = id,
                            NumberOfSets = tupleSetRep.Item1,
                            NumberOfReps = (viewModel.ExerciseUnitType == (int)TExerciseUnitType.RepetitionNumber) ? tupleSetRep.Item2 : 0,
                            ExecutionTime = (viewModel.ExerciseUnitType == (int)TExerciseUnitType.Time) ? tupleSetRep.Item2 : 0,
                            Weight = tupleSetRep.Item3
                        });
                        id++;
                    }

                    _trainingExercisesService.UpdateTrainingExercise(trainingExercise, true);

                    return RedirectToAction("View", "TrainingJournal", new { Area = "User", userId = viewModel.UserId, year = viewModel.Year, weekOfYear = viewModel.WeekOfYear, dayOfWeekSelected = viewModel.DayOfWeek });

                }
            }
            return View(viewModel);
        }
        
        // Delete a training journals
        // GET: /User/TrainingJournal/DeleteTrainingDay
        [HttpGet]
        public IActionResult DeleteTrainingExercise(string userId, int year, int weekOfYear, int dayOfWeek, int trainingDayId, int trainingExerciseId, bool confirmation = false)
        {
            if (string.IsNullOrWhiteSpace(userId) || year == 0 || weekOfYear == 0 || dayOfWeek < 0 || dayOfWeek > 6 || trainingDayId == 0 || trainingExerciseId == 0 || SessionUserId != userId)
                return RedirectToAction("Index");

            if (confirmation)
            {
                var actionResult = RedirectToAction("View", "TrainingJournal", new { Area = "User", userId = userId, year = year, weekOfYear = weekOfYear, dayOfWeekSelected = dayOfWeek });
                var key = new TrainingExerciseKey()
                {
                    UserId = userId,
                    Year = year,
                    WeekOfYear = weekOfYear,
                    DayOfWeek = dayOfWeek,
                    TrainingDayId = trainingDayId,
                    Id = trainingExerciseId
                };
                var trainingExercise = _trainingExercisesService.GetTrainingExercise(key);
                if (trainingExercise == null)
                    return actionResult;

                _trainingExercisesService.DeleteTrainingExercise(trainingExercise);

                return actionResult;
            }
            else
            {
                string title = Translation.TRAINING_DAY;
                string message = Translation.ARE_YOU_SURE_YOU_WANNA_DELETE_THIS_ELEMENT_PI;
                string returnUrlYes = Url.Action("DeleteTrainingExercise", "TrainingJournal", new { Area = "User", userId = userId, year = year, weekOfYear = weekOfYear, dayOfWeek = dayOfWeek, trainingDayId = trainingDayId, trainingExerciseId = trainingExerciseId, confirmation = true });
                string returnUrlNo = Url.Action("View", "TrainingJournal", new { Area = "User", userId = userId, year = year, weekOfYear = weekOfYear, dayOfWeekSelected = dayOfWeek });
                return RedirectToAction("Confirm", "Message", new { Area = "Site", title = title, message = message, returnUrlYes = returnUrlYes, returnUrlNo = returnUrlNo });
            }
        }

        // Create a training day
        // GET: /User/TrainingJournal/CreateTrainingDay
        [HttpGet]
        public IActionResult SwitchTrainingDay(string userId, int year, int weekOfYear, int dayOfWeek, int dayOfWeekSelected)
        {
           if (string.IsNullOrWhiteSpace(userId) || year == 0 || weekOfYear == 0 || dayOfWeek < 0 || dayOfWeek > 6 ||
                dayOfWeekSelected < 0 || dayOfWeekSelected > 6 || SessionUserId != userId)
                return RedirectToAction("Index");

            var actionResult = RedirectToAction("View", "TrainingJournal", new { Area = "User", userId = userId, year = year, weekOfYear = weekOfYear, dayOfWeekSelected = dayOfWeek });

            try
            {
                _trainingDaysService.SwitchDayOnTrainingDay(userId, year, weekOfYear, dayOfWeek, dayOfWeekSelected);
            }
            catch(Exception except)
            {
                _logger.LogError("Unable to switch day of training day", except);
            }
            return actionResult;
        }

        // Create a training day
        // GET: /User/TrainingJournal/CopyTrainingDay
        [HttpGet]
        public IActionResult CopyTrainingDay(string userId, int year, int weekOfYear, int dayOfWeek, int dayOfWeekSelected)
        {
            if (string.IsNullOrWhiteSpace(userId) || year == 0 || weekOfYear == 0 || dayOfWeek < 0 || dayOfWeek > 6 ||
                 dayOfWeekSelected < 0 || dayOfWeekSelected > 6 || SessionUserId != userId)
                return RedirectToAction("Index");

            var actionResult = RedirectToAction("View", "TrainingJournal", new { Area = "User", userId = userId, year = year, weekOfYear = weekOfYear, dayOfWeekSelected = dayOfWeek });

            try
            {
                _trainingDaysService.CopyDayOnTrainingDay(userId, year, weekOfYear, dayOfWeek, dayOfWeekSelected);
            }
            catch (Exception except)
            {
                _logger.LogError("Unable to copy day of training day", except);
            }
            return actionResult;
        }
    }
}
