using BodyReport.Resources;
using Message;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.Site.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        [Required]
        [StringLength(FieldsLength.Email.Max, MinimumLength = FieldsLength.Email.Min, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(FieldsLength.Password.Max, MinimumLength = FieldsLength.Password.Min, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = TRS.CONFIRM_PASSWORD, ResourceType = typeof(Translation))]
        [Compare("Password", ErrorMessageResourceName = TRS.THE_PASSWORD_AND_CONFIRMATION_PASSWORD_DO_NOT_MATCH, ErrorMessageResourceType = typeof(Translation))]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
