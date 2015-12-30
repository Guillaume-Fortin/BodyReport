using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Message
{
    public class TrainingDayKey
    {
        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Year
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Week of year
        /// </summary>
        public int WeekOfYear { get; set; }
        /// <summary>
        /// Day of week
        /// </summary>
        public int DayOfWeek { get; set; }
        /// <summary>
        /// Training Day Id
        /// </summary>
        public int TrainingDayId { get; set; }
    }

    public class TrainingDay : TrainingDayKey
    {
        /// <summary>
        /// Heure de début
        /// </summary>
        public DateTime BeginHour { get; set; }
        /// <summary>
        /// Heure de fin
        /// </summary>
        public DateTime EndHour { get; set; }
        /// <summary>
        /// Training journal day exercises
        /// </summary>
        public List<TrainingExercise> TrainingExercises { get; set; }
    }

    public class TrainingDayCriteria : CriteriaField
    {
        /// <summary>
        /// User Id
        /// </summary>
        public StringCriteria UserId { get; set; }

        /// <summary>
        /// Year
        /// </summary>
        public IntegerCriteria Year { get; set; }

        /// <summary>
        /// Week Of Year
        /// </summary>
        public IntegerCriteria WeekOfYear { get; set; }

        /// <summary>
        /// Day Of Year
        /// </summary>
        public IntegerCriteria DayOfWeek { get; set; }

        /// <summary>
        /// Day Of Year
        /// </summary>
        public IntegerCriteria TrainingDayId { get; set; }
    }
}
