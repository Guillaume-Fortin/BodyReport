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
    public class MusclesController : Controller
    {
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
        MuscleManager _manager = null;

        public MusclesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _manager = new MuscleManager(_dbContext);
        }

        // Get api/Muscle/Get
        [HttpGet]
        public Muscle Get(MuscleKey key)
        {
            return _manager.GetMuscle(key);
        }

        // Get api/Muscle/Find
        [HttpGet]
        public List<Muscle> Find(MuscleCriteria criteria)
        {
            return _manager.FindMuscles(criteria);
        }

        // POST api/Muscle/Post
        [HttpPost]
        public List<Muscle> Post([FromBody]List<Muscle> muscles)
        {
            List<Muscle> results = new List<Muscle>();
            if (muscles != null && muscles.Count() > 0)
            {
                foreach (var muscle in muscles)
                {
                    results.Add(_manager.UpdateMuscle(muscle));
                }
            }
            return results;
        }

        // DELETE api/Muscles
        [HttpDelete]
        public void Delete(MuscleKey key)
        {
            _manager.DeleteMuscle(key);
        }
    }
}
