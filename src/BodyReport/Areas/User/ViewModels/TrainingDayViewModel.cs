using BodyReport.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.User.ViewModels
{
    public class TrainingDayViewModel
    {
        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        public string UserId { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.YEAR, ResourceType = typeof(Translation))]
        public int Year { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.WEEK_NUMBER, ResourceType = typeof(Translation))]
        public int WeekOfYear { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.DAY_OF_WEEK, ResourceType = typeof(Translation))]
        public int DayOfWeek { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        public int TrainingDayId { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.BEGIN_HOUR, ResourceType = typeof(Translation))]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "00:00")]
        [DataType(DataType.Time)]
        public DateTime BeginHour { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.END_HOUR, ResourceType = typeof(Translation))]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "00:00")]
        [DataType(DataType.Time)]
        public DateTime EndHour { get; set; }
    }
}
