using BodyReport.Crud.Transformer;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    public class CountryModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public CountryModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public Country Get(CountryKey key)
        {
            if (key == null || key.Id == 0)
                return null;

            var row = _dbContext.Countries.Where(m => m.Id == key.Id).FirstOrDefault();
            if (row != null)
            {
                return CountryTransformer.ToBean(row);
            }
            return null;
        }

        /// <summary>
        /// Find data in database
        /// </summary>
        /// <returns></returns>
        public List<Country> Find(CriteriaField criteriaField = null)
        {
            List<Country> resultList = null;
            IQueryable<CountryRow> rowList = _dbContext.Countries;
            CriteriaTransformer.CompleteQuery(ref rowList, criteriaField);

            if (rowList != null && rowList.Count() > 0)
            {
                resultList = new List<Country>();
                foreach (var row in rowList)
                {
                    resultList.Add(CountryTransformer.ToBean(row));
                }
            }
            return resultList;
        }
    }
}
