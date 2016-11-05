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
        public UsersService(ApplicationDbContext dbContext) : base(dbContext)
        {
            _userManager = new UserManager(_dbContext);
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
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _userManager.DeleteUser(key);
                    //todo delete user infos, exercise etc.
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to delete user", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
        }
        public User UpdateUser(User user)
        {
            User result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _userManager.UpdateUser(user);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to update user", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }

        #region manage role
        public List<Role> FindRoles(RoleCriteria roleCriteria = null)
        {
            return _userManager.FindRoles(roleCriteria);
        }

        public Role CreateRole(Role role)
        {
            Role result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _userManager.CreateRole(role);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to create role", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }

        public Role GetRole(RoleKey key)
        {
            return _userManager.GetRole(key);
        }

        public Role UpdateRole(Role role)
        {
            Role result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _userManager.UpdateRole(role);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to update role", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }

        public void DeleteRole(RoleKey key)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _userManager.DeleteRole(key);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to delete role", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
        }
        #endregion
    }
}
