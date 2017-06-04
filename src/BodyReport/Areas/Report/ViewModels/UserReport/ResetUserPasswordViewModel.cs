using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.Report.ViewModels.UserReport
{
    public class ResetUserPasswordViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string CallbackUrl { get; set; }
    }
}
