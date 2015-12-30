using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Areas.User.ViewModels
{
    public class TrainingExerciseViewModel
    {
        public string UserId { get; set; }
        public int Year { get; set; }
        public int WeekOfYear { get; set; }
        public int DayOfWeek { get; set; }
        public int TrainingDayId { get; set; }
        public int BodyExerciseId { get; set; }
        public int NumberOfSets { get; set; }
        public int NumberOfReps { get; set; }
    }
}
