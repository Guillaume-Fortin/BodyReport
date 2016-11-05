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
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(BodyExercisesService));
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
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _bodyExerciseManager.CreateBodyExercise(bodyExercise);
                    transaction.Commit();
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
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _bodyExerciseManager.UpdateBodyExercise(bodyExercise);
                    transaction.Commit();
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
                    }
                    transaction.Commit();
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
