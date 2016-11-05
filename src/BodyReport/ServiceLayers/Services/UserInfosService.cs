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
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(UserInfosService));
        /// <summary>
        /// User info Manager
        /// </summary>
        UserInfoManager _userInfoManager = null;
        public UserInfosService(ApplicationDbContext dbContext) : base(dbContext)
        {
            _userInfoManager = new UserInfoManager(_dbContext);
        }

        public UserInfo GetUserInfo(UserInfoKey key)
        {
            return _userInfoManager.GetUserInfo(key);
        }

        public UserInfo UpdateUserInfo(UserInfo userInfo)
        {
            UserInfo result = null;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _userInfoManager.UpdateUserInfo(userInfo);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to update user info", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }
    }
}
