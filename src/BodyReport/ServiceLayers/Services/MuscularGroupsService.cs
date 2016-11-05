using BodyReport.Data;
using BodyReport.Manager;
using BodyReport.Message;
using BodyReport.ServiceLayers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Services
{
    public class MuscularGroupsService : BodyExercisesService, IMuscularGroupsService
    {
        /// <summary>
        /// Muscular Group Manager
        /// </summary>
        MuscularGroupManager _muscularGroupManager = null;
        public MuscularGroupsService(ApplicationDbContext dbContext) : base(dbContext)
        {
            _muscularGroupManager = new MuscularGroupManager(_dbContext);
        }

        public List<MuscularGroup> FindMuscularGroups()
        {
            return _muscularGroupManager.FindMuscularGroups();
        }

        public MuscularGroup CreateMuscularGroup(MuscularGroup muscularGroup)
        {
            return _muscularGroupManager.CreateMuscularGroup(muscularGroup);
        }

        public MuscularGroup GetMuscularGroup(MuscularGroupKey key)
        {
            return _muscularGroupManager.GetMuscularGroup(key);
        }

        public void DeleteMuscularGroup(MuscularGroupKey key)
        {
            _muscularGroupManager.DeleteMuscularGroup(key);
        }

        public MuscularGroup UpdateMuscularGroup(MuscularGroup muscularGroup)
        {
            return _muscularGroupManager.UpdateMuscularGroup(muscularGroup);
        }
    }
}
