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
    public class MusclesService : BodyReportService, IMusclesService
    {
        /// <summary>
        /// Body Exercise Manager
        /// </summary>
        MuscleManager _muscleManager = null;

        public MusclesService(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            _muscleManager = new MuscleManager(_dbContext);
        }

        public List<Muscle> FindMuscles(MuscleCriteria criteria = null)
        {
            return _muscleManager.FindMuscles(criteria);
        }
    }
}
