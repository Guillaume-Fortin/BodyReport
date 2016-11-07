using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface IUserRolesService
    {
        UserRole GetUserRole(UserRoleKey key);

        List<UserRole> FindUserRole(UserRoleCriteria userRoleCriteria = null);

        UserRole UpdateUserRole(UserRole userRole);
    }
}
