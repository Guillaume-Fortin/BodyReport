using BodyReport.Crud.Transformer;
using BodyReport.Data;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    /// <summary>
    /// Crud on Muscle table
    /// </summary>
    public class MuscleModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public MuscleModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="muscle">Data</param>
        /// <returns>insert data</returns>
        public Muscle Create(Muscle muscle)
        {
            if (muscle == null)
                return null;

            if (muscle.Id == 0)
            {
                var key = new MuscleKey();
                var sequencerManager = new SequencerManager();
                do
                {
                    key.Id = sequencerManager.GetNextValue(_dbContext, 4, "muscle");
                }
                while (Get(key) != null); // Test Record exist
                muscle.Id = key.Id;
            }

            if (muscle.Id == 0)
                return null;

            var row = new MuscleRow();
            MuscleTransformer.ToRow(muscle, row);
            _dbContext.Muscle.Add(row);
            _dbContext.SaveChanges();
            
            return MuscleTransformer.ToBean(row);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public Muscle Get(MuscleKey key)
        {
            if (key == null || key.Id == 0)
                return null;

            var row = _dbContext.Muscle.Where(m => m.Id == key.Id).FirstOrDefault();
            if (row != null)
            {
                return MuscleTransformer.ToBean(row);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Muscle> Find(MuscleCriteria muscleCriteria = null)
        {
            List<Muscle> resultList = null;
            IQueryable<MuscleRow> rowList = _dbContext.Muscle;
            CriteriaTransformer.CompleteQuery(ref rowList, muscleCriteria);

            if (rowList != null && rowList.Count() > 0)
            {
                resultList = new List<Muscle>();
                foreach (var row in rowList)
                {
                    resultList.Add(MuscleTransformer.ToBean(row));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="muscle">data</param>
        /// <returns>updated data</returns>
        public Muscle Update(Muscle muscle)
        {
            if (muscle == null || muscle.Id == 0)
                return null;

            var row = _dbContext.Muscle.Where(m => m.Id == muscle.Id).FirstOrDefault();
            if (row == null)
            { // No data in database
                return Create(muscle);
            }
            else
            { //Modify Data in database
                MuscleTransformer.ToRow(muscle, row);
                _dbContext.SaveChanges();

                return MuscleTransformer.ToBean(row);
            }
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(MuscleKey key)
        {
            if (key == null || key.Id == 0)
                return;

            var row = _dbContext.Muscle.Where(m => m.Id == key.Id).FirstOrDefault();
            if (row != null)
            {
                _dbContext.Muscle.Remove(row);
                _dbContext.SaveChanges();
            }
        }
    }
}
