using BodyReport.Crud.Transformer;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    public class TrainingJournalDayExerciseModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public TrainingJournalDayExerciseModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="trainingJournalDayExercise">Data</param>
        /// <returns>insert data</returns>
        public TrainingJournalDayExercise Create(TrainingJournalDayExercise trainingJournalDayExercise)
        {
            if (trainingJournalDayExercise == null || string.IsNullOrWhiteSpace(trainingJournalDayExercise.UserId) ||
                trainingJournalDayExercise.Year == 0 || trainingJournalDayExercise.WeekOfYear == 0||
                trainingJournalDayExercise.DayOfWeek == 0 || trainingJournalDayExercise.TrainingDayId == 0)
                return null;

            var row = new TrainingJournalDayExerciseRow();
            TrainingJournalDayExerciseTransformer.ToRow(trainingJournalDayExercise, row);
            _dbContext.TrainingJournalDayExercises.Add(row);
            _dbContext.SaveChanges();
            return TrainingJournalDayExerciseTransformer.ToBean(row);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public TrainingJournalDayExercise Get(TrainingJournalDayExerciseKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId) ||
                key.Year == 0 || key.WeekOfYear == 0 || key.DayOfWeek == 0)
                return null;

            var row = _dbContext.TrainingJournalDayExercises.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
                                                                t.WeekOfYear == key.WeekOfYear && t.DayOfWeek == key.DayOfWeek ||
                                                                t.TrainingDayId == key.TrainingDayId).FirstOrDefault();
            if (row != null)
            {
                return TrainingJournalDayExerciseTransformer.ToBean(row);
            }
            return null;
        }

        /// <summary>
        /// Find datas
        /// </summary>
        /// <returns></returns>
        public List<TrainingJournalDayExercise> Find(CriteriaField criteriaField = null)
        {
            List<TrainingJournalDayExercise> resultList = null;
            IQueryable<TrainingJournalDayExerciseRow> rowList = _dbContext.TrainingJournalDayExercises;
            CriteriaTransformer.CompleteQuery(ref rowList, criteriaField);
            rowList.OrderBy(t => t.DayOfWeek);

            if (rowList != null && rowList.Count() > 0)
            {
                resultList = new List<TrainingJournalDayExercise>();
                foreach (var row in rowList)
                {
                    resultList.Add(TrainingJournalDayExerciseTransformer.ToBean(row));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="trainingJournalDayExercise">data</param>
        /// <returns>updated data</returns>
        public TrainingJournalDayExercise Update(TrainingJournalDayExercise trainingJournalDayExercise)
        {
            if (trainingJournalDayExercise == null || string.IsNullOrWhiteSpace(trainingJournalDayExercise.UserId) ||
                trainingJournalDayExercise.Year == 0 || trainingJournalDayExercise.WeekOfYear == 0 ||
                trainingJournalDayExercise.DayOfWeek == 0 || trainingJournalDayExercise.TrainingDayId == 0)
                return null;

            var row = _dbContext.TrainingJournalDayExercises.Where(t => t.UserId == trainingJournalDayExercise.UserId &&
                                                                        t.Year == trainingJournalDayExercise.Year &&
                                                                        t.WeekOfYear == trainingJournalDayExercise.WeekOfYear &&
                                                                        t.DayOfWeek == trainingJournalDayExercise.DayOfWeek &&
                                                                        t.TrainingDayId == trainingJournalDayExercise.TrainingDayId).FirstOrDefault();
            if (row == null)
            { // No data in database
                return Create(trainingJournalDayExercise);
            }
            else
            { //Modify Data in database
                TrainingJournalDayExerciseTransformer.ToRow(trainingJournalDayExercise, row);
                _dbContext.SaveChanges();
                return TrainingJournalDayExerciseTransformer.ToBean(row);
            }
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(TrainingJournalDayExerciseKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId) || key.Year == 0 || key.WeekOfYear == 0 || key.DayOfWeek == 0)
                return;

            var row = _dbContext.TrainingJournalDayExercises.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
                                                                        t.WeekOfYear == key.WeekOfYear && t.DayOfWeek == key.DayOfWeek &&
                                                                        t.TrainingDayId == key.TrainingDayId).FirstOrDefault();
            if (row != null)
            {
                _dbContext.TrainingJournalDayExercises.Remove(row);
                _dbContext.SaveChanges();
            }
        }
    }
}
