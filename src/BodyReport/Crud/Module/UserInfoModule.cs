using BodyReport.Crud.Transformer;
using BodyReport.Data;
using BodyReport.Models;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    public class UserInfoModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public UserInfoModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="userInfo">UserInfo</param>
        /// <returns>insert data</returns>
        public UserInfo Create(UserInfo userInfo)
        {
            if (userInfo == null || string.IsNullOrWhiteSpace(userInfo.UserId))
                return null;
            
            var row = new UserInfoRow();
            UserInfoTransformer.ToRow(userInfo, row);
            _dbContext.UserInfo.Add(row);
            _dbContext.SaveChanges();
            return UserInfoTransformer.ToBean(row);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public UserInfo Get(UserInfoKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId))
                return null;

            var row = _dbContext.UserInfo.Where(m => m.UserId == key.UserId).FirstOrDefault();
            if (row != null)
            {
                return UserInfoTransformer.ToBean(row);
            }
            return null;
        }

        /// <summary>
        /// Find data in database
        /// </summary>
        /// <returns></returns>
        public List<UserInfo> Find(UserInfoCriteria userInfoCriteria = null)
        {
            List<UserInfo> resultList = null;
            IQueryable<UserInfoRow> rowList = _dbContext.UserInfo;
            CriteriaTransformer.CompleteQuery(ref rowList, userInfoCriteria);

            if (rowList != null)
            {
                foreach (var userInfoRow in rowList)
                {
                    if (resultList == null)
                        resultList = new List<UserInfo>();
                    resultList.Add(UserInfoTransformer.ToBean(userInfoRow));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="userInfo">data</param>
        /// <returns>updated data</returns>
        public UserInfo Update(UserInfo userInfo)
        {
            if (userInfo == null || string.IsNullOrWhiteSpace(userInfo.UserId))
                return null;

            var row = _dbContext.UserInfo.Where(m => m.UserId == userInfo.UserId).FirstOrDefault();
            if (row == null)
            { // No data in database
                return Create(userInfo);
            }
            else
            { //Modify Data in database
                UserInfoTransformer.ToRow(userInfo, row);
                _dbContext.SaveChanges();
                return UserInfoTransformer.ToBean(row);
            }
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(UserInfoKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId))
                return;

            var row = _dbContext.UserInfo.Where(m => m.UserId == key.UserId).FirstOrDefault();
            if (row != null)
            {
                _dbContext.UserInfo.Remove(row);
                _dbContext.SaveChanges();
            }
        }
    }
}
