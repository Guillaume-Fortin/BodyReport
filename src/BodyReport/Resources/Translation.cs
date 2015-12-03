using BodyReport.Framework;
using BodyReport.Models;
using Message;
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

        /// <summary>
        /// Get translation in database
        /// </summary>
        /// <param name="key">Translation key</param>
        /// <returns>Transaltion value</returns>
        public static string GetInDB(string key)
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
                    ApplicationDbContext dbContext = new ApplicationDbContext();
                    string value = dbContext.Translations.Where(t => t.CultureId == currentCultureId && t.Key.ToLower() == key.ToLower()).Select(t => t.Value).FirstOrDefault();
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
        public static string CHOOSE_FILE { get { return Get(TRS.CHOOSE_FILE); } }
        public static string MODIFY { get { return Get(TRS.MODIFY); } }
    }

    /// <summary>
    /// Home
    /// </summary>
    public class TRS
    {
        /// <summary>
        /// Home
        /// </summary>
        [Translation("Home")]
        public const string HOME = "HOME";
        /// <summary>
        /// User name
        /// </summary>
        [Translation("User name")]
        public const string USER_NAME = "USER_NAME";
        /// <summary>
        /// Email
        /// </summary>
        [Translation("Email")]
        public const string EMAIL = "EMAIL";
        /// <summary>
        /// Password
        /// </summary>
        [Translation("Password")]
        public const string PASSWORD = "PASSWORD";
        /// <summary>
        /// Confirm Password
        /// </summary>
        [Translation("Confirm Password")]
        public const string CONFIRM_PASSWORD = "CONFIRM_PASSWORD";
        /// <summary>
        /// Remember Me
        /// </summary>
        [Translation("Remember Me")]
        public const string REMEMBER_ME = "REMEMBER_ME";
        /// <summary>
        /// Register as a new user?
        /// </summary>
        [Translation("Register as a new user?")]
        public const string REGISTER_AS_NEW_USER_PI = "REGISTER_AS_NEW_USER_PI";
        /// <summary>
        /// Forgot your password?
        /// </summary>
        [Translation("Forgot your password?")]
        public const string FORGOT_YOUR_PASSWORD_PI = "FORGOT_YOUR_PASSWORD_PI";
        /// <summary>
        /// Log In
        /// </summary>
        [Translation("Log In")]
        public const string LOG_IN = "LOG_IN";
        /// <summary>
        /// Create a new account
        /// </summary>
        [Translation("Create a new account")]
        public const string CREATE_NEW_ACCOUNT = "CREATE_NEW_ACCOUNT";
        /// <summary>
        /// Register
        /// </summary>
        [Translation("Register")]
        public const string REGISTER = "REGISTER";
        /// <summary>
        /// Email is not valid
        /// </summary>
        [Translation("Email is not valid")]
        public const string EMAIL_IS_NOT_VALID = "EMAIL_IS_NOT_VALID";
        /// <summary>
        /// Hello
        /// </summary>
        [Translation("Hello")]
        public const string HELLO = "HELLO";
        /// <summary>
        /// The {0} is required
        /// </summary>
        [Translation("The {0} field is required")]
        public const string THE_P0_FIELD_IS_REQUIRED = "THE_P0_FIELD_IS_REQUIRED";
        /// <summary>
        /// The field {0} must be a string with a minimum length of {2} and a maximum length of {1}
        /// </summary>
        [Translation("The field {0} must be a string with a minimum length of {2} and a maximum length of {1}")]
        public const string THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1 = "THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1";
        /// <summary>
        /// The password and confirmation password do not match
        /// </summary>
        [Translation("The password and confirmation password do not match")]
        public const string THE_PASSWORD_AND_CONFIRMATION_PASSWORD_DO_NOT_MATCH = "THE_PASSWORD_AND_CONFIRMATION_PASSWORD_DO_NOT_MATCH";
        /// <summary>
        /// Use a local account to log in
        /// </summary>
        [Translation("Use a local account to log in")]
        public const string USE_A_LOCAL_ACCOUNT_TO_LOG_IN = "USE_A_LOCAL_ACCOUNT_TO_LOG_IN";
        /// <summary>
        /// Admin
        /// </summary>
        [Translation("Admin")]
        public const string ADMIN = "ADMIN";
        /// <summary>
        /// Body exercises
        /// </summary>
        [Translation("Body exercises")]
        public const string BODY_EXERCISES = "BODY_EXERCISES";
        // <summary>
        /// Name
        /// </summary>
        [Translation("Name")]
        public const string NAME = "NAME";
        /// <summary>
        /// Image
        /// </summary>
        [Translation("Image")]
        public const string IMAGE = "IMAGE";
        /// <summary>
        /// Action
        /// </summary>
        [Translation("Action")]
        public const string ACTION = "ACTION";
        /// <summary>
        /// Create
        /// </summary>
        [Translation("Create")]
        public const string CREATE = "CREATE";
        /// <summary>
        /// Create New
        /// </summary>
        [Translation("Create New")]
        public const string CREATE_NEW = "CREATE_NEW";
        /// <summary>
        /// Submit
        /// </summary>
        [Translation("Submit")]
        public const string SUBMIT = "SUBMIT";
        /// <summary>
        /// Choose file
        /// </summary>
        [Translation("Choose file")]
        public const string CHOOSE_FILE = "CHOOSE_FILE";
        /// <summary>
        /// Modify
        /// </summary>
        [Translation("Modify")]
        public const string MODIFY = "MODIFY";
    }
}