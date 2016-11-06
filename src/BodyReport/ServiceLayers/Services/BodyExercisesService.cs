using BodyReport.Data;
using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Message;
using BodyReport.ServiceLayers.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BodyReport.ServiceLayers.Services
{
    public class BodyExercisesService : BodyReportService, IBodyExercisesService
    {
        private const string _cacheName = "BodyExercisesCache";
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(BodyExercisesService));
        /// <summary>
        /// Body Exercise Manager
        /// </summary>
        BodyExerciseManager _bodyExerciseManager = null;
        public BodyExercisesService(ApplicationDbContext dbContext, ICachesService cacheService) : base(dbContext, cacheService)
        {
            _bodyExerciseManager = new BodyExerciseManager(_dbContext);
        }

        public BodyExercise CreateBodyExercise(BodyExercise bodyExercise)
        {
            BodyExercise result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _bodyExerciseManager.CreateBodyExercise(bodyExercise);
                    transaction.Commit();
                    //invalidate cache
                    InvalidateCache(_cacheName);
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to create bodyexercise", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }

        public BodyExercise GetBodyExercise(BodyExerciseKey key)
        {
            BodyExercise bodyExercise = null;
            string cacheKey = key == null ? "BodyExerciseKey_null" : key.GetCacheKey();
            if (key != null && !TryGetCacheData(cacheKey, out bodyExercise))
            {
                bodyExercise = _bodyExerciseManager.GetBodyExercise(key);
                SetCacheData(_cacheName, cacheKey, bodyExercise);
            }
            return bodyExercise;
        }

        public List<BodyExercise> FindBodyExercises(BodyExerciseCriteria criteria = null)
        {
            List<BodyExercise> bodyExerciseList = null;
            string cacheKey = criteria == null ? "BodyExerciseCriteria_null" : criteria.GetCacheKey();
            if (!TryGetCacheData(cacheKey, out bodyExerciseList))
            {
                bodyExerciseList = _bodyExerciseManager.FindBodyExercises(criteria);
                SetCacheData(_cacheName, cacheKey, bodyExerciseList);
            }
            return bodyExerciseList;
        }

        public BodyExercise UpdateBodyExercise(BodyExercise bodyExercise)
        {
            BodyExercise result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _bodyExerciseManager.UpdateBodyExercise(bodyExercise);
                    transaction.Commit();
                    //invalidate cache
                    InvalidateCache(_cacheName);
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to update bodyexercise", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }
        public List<BodyExercise> UpdateBodyExerciseList(List<BodyExercise> bodyExercises)
        {
            List<BodyExercise> results = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (bodyExercises != null && bodyExercises.Count() > 0)
                    {
                        results = new List<BodyExercise>();
                        foreach (var bodyExercise in bodyExercises)
                        {
                            results.Add(_bodyExerciseManager.UpdateBodyExercise(bodyExercise));
                        }
                        transaction.Commit();
                        //invalidate cache
                        InvalidateCache(_cacheName);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to create bodyexercise", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return results;
        }
        public void DeleteBodyExercise(BodyExerciseKey key)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _bodyExerciseManager.DeleteBodyExercise(key);
                    transaction.Commit();
                    //invalidate cache
                    InvalidateCache(_cacheName);
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to delete bodyexercise", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
        }
    }
}
