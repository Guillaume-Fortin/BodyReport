using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BodyReport.Data;
using BodyReport.Models;
using BodyReport.Services;
using BodyReport.Framework;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authentication.Cookies;
using BodyReport.Resources;
using BodyReport.Message;
using BodyReport.Models.Initializer;
using BodyReport.ServiceLayers.Interfaces;
using BodyReport.ServiceLayers.Services;
using Microsoft.AspNetCore.Authorization;
using BodyReport.Framework.CustomAttributes;

namespace BodyReport
{
    public class Startup
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(Startup));

        private IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets("aspnet-BodyReport-9b4c756c-e178-49cb-9314-50fd338dbd9a");
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            //Add WebApp Configuration (no dependency injection)
            WebAppConfiguration.Configuration = Configuration;
            _env = env;

            PopulateTranslationFile();
        }

        public IConfigurationRoot Configuration { get; }

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

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AllowLocalhost",
                                  policy => policy.Requirements.Add(new LoopBackAuthorizeRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, LoopBackAuthorizeHandler>();

            //Add service for manage cache data
            services.AddMemoryCache();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            ConfigureServiceDataLayer(services);

            services.Configure<RequestLocalizationOptions>(options =>
            {
                DefineLocalization(options);
            });

            WebAppConfiguration.ServiceProvider = services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Site/Home/Error");
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

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

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
        private Task PopulateDataBase()
        {
            var dataInitialzer = new ApplicationDataInitializer(new ApplicationDbContext(), _env);
            dataInitialzer.InitializeData();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Configura Service Data Layer
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureServiceDataLayer(IServiceCollection services)
        {
            services.AddTransient<IBodyExercisesService, BodyExercisesService>();
            services.AddTransient<ICitiesService, CitiesService>();
            services.AddTransient<ICountriesService, CountriesService>();
            services.AddTransient<IMusclesService, MusclesService>();
            services.AddTransient<IMuscularGroupsService, MuscularGroupsService>();
            services.AddTransient<ITrainingDaysService, TrainingDaysService>();
            services.AddTransient<ITrainingExercisesService, TrainingExercisesService>();
            services.AddTransient<ITrainingWeeksService, TrainingWeeksService>();
            services.AddTransient<ITranslationsService, TranslationsService>();
            services.AddTransient<IUserInfosService, UserInfosService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IUserRolesService, UserRolesService>();
            services.AddTransient<IRolesService, RolesService>();
            services.AddTransient<ICachesService, CachesService>();
        }
    }
}
