using BodyReport.Manager;
using BodyReport.Models;
using Message;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BodyReport.Areas.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    //[AllowAnonymous]
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

        // Get api/BodyExercises/Get
        [HttpGet]
        public BodyExercise Get(BodyExerciseKey key)
        {
            return _bodyExerciseManager.GetBodyExercise(key);
        }

        // Get api/BodyExercises/Find
        [HttpGet]
        public List<BodyExercise> Find(BodyExerciseCriteria criteria)
        {
            return _bodyExerciseManager.FindBodyExercises(criteria);
        }

        // POST api/BodyExercises/Post
        [HttpPost]
        public List<BodyExercise> Post([FromBody]List<BodyExercise> bodyExercises)
        {
            List<BodyExercise> results = new List<BodyExercise>();
            if (bodyExercises != null && bodyExercises.Count() > 0)
            {
                foreach (var bodyExercise in bodyExercises)
                {
                    results.Add(_bodyExerciseManager.UpdateBodyExercise(bodyExercise));
                }
            }
            return results;
        }

        // DELETE api/BodyExercises/Delete
        [HttpDelete]
        public void Delete(BodyExerciseKey key)
        {
            _bodyExerciseManager.DeleteBodyExercise(key);
        }
    }
}
