using BodyReport.Crud.Transformer;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    public class TrainingWeekModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public TrainingWeekModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="trainingJournal">Data</param>
        /// <returns>insert data</returns>
        public TrainingWeek Create(TrainingWeek trainingJournal)
        {
            if (trainingJournal == null || string.IsNullOrWhiteSpace(trainingJournal.UserId) ||
                trainingJournal.Year == 0 || trainingJournal.WeekOfYear == 0)
                return null;

            var row = new TrainingWeekRow();
            TrainingWeekTransformer.ToRow(trainingJournal, row);
            _dbContext.TrainingWeeks.Add(row);
            _dbContext.SaveChanges();
            return TrainingWeekTransformer.ToBean(row);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public TrainingWeek Get(TrainingWeekKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId) ||
                key.Year == 0 || key.WeekOfYear == 0)
                return null;

            var row = _dbContext.TrainingWeeks.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
                                                                t.WeekOfYear == key.WeekOfYear).FirstOrDefault();
            if (row != null)
            {
                return TrainingWeekTransformer.ToBean(row);
            }
            return null;
        }

        /// <summary>
        /// Find datas
        /// </summary>
        /// <returns></returns>
        public List<TrainingWeek> Find(CriteriaField criteriaField = null)
        {
            List<TrainingWeek> resultList = null;
            IQueryable<TrainingWeekRow> rowList = _dbContext.TrainingWeeks;
            CriteriaTransformer.CompleteQuery(ref rowList, criteriaField);
            rowList = rowList.OrderBy(t => t.UserId).OrderByDescending(t => t.Year).ThenByDescending(t => t.WeekOfYear);

            if (rowList != null && rowList.Count() > 0)
            {
                resultList = new List<TrainingWeek>();
                foreach (var trainingJournalRow in rowList)
                {
                    resultList.Add(TrainingWeekTransformer.ToBean(trainingJournalRow));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="trainingJournal">data</param>
        /// <returns>updated data</returns>
        public TrainingWeek Update(TrainingWeek trainingJournal)
        {
            if (trainingJournal == null || string.IsNullOrWhiteSpace(trainingJournal.UserId) ||
                trainingJournal.Year == 0 || trainingJournal.WeekOfYear == 0)
                return null;

            var trainingJournalRow = _dbContext.TrainingWeeks.Where(t => t.UserId == trainingJournal.UserId && t.Year == trainingJournal.Year &&
                                                                            t.WeekOfYear == trainingJournal.WeekOfYear).FirstOrDefault();
            if (trainingJournalRow == null)
            { // No data in database
                return Create(trainingJournal);
            }
            else
            { //Modify Data in database
                TrainingWeekTransformer.ToRow(trainingJournal, trainingJournalRow);
                _dbContext.SaveChanges();
                return TrainingWeekTransformer.ToBean(trainingJournalRow);
            }
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(TrainingWeekKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId) || key.Year == 0 || key.WeekOfYear == 0)
                return;

            var row = _dbContext.TrainingWeeks.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
                                                             t.WeekOfYear == key.WeekOfYear).FirstOrDefault();
            if (row != null)
            {
                _dbContext.TrainingWeeks.Remove(row);
                _dbContext.SaveChanges();
            }
        }
    }
}
