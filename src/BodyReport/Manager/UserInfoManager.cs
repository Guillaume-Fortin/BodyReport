using BodyReport.Crud.Module;
using BodyReport.Models;
using Framework;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    public class UserInfoManager : ServiceManager
    {
        UserInfoModule _userInfoModule = null;

        public UserInfoManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _userInfoModule = new UserInfoModule(_dbContext);
        }

        internal UserInfo GetUserInfo(UserInfoKey key)
        {
            var userInfo = _userInfoModule.Get(key);
            if(userInfo != null)   
                TransformMetricUnitToUserInfoUnit(userInfo);
            return userInfo;
        }

        public List<UserInfo> FindUserInfos(CriteriaField criteriaField = null)
        {
            var userInfoList = _userInfoModule.Find(criteriaField);
            if (userInfoList != null)
            {
                foreach (var userInfo in userInfoList)
                {
                    TransformMetricUnitToUserInfoUnit(userInfo);
                }
            }
            return userInfoList;
        }

        internal void DeleteUserInfo(UserInfoKey key)
        {
            _userInfoModule.Delete(key);
        }

        internal UserInfo UpdateUserInfo(UserInfo userInfo)
        {
            TransformUserInfoUnitToMetricUnit(userInfo);
            userInfo = _userInfoModule.Update(userInfo);
            TransformMetricUnitToUserInfoUnit(userInfo);
            return userInfo;
        }

        private void TransformUserInfoUnitToMetricUnit(UserInfo userInfo)
        {
            userInfo.Height = Utils.TransformLengthToUnitSytem(userInfo.Unit, TUnitType.Metric, userInfo.Height);
            userInfo.Weight = Utils.TransformWeightToUnitSytem(userInfo.Unit, TUnitType.Metric, userInfo.Weight);
        }

        private void TransformMetricUnitToUserInfoUnit(UserInfo userInfo)
        {
            userInfo.Height = Utils.TransformLengthToUnitSytem(TUnitType.Metric, userInfo.Unit, userInfo.Height);
            userInfo.Weight = Utils.TransformWeightToUnitSytem(TUnitType.Metric, userInfo.Unit, userInfo.Weight);
        }
    }
}
