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
    public class TrainingExerciseModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public TrainingExerciseModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="trainingJournalDayExercise">Data</param>
        /// <returns>insert data</returns>
        public TrainingExercise Create(TrainingExercise trainingJournalDayExercise)
        {
            if (trainingJournalDayExercise == null || string.IsNullOrWhiteSpace(trainingJournalDayExercise.UserId) ||
                trainingJournalDayExercise.Year == 0 || trainingJournalDayExercise.WeekOfYear == 0||
                trainingJournalDayExercise.DayOfWeek < 0 || trainingJournalDayExercise.DayOfWeek > 6 || trainingJournalDayExercise.TrainingDayId == 0 || trainingJournalDayExercise.Id == 0)
                return null;

            var row = new TrainingExerciseRow();
            TrainingExerciseTransformer.ToRow(trainingJournalDayExercise, row);
            _dbContext.TrainingExercise.Add(row);
            _dbContext.SaveChanges();
            return GetBean(row);
        }

        private TrainingExercise GetBean(TrainingExerciseRow row)
        {
            if (row == null)
                return null;

            var trainingExercise = TrainingExerciseTransformer.ToBean(row);
            if (row != null && trainingExercise != null)
            {
                if (!row.EccentricContractionTempo.HasValue)
                    trainingExercise.EccentricContractionTempo = 1;
                if (!row.StretchPositionTempo.HasValue)
                    trainingExercise.StretchPositionTempo = 0;
                if (!row.ConcentricContractionTempo.HasValue)
                    trainingExercise.ConcentricContractionTempo = 1;
                if (!row.ContractedPositionTempo.HasValue)
                    trainingExercise.ContractedPositionTempo = 0;
            }

            return trainingExercise;
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public TrainingExercise Get(TrainingExerciseKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId) ||
                key.Year == 0 || key.WeekOfYear == 0 || key.DayOfWeek < 0 || key.DayOfWeek > 6 || key.Id == 0)
                return null;

            var rowQuery = _dbContext.TrainingExercise.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
                                                              t.WeekOfYear == key.WeekOfYear && t.DayOfWeek == key.DayOfWeek &&
                                                              t.TrainingDayId == key.TrainingDayId && t.Id == key.Id);
            var row = rowQuery.FirstOrDefault();
            if (row != null)
            {
                return GetBean(row);
            }
            return null;
        }

        /// <summary>
        /// Find datas
        /// </summary>
        /// <returns></returns>
        public List<TrainingExercise> Find(TrainingExerciseCriteria trainingExerciseCriteria = null)
        {
            List<TrainingExercise> resultList = null;
            IQueryable<TrainingExerciseRow> rowList = _dbContext.TrainingExercise;
            CriteriaTransformer.CompleteQuery(ref rowList, trainingExerciseCriteria);
            rowList = rowList.OrderBy(t => t.Id);

            if (rowList != null && rowList.Count() > 0)
            {
                resultList = new List<TrainingExercise>();
                foreach (var row in rowList)
                {
                    resultList.Add(GetBean(row));
                }
            }
            return resultList;
        }

        public List<TrainingExercise> Find(CriteriaList<TrainingExerciseCriteria> trainingExerciseCriteriaList = null)
        {
            List<TrainingExercise> resultList = null;
            IQueryable<TrainingExerciseRow> rowList = _dbContext.TrainingExercise;
            CriteriaTransformer.CompleteQuery(ref rowList, trainingExerciseCriteriaList);
            rowList = rowList.OrderBy(t => t.Id);

            if (rowList != null)
            {
                foreach (var row in rowList)
                {
                    if (resultList == null)
                        resultList = new List<TrainingExercise>();
                    resultList.Add(GetBean(row));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="trainingJournalDayExercise">data</param>
        /// <returns>updated data</returns>
        public TrainingExercise Update(TrainingExercise trainingJournalDayExercise)
        {
            if (trainingJournalDayExercise == null || string.IsNullOrWhiteSpace(trainingJournalDayExercise.UserId) ||
                trainingJournalDayExercise.Year == 0 || trainingJournalDayExercise.WeekOfYear == 0 ||
                trainingJournalDayExercise.DayOfWeek < 0 || trainingJournalDayExercise.DayOfWeek > 6 || trainingJournalDayExercise.TrainingDayId == 0 || trainingJournalDayExercise.Id == 0)
                return null;

            var row = _dbContext.TrainingExercise.Where(t => t.UserId == trainingJournalDayExercise.UserId &&
                                                                        t.Year == trainingJournalDayExercise.Year &&
                                                                        t.WeekOfYear == trainingJournalDayExercise.WeekOfYear &&
                                                                        t.DayOfWeek == trainingJournalDayExercise.DayOfWeek &&
                                                                        t.TrainingDayId == trainingJournalDayExercise.TrainingDayId &&
                                                                        t.Id == trainingJournalDayExercise.Id).FirstOrDefault();
            if (row == null)
            { // No data in database
                return Create(trainingJournalDayExercise);
            }
            else
            { //Modify Data in database
                TrainingExerciseTransformer.ToRow(trainingJournalDayExercise, row);
                _dbContext.SaveChanges();
                return GetBean(row);
            }
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(TrainingExerciseKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId) || key.Year == 0 || key.WeekOfYear == 0 || key.DayOfWeek < 0 || key.DayOfWeek > 6 || key.Id == 0)
                return;

            var row = _dbContext.TrainingExercise.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
                                                                        t.WeekOfYear == key.WeekOfYear && t.DayOfWeek == key.DayOfWeek &&
                                                                        t.TrainingDayId == key.TrainingDayId && t.Id == key.Id).FirstOrDefault();
            if (row != null)
            {
                _dbContext.TrainingExercise.Remove(row);
                _dbContext.SaveChanges();
            }
        }
    }
}
