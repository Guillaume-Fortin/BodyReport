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
        private const string _cacheName = "CitiesCache";
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(CitiesService));
        public CitiesService(ApplicationDbContext dbContext, ICachesService cacheService) : base(dbContext, cacheService)
        {
        }
        public City GetCity(CityKey key)
        {
            City city = null;
            string cacheKey = key == null ? "CityKey_null" : key.GetCacheKey();
            if (key != null && !TryGetCacheData(cacheKey, out city))
            {
                city = GetCityManager().GetCity(key);
                SetCacheData(_cacheName, cacheKey, city);
            }
            return city;
        }

        public List<City> FindCities(CityCriteria criteria = null)
        {
            List<City> cityList = null;
            string cacheKey = criteria == null ? "CityCriteria_null" : criteria.GetCacheKey();
            if (!TryGetCacheData(cacheKey, out cityList))
            {
                cityList = GetCityManager().FindCities(criteria);
                SetCacheData(_cacheName, cacheKey, cityList);
            }
            return cityList;
        }
    }
}
