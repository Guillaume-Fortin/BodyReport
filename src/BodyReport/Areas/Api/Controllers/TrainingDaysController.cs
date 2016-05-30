using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BodyReport.Areas.User.ViewModels;
using BodyReport.Framework;
using BodyReport.Models;
using BodyReport.Manager;
using BodyReport.Resources;
using BodyReport.Services;
using BodyReport.Message;
using BodyReport.Message.WebApi;
using BodyReport.Message.WebApi.MultipleParameters;
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
        public IActionResult Get(TrainingDayKey trainingDayKey, TrainingDayScenario trainingDayScenario)
        {
            try
            {
                if (trainingDayKey == null || trainingDayScenario == null)
                    return BadRequest();
                var trainingday = _manager.GetTrainingDay(trainingDayKey, trainingDayScenario);
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
                    var trainingWeekScenario = new TrainingWeekScenario()  { ManageTrainingDay = false };
                    var trainingWeek = trainingWeekManager.GetTrainingWeek(trainingWeekKey, trainingWeekScenario);

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

        // Post api/TrainingDays/Update
        [HttpPost]
        public IActionResult Update([FromBody]TrainingDayWithScenario trainingDayWithScenario)
        {
            try
            {
                if (trainingDayWithScenario == null || trainingDayWithScenario.TrainingDay == null || trainingDayWithScenario.TrainingDayScenario == null)
                    return BadRequest();

                var trainingDay = trainingDayWithScenario.TrainingDay;
                var trainingDayScenario = trainingDayWithScenario.TrainingDayScenario;
                if (string.IsNullOrWhiteSpace(trainingDay.UserId) || trainingDay.Year == 0 || trainingDay.WeekOfYear == 0 ||
                    trainingDay.DayOfWeek < 0 || trainingDay.DayOfWeek > 6 || _userManager.GetUserId(User) != trainingDay.UserId)
                    return BadRequest();


                //Verify valid week of year
                if (trainingDay.WeekOfYear > 0 && trainingDay.WeekOfYear <= 52)
                {
                    //Verify trainingWeek exist
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            TrainingDayService service = new TrainingDayService(_dbContext);
                            trainingDay = service.UpdateTrainingDay(trainingDay, trainingDayScenario);
                            transaction.Commit();

                            if (trainingDay == null)
                                return BadRequest(new WebApiException(string.Format(Translation.IMPOSSIBLE_TO_UPDATE_P0, Translation.TRAINING_DAY)));
                            else
                                return new OkObjectResult(trainingDay);
                        }
                        catch (Exception exception)
                        {
                            //_logger.LogCritical("Unable to delete training week", exception);
                            transaction.Rollback();
                            throw exception;
                        }
                    }
                }
                else
                    return BadRequest();
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }
    }
}
