using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BodyReport.Message;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BodyReport.ServiceLayers.Interfaces;
using BodyReport.Framework;
using BodyReport.Message.Web;
using System;

namespace BodyReport.Areas.Api.Controllers
{
    [Area("Api")]
    [Authorize]
    public class TranslationsController : MvcController
    {
        // <summary>
        /// ServiceLayer
        /// </summary>
        ITranslationsService _translationsService;

        public TranslationsController(UserManager<ApplicationUser> userManager,
                                      ITranslationsService translationsService) : base(userManager)
        {
            _translationsService = translationsService;
        }

        // Get api/Translations/Find
        [HttpGet]
        public IActionResult Find()
        {
            try
            {
                var result = _translationsService.FindTranslation();
                return new OkObjectResult(result); //List<TranslationVal>
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // POST api/Translations/UpdateList
        [HttpPost]
        public IActionResult UpdateList([FromBody]List<TranslationVal> translations)
        {
            try
            {
                var result = _translationsService.UpdateTranslationList(translations);
                return new OkObjectResult(result); //List<TranslationVal>
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }
    }
}
