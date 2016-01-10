using BodyReport.Crud.Transformer;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    public class TrainingDayModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public TrainingDayModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="trainingJournalDay">Data</param>
        /// <returns>insert data</returns>
        public TrainingDay Create(TrainingDay trainingJournalDay)
        {
            if (trainingJournalDay == null || string.IsNullOrWhiteSpace(trainingJournalDay.UserId) ||
                trainingJournalDay.Year == 0 || trainingJournalDay.WeekOfYear == 0 ||
                trainingJournalDay.DayOfWeek < 0 || trainingJournalDay.DayOfWeek > 6 || trainingJournalDay.TrainingDayId == 0)
                return null;

            var row = new TrainingDayRow();
            TrainingDayTransformer.ToRow(trainingJournalDay, row);
            _dbContext.TrainingDays.Add(row);
            _dbContext.SaveChanges();
            return TrainingDayTransformer.ToBean(row);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public TrainingDay Get(TrainingDayKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId) ||
                key.Year == 0 || key.WeekOfYear == 0 || key.DayOfWeek < 0 || key.DayOfWeek > 6 || key.TrainingDayId == 0)
                return null;

            var row = _dbContext.TrainingDays.Where(t => t.UserId == key.UserId &&
                                                                t.Year == key.Year &&
                                                                t.WeekOfYear == key.WeekOfYear &&
                                                                t.DayOfWeek == key.DayOfWeek &&
                                                                t.TrainingDayId == key.TrainingDayId).FirstOrDefault();
            if (row != null)
            {
                return TrainingDayTransformer.ToBean(row);
            }
            return null;
        }

        /// <summary>
        /// Find datas
        /// </summary>
        /// <returns></returns>
        public List<TrainingDay> Find(CriteriaField criteriaField = null)
        {
            List<TrainingDay> resultList = null;
            IQueryable<TrainingDayRow> rowList = _dbContext.TrainingDays;
            CriteriaTransformer.CompleteQuery(ref rowList, criteriaField);
            rowList = rowList.OrderBy(t => t.DayOfWeek).OrderBy(t => t.BeginHour);

            if (rowList != null && rowList.Count() > 0)
            {
                resultList = new List<TrainingDay>();
                foreach (var trainingJournalDayRow in rowList)
                {
                    resultList.Add(TrainingDayTransformer.ToBean(trainingJournalDayRow));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="trainingJournalDay">data</param>
        /// <returns>updated data</returns>
        public TrainingDay Update(TrainingDay trainingJournalDay)
        {
            if (trainingJournalDay == null || string.IsNullOrWhiteSpace(trainingJournalDay.UserId) ||
                trainingJournalDay.Year == 0 || trainingJournalDay.WeekOfYear == 0 ||
                trainingJournalDay.DayOfWeek < 0 || trainingJournalDay.DayOfWeek > 6 || trainingJournalDay.TrainingDayId == 0)
                return null;

            var trainingJournalRow = _dbContext.TrainingDays.Where(t=>t.UserId == trainingJournalDay.UserId &&
                                                                             t.Year == trainingJournalDay.Year &&
                                                                             t.WeekOfYear == trainingJournalDay.WeekOfYear &&
                                                                             t.DayOfWeek == trainingJournalDay.DayOfWeek &&
                                                                             t.TrainingDayId == trainingJournalDay.TrainingDayId).FirstOrDefault();
            if (trainingJournalRow == null)
            { // No data in database
                return Create(trainingJournalDay);
            }
            else
            { //Modify Data in database
                TrainingDayTransformer.ToRow(trainingJournalDay, trainingJournalRow);
                _dbContext.SaveChanges();
                return TrainingDayTransformer.ToBean(trainingJournalRow);
            }
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(TrainingDayKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId) || key.Year == 0 || 
                key.WeekOfYear == 0 || key.DayOfWeek < 0 || key.DayOfWeek > 6 || key.TrainingDayId == 0)
                return;

            var row = _dbContext.TrainingDays.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
                                                                t.WeekOfYear == key.WeekOfYear && t.DayOfWeek == key.DayOfWeek &&
                                                                t.TrainingDayId == key.TrainingDayId).FirstOrDefault();
            if (row != null)
            {
                _dbContext.TrainingDays.Remove(row);
                _dbContext.SaveChanges();
            }
        }
    }
}
