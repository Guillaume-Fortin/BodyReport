using BodyReport.Resources;
using BodyReport.ViewModels.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BodyReport.Message;

namespace BodyReport.ViewModels.Admin
{
    public class SearchUserViewModel
    {
        [Display(Name = TRS.USER_NAME, ResourceType = typeof(Translation))]
        [StringLength(FieldsLength.UserName.Max, MinimumLength = 1, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        public string UserName { get; set; }

        [Display(Name = TRS.ID, ResourceType = typeof(Translation))]
        [StringLength(FieldsLength.UserId.Max, MinimumLength = 1, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        public string UserId { get; set; }
    }
}
