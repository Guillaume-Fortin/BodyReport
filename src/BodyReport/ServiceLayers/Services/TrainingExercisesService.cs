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
    public class TrainingExercisesService : BodyExercisesService, ITrainingExercisesService
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(TrainingExercisesService));
        /// <summary>
        /// Training Exercise Manager
        /// </summary>
        TrainingExerciseManager _trainingExerciseManager = null;
        public TrainingExercisesService(ApplicationDbContext dbContext, ICachesService cacheService) : base(dbContext, cacheService)
        {
            _trainingExerciseManager = new TrainingExerciseManager(_dbContext);
        }

        public TrainingExercise CreateTrainingExercise(TrainingExercise trainingExercise)
        {
            TrainingExercise result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _trainingExerciseManager.CreateTrainingExercise(trainingExercise);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to create training exercises", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }

        public TrainingExercise UpdateTrainingExercise(TrainingExercise trainingExercise, bool manageDeleteLinkItem)
        {
            TrainingExercise result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _trainingExerciseManager.UpdateTrainingExercise(trainingExercise, manageDeleteLinkItem);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to update training exercises", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }

        public TrainingExercise GetTrainingExercise(TrainingExerciseKey key)
        {
            return _trainingExerciseManager.GetTrainingExercise(key);
        }
        public List<TrainingExercise> FindTrainingExercise(TrainingExerciseCriteria trainingExerciseCriteria)
        {
            return _trainingExerciseManager.FindTrainingExercise(trainingExerciseCriteria);
        }

        public void DeleteTrainingExercise(TrainingExerciseKey key)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _trainingExerciseManager.DeleteTrainingExercise(key);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to delete training exercises", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
        }
    }
}
