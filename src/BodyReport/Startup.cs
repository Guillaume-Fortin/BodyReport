using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BodyReport.Models;
using BodyReport.Services;
using System.Globalization;
using Microsoft.Extensions.Localization;
using BodyReport.Framework;
using BodyReport.Resources;
using System.IO;
using BodyReport.Models.Initializer;
using BodyReport.Message;
using Microsoft.AspNetCore.Hosting;
using BodyReport.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using BodyReport.WebApiServices;
using BodyReport.ServiceLayers.Interfaces;
using BodyReport.ServiceLayers.Services;

namespace BodyReport
{
    public class Startup
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(Startup));

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }
            
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            //Add WebApp Configuration (no dependency injection)
            WebAppConfiguration.Configuration = Configuration;
            _env = env;

            PopulateTranslationFile();
        }

        public IConfigurationRoot Configuration { get; set; }
        private IHostingEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            var sqlLoggerFactory = new LoggerFactory();
            sqlLoggerFactory.AddConsole(Configuration.GetSection("SqlLogging"));

            var entityFramework = services;
            if (WebAppConfiguration.TDataBaseServerType == Message.TDataBaseServerType.PostgreSQL)
            {
                entityFramework = entityFramework.AddEntityFrameworkNpgsql().AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(WebAppConfiguration.DatabaseConnectionString).UseLoggerFactory(sqlLoggerFactory));
            }
            else if (WebAppConfiguration.TDataBaseServerType == Message.TDataBaseServerType.SqlServer)
            {
                entityFramework = entityFramework.AddEntityFrameworkSqlServer().AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(WebAppConfiguration.DatabaseConnectionString).UseLoggerFactory(sqlLoggerFactory));
            }
            else
                _logger.LogError("Unknown database connection type");

            //Configure identity policies
            // Add Identity services to the services container.
            services.AddIdentity<ApplicationUser, IdentityRole>(
                o => {
                    o.Password.RequireDigit = false;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequiredLength = 6;
                }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //Link my own Localizer
            services.AddSingleton<IStringLocalizerFactory, StringLocalizerFactory>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<IdentityOptions>(options =>
            {
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(10);
                options.Cookies.ApplicationCookie.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Site/Account/Login");
                options.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToAccessDenied = ctx => {
                        if (ctx.Request.Path.StartsWithSegments("/api"))
                        {
                            ctx.Response.StatusCode = 403;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        return Task.FromResult(0);
                    },
                    OnRedirectToLogin = ctx => {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                        {
                            ctx.Response.StatusCode = 401;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        return Task.FromResult(0);
                    },
                };
            });

            /*
            services.Configure<CookieAuthenticationOptions>(opt =>
            {
                opt.AccessDeniedPath = "/Site/Home/Index";
                opt.LogoutPath = "/Site/Account/Login";
                opt.LoginPath = "/Site/Account/Login";
            });
            */

            services.AddMvc().AddViewLocalization(options => options.ResourcesPath = "Resources").AddDataAnnotationsLocalization();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            ConfigureServiceDataLayer(services);

            services.Configure<RequestLocalizationOptions>(options =>
            {
                DefineLocalization(options);
            });
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
                app.UseExceptionHandler("/Site/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                /*try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                             .Database.Migrate();
                    }
                }
                catch { }*/
            }

            DefineLocalization(app);
            
            app.UseStaticFiles();
            
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, "images")),
                RequestPath = new PathString("/images")
            });

            app.UseIdentity();
            app.UseCookieAuthentication();

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                // add the new route here.
                routes.MapRoute(name: "Area",
                    template: "{area:exists}/{controller}/{action}",
                    defaults: new { area="Site", controller = "Home", action = "Index" });
                
                /*routes.MapRoute(
                    name: "default",
                    template: "{controller=Site/Home}/{action=Index}/{id?}");*/
            });

            PopulateDataBase();
        }

        private void DefineLocalization(RequestLocalizationOptions options)
        {
            var cultureInfos = new List<CultureInfo>();
            var uiCultureInfos = new List<CultureInfo>();
            foreach (string cultureName in Translation.SupportedCultureNames)
            {
                cultureInfos.Add(new CultureInfo(cultureName));
                uiCultureInfos.Add(new CultureInfo(cultureName));
            }

            options.SupportedCultures = cultureInfos;
            options.SupportedUICultures = uiCultureInfos;
            options.DefaultRequestCulture = new RequestCulture(Translation.SupportedCultureNames[0]);
        }

        private void DefineLocalization(IApplicationBuilder app)
        {
            var cultureInfos = new List<CultureInfo>();
            var uiCultureInfos = new List<CultureInfo>();
            foreach (string cultureName in Translation.SupportedCultureNames)
            {
                cultureInfos.Add(new CultureInfo(cultureName));
                uiCultureInfos.Add(new CultureInfo(cultureName));
            }

            var requestLocalizationOptions = new RequestLocalizationOptions();
            DefineLocalization(requestLocalizationOptions);
            app.UseRequestLocalization(requestLocalizationOptions);
        }
        
        /// <summary>
        /// Create or Update JSON translation files for web application
        /// </summary>
        private void PopulateTranslationFile()
        {
            String fileName;
            bool isDevelopmentCurrentTranslation;
            for (int i = 0; i < Translation.SupportedCultureNames.Length; i++)
            {
                isDevelopmentCurrentTranslation = i == 0;
                fileName = string.Format("Translation-{0}.json", Translation.SupportedCultureNames[i]);
                TranslationManager.Instance.CreateOrUpdateTranslationFile<TRS>(Path.Combine("Resources", fileName), isDevelopmentCurrentTranslation);
            }
        }

        /// <summary>
        /// Populate data in database
        /// </summary>
        private void PopulateDataBase()
        {
            var dataInitialzer = new ApplicationDataInitializer(new ApplicationDbContext(), _env);
            dataInitialzer.InitializeData();
        }

        /// <summary>
        /// Configura Service Data Layer
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureServiceDataLayer(IServiceCollection services)
        {
            services.AddTransient<IBodyExercisesService, BodyExercisesService>();
            services.AddTransient<IMusclesService, MusclesService>();
            services.AddTransient<IUserInfosService, UserInfosService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<ICitiesService, CitiesService>();
            services.AddTransient<ICountriesService, CountriesService>();
            services.AddTransient<ITrainingWeeksService, TrainingWeeksService>();
            services.AddTransient<ITrainingDaysService, TrainingDaysService>();
            services.AddTransient<ITrainingExercisesService, TrainingExercisesService>();
            services.AddTransient<IMuscularGroupsService, MuscularGroupsService> ();
        }
    }
}
