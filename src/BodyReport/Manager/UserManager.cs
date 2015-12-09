﻿using BodyReport.Crud.Module;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    /// <summary>
    /// Manage Users
    /// </summary>
    public class UserManager : ServiceManager
    {
        UserModule _userModule = null;
        RoleModule _roleModule = null;
        UserRoleModule _userRoleModule = null;

        public UserManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _userModule = new UserModule(_dbContext);
            _roleModule = new RoleModule(_dbContext);
            _userRoleModule = new UserRoleModule(_dbContext);
        }

        internal User GetUser(UserKey key, bool manageRole = true)
        {
            return _userModule.Get(key, manageRole);
        }

        public List<User> FindUsers(CriteriaField criteriaField = null, bool manageRole = true)
        {
            var userList = _userModule.Find(criteriaField);

            if(userList != null && manageRole)
            {
                foreach(var user in userList)
                {
                    if(user.Role == null)
                    {
                        var userRoleCriteria = new UserRoleCriteria();
                        userRoleCriteria.UserId = new StringCriteria() { EqualList = new List<string>() { user.Id } };
                        var userRoleList = _userRoleModule.Find(userRoleCriteria);
                        if (userRoleList != null)
                        {
                            foreach (var userRole in userRoleList)
                            {
                                user.Role = _roleModule.Get(new RoleKey() { Id = userRole.RoleId });
                                break;
                            }
                        }
                    }
                }
            }

            return userList;
        }
        
        internal void DeleteUser(UserKey key)
        {
            _userModule.Delete(key);
        }

        internal User UpdateUser(User user)
        {
            user = _userModule.Update(user);
            return user;
        }

        #region manage role
        public List<Role> FindRoles(CriteriaField criteriaField = null)
        {
            return _roleModule.Find(criteriaField);
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
        #endregion
    }
}
