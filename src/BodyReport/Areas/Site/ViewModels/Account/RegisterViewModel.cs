using BodyReport.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BodyReport.Message;

namespace BodyReport.Areas.Site.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType=typeof(Translation))]
        [StringLength(FieldsLength.UserName.Max, MinimumLength = FieldsLength.UserName.Min, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.USER_NAME, ResourceType = typeof(Translation))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [StringLength(FieldsLength.Email.Max, MinimumLength = FieldsLength.Email.Min, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        [EmailAddress(ErrorMessageResourceName = TRS.EMAIL_IS_NOT_VALID, ErrorMessageResourceType=typeof(Translation))]
        [Display(Name = TRS.EMAIL, ResourceType = typeof(Translation))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [StringLength(FieldsLength.Password.Max, MinimumLength = FieldsLength.Password.Min, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        [DataType(DataType.Password)]
        [Display(Name = TRS.PASSWORD, ResourceType = typeof(Translation))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = TRS.CONFIRM_PASSWORD, ResourceType = typeof(Translation))]
        [Compare("Password",ErrorMessageResourceName = TRS.THE_PASSWORD_AND_CONFIRMATION_PASSWORD_DO_NOT_MATCH, ErrorMessageResourceType = typeof(Translation))]
        public string ConfirmPassword { get; set; }
    }
}