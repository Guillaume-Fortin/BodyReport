using BodyReport.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BodyReport.Message;

namespace BodyReport.Areas.User.ViewModels
{
    public class UserProfilViewModel
    {
        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        public string UserId { get; set; }

        [Display(Name = TRS.NAME, ResourceType = typeof(Translation))]
        public string Name { get; set; }

        [Display(Name = TRS.EMAIL, ResourceType = typeof(Translation))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.SEX, ResourceType = typeof(Translation))]
        public int SexId { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.HEIGHT, ResourceType = typeof(Translation))]
        [Range(0, 250)] // 250 cm = 98 Inches
        public double Height { get; set; }
        
        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.UNIT, ResourceType = typeof(Translation))]
        public int Unit { get; set; }

        [Required(ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.WEIGHT, ResourceType = typeof(Translation))]
        [Range(0, 660)] // 660 pounds = 300 kilo
        public double Weight { get; set; }

        [Display(Name = TRS.ZIP_CODE, ResourceType = typeof(Translation))]
        public string ZipCode { get; set; }

        [Display(Name = TRS.COUNTRY, ResourceType = typeof(Translation))]
        public int CountryId { get; set; }

        [Display(Name = TRS.TIME_ZONE, ResourceType = typeof(Translation))]
        public string TimeZoneName { get; set; }

        [Display(Name = TRS.IMAGE, ResourceType = typeof(Translation))]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
