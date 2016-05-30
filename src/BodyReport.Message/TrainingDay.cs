using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
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

        /// <summary>
        /// Equals by key
        /// </summary>
        /// <returns></returns>
        public static bool IsEqualByKey(TrainingDayKey key1, TrainingDayKey key2)
        {
            return key1.UserId == key2.UserId && key1.Year == key2.Year && key1.WeekOfYear == key2.WeekOfYear &&
                   key1.DayOfWeek == key2.DayOfWeek && key1.TrainingDayId == key2.TrainingDayId;
        }
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
		/// Modification Date
		/// </summary>
		public DateTime ModificationDate
        {
            get;
            set;
        }
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

    public class TrainingDayScenario
    {
        public bool ManageExercise { get; set; } = true;

        public TrainingDayScenario()
        {
        }
    }
}
