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
    public class TrainingDaysService : BodyReportService, ITrainingDaysService
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(TrainingDaysService));
        /// <summary>
        /// TrainingDay Manager
        /// </summary>
        TrainingDayManager _trainingDayManager = null;
        public TrainingDaysService(ApplicationDbContext dbContext) : base(dbContext)
        {
            _trainingDayManager = new TrainingDayManager(_dbContext);
        }

        public TrainingDay CreateTrainingDay(TrainingDay trainingDay)
        {
            TrainingDay result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _trainingDayManager.CreateTrainingDay(trainingDay);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to create training day", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }

        public TrainingDay GetTrainingDay(TrainingDayKey key, TrainingDayScenario trainingDayScenario)
        {
            return _trainingDayManager.GetTrainingDay(key, trainingDayScenario);
        }

        public List<TrainingDay> FindTrainingDay(TrainingDayCriteria trainingDayCriteria, TrainingDayScenario trainingDayScenario)
        {
            return _trainingDayManager.FindTrainingDay(trainingDayCriteria, trainingDayScenario);
        }

        public TrainingDay UpdateTrainingDay(TrainingDay trainingDay, TrainingDayScenario trainingDayScenario)
        {
            TrainingDay result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _trainingDayManager.UpdateTrainingDay(trainingDay, trainingDayScenario);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to update training day", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }

        public void DeleteTrainingDay(TrainingDayKey key)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _trainingDayManager.DeleteTrainingDay(key);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to delete training day", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
        }

        public void SwitchDayOnTrainingDay(string userId, int year, int weekOfYear, int dayOfWeek, int switchDayOfWeek)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _trainingDayManager.SwitchDayOnTrainingDay(userId, year, weekOfYear, dayOfWeek, switchDayOfWeek);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to SwitchDayOnTrainingDay", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
        }
    }
}
