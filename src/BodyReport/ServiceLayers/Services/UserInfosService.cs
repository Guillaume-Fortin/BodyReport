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
    public class UserInfosService : BodyReportService, IUserInfosService
    {
        private const string _cacheName = "UserInfosCache";
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(UserInfosService));

        public UserInfosService(ApplicationDbContext dbContext, ICachesService cacheService) : base(dbContext, cacheService)
        {
        }

        public UserInfo GetUserInfo(UserInfoKey key)
        {
            UserInfo userInfo = null;
            string cacheKey = key == null ? "UserInfoKey_null" : key.GetCacheKey();
            if (key != null && !TryGetCacheData(cacheKey, out userInfo))
            {
                userInfo = GetUserInfoManager().GetUserInfo(key);
                SetCacheData(_cacheName, cacheKey, userInfo);
            }
            return userInfo;
        }

        public UserInfo UpdateUserInfo(UserInfo userInfo)
        {
            UserInfo result = null;
            BeginTransaction();
            try
            {
                result = GetUserInfoManager().UpdateUserInfo(userInfo);
                CommitTransaction();
                //invalidate cache
                InvalidateCache(_cacheName);
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to update user info", exception);
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
