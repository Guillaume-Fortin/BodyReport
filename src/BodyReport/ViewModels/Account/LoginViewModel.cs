using BodyReport.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.USER_NAME, ResourceType = typeof(Translation))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [DataType(DataType.Password)]
        [Display(Name = TRS.PASSWORD, ResourceType = typeof(Translation))]
        public string Password { get; set; }

        [Display(Name = TRS.REMEMBER_ME, ResourceType = typeof(Translation))]
        public bool RememberMe { get; set; }
    }
}
