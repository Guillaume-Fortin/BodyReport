using BodyReport.Crud.Transformer;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    public class TrainingJournalModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public TrainingJournalModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="trainingJournal">Data</param>
        /// <returns>insert data</returns>
        public TrainingJournal Create(TrainingJournal trainingJournal)
        {
            if (trainingJournal == null || string.IsNullOrWhiteSpace(trainingJournal.UserId) ||
                trainingJournal.Year == 0 || trainingJournal.WeekOfYear == 0)
                return null;

            var row = new TrainingJournalRow();
            TrainingJournalTransformer.ToRow(trainingJournal, row);
            _dbContext.TrainingJournals.Add(row);
            _dbContext.SaveChanges();
            return TrainingJournalTransformer.ToBean(row);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public TrainingJournal Get(TrainingJournalKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId) ||
                key.Year == 0 || key.WeekOfYear == 0)
                return null;

            var row = _dbContext.TrainingJournals.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
                                                                t.WeekOfYear == key.WeekOfYear).FirstOrDefault();
            if (row != null)
            {
                return TrainingJournalTransformer.ToBean(row);
            }
            return null;
        }

        /// <summary>
        /// Find datas
        /// </summary>
        /// <returns></returns>
        public List<TrainingJournal> Find(CriteriaField criteriaField = null)
        {
            List<TrainingJournal> resultList = null;
            IQueryable<TrainingJournalRow> rowList = _dbContext.TrainingJournals;
            CriteriaTransformer.CompleteQuery(ref rowList, criteriaField);
            rowList.OrderBy(t => t.UserId).OrderBy(t => t.Year).OrderBy(t => t.WeekOfYear);

            if (rowList != null && rowList.Count() > 0)
            {
                resultList = new List<TrainingJournal>();
                foreach (var trainingJournalRow in rowList)
                {
                    resultList.Add(TrainingJournalTransformer.ToBean(trainingJournalRow));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="trainingJournal">data</param>
        /// <returns>updated data</returns>
        public TrainingJournal Update(TrainingJournal trainingJournal)
        {
            if (trainingJournal == null || string.IsNullOrWhiteSpace(trainingJournal.UserId) ||
                trainingJournal.Year == 0 || trainingJournal.WeekOfYear == 0)
                return null;

            var trainingJournalRow = _dbContext.TrainingJournals.Where(t => t.UserId == trainingJournal.UserId && t.Year == trainingJournal.Year &&
                                                                            t.WeekOfYear == trainingJournal.WeekOfYear).FirstOrDefault();
            if (trainingJournalRow == null)
            { // No data in database
                return Create(trainingJournal);
            }
            else
            { //Modify Data in database
                TrainingJournalTransformer.ToRow(trainingJournal, trainingJournalRow);
                _dbContext.SaveChanges();
                return TrainingJournalTransformer.ToBean(trainingJournalRow);
            }
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(TrainingJournalKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.UserId) || key.Year == 0 || key.WeekOfYear == 0)
                return;

            var row = _dbContext.TrainingJournals.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
                                                             t.WeekOfYear == key.WeekOfYear).FirstOrDefault();
            if (row != null)
            {
                _dbContext.TrainingJournals.Remove(row);
                _dbContext.SaveChanges();
            }
        }
    }
}
