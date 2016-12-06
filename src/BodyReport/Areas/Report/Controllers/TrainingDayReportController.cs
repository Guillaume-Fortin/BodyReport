using BodyReport.Areas.User.ViewModels;
using BodyReport.Areas.User.ViewModels.Transformer;
using BodyReport.Framework;
using BodyReport.Message;
using BodyReport.Models;
using BodyReport.ServiceLayers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace BodyReport.Areas.Report.Controllers
{
    [Area("Report")]
    [Authorize(Policy = "AllowLocalhost")]
    public class TrainingDayReportController : MvcController
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(TrainingDayReportController));
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

        public TrainingDayReportController(UserManager<ApplicationUser> userManager,
                                           IUsersService usersService,
                                           IUserInfosService userInfosService,
                                           ITrainingWeeksService trainingWeeksService,
                                           ITrainingDaysService trainingDaysService,
                                           ITrainingExercisesService trainingExercisesService,
                                           IBodyExercisesService bodyExercisesService) : base(userManager)
        {
            _usersService = usersService;
            _userInfosService = userInfosService;
            _trainingWeeksService = trainingWeeksService;
            _trainingDaysService = trainingDaysService;
            _trainingExercisesService = trainingExercisesService;
            _bodyExercisesService = bodyExercisesService;
        }

        private TUnitType GetUserUnit(string userId)
        {
            TUnitType result = TUnitType.Imperial;

            if (userId != null)
            {
                var userInfo = _userInfosService.GetUserInfo(new UserInfoKey() { UserId = userId });
                if (userInfo != null)
                    result = userInfo.Unit;
            }
            return result;
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

        //
        // GET: /Report/TrainingDayReport/Index
        public IActionResult Index(string userId, int year, int weekOfYear, int dayOfWeek, int? trainingDayId, bool displayImages, string userIdViewer)
        {
            var userInfo = _userInfosService.GetUserInfo(new UserInfoKey() { UserId = userId });
            if (userInfo == null)
                userInfo = new UserInfo();

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
                trainingWeek = new TrainingWeek();

            //Unit viewer convertion
            if (string.IsNullOrEmpty(userIdViewer))
            {
                userIdViewer = SessionUserId;
            }
            var viewerUnit = GetUserUnit(userIdViewer);
            var userUnit = GetUserUnit(userId);
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
                    if (!trainingDayId.HasValue || trainingDay.TrainingDayId == trainingDayId)
                    {
                        if (trainingDay.DayOfWeek == dayOfWeek)
                        { // Get only current
                            trainingDayViewModels.Add(TrainingViewModelTransformer.TrainingDayToViewModel(trainingDay, userInfo));

                            var trainingExercises = FindTrainingExercise(trainingDay);
                            if (trainingExercises != null)
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
            }

            ViewBag.DayOfWeek = dayOfWeek;
            ViewBag.displayImages = displayImages;
            ViewBag.ViewerUnit = viewerUnit;
            return View(new Tuple<TrainingWeekViewModel, List<TrainingDayViewModel>, List<TrainingExerciseViewModel>>(trainingWeekViewModel, trainingDayViewModels, trainingExerciseViewModels));
        }

        //
        // GET: /Report/TrainingDayReport/Pdf
        public IActionResult Pdf(string userId, int year, int weekOfYear, int dayOfWeek, int? trainingDayId, bool displayImages)
        {
            string outputFileName = string.Format("TrainingDayReport_{0}_{1}_{2}_{3}.pdf", year, weekOfYear, dayOfWeek, trainingDayId.HasValue ? trainingDayId.Value.ToString() : "all");
            string reportPath = System.IO.Path.Combine("trainingDay", userId);
			string reportUrl = string.Format("http://localhost:5000/Report/TrainingDayReport/Index?userId={0}&year={1}&weekOfYear={2}&dayOfWeek={3}&trainingDayId{4}&displayImages={5}&culture={6}&userIdViewer={7}",
                                             userId, year, weekOfYear, dayOfWeek, trainingDayId.HasValue ? trainingDayId.Value.ToString() : "null", displayImages, CultureInfo.CurrentUICulture.ToString(), SessionUserId); 
            return RedirectToAction("Pdf", "Report", new { area = "Report", reportPath = reportPath, reportUrl = reportUrl, outputFileName = outputFileName });
        }
    }
}
