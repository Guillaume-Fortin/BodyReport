using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Framework.CustomAttributes
{
    /// <summary>
    /// Authorize Handler fo user, admin and localhost request
    /// </summary>
    public class LoopBackAuthorizeHandler : AuthorizationHandler<LoopBackAuthorizeRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LoopBackAuthorizeRequirement requirement)
        {
            if(context.User != null && (context.User.IsInRole("User") || context.User.IsInRole("Admin")))
            {
                context.Succeed(requirement);
            }
            else
            {
                var mvcContext = context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext;

                if (mvcContext != null && mvcContext.HttpContext != null && mvcContext.HttpContext.Request != null &&
                    mvcContext.HttpContext.Request.Host != null && mvcContext.HttpContext.Request.Host.Host == "localhost")
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
