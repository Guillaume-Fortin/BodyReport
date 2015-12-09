﻿using BodyReport.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ViewModels.Admin
{
    public class UserViewModel
    {
        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.ID, ResourceType = typeof(Translation))]
        [StringLength(450, MinimumLength = 1, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [StringLength(256, MinimumLength = 4, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.NAME, ResourceType = typeof(Translation))]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.EMAIL, ResourceType = typeof(Translation))]
        [StringLength(256, MinimumLength = 4, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.ROLE, ResourceType = typeof(Translation))]
        public string RoleId { get; set; } = string.Empty;

        [Display(Name = TRS.ROLE, ResourceType = typeof(Translation))]
        public string RoleName { get; set; } = string.Empty;
    }
}
