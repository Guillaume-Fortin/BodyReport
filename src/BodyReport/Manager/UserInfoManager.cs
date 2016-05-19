using BodyReport.Crud.Module;
using BodyReport.Data;
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
            return _userInfoModule.Get(key);
        }

        public List<UserInfo> FindUserInfos(CriteriaField criteriaField = null)
        {
            return _userInfoModule.Find(criteriaField);
        }

        internal void DeleteUserInfo(UserInfoKey key)
        {
            _userInfoModule.Delete(key);
        }

        internal UserInfo UpdateUserInfo(UserInfo userInfo)
        {
            return _userInfoModule.Update(userInfo);
        }
    }
}
