using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.User.ViewModels
{
    public class TrainingJournalViewModel
    {
        public string UserId { get; set; }
        public int Year { get; set; }
        public int WeekOfYear { get; set; }
        public double UserHeight { get; set; }
        public double UserWeight { get; set; }
        public TUnitType UserUnit { get; set; }
        public List<TrainingJournalDayViewModel> TrainingJournalDays { get; set; }
    }
}
