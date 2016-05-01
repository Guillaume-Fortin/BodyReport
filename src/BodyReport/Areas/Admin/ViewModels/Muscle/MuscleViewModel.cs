using BodyReport.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Message;

namespace BodyReport.ViewModels.Admin
{
    public class MuscleViewModel
    {
        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.ID, ResourceType = typeof(Translation))]
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [StringLength(FieldsLength.MuscleName.Max, MinimumLength = FieldsLength.MuscleName.Min, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.NAME, ResourceType = typeof(Translation))]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.MUSCULAR_GROUP, ResourceType = typeof(Translation))]
        public int MuscularGroupId { get; set; }

        [Display(Name = TRS.MUSCULAR_GROUP, ResourceType = typeof(Translation))]
        public string MuscularGroupName { get; set; } = string.Empty;
    }
}
