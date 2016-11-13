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
    public class RolesService : BodyReportService, IRolesService
    {
        private const string _cacheName = "RolesCache";
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(RolesService));

        public RolesService(ApplicationDbContext dbContext, ICachesService cacheService) : base(dbContext, cacheService)
        {
        }

        public Role CreateRole(Role role)
        {
            Role result = null;
            BeginTransaction();
            try
            {
                result = GetRoleManager().CreateRole(role);
                CommitTransaction();
                //invalidate cache
                InvalidateCache(_cacheName);
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to create role", exception);
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
            return result;
        }

        public Role GetRole(RoleKey key)
        {
            Role role = null;
            string cacheKey = key == null ? "RoleKey_null" : key.GetCacheKey();
            if (key != null && !TryGetCacheData(cacheKey, out role))
            {
                role = GetRoleManager().GetRole(key);
                SetCacheData(_cacheName, cacheKey, role);
            }
            return role;
        }
        
        public List<Role> FindRoles(RoleCriteria criteria = null)
        {
            List<Role> roleList = null;
            string cacheKey = criteria == null ? "RoleCriteria_null" : criteria.GetCacheKey();
            if (!TryGetCacheData(cacheKey, out roleList))
            {
                roleList = GetRoleManager().FindRoles(criteria);
                SetCacheData(_cacheName, cacheKey, roleList);
            }
            return roleList;
        }

        public Role UpdateRole(Role role)
        {
            Role result = null;
            BeginTransaction();
            try
            {
                result = GetRoleManager().UpdateRole(role);
                CommitTransaction();
                //invalidate cache
                InvalidateCache(_cacheName);
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to update role", exception);
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
            return result;
        }

        public void DeleteRole(RoleKey key)
        {
            BeginTransaction();
            try
            {
                GetRoleManager().DeleteRole(key);
                CommitTransaction();
                //invalidate cache
                InvalidateCache(_cacheName);
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to delete role", exception);
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
