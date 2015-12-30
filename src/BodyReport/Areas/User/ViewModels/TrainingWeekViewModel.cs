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
        public int WeekOfYear { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.HEIGHT, ResourceType = typeof(Translation))]
        public double UserHeight { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.WEIGHT, ResourceType = typeof(Translation))]
        public double UserWeight { get; set; }
    }
}
