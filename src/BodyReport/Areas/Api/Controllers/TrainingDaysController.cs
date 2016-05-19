using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BodyReport.Areas.User.ViewModels;
using BodyReport.Framework;
using BodyReport.Models;
using BodyReport.Manager;
using BodyReport.Resources;
using BodyReport.Services;
using Message;
using Message.WebApi;
using Message.WebApi.MultipleParameters;
using BodyReport.Data;

namespace BodyReport.Areas.Api.Controllers
{
	[Area("Api")]
    public class TrainingDaysController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        /// <summary>
		/// Database db context
		/// </summary>
		ApplicationDbContext _dbContext = null;
        TrainingDayManager _manager = null;

        public TrainingDaysController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _manager = new TrainingDayManager(_dbContext);
        }

        // Get api/TrainingDays/Get
        [HttpGet]
        public IActionResult Get(TrainingDayKey trainingDayKey, bool manageExercise = false)
        {
            try
            {
                if (trainingDayKey == null)
                    return BadRequest();
                var trainingday = _manager.GetTrainingDay(trainingDayKey, manageExercise);
                return new OkObjectResult(trainingday);
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // Post api/TrainingDays/Find
        [HttpPost]
        public IActionResult Find([FromBody]TrainingDayFinder trainingDayFinder)
        {
            try
            {
                if (trainingDayFinder == null)
                    return BadRequest();

                var trainingDayCriteria = trainingDayFinder.TrainingDayCriteria;
                var trainingDayScenario = trainingDayFinder.TrainingDayScenario;

                if (trainingDayCriteria == null || trainingDayCriteria.UserId == null)
                    return BadRequest();
                return new OkObjectResult(_manager.FindTrainingDay(trainingDayCriteria, trainingDayScenario));
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // Post api/TrainingDays/Create
        [HttpPost]
        public IActionResult Create([FromBody]TrainingDayViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrWhiteSpace(viewModel.UserId) || viewModel.Year == 0 || viewModel.WeekOfYear == 0 ||
                        viewModel.DayOfWeek < 0 || viewModel.DayOfWeek > 6 || _userManager.GetUserId(User) != viewModel.UserId)
                        return BadRequest();

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
                        return BadRequest(new WebApiException(string.Format(Translation.P0_NOT_EXIST, Translation.TRAINING_WEEK)));

                    //Verify valid week of year
                    if (viewModel.WeekOfYear > 0 && viewModel.WeekOfYear <= 52)
                    {
                        var trainingDay = ControllerUtils.TransformViewModelToTrainingDay(viewModel);
                        TrainingDayService service = new TrainingDayService(_dbContext);
                        trainingDay = service.CreateTrainingDay(trainingDay);
                        if (trainingDay == null)
                            return BadRequest(new WebApiException(string.Format(Translation.IMPOSSIBLE_TO_CREATE_P0, Translation.TRAINING_DAY)));
                        else
                            return new OkObjectResult(trainingDay);
                    }
                    else
                        return BadRequest();
                }
                else
                    return BadRequest(new WebApiException(ControllerUtils.GetModelStateError(ModelState)));
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }
    }
}
