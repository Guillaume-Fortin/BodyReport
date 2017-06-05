using BodyReport.Framework;
using BodyReport.Message;
using BodyReport.Models;
using BodyReport.Resources;
using BodyReport.ServiceLayers.Interfaces;
using BodyReport.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BodyReport.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class MailingController : MvcController
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(BodyExerciseController));
        /// <summary>
        /// Service layer users
        /// </summary>
        private readonly IUsersService _usersService;
        /// <summary>
        /// Report Service
        /// </summary>
        private IReportService _reportService;
        /// <summary>
        /// Email sender
        /// </summary>
        private readonly IEmailSender _emailSender;

        public MailingController(UserManager<ApplicationUser> userManager, IUsersService usersService, IReportService reportService, IEmailSender emailSender) : base(userManager)
        {
            _reportService = reportService;
            _emailSender = emailSender;
            _usersService = usersService;
        }

        /// <summary>
        /// Mailing test
        /// GET: /Admin/Mailing/Index
        /// </summary>
        /// <param name="type">Mailing test type</param>
        /// <param name="preview">True if only for preview</param>
        /// <returns>Result Page</returns>
        [HttpGet]
        public async Task<IActionResult> Index(TMailingTestType? type, Boolean preview = false)
        {
            if(type == null)
                return View();

            MailTestResult mailTestResult = null;
            try
            {
                if (type == TMailingTestType.USER_CONFIRM_ACCOUNT)
                    mailTestResult = await UserConfirmAccountMailAsync(preview);
                else if (type == TMailingTestType.USER_RESET_PASSWORD)
                    mailTestResult = await UserResetPasswordMailAsync(preview);
                else if (type == TMailingTestType.USER_ACCOUNT_VALIDATED)
                    mailTestResult = await UserAccountValidatedAsync(preview);
                else if (type == TMailingTestType.ADMIN_NEW_USER_ACCOUNT)
                    mailTestResult = await AdminNewUserAccountCreatedAsync(preview);
            }
            catch(Exception except)
            {
                ViewBag.TestException = except;
            }

            ViewBag.TestResult = mailTestResult?.Result ?? false;
            if (preview)
                ViewBag.PreviewResult = mailTestResult?.MailData;

            return View();
        }

        /// <summary>
        /// Test : Preview or send Email for user reset password
        /// </summary>
        /// <param name="preview">True if only for preview</param>
        /// <returns>Mail test result</returns>
        private async Task<MailTestResult> UserResetPasswordMailAsync(bool preview)
        {
            var user = _usersService.GetUser(new UserKey() { Id = SessionUserId });
            var callbackUrl = Url.Action("ResetPassword", "Account", new { area = "Site", userId = user.Id, code = "nothing" }, protocol: HttpContext.Request.Scheme);
            string reportData = await _reportService.CreateReportForResetUserPasswordAsync(user.Id, callbackUrl);
            if(!preview)
                await _emailSender.SendEmailAsync(user.Email, Translation.RESET_YOUR_PASSWORD, reportData);
            return new MailTestResult() { Result = true, MailData = reportData };
        }

        /// <summary>
        /// Test : Preview or send Email for confirmuser account
        /// </summary>
        /// <param name="preview">True if only for preview</param>
        /// <returns>Mail test result</returns>
        private async Task<MailTestResult> UserConfirmAccountMailAsync(bool preview)
        {
            var user = _usersService.GetUser(new UserKey() { Id = SessionUserId });

            var code = "1234567890123456789";
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

            string reportData = await _reportService.CreateReportForConfirmUserAccountAsync(user.Id, callbackUrl);
            if (!preview)
                await _emailSender.SendEmailAsync(user.Email, Translation.CONFIRM_USER_ACCOUNT, reportData);
            return new MailTestResult() { Result = true, MailData = reportData };
        }

        /// <summary>
        /// Test : Preview or send Email for notify admin when new user account created
        /// </summary>
        /// <param name="preview">True if only for preview</param>
        /// <returns>Mail test result</returns>
        private async Task<MailTestResult> AdminNewUserAccountCreatedAsync(bool preview)
        {
            var user = _usersService.GetUser(new UserKey() { Id = SessionUserId });

            string reportData = await _reportService.CreateReportForAdminNewUserAccountCreatedAsync(user.Id);
            if (!preview)
                await _emailSender.SendEmailAsync(user.Email, Translation.CONFIRM_USER_ACCOUNT, reportData);
            return new MailTestResult() { Result = true, MailData = reportData };
        }

        /// <summary>
        /// Test : Preview or send Email for notify user when his account is validated by admin
        /// </summary>
        /// <param name="preview">True if only for preview</param>
        /// <returns>Mail test result</returns>
        private async Task<MailTestResult> UserAccountValidatedAsync(bool preview)
        {
            var user = _usersService.GetUser(new UserKey() { Id = SessionUserId });

            string reportData = await _reportService.CreateReportForUserAccountValidatedAsync(user.Id);
            if (!preview)
                await _emailSender.SendEmailAsync(user.Email, Translation.USER_ACCOUNT_VALIDATED, reportData);
            return new MailTestResult() { Result = true, MailData = reportData };
        }
    }
}
