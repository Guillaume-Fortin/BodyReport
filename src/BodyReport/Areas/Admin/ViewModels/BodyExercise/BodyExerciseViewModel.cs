using BodyReport.Resources;
using Message;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ViewModels.Admin
{
    public class BodyExerciseViewModel
    {
        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        public int Id { get; set; } = 0;

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [StringLength(FieldsLength.BodyExerciseName.Max, MinimumLength = FieldsLength.BodyExerciseName.Min, ErrorMessageResourceName = TRS.THE_FIELD_P0_MUST_BE_A_STRING_WITH_A_MINIMUM_LENGTH_OF_P2_AND_A_MAXIMUM_LENGTH_OF_P1, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.NAME, ResourceType = typeof(Translation))]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.MUSCLE, ResourceType = typeof(Translation))]
        public int MuscleId { get; set; }

        [Display(Name = TRS.MUSCLE, ResourceType = typeof(Translation))]
        public string MuscleName { get; set; } = string.Empty;

        [Display(Name = TRS.IMAGE, ResourceType = typeof(Translation))]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
