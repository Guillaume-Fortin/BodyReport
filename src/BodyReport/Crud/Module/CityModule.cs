using BodyReport.Crud.Transformer;
using BodyReport.Data;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    public class CityModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public CityModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public City Get(CityKey key)
        {
            if (key == null || (key.CountryId == 0 && string.IsNullOrWhiteSpace(key.ZipCode) && key.Id == 0))
                return null;

            var rowList = _dbContext.City.Where(m=> 1==1);
            if(key.CountryId > 0)
                rowList = rowList.Where(m => m.CountryId == key.CountryId);
            if(!string.IsNullOrWhiteSpace(key.ZipCode))
                rowList = rowList.Where(m => m.ZipCode == key.ZipCode);
            if(key.Id > 0)
                rowList = rowList.Where(m => m.Id == key.Id);

            var row = rowList.FirstOrDefault();
            if (row != null)
            {
                return CityTransformer.ToBean(row);
            }
            return null;
        }

        /// <summary>
        /// Find data in database
        /// </summary>
        /// <returns></returns>
        public List<City> Find(CriteriaField criteriaField = null)
        {
            List<City> resultList = null;
            IQueryable<CityRow> rowList = _dbContext.City;
            CriteriaTransformer.CompleteQuery(ref rowList, criteriaField);

            if (rowList != null && rowList.Count() > 0)
            {
                resultList = new List<City>();
                foreach (var row in rowList)
                {
                    resultList.Add(CityTransformer.ToBean(row));
                }
            }
            return resultList;
        }
    }
}
