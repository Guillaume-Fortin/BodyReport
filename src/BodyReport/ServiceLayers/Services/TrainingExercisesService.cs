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

        public TrainingExercisesService(ApplicationDbContext dbContext, ICachesService cacheService) : base(dbContext, cacheService)
        {
        }

        public TrainingExercise CreateTrainingExercise(TrainingExercise trainingExercise)
        {
            TrainingExercise result = null;
            BeginTransaction();
            try
            {
                result = GetTrainingExerciseManager().CreateTrainingExercise(trainingExercise);
                CommitTransaction();
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to create training exercises", exception);
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
            return result;
        }

        public TrainingExercise UpdateTrainingExercise(TrainingExercise trainingExercise, bool manageDeleteLinkItem)
        {
            TrainingExercise result = null;
            BeginTransaction();
            try
            {
                result = GetTrainingExerciseManager().UpdateTrainingExercise(trainingExercise, manageDeleteLinkItem);
                CommitTransaction();
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to update training exercises", exception);
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
            return result;
        }

        public TrainingExercise GetTrainingExercise(TrainingExerciseKey key)
        {
            return GetTrainingExerciseManager().GetTrainingExercise(key);
        }
        public List<TrainingExercise> FindTrainingExercise(TrainingExerciseCriteria trainingExerciseCriteria)
        {
            return GetTrainingExerciseManager().FindTrainingExercise(trainingExerciseCriteria);
        }

        public void DeleteTrainingExercise(TrainingExerciseKey key)
        {
            BeginTransaction();
            try
            {
                GetTrainingExerciseManager().DeleteTrainingExercise(key);
                CommitTransaction();
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to delete training exercises", exception);
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
        }
    }
}
