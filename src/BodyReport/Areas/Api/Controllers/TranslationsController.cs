using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Resources;
using Message;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BodyReport.Areas.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    //[AllowAnonymous]
    [Area("Api")]
    public class TranslationsController : Controller
    {
        /// <summary>
        /// Database db context
        /// </summary>
        ApplicationDbContext _dbContext = null;
        TranslationManager _manager = null;

        public TranslationsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _manager = new TranslationManager(_dbContext);
        }

        // Get api/Translations/Find
        [HttpGet]
        public List<TranslationVal> Find()
        {
            return _manager.FindTranslation();
        }

        // POST api/Translations/Post
        [HttpPost]
        public List<TranslationVal> Post([FromBody]List<TranslationVal> translations)
        {
            List<TranslationVal> results = new List<TranslationVal>();
            if (translations != null && translations.Count > 0)
            {
                foreach (var translation in translations)
                {
                    results.Add(_manager.UpdateTranslation(translation));
                }
            }
            return results;
        }
    }
}
