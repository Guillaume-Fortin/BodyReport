using BodyReport.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Message;

namespace BodyReport.Areas.Site.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [StringLength(FieldsLength.UserName.Max, MinimumLength = FieldsLength.UserName.Min, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.USER_NAME, ResourceType = typeof(Translation))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [StringLength(FieldsLength.Password.Max, MinimumLength = FieldsLength.Password.Min, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        [DataType(DataType.Password)]
        [Display(Name = TRS.PASSWORD, ResourceType = typeof(Translation))]
        public string Password { get; set; }

        [Display(Name = TRS.REMEMBER_ME, ResourceType = typeof(Translation))]
        public bool RememberMe { get; set; }
    }
}
