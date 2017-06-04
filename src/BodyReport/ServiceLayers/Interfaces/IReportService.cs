using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface IReportService
    {
        /// <summary>
        /// Generate mail report (html) for reset user password with token
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="callbackUrl">url link for reset password with token</param>
        /// <returns>html string</returns>
        Task<string> CreateReportForResetUserPasswordAsync(string userId, string callbackUrl);
        /// <summary>
        /// Generate mail report (html) for confirm user account with token
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="callbackUrl">url link for confirm user account with token</param>
        /// <returns>html string</returns>
        Task<string> CreateReportForConfirmUserAccountAsync(string userId, string callbackUrl);
        /// <summary>
        /// Generate mail report (html) for notify user account created
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>html string</returns>
        Task<string> CreateReportForAdminNewUserAccountCreatedAsync(string userId);
        /// <summary>
        /// Generate mail report (html) for notify user account validated by admin
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>html string</returns>
        Task<string> CreateReportForUserAccountValidatedAsync(string userId);
    }
}
