using Framework;
using Message;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Framework
{
    public static class WebAppConfiguration
    {
        public static IConfigurationRoot Configuration { get; set; }
        
        public static string DatabaseConnectionString
        {
            get
            {
                return GetStringParameterValue("Data:DefaultConnection:ConnectionString");
            }
        }

        public static TDataBaseServerType TDataBaseServerType
        {
            get
            {
                return Utils.IntToEnum<TDataBaseServerType>(GetIntParameterValue("Data:DefaultConnection:DataBaseServerType"));
            }
        }

        public static string SmtpServer
        {
            get
            {
                return GetStringParameterValue("Smtp:Server");
            }
        }

        public static int SmtpPort
        {
            get
            {
                return GetIntParameterValue("Smtp:Port");
            }
        }

        public static string SmtpEmail
        {
            get
            {
                return GetStringParameterValue("Smtp:Email");
            }
        }

        public static string SmtpUserName
        {
            get
            {
                return GetStringParameterValue("Smtp:UserName");
            }
        }

        public static string SmtpPassword
        {
            get
            {
                return GetStringParameterValue("Smtp:Password");
            }
        }

        private static string GetStringParameterValue(string key)
        {
            return Configuration[key];
        }

        private static int GetIntParameterValue(string key)
        {
            int value;
            string stringValue = GetStringParameterValue(key);
            if (int.TryParse(stringValue, out value))
            {
                return value;
            }
            else
                return 0;
        }

        #region logger

        /// <summary>
        /// new LoggerFactory().AddConsole();
        /// </summary>
        private static ILoggerFactory _loggerFactory = new LoggerFactory().AddConsole();

        /// <summary>
        /// Get a new logger attached for specific class
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        public static ILogger CreateLogger(Type classType)
        {
            return _loggerFactory.CreateLogger(classType.FullName);
        }

        #endregion
    }
}
