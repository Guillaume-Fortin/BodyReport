using BodyReport.Crud.Transformer;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    public class TrainingJournalDayModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public TrainingJournalDayModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="trainingJournalDay">Data</param>
        /// <returns>insert data</returns>
        public TrainingJournalDay Create(TrainingJournalDay trainingJournalDay)
        {
            if (trainingJournalDay == null || string.IsNullOrWhiteSpace(trainingJournalDay.UserId) ||
                trainingJournalDay.Year == 0 || trainingJournalDay.WeekOfYear == 0 ||
                trainingJournalDay.DayOfWeek == 0 || trainingJournalDay.TrainingDayId == 0)
                return null;

            var row = new TrainingJournalDayRow();
            TrainingJournalDayTransformer.ToRow(trainingJournalDay, row);
            _dbContext.TrainingJournalDays.Add(row);
            _dbContext.SaveChanges();
            return TrainingJournalDayTransformer.ToBean(row);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public TrainingJournalDay Get(TrainingJournalDayKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId) ||
                key.Year == 0 || key.WeekOfYear == 0 || key.DayOfWeek == 0 || key.TrainingDayId == 0)
                return null;

            var row = _dbContext.TrainingJournalDays.Where(t => t.UserId == key.UserId &&
                                                                t.Year == key.Year &&
                                                                t.WeekOfYear == key.WeekOfYear &&
                                                                t.DayOfWeek == key.DayOfWeek &&
                                                                t.TrainingDayId == key.TrainingDayId).FirstOrDefault();
            if (row != null)
            {
                return TrainingJournalDayTransformer.ToBean(row);
            }
            return null;
        }

        /// <summary>
        /// Find datas
        /// </summary>
        /// <returns></returns>
        public List<TrainingJournalDay> Find(CriteriaField criteriaField = null)
        {
            List<TrainingJournalDay> resultList = null;
            IQueryable<TrainingJournalDayRow> rowList = _dbContext.TrainingJournalDays;
            CriteriaTransformer.CompleteQuery(ref rowList, criteriaField);
            rowList.OrderBy(t => t.DayOfWeek);

            if (rowList != null && rowList.Count() > 0)
            {
                resultList = new List<TrainingJournalDay>();
                foreach (var trainingJournalDayRow in rowList)
                {
                    resultList.Add(TrainingJournalDayTransformer.ToBean(trainingJournalDayRow));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="trainingJournalDay">data</param>
        /// <returns>updated data</returns>
        public TrainingJournalDay Update(TrainingJournalDay trainingJournalDay)
        {
            if (trainingJournalDay == null || string.IsNullOrWhiteSpace(trainingJournalDay.UserId) ||
                trainingJournalDay.Year == 0 || trainingJournalDay.WeekOfYear == 0 ||
                trainingJournalDay.DayOfWeek == 0 || trainingJournalDay.TrainingDayId == 0)
                return null;

            var trainingJournalRow = _dbContext.TrainingJournalDays.Where(t=>t.UserId == trainingJournalDay.UserId &&
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
                TrainingJournalDayTransformer.ToRow(trainingJournalDay, trainingJournalRow);
                _dbContext.SaveChanges();
                return TrainingJournalDayTransformer.ToBean(trainingJournalRow);
            }
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(TrainingJournalDayKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId) || key.Year == 0 || 
                key.WeekOfYear == 0 || key.DayOfWeek == 0 || key.TrainingDayId == 0)
                return;

            var row = _dbContext.TrainingJournalDays.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
                                                                t.WeekOfYear == key.WeekOfYear && t.DayOfWeek == key.DayOfWeek &&
                                                                t.TrainingDayId == key.TrainingDayId).FirstOrDefault();
            if (row != null)
            {
                _dbContext.TrainingJournalDays.Remove(row);
                _dbContext.SaveChanges();
            }
        }
    }
}
