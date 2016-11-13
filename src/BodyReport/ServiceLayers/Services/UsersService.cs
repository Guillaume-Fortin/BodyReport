using BodyReport.Data;
using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Message;
using BodyReport.ServiceLayers.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BodyReport.ServiceLayers.Services
{
    public class UsersService : BodyReportService, IUsersService
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(UsersService));

        public UsersService(ApplicationDbContext dbContext, IUserRolesService userRolesService, IRolesService rolesService, ICachesService cacheService) : base(dbContext, cacheService)
        {
        }

        public User GetUser(UserKey key, bool manageRole = true)
        {
            return GetUserManager().GetUser(key, manageRole);
        }
        public List<User> FindUsers(out int totalRecords, UserCriteria userCriteria = null, bool manageRole = true, int currentRecordIndex = 0, int maxRecord = 0)
        {
            return GetUserManager().FindUsers(out totalRecords, userCriteria, manageRole, currentRecordIndex, maxRecord);
        }
        public void DeleteUser(UserKey key)
        {
            BeginTransaction();
            try
            {
                GetUserManager().DeleteUser(key);
                //todo delete user infos, exercise etc.
                CommitTransaction();
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to delete user", exception);
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
        }
        public User UpdateUser(User user)
        {
            User result = null;
            BeginTransaction();
            try
            {
                result = GetUserManager().UpdateUser(user);
                CommitTransaction();
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to update user", exception);
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
