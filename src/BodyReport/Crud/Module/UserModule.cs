using BodyReport.Crud.Transformer;
using BodyReport.Data;
using BodyReport.Models;
using Message;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    public class UserModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public UserModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public User Get(UserKey key, bool manageRole=true)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.Id))
                return null;

            User user = null;
            if (manageRole)
            {
                var joinQuery =
                     from dbUser in _dbContext.Users
                     join dbUserRole in _dbContext.UserRoles on dbUser.Id equals dbUserRole.UserId into ps
                     from dbUserRole in ps.DefaultIfEmpty()
                     where dbUser.Id == key.Id
                     select new { User = dbUser, UserRole = dbUserRole };

                var joinRow = joinQuery.FirstOrDefault();

                if (joinRow != null)
                {
                    user = UserTransformer.ToBean(joinRow.User);
                    if (user != null && joinRow.UserRole != null)
                    {
                        RoleModule roleModule = new RoleModule(_dbContext);
                        user.Role = roleModule.Get(new RoleKey { Id = joinRow.UserRole.RoleId });
                    }
                }
            }
            else
            {
                ApplicationUser row = _dbContext.Users.Where(m => m.Id == key.Id).FirstOrDefault();

                if (row != null)
                {
                    user = UserTransformer.ToBean(row);
                }
            }

            return user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<User> Find(out int totalRecords, UserCriteria userCriteria = null, int currentRecordIndex = 0, int maxRecord = 0)
        {
            totalRecords = 0;
            List<User> resultList = null;
            IQueryable<ApplicationUser> rowList = _dbContext.Users;
            CriteriaTransformer.CompleteQuery(ref rowList, userCriteria);
            rowList = rowList.OrderBy(u => u.UserName);
            
            if (maxRecord > 0)
            {
                totalRecords = rowList.Count();
                rowList = rowList.Skip(currentRecordIndex).Take(maxRecord);
            }

            if (rowList != null && rowList.Count() > 0)
            {
                resultList = new List<User>();
                foreach (var applicationUserRow in rowList)
                {
                    resultList.Add(UserTransformer.ToBean(applicationUserRow));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="user">data</param>
        /// <returns>updated data</returns>
        public User Update(User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Id))
                return null;

            bool loadRole = false;
            var row = _dbContext.Users.Where(m => m.Id == user.Id).FirstOrDefault();
            if (row != null)
            { //Modify Data in database
                UserTransformer.ToRow(user, row);

                //Update UserRole
                if (user.Role != null)
                {
                    loadRole = true;
                    var userRoleRowList = _dbContext.UserRoles.Where(ur => ur.UserId == user.Id);
                    _dbContext.UserRoles.RemoveRange(userRoleRowList);
                    _dbContext.SaveChanges();

                    var userRoleRow = new IdentityUserRole<string>() { UserId = user.Id, RoleId = user.Role.Id };
                    _dbContext.UserRoles.Add(userRoleRow);
                }
                
                _dbContext.SaveChanges();
                return Get(user, loadRole); // for reload all data
            }
            return null;
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(UserKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.Id))
                return;

            var row = _dbContext.Users.Where(m => m.Id == key.Id).FirstOrDefault();
            if (row != null)
            {
                _dbContext.Users.Remove(row);
                _dbContext.SaveChanges();
            }
        }
    }
}
