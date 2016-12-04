using BodyReport.Data;
using BodyReport.Framework;
using BodyReport.Models;
using BodyReport.Message;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Resources
{
    public class Translation
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(Translation));

        /// <summary>
        /// Supported culture names
        /// </summary>
        public readonly static string[] SupportedCultureNames = new string[] { "en-US", "fr-FR" };
        
        /// <summary>
        /// Get translation in json file
        /// </summary>
        /// <param name="key">Translation key</param>
        /// <returns>Transaltion value</returns>
        public static string Get(string key)
        {
            return GetLocalizer()[key];
        }

        /// <summary>
        /// Get override string localizer
        /// </summary>
        /// <returns></returns>
        private static IStringLocalizer GetLocalizer()
        {
            StringLocalizerFactory sl = new StringLocalizerFactory();
            return sl.Create("Translation", "Resources");
        }

        private static int GetCurrentCultureId()
        {
            int currentCultureId = 0;
            string culture = CultureInfo.CurrentCulture.Name;
            for (int i = 0; i < SupportedCultureNames.Length; i++)
            {
                if (SupportedCultureNames[i].ToLower() == culture.ToLower())
                {
                    currentCultureId = i;
                    break;
                }
            }
            return currentCultureId;
        }

        public static void DeleteInDB(string key, ApplicationDbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = new ApplicationDbContext();

            var localizer = GetLocalizer() as StringLocalizer;
            localizer.RemoveTranslationInDictionnary("DB_" + key);

            int currentCultureId = GetCurrentCultureId();
            var rows = dbContext.Translation.Where(t => t.Key.ToLower() == key.ToLower());
            foreach (TranslationRow row in rows)
            {
                dbContext.Translation.Remove(row);
            }
            dbContext.SaveChanges();
        }

        public static void UpdateInDB(string key, string value, ApplicationDbContext dbContext = null, int cultureId=-1)
        {
            if (dbContext == null)
                dbContext = new ApplicationDbContext();

            var localizer = GetLocalizer() as StringLocalizer;
            localizer.RemoveTranslationInDictionnary("DB_" + key);

            int currentCultureId;
            if (cultureId == -1)
                currentCultureId = GetCurrentCultureId();
            else
                currentCultureId = cultureId;
            TranslationRow row = dbContext.Translation.Where(t => t.CultureId == currentCultureId && t.Key.ToLower() == key.ToLower()).FirstOrDefault();
            if(row == null)
            {
                row = new TranslationRow();
                row.CultureId = currentCultureId;
                row.Key = key;
                row.Value = value;
                dbContext.Translation.Add(row);
            }
            else
                row.Value = value;
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Get translation in database
        /// </summary>
        /// <param name="key">Translation key</param>
        /// <returns>Transaltion value</returns>
        public static string GetInDB(string key, ApplicationDbContext dbContext = null)
        {
            string result = key;
            var localizer = GetLocalizer() as StringLocalizer;
            if (localizer.IsTranslationInDictionnaryExist("DB_" + key))
            {
                //Get translation in memory
                result = GetLocalizer()["DB_" + key];
            }
            else
            {
                try
                {
                    if (dbContext == null)
                        dbContext = new ApplicationDbContext();
                    int currentCultureId = GetCurrentCultureId();
                    string value = dbContext.Translation.Where(t => t.CultureId == currentCultureId && t.Key.ToLower() == key.ToLower()).Select(t => t.Value).FirstOrDefault();
                    if (value != null)
                        result = value;
                    else
                        _logger.LogInformation(string.Format("Translation database not found {0}", key));
                }
                catch (Exception except)
                {
                    _logger.LogCritical("Get translation database error", except);
                }
                //Add translation in memory
                localizer.AddTranslationInDictionnary("DB_" + key, result);
            }
            
            return result;
        }

        public static string HOME { get { return Get(TRS.HOME); } }
        public static string USER_NAME { get { return Get(TRS.USER_NAME); } }
        public static string EMAIL { get { return Get(TRS.EMAIL); } }
        public static string PASSWORD { get { return Get(TRS.PASSWORD); } }
        public static string CONFIRM_PASSWORD { get { return Get(TRS.CONFIRM_PASSWORD); } }
        public static string REMEMBER_ME { get { return Get(TRS.REMEMBER_ME); } }
        public static string REGISTER_AS_NEW_USER_PI { get { return Get(TRS.REGISTER_AS_NEW_USER_PI); } }
        public static string FORGOT_YOUR_PASSWORD_PI { get { return Get(TRS.FORGOT_YOUR_PASSWORD_PI); } }
        public static string LOG_IN { get { return Get(TRS.LOG_IN); } }
        public static string CREATE_NEW_ACCOUNT { get { return Get(TRS.CREATE_NEW_ACCOUNT); } }
        public static string REGISTER { get { return Get(TRS.REGISTER); } }
        public static string HELLO { get { return Get(TRS.HELLO); } }
        public static string EMAIL_IS_NOT_VALID { get { return Get(TRS.EMAIL_IS_NOT_VALID); } }
        public static string THE_P0_FIELD_IS_REQUIRED { get { return Get(TRS.THE_P0_FIELD_IS_REQUIRED); } }
        public static string THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1 { get { return Get(TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1); } }
        public static string THE_PASSWORD_AND_CONFIRMATION_PASSWORD_DO_NOT_MATCH { get { return Get(TRS.THE_PASSWORD_AND_CONFIRMATION_PASSWORD_DO_NOT_MATCH); } }
        public static string USE_A_LOCAL_ACCOUNT_TO_LOG_IN { get { return Get(TRS.USE_A_LOCAL_ACCOUNT_TO_LOG_IN); } }
        public static string ADMIN { get { return Get(TRS.ADMIN); } }
        public static string BODY_EXERCISES { get { return Get(TRS.BODY_EXERCISES); } }
        public static string NAME { get { return Get(TRS.NAME); } }
        public static string IMAGE { get { return Get(TRS.IMAGE); } }
        public static string ACTION { get { return Get(TRS.ACTION); } }
        public static string CREATE { get { return Get(TRS.CREATE); } }
        public static string CREATE_NEW { get { return Get(TRS.CREATE_NEW); } }
        public static string SUBMIT { get { return Get(TRS.SUBMIT); } }
        public static string FILE { get { return Get(TRS.FILE); } }
        public static string CHOOSE_FILE { get { return Get(TRS.CHOOSE_FILE); } }
        public static string MODIFY { get { return Get(TRS.MODIFY); } }
        public static string USER { get { return Get(TRS.USER); } }
        public static string USERS { get { return Get(TRS.USERS); } }
        public static string ROLE { get { return Get(TRS.ROLE); } }
        public static string MANAGE { get { return Get(TRS.MANAGE); } }
        public static string LIST_OF_USERS { get { return Get(TRS.LIST_OF_USERS); } }
        public static string ID { get { return Get(TRS.ID); } }
        public static string LIST_OF_ROLES { get { return Get(TRS.LIST_OF_ROLES); } }
        public static string NORMALIZED_NAME { get { return Get(TRS.NORMALIZED_NAME); } }
        public static string EDIT { get { return Get(TRS.EDIT); } }
        public static string DELETE { get { return Get(TRS.DELETE); } }
        public static string SUSPEND { get { return Get(TRS.SUSPEND); } }
        public static string SUSPENDED { get { return Get(TRS.SUSPENDED); } }
        public static string ACTIVATE { get { return Get(TRS.ACTIVATE); } }
        public static string SEARCH { get { return Get(TRS.SEARCH); } }
        public static string MUSCULAR_GROUP { get { return Get(TRS.MUSCULAR_GROUP); } }
        public static string LIST_OF_MUSCULAR_GROUP { get { return Get(TRS.LIST_OF_MUSCULAR_GROUP); } }
        public static string MUSCLES { get { return Get(TRS.MUSCLES); } }
        public static string MUSCLE { get { return Get(TRS.MUSCLE); } }
        public static string MY_PROFILE { get { return Get(TRS.MY_PROFILE); } }
        public static string ACCOUNT_INFORMATION { get { return Get(TRS.ACCOUNT_INFORMATION); } }
        public static string SEX { get { return Get(TRS.SEX); } }
        public static string HEIGHT { get { return Get(TRS.HEIGHT); } }
        public static string WEIGHT { get { return Get(TRS.WEIGHT); } }
        public static string ZIP_CODE { get { return Get(TRS.ZIP_CODE); } }
        public static string CITY { get { return Get(TRS.CITY); } }
        public static string COUNTRY { get { return Get(TRS.COUNTRY); } }
        public static string OBJECTIVES_OF_BODY_BUILDING { get { return Get(TRS.OBJECTIVES_OF_BODY_BUILDING); } }
        public static string MORPHOLOGY { get { return Get(TRS.MORPHOLOGY); } }
        public static string MAN { get { return Get(TRS.MAN); } }
        public static string WOMAN { get { return Get(TRS.WOMAN); } }
        public static string UNIT { get { return Get(TRS.UNIT); } }
        public static string IMPERIAL { get { return Get(TRS.IMPERIAL); } }
        public static string METRIC { get { return Get(TRS.METRIC); } }
        public static string UNIT_SYSTEM_INFO { get { return Get(TRS.UNIT_SYSTEM_INFO); } }
        public static string POUND { get { return Get(TRS.POUND); } }
        public static string INCH { get { return Get(TRS.INCH); } }
        public static string UNKNOWN { get { return Get(TRS.UNKNOWN); } }
        public static string NOT_SPECIFIED { get { return Get(TRS.NOT_SPECIFIED); } }
        public static string INVALID_INPUT_2P { get { return Get(TRS.INVALID_INPUT_2P); } }
        public static string TRAINING_JOURNAL { get { return Get(TRS.TRAINING_JOURNAL); } }
        public static string TRAINING_WEEK { get { return Get(TRS.TRAINING_WEEK); } }
        public static string TRAINING_DAY { get { return Get(TRS.TRAINING_DAY); } }
        public static string TRAINING_EXERCISE { get { return Get(TRS.TRAINING_EXERCISE); } }
        public static string WEEK_NUMBER { get { return Get(TRS.WEEK_NUMBER); } }
        public static string YEAR { get { return Get(TRS.YEAR); } }
        public static string FROM_THE_P0TH_TO_THE_P1TH_OF_P2_P3{ get { return Get(TRS.FROM_THE_P0TH_TO_THE_P1TH_OF_P2_P3); } }
        public static string MONDAY { get { return Get(TRS.MONDAY); } }
        public static string TUESDAY { get { return Get(TRS.TUESDAY); } }
        public static string WEDNESDAY { get { return Get(TRS.WEDNESDAY); } }
        public static string THURSDAY { get { return Get(TRS.THURSDAY); } }
        public static string FRIDAY { get { return Get(TRS.FRIDAY); } }
        public static string SATURDAY { get { return Get(TRS.SATURDAY); } }
        public static string SUNDAY { get { return Get(TRS.SUNDAY); } }
        public static string IMPOSSIBLE_TO_CREATE_NEW_TRAINING_JOURNAL { get { return Get(TRS.IMPOSSIBLE_TO_CREATE_NEW_TRAINING_JOURNAL); } }
        public static string DATE { get { return Get(TRS.DATE); } }
        public static string P0_ALREADY_EXIST { get { return Get(TRS.P0_ALREADY_EXIST); } }
        public static string IMPOSSIBLE_TO_CREATE_P0 { get { return Get(TRS.IMPOSSIBLE_TO_CREATE_P0); } }
        public static string IMPOSSIBLE_TO_UPDATE_P0 { get { return Get(TRS.IMPOSSIBLE_TO_UPDATE_P0); } }
        public static string P0_NOT_EXIST { get { return Get(TRS.P0_NOT_EXIST); } }
        public static string VIEW { get { return Get(TRS.VIEW); } }
        public static string CREATE_IT_PE { get { return Get(TRS.CREATE_IT_PE); } }
        public static string CREATE_NEW_PE { get { return Get(TRS.CREATE_NEW_PE); } }
        public static string YOU_HAVENT_CREATE_THIS_TRAINING_DAY { get { return Get(TRS.YOU_HAVENT_CREATE_THIS_TRAINING_DAY); } }
        public static string BEGIN_HOUR { get { return Get(TRS.BEGIN_HOUR); } }
        public static string END_HOUR { get { return Get(TRS.END_HOUR); } }
        public static string THE_FIELD_P0_SHOULD_BE_A_NUMBER_BETWEEN_P1_AND_P2 { get { return Get(TRS.THE_FIELD_P0_SHOULD_BE_A_NUMBER_BETWEEN_P1_AND_P2); } }
        public static string YES { get { return Get(TRS.YES); } }
        public static string NO { get { return Get(TRS.NO); } }
        public static string CONFIRMATION { get { return Get(TRS.CONFIRMATION); } }
        public static string ARE_YOU_SURE_YOU_WANNA_DELETE_THIS_ELEMENT_PI { get { return Get(TRS.ARE_YOU_SURE_YOU_WANNA_DELETE_THIS_ELEMENT_PI); } }
        public static string FROM { get { return Get(TRS.FROM); } }
        public static string TO { get { return Get(TRS.TO); } }
        public static string ADD_EXERCISES { get { return Get(TRS.ADD_EXERCISES); } }
        public static string REST_TIME { get { return Get(TRS.REST_TIME); } }
        public static string REPS { get { return Get(TRS.REPS); } }
        public static string ADD_REPS { get { return Get(TRS.ADD_REPS); } }
        public static string DELETE_REPS { get { return Get(TRS.DELETE_REPS); } }
        public static string SETS { get { return Get(TRS.SETS); } }
        public static string VALIDATE { get { return Get(TRS.VALIDATE); } }
        public static string DAY_OF_WEEK { get { return Get(TRS.DAY_OF_WEEK); } }
        public static string URL { get { return Get(TRS.URL); } }
        public static string DATA { get { return Get(TRS.DATA); } }
        public static string REGISTRATION_DATE { get { return Get(TRS.REGISTRATION_DATE); } }
        public static string LAST_LOGIN_DATE { get { return Get(TRS.LAST_LOGIN_DATE); } }
        public static string COPY { get { return Get(TRS.COPY); } }
        public static string ORIGIN_TRAINING_WEEK { get { return Get(TRS.ORIGIN_TRAINING_WEEK); } }
        public static string NEW_TRAINING_WEEK { get { return Get(TRS.NEW_TRAINING_WEEK); } }
        public static string INVALID_LOGIN_ATTEMPT { get { return Get(TRS.INVALID_LOGIN_ATTEMPT); } }
        public static string YOU_NEED_TO_CONFIRM_YOUR_EMAIL { get { return Get(TRS.YOU_NEED_TO_CONFIRM_YOUR_EMAIL); } }
        public static string USER_LOGGED_IN { get { return Get(TRS.USER_LOGGED_IN); } }
        public static string USER_ACCOUNT_LOCKED_OUT { get { return Get(TRS.USER_ACCOUNT_LOCKED_OUT); } }
        public static string IMPORT { get { return Get(TRS.IMPORT); } }
        public static string INSERT_ONLY { get { return Get(TRS.INSERT_ONLY); } }
        public static string TIME_ZONE { get { return Get(TRS.TIME_ZONE); } }
        public static string SWITCH_TRAINING_DAY { get { return Get(TRS.SWITCH_TRAINING_DAY); } }
        public static string RESET_YOUR_PASSWORD { get { return Get(TRS.RESET_YOUR_PASSWORD); } }
        public static string RESET { get { return Get(TRS.RESET); } }
        public static string THANK_YOU_FOR_CONFIRMING_YOUR_EMAIL { get { return Get(TRS.THANK_YOU_FOR_CONFIRMING_YOUR_EMAIL); } }
        public static string CLICK_HERE_TO_LOG_IN { get { return Get(TRS.CLICK_HERE_TO_LOG_IN); } }
        public static string ENTER_YOUR_EMAIL { get { return Get(TRS.ENTER_YOUR_EMAIL); } }
        public static string FORGOT_PASSWORD_CONFIRMATION { get { return Get(TRS.FORGOT_PASSWORD_CONFIRMATION); } }
        public static string PLEASE_CHECK_YOUR_EMAIL_TO_RESET_YOUR_PASSWORD { get { return Get(TRS.PLEASE_CHECK_YOUR_EMAIL_TO_RESET_YOUR_PASSWORD); } }
        public static string LOCKED_OUT { get { return Get(TRS.LOCKED_OUT); } }
        public static string THIS_ACCOUNT_HAS_BEEN_LOCKED_OUT_P_PLEASE_TRY_AGAIN_LATER { get { return Get(TRS.THIS_ACCOUNT_HAS_BEEN_LOCKED_OUT_P_PLEASE_TRY_AGAIN_LATER); } }
        public static string RESET_PASSWORD_CONFIRMATION { get { return Get(TRS.RESET_PASSWORD_CONFIRMATION); } }
        public static string YOUR_PASSWORD_HAS_BEEN_RESET { get { return Get(TRS.YOUR_PASSWORD_HAS_BEEN_RESET); } }
        public static string CONFIRM_EMAIL { get { return Get(TRS.CONFIRM_EMAIL); } }
        public static string EMAIL_CONFIRMED { get { return Get(TRS.EMAIL_CONFIRMED); } }
        public static string PRINT { get { return Get(TRS.PRINT); } }
        public static string PDF { get { return Get(TRS.PDF); } }
    }
}