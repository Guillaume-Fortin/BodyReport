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
    public class UserRolesService : BodyExercisesService, IUserRolesService
    {
        private const string _cacheName = "UserRolesCache";
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(RolesService));

        public UserRolesService(ApplicationDbContext dbContext, ICachesService cacheService) : base(dbContext, cacheService)
        {
        }

        public UserRole GetUserRole(UserRoleKey key)
        {
            UserRole userRole = null;
            string cacheKey = key == null ? "UserRoleKey_null" : key.GetCacheKey();
            if (key != null && !TryGetCacheData(cacheKey, out userRole))
            {
                userRole = GetUserRoleManager().Get(key);
                SetCacheData(_cacheName, cacheKey, userRole);
            }
            return userRole;
        }

        public List<UserRole> FindUserRole(UserRoleCriteria criteria = null)
        {
            List<UserRole> userRoleList = null;
            string cacheKey = criteria == null ? "UserRoleCriteria_null" : criteria.GetCacheKey();
            if (!TryGetCacheData(cacheKey, out userRoleList))
            {
                userRoleList = GetUserRoleManager().Find(criteria);
                SetCacheData(_cacheName, cacheKey, userRoleList);
            }
            return userRoleList;
        }

        public UserRole UpdateUserRole(UserRole userRole)
        {
            UserRole result = null;
            BeginTransaction();
            try
            {
                result = GetUserRoleManager().UpdateUserRole(userRole);
                //todo delete user infos, exercise etc.
                CommitTransaction();
                //Invalidate cache
                InvalidateCache(_cacheName);
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
            return result;
        }
    }
}
