using BodyReport.Data;
using BodyReport.Manager;
using BodyReport.Message;
using BodyReport.ServiceLayers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Services
{
    public class UserInfosService : BodyReportService, IUserInfosService
    {
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
            if (userInfo != null)
            {
                result = _userInfoManager.UpdateUserInfo(userInfo);
            }
            return result;
        }
    }
}
