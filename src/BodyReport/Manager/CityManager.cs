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
    public class CityManager : BodyReportManager
    {
        CityModule _module = null;

        public CityManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _module = new CityModule(DbContext);
        }

        internal City GetCity(CityKey key)
        {
            return _module.Get(key);
        }

        public List<City> FindCities(CityCriteria cityCriteria = null)
        {
            return _module.Find(cityCriteria);
        }
    }
}
