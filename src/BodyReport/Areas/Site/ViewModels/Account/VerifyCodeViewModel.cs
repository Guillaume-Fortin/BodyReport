using BodyReport.Message;
using BodyReport.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.Site.ViewModels.Account
{
    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }
        
        [Display(Name = TRS.REMEMBER_THIS_BROWSER_PI, ResourceType = typeof(Translation))]
        public bool RememberBrowser { get; set; }
        
        [Display(Name = TRS.REMEMBER_ME_PI, ResourceType = typeof(Translation))]
        public bool RememberMe { get; set; }
    }
}
