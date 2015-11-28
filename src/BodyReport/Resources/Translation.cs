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
        public static string EMAIL { get { return Get(TRS.EMAIL); } }
        public static string PASSWORD { get { return Get(TRS.PASSWORD); } }
        public static string REMEMBER_ME { get { return Get(TRS.REMEMBER_ME); } }
        public static string REGISTER_AS_NEW_USER_PI { get { return Get(TRS.REGISTER_AS_NEW_USER_PI); } }
        public static string FORGOT_YOUR_PASSWORD_PI { get { return Get(TRS.FORGOT_YOUR_PASSWORD_PI); } }
        public static string LOG_IN { get { return Get(TRS.LOG_IN); } }
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
    }
}
