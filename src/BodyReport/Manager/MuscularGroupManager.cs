using BodyReport.Crud.Module;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    /// <summary>
    /// Manage Muscular groups
    /// </summary>
    public class MuscularGroupManager : ServiceManager
    {
        public MuscularGroupManager(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public List<MuscularGroup> FindMuscularGroups()
        {
            var muscularGroupModule = new MuscularGroupModule(_dbContext);
            return muscularGroupModule.Find();
        }
    }
}
