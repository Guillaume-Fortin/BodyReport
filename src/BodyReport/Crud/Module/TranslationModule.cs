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
    /// Crud on Translation
    /// </summary>
    public class TranslationModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public TranslationModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="translation">Data</param>
        /// <returns>insert data</returns>
        public TranslationVal Create(TranslationVal translation)
        {
            if (translation == null || translation.CultureId < 0 || string.IsNullOrWhiteSpace(translation.Key))
                return null;
            
            var row = new TranslationRow();
            TranslationTransformer.ToRow(translation, row);
            _dbContext.Translations.Add(row);
            _dbContext.SaveChanges();

            return TranslationTransformer.ToBean(row);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public TranslationVal Get(TranslationValKey key)
        {
            if (key == null || key.CultureId < 0 || string.IsNullOrWhiteSpace(key.Key))
                return null;

            var row = _dbContext.Translations.Where(m => m.CultureId == key.CultureId && m.Key == key.Key).FirstOrDefault();
            if (row != null)
            {
                return TranslationTransformer.ToBean(row);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TranslationVal> Find()
        {
            List<TranslationVal> resultList = null;
            IQueryable<TranslationRow> rowList = _dbContext.Translations;

            if (rowList != null && rowList.Count() > 0)
            {
                resultList = new List<TranslationVal>();
                foreach (var row in rowList)
                {
                    resultList.Add(TranslationTransformer.ToBean(row));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="translation">data</param>
        /// <returns>updated data</returns>
        public TranslationVal Update(TranslationVal translation)
        {
            if (translation == null || translation.CultureId < 0 || string.IsNullOrWhiteSpace(translation.Key))
                return null;

            var row = _dbContext.Translations.Where(m => m.CultureId == translation.CultureId && m.Key == translation.Key).FirstOrDefault();
            if (row == null)
            { // No data in database
                return Create(translation);
            }
            else
            { //Modify Data in database
                TranslationTransformer.ToRow(translation, row);
                _dbContext.SaveChanges();

                return TranslationTransformer.ToBean(row);
            }
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(TranslationValKey key)
        {
            if (key == null || key.CultureId < 0 || string.IsNullOrWhiteSpace(key.Key))
                return;

            var row = _dbContext.Translations.Where(m => m.CultureId == key.CultureId && m.Key == key.Key).FirstOrDefault();
            if (row != null)
            {
                _dbContext.Translations.Remove(row);
                _dbContext.SaveChanges();
            }
        }
    }
}
