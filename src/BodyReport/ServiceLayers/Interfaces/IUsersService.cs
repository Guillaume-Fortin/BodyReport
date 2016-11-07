using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface IUsersService
    {
        User GetUser(UserKey key, bool manageRole = true);
        List<User> FindUsers(out int totalRecords, UserCriteria userCriteria = null, bool manageRole = true, int currentRecordIndex = 0, int maxRecord = 0);
        void DeleteUser(UserKey key);
        User UpdateUser(User user);
    }
}
