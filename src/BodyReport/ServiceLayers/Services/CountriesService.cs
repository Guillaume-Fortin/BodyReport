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
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(CountriesService));
        /// <summary>
        /// City Manager
        /// </summary>
        CountryManager _countryManager = null;
        public CountriesService(ApplicationDbContext dbContext) : base(dbContext)
        {
            _countryManager = new CountryManager(_dbContext);
        }

        public Country GetCountry(CountryKey key)
        {
            return _countryManager.GetCountry(key);
        }

        public List<Country> FindCountries(CountryCriteria countryCriteria = null)
        {
            return _countryManager.FindCountries(countryCriteria);
        }
    }
}
