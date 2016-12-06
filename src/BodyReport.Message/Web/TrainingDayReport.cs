using System;
using System.Collections.Generic;
using System.Text;

namespace BodyReport.Message.Web
{
    public class TrainingDayReport
    {
        public string UserId { get; set; }
        public int Year { get; set; }
        public int WeekOfYear { get; set; }
        public int DayOfWeek { get; set; }
        public int? TrainingDayId { get; set; }
        public bool DisplayImages { get; set; }
    }
}
