using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.User.ViewModels
{
    public class TrainingJournalDayViewModel
    {
        public string UserId { get; set; }
        public int Year { get; set; }
        public int WeekOfYear { get; set; }
        public int DayOfWeek { get; set; }
        public DateTime BeginHour { get; set; }
        public DateTime EndHour { get; set; }
        public List<TrainingJournalDayExerciseViewModel> TrainingJournalDayExercises { get; set; }
    }
}
