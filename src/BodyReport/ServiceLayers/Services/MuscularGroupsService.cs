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
    public class MuscularGroupsService : BodyExercisesService, IMuscularGroupsService
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(MuscularGroupsService));
        /// <summary>
        /// Muscular Group Manager
        /// </summary>
        MuscularGroupManager _muscularGroupManager = null;
        public MuscularGroupsService(ApplicationDbContext dbContext) : base(dbContext)
        {
            _muscularGroupManager = new MuscularGroupManager(_dbContext);
        }

        public List<MuscularGroup> FindMuscularGroups()
        {
            return _muscularGroupManager.FindMuscularGroups();
        }

        public MuscularGroup CreateMuscularGroup(MuscularGroup muscularGroup)
        {
            MuscularGroup result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _muscularGroupManager.CreateMuscularGroup(muscularGroup);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to create muscular group", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }

        public MuscularGroup GetMuscularGroup(MuscularGroupKey key)
        {
            return _muscularGroupManager.GetMuscularGroup(key);
        }

        public void DeleteMuscularGroup(MuscularGroupKey key)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _muscularGroupManager.DeleteMuscularGroup(key);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to delete muscular group", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
        }

        public MuscularGroup UpdateMuscularGroup(MuscularGroup muscularGroup)
        {
            MuscularGroup result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _muscularGroupManager.UpdateMuscularGroup(muscularGroup);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to update muscular group", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }

        public List<MuscularGroup> UpdateMuscularGroupList(List<MuscularGroup> muscularGroups)
        {
            List<MuscularGroup> result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (muscularGroups != null && muscularGroups.Count() > 0)
                    {
                        result = new List<MuscularGroup>();
                        foreach (var muscularGroup in muscularGroups)
                        {
                            result.Add(_muscularGroupManager.UpdateMuscularGroup(muscularGroup));
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to update muscular group list", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }
    }
}
