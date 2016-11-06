using BodyReport.Data;
using BodyReport.Framework;
using BodyReport.Framework.Exceptions;
using BodyReport.Manager;
using BodyReport.Message;
using BodyReport.Message.Web;
using BodyReport.ServiceLayers.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Services
{
    public class TrainingWeeksService : BodyReportService, ITrainingWeeksService
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(TrainingWeeksService));
        /// <summary>
        /// TrainingWeek Manager
        /// </summary>
        TrainingWeekManager _trainingWeekManager = null;
        public TrainingWeeksService(ApplicationDbContext dbContext, ICachesService cacheService) : base(dbContext, cacheService)
        {
            _trainingWeekManager = new TrainingWeekManager(_dbContext);
        }

        public TrainingWeek CreateTrainingWeek(TrainingWeek trainingWeek)
        {
            TrainingWeek result = null;
            //Create data in database
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _trainingWeekManager.CreateTrainingWeek(trainingWeek);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to create training week", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }

        public TrainingWeek UpdateTrainingWeek(TrainingWeek trainingWeek, TrainingWeekScenario trainingWeekScenario)
        {
            TrainingWeek result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _trainingWeekManager.UpdateTrainingWeek(trainingWeek, trainingWeekScenario);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to update training week", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }

        public TrainingWeek GetTrainingWeek(TrainingWeekKey key, TrainingWeekScenario trainingWeekScenario)
        {
            return _trainingWeekManager.GetTrainingWeek(key, trainingWeekScenario);
        }

        public List<TrainingWeek> FindTrainingWeek(TrainingWeekCriteria trainingWeekCriteria, TrainingWeekScenario trainingWeekScenario)
        {
            return _trainingWeekManager.FindTrainingWeek(trainingWeekCriteria, trainingWeekScenario);
        }

        public List<TrainingWeek> FindTrainingWeek(CriteriaList<TrainingWeekCriteria> trainingWeekCriteriaList, TrainingWeekScenario trainingWeekScenario)
        {
            return _trainingWeekManager.FindTrainingWeek(trainingWeekCriteriaList, trainingWeekScenario);
        }

        public void DeleteTrainingWeek(TrainingWeekKey key)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _trainingWeekManager.DeleteTrainingWeek(key);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to delete training week", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
        }

        public bool CopyTrainingWeek(string currentUserId, CopyTrainingWeek copyTrainingWeek, out TrainingWeek newTrainingWeek)
        {
            bool result = false;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _trainingWeekManager.CopyTrainingWeek(currentUserId, copyTrainingWeek, out newTrainingWeek);
                    transaction.Commit();
                    result = true;
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to copy training week", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }
    }
}
