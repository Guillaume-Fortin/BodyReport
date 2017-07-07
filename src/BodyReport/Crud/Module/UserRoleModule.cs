using BodyReport.Crud.Transformer;
using BodyReport.Data;
using BodyReport.Message;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    public class UserRoleModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public UserRoleModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="userRole">Data</param>
        /// <returns>insert data</returns>
        public UserRole Create(UserRole userRole)
        {
            if (userRole == null || string.IsNullOrEmpty(userRole.UserId) || string.IsNullOrEmpty(userRole.RoleId))
                return null;

            var row = new IdentityUserRole<string>();
            UserRoleTransformer.ToRow(userRole, row);
            _dbContext.UserRoles.Add(row);
            _dbContext.SaveChanges();
            return UserRoleTransformer.ToBean(row);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public UserRole Get(UserRoleKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId) || string.IsNullOrWhiteSpace(key.RoleId))
                return null;
            
            var row = _dbContext.UserRoles.Where(m => m.UserId == key.UserId && m.RoleId == key.RoleId).FirstOrDefault();

            if (row != null)
            {
                return UserRoleTransformer.ToBean(row);
            }

            return null;
        }

        /// <summary>
        /// Find body exercises
        /// </summary>
        /// <returns></returns>
        public List<UserRole> Find(UserRoleCriteria userRoleCriteria = null)
        {
            List<UserRole> resultList = null;
            IQueryable<IdentityUserRole<string>> rowList = _dbContext.UserRoles;
            CriteriaTransformer.CompleteQuery(ref rowList, userRoleCriteria);

            if (rowList != null)
            {
                foreach (var row in rowList)
                {
                    if (resultList == null)
                        resultList = new List<UserRole>();
                    resultList.Add(UserRoleTransformer.ToBean(row));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="userRole">data</param>
        /// <returns>updated data</returns>
        public UserRole Update(UserRole userRole)
        {
            if (userRole == null || string.IsNullOrEmpty(userRole.UserId) || string.IsNullOrEmpty(userRole.RoleId))
                return null;

            var userRoleRowList = _dbContext.UserRoles.Where(ur => ur.UserId == userRole.UserId);
            _dbContext.UserRoles.RemoveRange(userRoleRowList);
            _dbContext.SaveChanges();

            var userRoleRow = new IdentityUserRole<string>() { UserId = userRole.UserId, RoleId = userRole.RoleId };
            _dbContext.UserRoles.Add(userRoleRow);

            _dbContext.SaveChanges();
            
             return UserRoleTransformer.ToBean(userRoleRow);
        }
    }
}
