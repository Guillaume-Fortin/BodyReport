using BodyReport.Resources;
using Message;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.Api.ViewModels
{
    public class RegisterAccountViewModel
    {
        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [StringLength(10, MinimumLength = 4, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.USER_NAME, ResourceType = typeof(Translation))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [EmailAddress(ErrorMessageResourceName = TRS.EMAIL_IS_NOT_VALID, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.EMAIL, ResourceType = typeof(Translation))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        [DataType(DataType.Password)]
        [Display(Name = TRS.PASSWORD, ResourceType = typeof(Translation))]
        public string Password { get; set; }
    }
}
