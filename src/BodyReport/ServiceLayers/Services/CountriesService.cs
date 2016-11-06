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
    public class CountriesService : BodyReportService, ICountriesService
    {
        private const string _cacheName = "CountriesCache";
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(CountriesService));
        /// <summary>
        /// City Manager
        /// </summary>
        CountryManager _countryManager = null;
        public CountriesService(ApplicationDbContext dbContext, ICachesService cacheService) : base(dbContext, cacheService)
        {
            _countryManager = new CountryManager(_dbContext);
        }

        public Country GetCountry(CountryKey key)
        {
            Country country = null;
            string cacheKey = key == null ? "CountryKey_null" : key.GetCacheKey();
            if (key != null && !TryGetCacheData(cacheKey, out country))
            {
                country = _countryManager.GetCountry(key);
                SetCacheData(_cacheName, cacheKey, country);
            }
            return country;
        }

        public List<Country> FindCountries(CountryCriteria criteria = null)
        {
            List<Country> countryList = null;
            string cacheKey = criteria == null ? "CountryCriteria_null" : criteria.GetCacheKey();
            if (!TryGetCacheData(cacheKey, out countryList))
            {
                countryList = _countryManager.FindCountries(criteria);
                SetCacheData(_cacheName, cacheKey, countryList);
            }
            return countryList;
        }
    }
}
