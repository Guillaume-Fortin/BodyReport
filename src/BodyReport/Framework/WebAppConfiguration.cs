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
                return Configuration["Data:DefaultConnection:ConnectionString"];
            }
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
