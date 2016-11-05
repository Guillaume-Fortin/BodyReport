using BodyReport.Data;
using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Message;
using BodyReport.ServiceLayers.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Services
{
    public class MusclesService : BodyReportService, IMusclesService
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(MusclesService));
        /// <summary>
        /// Body Exercise Manager
        /// </summary>
        MuscleManager _muscleManager = null;

        public MusclesService(ApplicationDbContext dbContext) : base(dbContext)
        {
            _muscleManager = new MuscleManager(_dbContext);
        }

        public Muscle GetMuscle(MuscleKey key)
        {
            return _muscleManager.GetMuscle(key);
        }

        public List<Muscle> FindMuscles(MuscleCriteria criteria = null)
        {
            return _muscleManager.FindMuscles(criteria);
        }

        public Muscle UpdateMuscle(Muscle muscle)
        {
            Muscle result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _muscleManager.UpdateMuscle(muscle);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to update muscle", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }

        public List<Muscle> UpdateMuscleList(List<Muscle> muscles)
        {
            List<Muscle> results = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (muscles != null && muscles.Count() > 0)
                    {
                        results = new List<Muscle>();
                        foreach (var muscle in muscles)
                        {
                            results.Add(_muscleManager.UpdateMuscle(muscle));
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to update muscle list", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return results;
        }

        public void DeleteMuscle(MuscleKey key)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _muscleManager.DeleteMuscle(key);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to update muscle", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
        }
    }
}
