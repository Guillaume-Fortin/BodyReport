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

        public TrainingWeeksService(ApplicationDbContext dbContext, ICachesService cacheService) : base(dbContext, cacheService)
        {
        }

        public TrainingWeek CreateTrainingWeek(TrainingWeek trainingWeek)
        {
            TrainingWeek result = null;
            //Create data in database
            BeginTransaction();
            try
            {
                result = GetTrainingWeekManager().CreateTrainingWeek(trainingWeek);
                CommitTransaction();
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to create training week", exception);
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
            return result;
        }

        public TrainingWeek UpdateTrainingWeek(TrainingWeek trainingWeek, TrainingWeekScenario trainingWeekScenario)
        {
            TrainingWeek result = null;
            BeginTransaction();
            try
            {
                result = GetTrainingWeekManager().UpdateTrainingWeek(trainingWeek, trainingWeekScenario);
                CommitTransaction();
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to update training week", exception);
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
            return result;
        }

        public TrainingWeek GetTrainingWeek(TrainingWeekKey key, TrainingWeekScenario trainingWeekScenario)
        {
            return GetTrainingWeekManager().GetTrainingWeek(key, trainingWeekScenario);
        }

        public List<TrainingWeek> FindTrainingWeek(TrainingWeekCriteria trainingWeekCriteria, TrainingWeekScenario trainingWeekScenario)
        {
            return GetTrainingWeekManager().FindTrainingWeek(trainingWeekCriteria, trainingWeekScenario);
        }

        public List<TrainingWeek> FindTrainingWeek(CriteriaList<TrainingWeekCriteria> trainingWeekCriteriaList, TrainingWeekScenario trainingWeekScenario)
        {
            return GetTrainingWeekManager().FindTrainingWeek(trainingWeekCriteriaList, trainingWeekScenario);
        }

        public void DeleteTrainingWeek(TrainingWeekKey key)
        {
            BeginTransaction();
            try
            {
                GetTrainingWeekManager().DeleteTrainingWeek(key);
                CommitTransaction();
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to delete training week", exception);
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
        }

        public bool CopyTrainingWeek(string currentUserId, CopyTrainingWeek copyTrainingWeek, out TrainingWeek newTrainingWeek)
        {
            bool result = false;
            BeginTransaction();
            try
            {
                result = GetTrainingWeekManager().CopyTrainingWeek(currentUserId, copyTrainingWeek, out newTrainingWeek);
                CommitTransaction();
                result = true;
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to copy training week", exception);
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
            return result;
        }
    }
}
