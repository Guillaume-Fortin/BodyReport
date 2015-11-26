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
        [Required]
        [EmailAddress]
        [Display(Name = TRS.EMAIL, ResourceType = typeof(Translation))]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = TRS.PASSWORD, ResourceType = typeof(Translation))]
        public string Password { get; set; }

        [Display(Name = TRS.REMEMBER_ME, ResourceType = typeof(Translation))]
        public bool RememberMe { get; set; }
    }
}
