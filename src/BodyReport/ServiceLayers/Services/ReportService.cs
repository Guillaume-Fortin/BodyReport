using BodyReport.Data;
using BodyReport.Framework;
using BodyReport.Models;
using BodyReport.ServiceLayers.Interfaces;
using BodyReport.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BodyReport.Areas.Report.ViewModels.UserReport;

namespace BodyReport.ServiceLayers.Services
{
    public class ReportService : BodyReportService, IReportService
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(ReportService));
        /// <summary>
        /// user manager
        /// </summary>
        private readonly UserManager<ApplicationUser> _identityUserManager;
        /// <summary>
        /// Email Sender
        /// </summary>
        private readonly IEmailSender _emailSender;
        /// <summary>
        /// View helper for generate view html
        /// </summary>
        private readonly ViewHelper _viewHelper;

        public ReportService(ApplicationDbContext dbContext,
                             ICachesService cacheService, 
                             UserManager<ApplicationUser> userManager,
                             IEmailSender emailSender,
                             ViewHelper viewHelper) : base(dbContext, cacheService)
        {
            _identityUserManager = userManager;
            _emailSender = emailSender;
            _viewHelper = viewHelper;
        }

        /// <inheritDoc/>
        public async Task<string> CreateReportForResetUserPasswordAsync(string userId, string callbackUrl)
        {
            string name = "~/Areas/Report/Views/UserReport/ResetUserPassword.cshtml";

            var user = await _identityUserManager.FindByIdAsync(userId);
            if (user != null)
            {
                var model = new ResetUserPasswordViewModel() { UserId = userId, UserName = user.UserName, CallbackUrl = callbackUrl };
                return await _viewHelper.RenderViewToStringAsync(name, model);
            }
            else
                throw new Exception("User not found");
        }

        /// <inheritDoc/>
        public async Task<string> CreateReportForConfirmUserAccountAsync(string userId, string callbackUrl)
        {
            string name = "~/Areas/Report/Views/UserReport/ConfirmUserAccount.cshtml";

            var user = await _identityUserManager.FindByIdAsync(userId);

            if (user != null)
            {
                var model = new ConfirmUserAccountViewModel() { UserId = userId, UserName = user.UserName, CallbackUrl = callbackUrl };
                return await _viewHelper.RenderViewToStringAsync(name, model);
            }
            else
                throw new Exception("User not found");
        }

        /// <inheritDoc/>
        public async Task<string> CreateReportForAdminNewUserAccountCreatedAsync(string userId)
        {
            string name = "~/Areas/Report/Views/UserReport/AdminNewUserAccountCreated.cshtml";

            var user = await _identityUserManager.FindByIdAsync(userId);

            if (user != null)
            {
                var model = new NewUserAccountCreatedViewModel() { UserId = userId, UserName = user.UserName, UserEmail = user.Email };
                return await _viewHelper.RenderViewToStringAsync(name, model);
            }
            else
                throw new Exception("User not found");
        }

        /// <inheritDoc/>
        public async Task<string> CreateReportForUserAccountValidatedAsync(string userId)
        {
            string name = "~/Areas/Report/Views/UserReport/UserAccountValidated.cshtml";

            var user = await _identityUserManager.FindByIdAsync(userId);

            if (user != null)
            {
                var model = new UserAccountValidatedViewModel() { UserName = user.UserName };
                return await _viewHelper.RenderViewToStringAsync(name, model);
            }
            else
                throw new Exception("User not found");
        }
    }
}
