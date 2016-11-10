using BodyReport.Crud.Module;
using BodyReport.Data;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    public class UserRoleManager : BodyReportManager
    {
        UserRoleModule _userRoleModule = null;

        public UserRoleManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _userRoleModule = new UserRoleModule(_dbContext);
        }

        public UserRole Get(UserRoleKey key)
        {
            return _userRoleModule.Get(key);
        }

        public List<UserRole> Find(UserRoleCriteria userRoleCriteria = null)
        {
            return _userRoleModule.Find(userRoleCriteria);
        }

        public UserRole UpdateUserRole(UserRole userRole)
        {
            return _userRoleModule.Update(userRole);
        }
    }
}
