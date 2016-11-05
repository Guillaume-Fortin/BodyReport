using BodyReport.Data;
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
    public class TranslationsService : BodyExercisesService, ITranslationsService
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = Framework.WebAppConfiguration.CreateLogger(typeof(TranslationsService));
        /// <summary>
        /// Body Exercise Manager
        /// </summary>
        TranslationManager _translationManager = null;
        public TranslationsService(ApplicationDbContext dbContext) : base(dbContext)
        {
            _translationManager = new TranslationManager(_dbContext);
        }

        public List<TranslationVal> FindTranslation()
        {
            return _translationManager.FindTranslation();
        }

        public TranslationVal UpdateTranslation(TranslationVal translation)
        {
            TranslationVal result = null;
            //Create data in database
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _translationManager.UpdateTranslation(translation);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to update translation", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return result;
        }

        public List<TranslationVal> UpdateTranslationList(List<TranslationVal> translations)
        {
            List<TranslationVal> results = null;
            //Create data in database
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (translations != null && translations.Count > 0)
                    {
                        results = new List<TranslationVal>();
                        foreach (var translation in translations)
                        {
                            results.Add(_translationManager.UpdateTranslation(translation));
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    _logger.LogCritical("Unable to update translation list", exception);
                    transaction.Rollback();
                    throw exception;
                }
            }
            return results;
        }
    }
}
