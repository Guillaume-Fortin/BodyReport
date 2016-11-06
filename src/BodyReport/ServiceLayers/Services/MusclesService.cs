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
        private const string _cacheName = "MusclesCache";
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(MusclesService));
        /// <summary>
        /// Body Exercise Manager
        /// </summary>
        MuscleManager _muscleManager = null;

        public MusclesService(ApplicationDbContext dbContext, ICachesService cacheService) : base(dbContext, cacheService)
        {
            _muscleManager = new MuscleManager(_dbContext);
        }

        public Muscle GetMuscle(MuscleKey key)
        {
            Muscle muscle = null;
            string cacheKey = key == null ? "MuscleKey_null" : key.GetCacheKey();
            if (key != null && !TryGetCacheData(cacheKey, out muscle))
            {
                muscle = _muscleManager.GetMuscle(key);
                SetCacheData(_cacheName, cacheKey, muscle);
            }
            return muscle;
        }

        public List<Muscle> FindMuscles(MuscleCriteria criteria = null)
        {
            List<Muscle> muscleList = null;
            string cacheKey = criteria == null ? "MuscleCriteria_null" : criteria.GetCacheKey();
            if (!TryGetCacheData(cacheKey, out muscleList))
            {
                muscleList = _muscleManager.FindMuscles(criteria);
                SetCacheData(_cacheName, cacheKey, muscleList);
            }
            return muscleList;
        }

        public Muscle CreateMuscle(Muscle muscle)
        {
            Muscle result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _muscleManager.CreateMuscle(muscle);
                    transaction.Commit();
                    //invalidate cache
                    InvalidateCache(_cacheName);
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to create muscle", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
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
                    //invalidate cache
                    InvalidateCache(_cacheName);
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
                    //invalidate cache
                    InvalidateCache(_cacheName);
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
                    //invalidate cache
                    InvalidateCache(_cacheName);
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to delete muscle", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
        }
    }
}
