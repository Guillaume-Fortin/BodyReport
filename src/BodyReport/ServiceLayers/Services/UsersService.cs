using BodyReport.Data;
using BodyReport.Manager;
using BodyReport.Message;
using BodyReport.ServiceLayers.Interfaces;
using System.Collections.Generic;

namespace BodyReport.ServiceLayers.Services
{
    public class UsersService : BodyReportService, IUsersService
    {
        /// <summary>
        /// User info Manager
        /// </summary>
        UserManager _userManager = null;
        public UsersService(ApplicationDbContext dbContext) : base(dbContext)
        {
            _userManager = new UserManager(_dbContext);
        }

        public User GetUser(UserKey key, bool manageRole = true)
        {
            return _userManager.GetUser(key, manageRole);
        }
        public List<User> FindUsers(out int totalRecords, UserCriteria userCriteria = null, bool manageRole = true, int currentRecordIndex = 0, int maxRecord = 0)
        {
            return _userManager.FindUsers(out totalRecords, userCriteria, manageRole, currentRecordIndex, maxRecord);
        }
        public void DeleteUser(UserKey key)
        {
            _userManager.DeleteUser(key);
        }
        public User UpdateUser(User user)
        {
            return _userManager.UpdateUser(user);
        }

        #region manage role
        public List<Role> FindRoles(RoleCriteria roleCriteria = null)
        {
            return _userManager.FindRoles(roleCriteria);
        }

        public Role CreateRole(Role role)
        {
            return _userManager.CreateRole(role);
        }

        public Role GetRole(RoleKey key)
        {
            return _userManager.GetRole(key);
        }

        public Role UpdateRole(Role role)
        {
            return _userManager.UpdateRole(role);
        }

        public void DeleteRole(RoleKey key)
        {
            _userManager.DeleteRole(key);
        }
        #endregion
    }
}
