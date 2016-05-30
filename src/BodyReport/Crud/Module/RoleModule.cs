using BodyReport.Crud.Transformer;
using BodyReport.Data;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Message;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    public class RoleModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public RoleModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="role">Data</param>
        /// <returns>insert data</returns>
        public Role Create(Role role)
        {
            if (role == null)
                return null;

            if (string.IsNullOrWhiteSpace(role.Id))
            {
                int newId;
                var key = new RoleKey();
                var sequencerManager = new SequencerManager();
                do
                {
                    newId = sequencerManager.GetNextValue(_dbContext, 3, "role");
                    key.Id = newId.ToString();
                }
                while (Get(key) != null); // Test Record exist

                if (newId > 0)
                    role.Id = newId.ToString();
            }

            if (string.IsNullOrWhiteSpace(role.Id))
                return null;

            var row = new IdentityRole();
            RoleTransformer.ToRow(role, row);
            _dbContext.Roles.Add(row);
            _dbContext.SaveChanges();
            return RoleTransformer.ToBean(row);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public Role Get(RoleKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.Id))
                return null;

            var row = _dbContext.Roles.Where(m => m.Id == key.Id).FirstOrDefault();
            if (row != null)
            {
                return RoleTransformer.ToBean(row);
            }
            return null;
        }

        /// <summary>
        /// Find IdentityRole in database
        /// </summary>
        /// <returns></returns>
        public List<Role> Find(RoleCriteria roleCriteria = null)
        {
            List<Role> resultList = null;
            IQueryable<IdentityRole> rowList = _dbContext.Roles;
            CriteriaTransformer.CompleteQuery(ref rowList, roleCriteria);

            if (rowList != null && rowList.Count() > 0)
            {
                resultList = new List<Role>();
                foreach (var roleRow in rowList)
                {
                    resultList.Add(RoleTransformer.ToBean(roleRow));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="role">data</param>
        /// <returns>updated data</returns>
        public Role Update(Role role)
        {
            if (role == null || string.IsNullOrWhiteSpace(role.Id))
                return null;

            var row = _dbContext.Roles.Where(m => m.Id == role.Id).FirstOrDefault();
            if (row == null)
            { // No data in database
                return Create(role);
            }
            else
            { //Modify Data in database
                RoleTransformer.ToRow(role, row);
                _dbContext.SaveChanges();
                return RoleTransformer.ToBean(row);
            }
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(RoleKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.Id))
                return;

            var row = _dbContext.Roles.Where(m => m.Id == key.Id).FirstOrDefault();
            if (row != null)
            {
                _dbContext.Roles.Remove(row);
                _dbContext.SaveChanges();
            }
        }
    }
}
