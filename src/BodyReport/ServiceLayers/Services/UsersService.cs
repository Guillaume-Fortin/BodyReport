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
        /// <summary>
        /// User info Manager
        /// </summary>
        UserManager _userManager = null;
        public UsersService(ApplicationDbContext dbContext, IUserRolesService userRolesService, IRolesService rolesService, ICachesService cacheService) : base(dbContext, cacheService)
        {
            _userManager = new UserManager(_dbContext, userRolesService, rolesService);
            ((BodyReportService)userRolesService).SetDbContext(dbContext); // for use same transaction
            ((BodyReportService)rolesService).SetDbContext(dbContext);// for use same transaction
        }

        public User GetUser(UserKey key, bool manageRole = true)
        {
            return _userManager.GetUser(key, manageRole);
        }
        public List<User> FindUsers(out int totalRecords, UserCriteria userCriteria = null, bool manageRole = true, int currentRecordIndex = 0, int maxRecord = 0)
        {
            return _userManager.FindUsers(out totalRecords, userCriteria, manageRole, currentRecordIndex, maxRecord);
        }
        public void DeleteUser(UserKey key)
        {
            BeginTransaction();
            try
            {
                _userManager.DeleteUser(key);
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
                result = _userManager.UpdateUser(user);
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
