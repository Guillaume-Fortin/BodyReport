using BodyReport.Resources;
using System.ComponentModel.DataAnnotations;

namespace BodyReport.Areas.Api.ViewModels
{
    public class InputDataViewModel
    {
        [Required(AllowEmptyStrings = true, ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.URL, ResourceType = typeof(Translation))]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Url { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true, ErrorMessageResourceName = TRS.THE_P0_FIELD_IS_REQUIRED, ErrorMessageResourceType = typeof(Translation))]
        [Display(Name = TRS.DATA, ResourceType = typeof(Translation))]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Data { get; set; } = string.Empty;
    }
}
