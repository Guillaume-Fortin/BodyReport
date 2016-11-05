using BodyReport.Data;
using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Message;
using BodyReport.ServiceLayers.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Services
{
    public class CitiesService : BodyReportService, ICitiesService
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(CitiesService));
        /// <summary>
        /// City Manager
        /// </summary>
        CityManager _cityManager = null;
        public CitiesService(ApplicationDbContext dbContext) : base(dbContext)
        {
            _cityManager = new CityManager(_dbContext);
        }
        public City GetCity(CityKey key)
        {
            return _cityManager.GetCity(key);
        }

        public List<City> FindCities(CityCriteria cityCriteria = null)
        {
            return _cityManager.FindCities(cityCriteria);
        }
    }
}
