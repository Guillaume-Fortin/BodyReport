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
    public class MuscularGroupsController : Controller
    {
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
        MuscularGroupManager _manager = null;

        public MuscularGroupsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _manager = new MuscularGroupManager(_dbContext);
        }

        // Get api/MuscularGroups/Get
        [HttpGet]
        public MuscularGroup Get(MuscularGroupKey key)
        {
            return _manager.GetMuscularGroup(key);
        }

        // Get api/MuscularGroups/Find
        [HttpGet]
        public List<MuscularGroup> Find()
        {
            return _manager.FindMuscularGroups();
        }

        // POST api/MuscularGroups/Post
        [HttpPost]
        public List<MuscularGroup> Post([FromBody]List<MuscularGroup> muscularGroups)
        {
            List<MuscularGroup> results = new List<MuscularGroup>();
            if (muscularGroups != null && muscularGroups.Count() > 0)
            {
                foreach (var muscularGroup in muscularGroups)
                {
                    results.Add(_manager.UpdateMuscularGroup(muscularGroup));
                }
            }
            return results;
        }

        // DELETE api/MuscularGroups/Delete
        [HttpDelete]
        public void Delete(MuscularGroupKey key)
        {
            _manager.DeleteMuscularGroup(key);
        }
    }
}
