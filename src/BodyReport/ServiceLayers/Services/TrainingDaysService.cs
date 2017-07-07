using BodyReport.Data;
using BodyReport.Framework;
using BodyReport.Message;
using BodyReport.ServiceLayers.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BodyReport.ServiceLayers.Services
{
    public class TrainingDaysService : BodyReportService, ITrainingDaysService
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(TrainingDaysService));

        public TrainingDaysService(ApplicationDbContext dbContext, ICachesService cacheService) : base(dbContext, cacheService)
        {
        }

        public TrainingDay CreateTrainingDay(TrainingDay trainingDay)
        {
            TrainingDay result = null;
            BeginTransaction();
            try
            {
                result = GetTrainingDayManager().CreateTrainingDay(trainingDay);
                CommitTransaction();
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to create training day", exception);
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
            return result;
        }

        public TrainingDay GetTrainingDay(TrainingDayKey key, TrainingDayScenario trainingDayScenario)
        {
            return GetTrainingDayManager().GetTrainingDay(key, trainingDayScenario);
        }

        public List<TrainingDay> FindTrainingDay(TUnitType userUnit, TrainingDayCriteria trainingDayCriteria, TrainingDayScenario trainingDayScenario)
        {
            return GetTrainingDayManager().FindTrainingDay(userUnit, trainingDayCriteria, trainingDayScenario);
        }

        public TrainingDay UpdateTrainingDay(TrainingDay trainingDay, TrainingDayScenario trainingDayScenario)
        {
            TrainingDay result = null;
            BeginTransaction();
            try
            {
                result = GetTrainingDayManager().UpdateTrainingDay(trainingDay, trainingDayScenario);
                CommitTransaction();
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to update training day", exception);
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
            return result;
        }

        public void DeleteTrainingDay(TrainingDayKey key)
        {
            BeginTransaction();
            try
            {
                GetTrainingDayManager().DeleteTrainingDay(key);
                CommitTransaction();
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to delete training day", exception);
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
        }

        public void SwitchDayOnTrainingDay(string userId, int year, int weekOfYear, int dayOfWeek, int switchDayOfWeek)
        {
            BeginTransaction();
            try
            {
                GetTrainingDayManager().SwitchDayOnTrainingDay(userId, year, weekOfYear, dayOfWeek, switchDayOfWeek);
                CommitTransaction();
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to SwitchDayOnTrainingDay", exception);
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
