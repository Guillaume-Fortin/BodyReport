using BodyReport.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Message;

namespace BodyReport.Areas.User.ViewModels
{
    public class CopyTrainingWeekViewModel
    {
        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        public string UserId { get; set; }

        [Display(Name = TRS.NAME, ResourceType = typeof(Translation))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.YEAR, ResourceType = typeof(Translation))]
        public int OriginYear { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.WEEK_NUMBER, ResourceType = typeof(Translation))]
        [Range(1, 52, ErrorMessageResourceName = TRS.THE_FIELD_P0_SHOULD_BE_A_NUMBER_BETWEEN_P1_AND_P2, ErrorMessageResourceType = typeof(Translation))]
        public int OriginWeekOfYear { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.YEAR, ResourceType = typeof(Translation))]
        public int Year { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.WEEK_NUMBER, ResourceType = typeof(Translation))]
        [Range(1, 52, ErrorMessageResourceName = TRS.THE_FIELD_P0_SHOULD_BE_A_NUMBER_BETWEEN_P1_AND_P2, ErrorMessageResourceType = typeof(Translation))]
        public int WeekOfYear { get; set; }
    }
}
