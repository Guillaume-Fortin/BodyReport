using BodyReport.Data;
using BodyReport.Manager;
using BodyReport.Resources;
using BodyReport.ServiceLayers.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Globalization;

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

        #region manager accessor
        
        /// <summary>
        /// TrainingDay Manager
        /// </summary>
        private TrainingDayManager _trainingDayManager = null;

        /// <summary>
        /// Body Exercise Manager
        /// </summary>
        private BodyExerciseManager _bodyExerciseManager = null;

        /// <summary>
        /// City Manager
        /// </summary>
        private CityManager _cityManager = null;

        /// <summary>
        /// Country Manager
        /// </summary>
        private CountryManager _countryManager = null;

        /// <summary>
        /// Body Exercise Manager
        /// </summary>
        private MuscleManager _muscleManager = null;

        /// <summary>
        /// Muscular Group Manager
        /// </summary>
        private MuscularGroupManager _muscularGroupManager = null;

        /// <summary>
        /// Role Manager
        /// </summary>
        private RoleManager _roleManager = null;

        /// <summary>
        /// Training Exercise Manager
        /// </summary>
        private TrainingExerciseManager _trainingExerciseManager = null;

        /// <summary>
        /// TrainingWeek Manager
        /// </summary>
        private TrainingWeekManager _trainingWeekManager = null;

        /// <summary>
        /// Translation Manager
        /// </summary>
        private TranslationManager _translationManager = null;

        /// <summary>
        /// User info Manager
        /// </summary>
        private UserInfoManager _userInfoManager = null;

        /// <summary>
        /// User role Manager
        /// </summary>
        private UserRoleManager _userRoleManager = null;

        /// <summary>
        /// User info Manager
        /// </summary>
        private UserManager _userManager = null;

        /// <summary>
        /// Need to recreate or create new manager if null or if bd context changed
        /// </summary>
        /// <typeparam name="T">Manager type</typeparam>
        /// <param name="manager">manager</param>
        /// <returns>true if a new manager must be create</returns>
        private bool NeedCreateNewManager<T>(T manager) where T : BodyReportManager
        {
            if (manager == null || manager.DbContext != _dbContext)
                return true;
            else
                return false;
        }

        public TrainingDayManager GetTrainingDayManager()
        {
            if (NeedCreateNewManager(_trainingDayManager))
                _trainingDayManager = new TrainingDayManager(_dbContext);
            return _trainingDayManager;
        }

        public BodyExerciseManager GetBodyExerciseManager()
        {
            if (NeedCreateNewManager(_bodyExerciseManager))
                _bodyExerciseManager = new BodyExerciseManager(_dbContext);
            return _bodyExerciseManager;
        }

        public CityManager GetCityManager()
        {
            if (NeedCreateNewManager(_cityManager))
                _cityManager = new CityManager(_dbContext);
            return _cityManager;
        }

        public CountryManager GetCountryManager()
        {
            if (NeedCreateNewManager(_countryManager))
                _countryManager = new CountryManager(_dbContext);
            return _countryManager;
        }

        public MuscleManager GetMuscleManager()
        {
            if (NeedCreateNewManager(_muscleManager))
                _muscleManager = new MuscleManager(_dbContext);
            return _muscleManager;
        }

        public MuscularGroupManager GetMuscularGroupManager()
        {
            if (NeedCreateNewManager(_muscularGroupManager))
                _muscularGroupManager = new MuscularGroupManager(_dbContext);
            return _muscularGroupManager;
        }

        public RoleManager GetRoleManager()
        {
            if (NeedCreateNewManager(_roleManager))
                _roleManager = new RoleManager(_dbContext);
            return _roleManager;
        }

        public TrainingExerciseManager GetTrainingExerciseManager()
        {
            if (NeedCreateNewManager(_trainingExerciseManager))
                _trainingExerciseManager = new TrainingExerciseManager(_dbContext);
            return _trainingExerciseManager;
        }

        public TrainingWeekManager GetTrainingWeekManager()
        {
            if (NeedCreateNewManager(_trainingWeekManager))
                _trainingWeekManager = new TrainingWeekManager(_dbContext);
            return _trainingWeekManager;
        }

        public TranslationManager GetTranslationManager()
        {
            if (NeedCreateNewManager(_translationManager))
                _translationManager = new TranslationManager(_dbContext);
            return _translationManager;
        }

        public UserInfoManager GetUserInfoManager()
        {
            if (NeedCreateNewManager(_userInfoManager))
                _userInfoManager = new UserInfoManager(_dbContext);
            return _userInfoManager;
        }

        public UserRoleManager GetUserRoleManager()
        {
            if (NeedCreateNewManager(_userRoleManager))
                _userRoleManager = new UserRoleManager(_dbContext);
            return _userRoleManager;
        }

        public UserManager GetUserManager()
        {
            if (NeedCreateNewManager(_userManager))
                _userManager = new UserManager(_dbContext);
            return _userManager;
        }

        #endregion
    }
}
