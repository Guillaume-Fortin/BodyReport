using BodyReport.Data;
using BodyReport.Resources;
using BodyReport.ServiceLayers.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers
{
    public class BodyReportService
    {
        /// <summary>
        /// Database db context
        /// </summary>
        protected ApplicationDbContext _dbContext = null;

        /// <summary>
        /// Cache service
        /// </summary>
        private ICachesService _cacheService;
        
        public BodyReportService(ApplicationDbContext dbContext, ICachesService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
        }

        public void SetDbContext(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private string CompleteCacheKeyWithCulture(string cacheKey, string culture = null)
        {
            if(culture == null)
                culture = CultureInfo.CurrentCulture.Name;
            return string.Format("{0}_{1}", culture, cacheKey);
        }

        public T GetCacheData<T>(string cacheKey)
        {
            return _cacheService.GetData<T>(CompleteCacheKeyWithCulture(cacheKey));
        }

        public bool TryGetCacheData<T>(string cacheKey, out T data)
        {
            return _cacheService.TryGetData<T>(CompleteCacheKeyWithCulture(cacheKey), out data);
        }

        public void SetCacheData<T>(string cacheName, string cacheKey, T data)
        {
            var cacheOption = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5)).SetAbsoluteExpiration(TimeSpan.FromHours(1));
            _cacheService.SetData<T>(cacheName, CompleteCacheKeyWithCulture(cacheKey), data, cacheOption);
        }

        public void InvalidateCache(string cacheName)
        {
            _cacheService.InvalidateCache(cacheName);
        }

        public void RemoveCacheData(string cacheKey)
        {
            foreach (string cultureName in Translation.SupportedCultureNames)
            {
                _cacheService.RemoveCache(CompleteCacheKeyWithCulture(cacheKey, cultureName));
            }
        }

        #region Manage database transaction

        private bool _isParentTransaction = true;
        protected void BeginTransaction()
        {
            _isParentTransaction = _dbContext.Database.CurrentTransaction == null;
            if (_isParentTransaction)
            {
                _dbContext.Database.BeginTransaction();
            }
        }

        protected void CommitTransaction()
        {
            if (_isParentTransaction && _dbContext.Database.CurrentTransaction != null)
                _dbContext.Database.CurrentTransaction.Commit();
        }

        protected void RollbackTransaction()
        {
            if (_isParentTransaction && _dbContext.Database.CurrentTransaction != null)
                _dbContext.Database.CurrentTransaction.Rollback();
        }

        protected void EndTransaction()
        {
            if(_isParentTransaction && _dbContext.Database.CurrentTransaction != null)
            {
                _dbContext.Database.CurrentTransaction.Dispose();
            }
        }

        #endregion
    }
}
