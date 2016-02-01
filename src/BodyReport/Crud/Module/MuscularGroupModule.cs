using BodyReport.Crud.Transformer;
using BodyReport.Manager;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    /// <summary>
    /// Crud on MuscularGroup table
    /// </summary>
    public class MuscularGroupModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public MuscularGroupModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="muscularGroup">Data</param>
        /// <returns>insert data</returns>
        public MuscularGroup Create(MuscularGroup muscularGroup)
        {
            if (muscularGroup == null)
                return null;
            
            if (muscularGroup.Id == 0)
            {
                var key = new MuscularGroupKey();
                var sequencerManager = new SequencerManager();
                do
                {
                    key.Id = sequencerManager.GetNextValue(_dbContext, 1, "muscularGroup");
                }
                while (Get(key) != null); // Test Record exist
                muscularGroup.Id = key.Id;
            }

            if(muscularGroup.Id == 0)
                return null;

            var muscularGroupRow = new MuscularGroupRow();
            MuscularGroupTransformer.ToRow(muscularGroup, muscularGroupRow);
            _dbContext.MuscularGroups.Add(muscularGroupRow);
            _dbContext.SaveChanges();
            return MuscularGroupTransformer.ToBean(muscularGroupRow);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public MuscularGroup Get(MuscularGroupKey key)
        {
            if (key == null || key.Id == 0)
                return null;

            var muscularGroupRow = _dbContext.MuscularGroups.Where(m => m.Id == key.Id).FirstOrDefault();
            if (muscularGroupRow != null)
            {
                return MuscularGroupTransformer.ToBean(muscularGroupRow);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<MuscularGroup> Find()
        {
            List<MuscularGroup> resultList = null;
            var muscularGroupRowList = _dbContext.MuscularGroups;
            if (muscularGroupRowList != null && muscularGroupRowList.Count() > 0)
            {
                resultList = new List<MuscularGroup>();
                foreach (var muscularGroupRow in muscularGroupRowList)
                {
                    resultList.Add(MuscularGroupTransformer.ToBean(muscularGroupRow));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="muscularGroup">data</param>
        /// <returns>updated data</returns>
        public MuscularGroup Update(MuscularGroup muscularGroup)
        {
            if (muscularGroup == null || muscularGroup.Id == 0)
                return null;

            var muscularGroupRow = _dbContext.MuscularGroups.Where(m => m.Id == muscularGroup.Id).FirstOrDefault();
            if (muscularGroupRow == null)
            { // No data in database
                return Create(muscularGroup);
            }
            else
            { //Modify Data in database
                MuscularGroupTransformer.ToRow(muscularGroup, muscularGroupRow);
                _dbContext.SaveChanges();
                return MuscularGroupTransformer.ToBean(muscularGroupRow);
            }
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(MuscularGroupKey key)
        {
            if (key == null || key.Id == 0)
                return;

            var muscularGroupRow = _dbContext.MuscularGroups.Where(m => m.Id == key.Id).FirstOrDefault();
            if (muscularGroupRow != null)
            {
                _dbContext.MuscularGroups.Remove(muscularGroupRow);
                _dbContext.SaveChanges();
            }
        }
    }
}
