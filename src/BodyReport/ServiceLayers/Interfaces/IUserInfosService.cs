using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface IUserInfosService
    {
        UserInfo GetUserInfo(UserInfoKey key);
        UserInfo UpdateUserInfo(UserInfo userInfo);
    }
}
