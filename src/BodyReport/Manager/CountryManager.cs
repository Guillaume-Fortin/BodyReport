using BodyReport.Crud.Module;
using BodyReport.Data;
using BodyReport.Models;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    public class CountryManager : BodyReportManager
    {
        CountryModule _module = null;

        public CountryManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _module = new CountryModule(DbContext);
        }

        internal Country GetCountry(CountryKey key)
        {
            return _module.Get(key);
        }

        public List<Country> FindCountries(CountryCriteria countryCriteria = null)
        {
            return _module.Find(countryCriteria);
        }
    }
}
