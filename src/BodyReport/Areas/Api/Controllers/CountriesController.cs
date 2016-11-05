using BodyReport.Data;
using BodyReport.Framework;
using BodyReport.Manager;
using BodyReport.Message;
using BodyReport.Message.Web;
using BodyReport.Models;
using BodyReport.ServiceLayers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.Api.Controllers
{
    [Authorize]
    [Area("Api")]
    public class CountriesController : MvcController
    {
        /// <summary>
        /// ServiceLayer Countries
        /// </summary>
        ICountriesService _countriesService = null;

        public CountriesController(UserManager<ApplicationUser> userManager,
                                   ICountriesService countriesService) : base(userManager)
        {
            _countriesService = countriesService;
        }

        // Get api/Countries/Find
        [HttpGet]
        public IActionResult Find(CountryCriteria criteria)
        {
            try
            {
                var result = _countriesService.FindCountries(criteria);
                return new OkObjectResult(result); // Country
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }
    }
}
