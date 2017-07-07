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
        public TrainingDay Create(TrainingDay trainingJournalDay, TUnitType userUnit)
        {
            if (trainingJournalDay == null || string.IsNullOrWhiteSpace(trainingJournalDay.UserId) ||
                trainingJournalDay.Year == 0 || trainingJournalDay.WeekOfYear == 0 ||
                trainingJournalDay.DayOfWeek < 0 || trainingJournalDay.DayOfWeek > 6 || trainingJournalDay.TrainingDayId == 0)
                return null;

            var row = new TrainingDayRow();
            TrainingDayTransformer.ToRow(trainingJournalDay, row);
            _dbContext.TrainingDay.Add(row);
            _dbContext.SaveChanges();
            return TrainingDayTransformer.ToBean(row, userUnit);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public TrainingDay Get(TrainingDayKey key, TUnitType userUnit)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId) ||
                key.Year == 0 || key.WeekOfYear == 0 || key.DayOfWeek < 0 || key.DayOfWeek > 6 || key.TrainingDayId == 0)
                return null;

            var row = _dbContext.TrainingDay.Where(t => t.UserId == key.UserId &&
                                                                t.Year == key.Year &&
                                                                t.WeekOfYear == key.WeekOfYear &&
                                                                t.DayOfWeek == key.DayOfWeek &&
                                                                t.TrainingDayId == key.TrainingDayId).FirstOrDefault();
            if (row != null)
            {
                return TrainingDayTransformer.ToBean(row, userUnit);
            }
            return null;
        }

        /// <summary>
        /// Find datas
        /// </summary>
        /// <returns></returns>
        public List<TrainingDay> Find(TUnitType userUnit, TrainingDayCriteria trainingDayCriteria = null)
        {
            List<TrainingDay> resultList = null;
            IQueryable<TrainingDayRow> rowList = _dbContext.TrainingDay;
            CriteriaTransformer.CompleteQuery(ref rowList, trainingDayCriteria);
            rowList = rowList.OrderBy(t => t.DayOfWeek).OrderBy(t => t.BeginHour);

            if (rowList != null)
            {
                foreach (var trainingJournalDayRow in rowList)
                {
                    if (resultList == null)
                        resultList = new List<TrainingDay>();
                    resultList.Add(TrainingDayTransformer.ToBean(trainingJournalDayRow, userUnit));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="trainingJournalDay">data</param>
        /// <returns>updated data</returns>
        public TrainingDay Update(TrainingDay trainingJournalDay, TUnitType userUnit)
        {
            if (trainingJournalDay == null || string.IsNullOrWhiteSpace(trainingJournalDay.UserId) ||
                trainingJournalDay.Year == 0 || trainingJournalDay.WeekOfYear == 0 ||
                trainingJournalDay.DayOfWeek < 0 || trainingJournalDay.DayOfWeek > 6 || trainingJournalDay.TrainingDayId == 0)
                return null;

            var trainingJournalRow = _dbContext.TrainingDay.Where(t=>t.UserId == trainingJournalDay.UserId &&
                                                                             t.Year == trainingJournalDay.Year &&
                                                                             t.WeekOfYear == trainingJournalDay.WeekOfYear &&
                                                                             t.DayOfWeek == trainingJournalDay.DayOfWeek &&
                                                                             t.TrainingDayId == trainingJournalDay.TrainingDayId).FirstOrDefault();
            if (trainingJournalRow == null)
            { // No data in database
                return Create(trainingJournalDay, userUnit);
            }
            else
            { //Modify Data in database
                TrainingDayTransformer.ToRow(trainingJournalDay, trainingJournalRow);
                _dbContext.SaveChanges();
                return TrainingDayTransformer.ToBean(trainingJournalRow, userUnit);
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

            var row = _dbContext.TrainingDay.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
                                                                t.WeekOfYear == key.WeekOfYear && t.DayOfWeek == key.DayOfWeek &&
                                                                t.TrainingDayId == key.TrainingDayId).FirstOrDefault();
            if (row != null)
            {
                _dbContext.TrainingDay.Remove(row);
                _dbContext.SaveChanges();
            }
        }
    }
}
