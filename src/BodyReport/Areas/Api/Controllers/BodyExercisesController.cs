using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BodyReport.Message;
using BodyReport.Data;
using System;
using BodyReport.Message.Web;
using BodyReport.ServiceLayers.Interfaces;

namespace BodyReport.Areas.Api.Controllers
{
    [Authorize]
    [Area("Api")]
    public class BodyExercisesController : Controller
    {
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
        /// <summary>
        /// ServiceLayer BodyExercisesService
        /// </summary>
        IBodyExercisesService _bodyExercisesService = null;

        public BodyExercisesController(ApplicationDbContext dbContext, IBodyExercisesService bodyExercisesService)
        {
            _dbContext = dbContext;
            _bodyExercisesService = bodyExercisesService;
        }

        // POST api/BodyExercises/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody]BodyExercise bodyExercise)
        {
            try
            {
                BodyExercise result = _bodyExercisesService.CreateBodyExercise(bodyExercise);
                return new OkObjectResult(result); // BodyExercise
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // Get api/BodyExercises/Get
        [HttpGet]
        public IActionResult Get(BodyExerciseKey key)
        {
            try
            {
                var result = _bodyExercisesService.GetBodyExercise(key);
                return new OkObjectResult(result); // BodyExercise
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // Get api/BodyExercises/Find
        [HttpGet]
        public IActionResult Find(BodyExerciseCriteria criteria)
        {
            try
            {
                var result = _bodyExercisesService.FindBodyExercises(criteria);
                return new OkObjectResult(result); // List<BodyExercise>
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // POST api/BodyExercises/Update
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Update([FromBody]BodyExercise bodyExercise)
        {
            try
            {
                BodyExercise result = _bodyExercisesService.UpdateBodyExercise(bodyExercise);
                return new OkObjectResult(result); // BodyExercise
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // POST api/BodyExercises/UpdateList
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateList([FromBody]List<BodyExercise> bodyExercises)
        {
            try
            {
                List<BodyExercise> results = _bodyExercisesService.UpdateBodyExerciseList(bodyExercises);
                return new OkObjectResult(results); // List<BodyExercise>
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // POST api/BodyExercises/
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete([FromBody]BodyExerciseKey key)
        {
            try
            {
                _bodyExercisesService.DeleteBodyExercise(key);
                return new OkObjectResult(true); // bool
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }
    }
}
