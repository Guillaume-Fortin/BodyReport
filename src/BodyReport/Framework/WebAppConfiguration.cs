using Microsoft.Extensions.Configuration;
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
    }
}
