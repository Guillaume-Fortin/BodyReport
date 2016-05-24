using BodyReport.Crud.Transformer;
using BodyReport.Data;
using Message;
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

            if (rowList != null && rowList.Count() > 0)
            {
                resultList = new List<UserRole>();
                foreach (var row in rowList)
                {
                    resultList.Add(UserRoleTransformer.ToBean(row));
                }
            }
            return resultList;
        }
    }
}
