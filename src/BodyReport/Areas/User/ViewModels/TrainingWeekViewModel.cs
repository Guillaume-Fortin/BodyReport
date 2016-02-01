using BodyReport.Resources;
using Message;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.User.ViewModels
{
    public class TrainingWeekViewModel
    {
        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        public string UserId { get; set; }

        [Display(Name = TRS.NAME, ResourceType = typeof(Translation))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.YEAR, ResourceType = typeof(Translation))]
        public int Year { get; set; }
        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.WEEK_NUMBER, ResourceType = typeof(Translation))]
        [Range(1, 52, ErrorMessageResourceName = TRS.THE_FIELD_P0_SHOULD_BE_A_NUMBER_BETWEEN_P1_AND_P2, ErrorMessageResourceType = typeof(Translation))]
        public int WeekOfYear { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.HEIGHT, ResourceType = typeof(Translation))]
        [Range(0, 250)] // 250 cm = 98 Inches
        public double UserHeight { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.WEIGHT, ResourceType = typeof(Translation))]
        [Range(0, 660)] // 660 pounds = 300 kilo
        public double UserWeight { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.UNIT, ResourceType = typeof(Translation))]
        public int Unit { get; set; }
    }
}
