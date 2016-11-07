using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface IRolesService
    {
        List<Role> FindRoles(RoleCriteria roleCriteria = null);

        Role CreateRole(Role role);

        Role GetRole(RoleKey key);

        Role UpdateRole(Role role);

        void DeleteRole(RoleKey key);
    }
}
