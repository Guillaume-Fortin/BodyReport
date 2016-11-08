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
        private const string _cacheName = "TranslationsCache";
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = Framework.WebAppConfiguration.CreateLogger(typeof(TranslationsService));
        /// <summary>
        /// Body Exercise Manager
        /// </summary>
        TranslationManager _translationManager = null;
        public TranslationsService(ApplicationDbContext dbContext, ICachesService cacheService) : base(dbContext, cacheService)
        {
            _translationManager = new TranslationManager(_dbContext);
        }

        public List<TranslationVal> FindTranslation()
        {
            List<TranslationVal> translationList = null;
            //string cacheKey = criteria == null ? "TranslationValCriteria_null" : criteria.GetCacheKey();
            string cacheKey = string.Format("TranslationValCriteria_null");
            if (!TryGetCacheData(cacheKey, out translationList))
            {
                translationList = _translationManager.FindTranslation();
                SetCacheData(_cacheName, cacheKey, translationList);
            }
            return translationList;
        }

        public TranslationVal UpdateTranslation(TranslationVal translation)
        {
            TranslationVal result = null;
            //Create data in database
            BeginTransaction();
            try
            {
                result = _translationManager.UpdateTranslation(translation);
                CommitTransaction();
                //invalidate cache
                InvalidateCache(_cacheName);
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to update translation", exception);
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
            return result;
        }

        public List<TranslationVal> UpdateTranslationList(List<TranslationVal> translations)
        {
            List<TranslationVal> results = null;
            //Create data in database
            BeginTransaction();
            try
            {
                if (translations != null && translations.Count > 0)
                {
                    results = new List<TranslationVal>();
                    foreach (var translation in translations)
                    {
                        results.Add(_translationManager.UpdateTranslation(translation));
                    }
                    CommitTransaction();
                    //invalidate cache
                    InvalidateCache(_cacheName);
                }
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable to update translation list", exception);
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
            return results;
        }
    }
}
