using BodyReport.Framework;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Resources
{
    public class Translation
    {
        public static string Get(string key)
        {
            StringLocalizerFactory sl = new StringLocalizerFactory();
            var localizer = sl.Create("Translation", "Resources");
            return localizer[key];
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
