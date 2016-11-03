using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BodyReport.Message;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Data;
using System;
using BodyReport.Message.WebApi;

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
        BodyExerciseManager _bodyExerciseManager = null;

        public BodyExercisesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _bodyExerciseManager = new BodyExerciseManager(_dbContext);
        }

        // POST api/BodyExercises/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody]BodyExercise bodyExercise)
        {
            try
            {
                BodyExercise result = null;
                if (bodyExercise != null)
                {
                    result = _bodyExerciseManager.CreateBodyExercise(bodyExercise);
                }
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
                var result = _bodyExerciseManager.GetBodyExercise(key);
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
                var result = _bodyExerciseManager.FindBodyExercises(criteria);
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
                BodyExercise result = null;
                if (bodyExercise != null)
                {
                    result = _bodyExerciseManager.UpdateBodyExercise(bodyExercise);
                }
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
                List<BodyExercise> results = new List<BodyExercise>();
                if (bodyExercises != null && bodyExercises.Count() > 0)
                {
                    foreach (var bodyExercise in bodyExercises)
                    {
                        results.Add(_bodyExerciseManager.UpdateBodyExercise(bodyExercise));
                    }
                }
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
                _bodyExerciseManager.DeleteBodyExercise(key);
                return new OkObjectResult(true); // bool
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }
    }
}
