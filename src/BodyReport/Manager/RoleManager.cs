using BodyReport.Crud.Module;
using BodyReport.Data;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    public class RoleManager : BodyReportManager
    {
        RoleModule _roleModule = null;

        public RoleManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _roleModule = new RoleModule(_dbContext);
        }

        public List<Role> FindRoles(RoleCriteria roleCriteria = null)
        {
            return _roleModule.Find(roleCriteria);
        }

        internal Role CreateRole(Role role)
        {
            return _roleModule.Create(role);
        }

        internal Role GetRole(RoleKey key)
        {
            return _roleModule.Get(key);
        }

        internal Role UpdateRole(Role role)
        {
            return _roleModule.Update(role);
        }

        internal void DeleteRole(RoleKey key)
        {
            _roleModule.Delete(key);
        }
    }
}
