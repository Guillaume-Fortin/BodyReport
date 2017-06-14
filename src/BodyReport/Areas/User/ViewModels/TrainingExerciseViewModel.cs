using BodyReport.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BodyReport.Message;

namespace BodyReport.Areas.User.ViewModels
{
    public class TrainingExerciseViewModel
    {
        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        public string UserId { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        public int Year { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        public int WeekOfYear { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        public int DayOfWeek { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        public int TrainingDayId { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        public int TrainingExerciseId { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.BODY_EXERCISES, ResourceType = typeof(Translation))]
        public int BodyExerciseId { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.EXERCISE_UNIT_TYPE, ResourceType = typeof(Translation))]
        public int ExerciseUnitType { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Range(0, 300, ErrorMessageResourceName = TRS.THE_FIELD_P0_SHOULD_BE_A_NUMBER_BETWEEN_P1_AND_P2, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.REST_TIME, ResourceType = typeof(Translation))]
        public int RestTime { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Range(0, 300, ErrorMessageResourceName = TRS.THE_FIELD_P0_SHOULD_BE_A_NUMBER_BETWEEN_P1_AND_P2, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.ECCENTRIC_CONTRACTION, ResourceType = typeof(Translation))]
        public int EccentricContractionTempo { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Range(0, 300, ErrorMessageResourceName = TRS.THE_FIELD_P0_SHOULD_BE_A_NUMBER_BETWEEN_P1_AND_P2, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.STRETCH_POSITION, ResourceType = typeof(Translation))]
        public int StretchPositionTempo { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Range(0, 300, ErrorMessageResourceName = TRS.THE_FIELD_P0_SHOULD_BE_A_NUMBER_BETWEEN_P1_AND_P2, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.CONCENTRIC_CONTRACTION, ResourceType = typeof(Translation))]
        public int ConcentricContractionTempo { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Range(0, 300, ErrorMessageResourceName = TRS.THE_FIELD_P0_SHOULD_BE_A_NUMBER_BETWEEN_P1_AND_P2, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.CONTRACTED_POSITION, ResourceType = typeof(Translation))]
        public int ContractedPositionTempo { get; set; }

        [Display(Name = TRS.REPS, ResourceType = typeof(Translation))]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public List<int?> Reps { get; set; } = new List<int?>();

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.UNIT, ResourceType = typeof(Translation))]
        public int Unit { get; set; }

        [Display(Name = TRS.WEIGHT, ResourceType = typeof(Translation))]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public List<double?> Weights { get; set; } = new List<double?>();

        [Display(Name = TRS.EXECUTION_TIME, ResourceType = typeof(Translation))]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public List<int?> ExecutionTimes { get; set; } = new List<int?>();

        [Display(Name = TRS.NAME, ResourceType = typeof(Translation))]
        public string BodyExerciseName { get; set; }
        public string BodyExerciseImage { get; set; }
        public List<Tuple<int, int, double>> TupleSetReps { get; set; }
    }
}
