using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace BodyReport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                             .AddEnvironmentVariables("")
                             .Build();
            var url = config["ASPNETCORE_URLS"] ?? "http://*:5000";
            var env = config["ASPNETCORE_ENVIRONMENT"] ?? "Development";

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(url)
                .UseEnvironment(env)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
