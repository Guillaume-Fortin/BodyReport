using System;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BodyReport.Models;
using BodyReport.Manager;
using Message;
using Message.WebApi;
using Message.WebApi.MultipleParameters;
using System.Security.Claims;
using BodyReport.Areas.User.ViewModels;
using BodyReport.Resources;
using BodyReport.Framework;
using BodyReport.Services;

namespace BodyReport.Areas.Api.Controllers
{
	[Area("Api")]
    public class TrainingDaysController : Controller
    {
        /// <summary>
		/// Database db context
		/// </summary>
		ApplicationDbContext _dbContext = null;
        TrainingDayManager _manager = null;

        public TrainingDaysController(ApplicationDbContext dbContext)
        {
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
                    return HttpBadRequest();
                var trainingday = _manager.GetTrainingDay(trainingDayKey, manageExercise);
                return new HttpOkObjectResult(trainingday);
            }
            catch (Exception exception)
            {
                return HttpBadRequest(new WebApiException("Error", exception));
            }
        }

        // Post api/TrainingDays/Find
        [HttpPost]
        public IActionResult Find([FromBody]TrainingDayFinder trainingDayFinder)
        {
            try
            {
                if (trainingDayFinder == null)
                    return HttpBadRequest();

                var trainingDayCriteria = trainingDayFinder.TrainingDayCriteria;
                var trainingDayScenario = trainingDayFinder.TrainingDayScenario;

                if (trainingDayCriteria == null || trainingDayCriteria.UserId == null)
                    return HttpBadRequest();
                return new HttpOkObjectResult(_manager.FindTrainingDay(trainingDayCriteria, trainingDayScenario));
            }
            catch (Exception exception)
            {
                return HttpBadRequest(new WebApiException("Error", exception));
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
                        viewModel.DayOfWeek < 0 || viewModel.DayOfWeek > 6 || User.GetUserId() != viewModel.UserId)
                        return HttpBadRequest();

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
                        return HttpBadRequest(new WebApiException(string.Format(Translation.P0_NOT_EXIST, Translation.TRAINING_WEEK)));

                    //Verify valid week of year
                    if (viewModel.WeekOfYear > 0 && viewModel.WeekOfYear <= 52)
                    {
                        var trainingDay = ControllerUtils.TransformViewModelToTrainingDay(viewModel);
                        TrainingDayService service = new TrainingDayService(_dbContext);
                        trainingDay = service.CreateTrainingDay(trainingDay);
                        if (trainingDay == null)
                            return HttpBadRequest(new WebApiException(string.Format(Translation.IMPOSSIBLE_TO_CREATE_P0, Translation.TRAINING_DAY)));
                        else
                            return new HttpOkObjectResult(trainingDay);
                    }
                    else
                        return HttpBadRequest();
                }
                else
                    return HttpBadRequest(new WebApiException(ControllerUtils.GetModelStateError(ModelState)));
            }
            catch (Exception exception)
            {
                return HttpBadRequest(new WebApiException("Error", exception));
            }
        }
    }
}
