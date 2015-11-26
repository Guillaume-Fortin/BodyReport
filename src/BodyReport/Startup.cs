using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BodyReport.Models;
using BodyReport.Services;
using Microsoft.AspNet.Localization;
using System.Globalization;
using Microsoft.Extensions.Localization;
using BodyReport.Framework;
using Framework;
using BodyReport.Resources;
using System.IO;

namespace BodyReport
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            CreateOrUpdateTranslationFile();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //Link my own Localizer
            services.AddSingleton<IStringLocalizerFactory, StringLocalizerFactory>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc().AddViewLocalization(options => options.ResourcesPath = "Resources").AddDataAnnotationsLocalization();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                             .Database.Migrate();
                    }
                }
                catch { }
            }

            DefineLocalization(app);

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseStaticFiles();

            app.UseIdentity();

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void DefineLocalization(IApplicationBuilder app)
        {
            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                SupportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("fr-FR"),
                    new CultureInfo("en-US")
                },
                SupportedUICultures = new List<CultureInfo>
                {
                    new CultureInfo("fr-FR"),
                    new CultureInfo("en-US")
                }
            };

            app.UseRequestLocalization(requestLocalizationOptions, defaultRequestCulture: new RequestCulture("en-US"));
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
        
        private void CreateOrUpdateTranslationFile()
        {
            TranslationManager.Instance.CreateOrUpdateTranslationFile<TRS>(Path.Combine("Resources", "Translation-en-US.json"));
        }
    }
}
