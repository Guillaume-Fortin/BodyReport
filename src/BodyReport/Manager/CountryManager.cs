using BodyReport.Crud.Module;
using BodyReport.Data;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    public class CountryManager : ServiceManager
    {
        CountryModule _module = null;

        public CountryManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _module = new CountryModule(_dbContext);
        }

        internal Country GetCountry(CountryKey key)
        {
            return _module.Get(key);
        }

        public List<Country> FindCountries(CriteriaField criteriaField = null)
        {
            return _module.Find(criteriaField);
        }
    }
}
