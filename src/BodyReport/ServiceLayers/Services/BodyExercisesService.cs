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
    public class BodyExercisesService : BodyReportService, IBodyExercisesService
    {
        /// <summary>
        /// Body Exercise Manager
        /// </summary>
        BodyExerciseManager _bodyExerciseManager = null;
        public BodyExercisesService(ApplicationDbContext dbContext) : base(dbContext)
        {
            _bodyExerciseManager = new BodyExerciseManager(_dbContext);
        }

        public BodyExercise CreateBodyExercise(BodyExercise bodyExercise)
        {
            BodyExercise result = null;
            if (bodyExercise != null)
            {
                result = _bodyExerciseManager.CreateBodyExercise(bodyExercise);
            }
            return result;
        }

        public BodyExercise GetBodyExercise(BodyExerciseKey key)
        {
            BodyExercise result = null;
            if (key != null)
            {
                result = _bodyExerciseManager.GetBodyExercise(key);
            }
            return result;
        }

        public List<BodyExercise> FindBodyExercises(BodyExerciseCriteria criteria = null)
        {
            return _bodyExerciseManager.FindBodyExercises(criteria);
        }

        public BodyExercise UpdateBodyExercise(BodyExercise bodyExercise)
        {
            BodyExercise result = null;
            if (bodyExercise != null)
            {
                result = _bodyExerciseManager.UpdateBodyExercise(bodyExercise);
            }
            return result;
        }
        public List<BodyExercise> UpdateBodyExerciseList(List<BodyExercise> bodyExercises)
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
        public void DeleteBodyExercise(BodyExerciseKey key)
        {
            _bodyExerciseManager.DeleteBodyExercise(key);
        }
    }
}
