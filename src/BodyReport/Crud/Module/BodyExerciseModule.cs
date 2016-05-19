using BodyReport.Crud.Transformer;
using BodyReport.Data;
using BodyReport.Manager;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    public class BodyExerciseModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public BodyExerciseModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="bodyExercise">Data</param>
        /// <returns>insert data</returns>
        public BodyExercise Create(BodyExercise bodyExercise)
        {
            if (bodyExercise == null)
                return null;

            if (bodyExercise.Id == 0)
            {
                var key = new BodyExerciseKey();
                var sequencerManager = new SequencerManager();
                do
                {
                    key.Id = sequencerManager.GetNextValue(_dbContext, 2, "bodyExercise");
                }
                while (Get(key) != null); // Test Record exist
                bodyExercise.Id = key.Id;
            }

            if (bodyExercise.Id == 0)
                return null;

            var bodyExerciseRow = new BodyExerciseRow();
            BodyExerciseTransformer.ToRow(bodyExercise, bodyExerciseRow);
            _dbContext.BodyExercise.Add(bodyExerciseRow);
            _dbContext.SaveChanges();
            return BodyExerciseTransformer.ToBean(bodyExerciseRow);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public BodyExercise Get(BodyExerciseKey key)
        {
            if (key == null || key.Id == 0)
                return null;

            var bodyExerciseRow = _dbContext.BodyExercise.Where(m => m.Id == key.Id).FirstOrDefault();
            if (bodyExerciseRow != null)
            {
                return BodyExerciseTransformer.ToBean(bodyExerciseRow);
            }
            return null;
        }

        /// <summary>
        /// Find body exercises
        /// </summary>
        /// <returns></returns>
        public List<BodyExercise> Find(CriteriaField criteriaField = null)
        {
            List<BodyExercise> resultList = null;
            IQueryable<BodyExerciseRow> muscularGroupRowList = _dbContext.BodyExercise;
            CriteriaTransformer.CompleteQuery(ref muscularGroupRowList, criteriaField);

            if (muscularGroupRowList != null && muscularGroupRowList.Count() > 0)
            {
                resultList = new List<BodyExercise>();
                foreach (var muscularGroupRow in muscularGroupRowList)
                {
                    resultList.Add(BodyExerciseTransformer.ToBean(muscularGroupRow));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="bodyExercise">data</param>
        /// <returns>updated data</returns>
        public BodyExercise Update(BodyExercise bodyExercise)
        {
            if (bodyExercise == null || bodyExercise.Id == 0)
                return null;

            var bodyExerciseRow = _dbContext.BodyExercise.Where(m => m.Id == bodyExercise.Id).FirstOrDefault();
            if (bodyExerciseRow == null)
            { // No data in database
                return Create(bodyExercise);
            }
            else
            { //Modify Data in database
                BodyExerciseTransformer.ToRow(bodyExercise, bodyExerciseRow);
                _dbContext.SaveChanges();
                return BodyExerciseTransformer.ToBean(bodyExerciseRow);
            }
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(BodyExerciseKey key)
        {
            if (key == null || key.Id == 0)
                return;

            var bodyExerciseRow = _dbContext.BodyExercise.Where(m => m.Id == key.Id).FirstOrDefault();
            if (bodyExerciseRow != null)
            {
                _dbContext.BodyExercise.Remove(bodyExerciseRow);
                _dbContext.SaveChanges();
            }
        }
    }
}
