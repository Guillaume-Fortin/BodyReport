using BodyReport.Data;
using BodyReport.Manager;
using BodyReport.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.Api.Controllers
{
    [Authorize]
    [Area("Api")]
    public class CountriesController : Controller
    {
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
        CountryManager _countryManager = null;

        public CountriesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _countryManager = new CountryManager(_dbContext);
        }

        // Get api/Countries/Find
        [HttpGet]
        public List<Country> Find(CountryCriteria criteria)
        {
            return _countryManager.FindCountries(criteria);
        }
    }
}
