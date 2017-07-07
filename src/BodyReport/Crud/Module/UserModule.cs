using BodyReport.Crud.Transformer;
using BodyReport.Data;
using BodyReport.Models;
using BodyReport.Message;
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
        public User Get(UserKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.Id))
                return null;

            User user = null;
            ApplicationUser row = _dbContext.Users.Where(m => m.Id == key.Id).FirstOrDefault();

            if (row != null)
            {
                user = UserTransformer.ToBean(row);
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
            if(userCriteria == null || userCriteria.FieldSortList == null || userCriteria.FieldSortList.Count == 0)
                rowList = rowList.OrderBy(u => u.UserName);

            if (maxRecord > 0)
            {
                totalRecords = rowList.Count();
                rowList = rowList.Skip(currentRecordIndex).Take(maxRecord);
            }

            if (rowList != null)
            {
                foreach (var applicationUserRow in rowList)
                {
                    if (resultList == null)
                        resultList = new List<User>();
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
            
            var row = _dbContext.Users.Where(m => m.Id == user.Id).FirstOrDefault();
            if (row != null)
            { //Modify Data in database
                UserTransformer.ToRow(user, row);
                _dbContext.SaveChanges();
                return Get(user);
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
