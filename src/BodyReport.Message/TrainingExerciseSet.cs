using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public class TrainingExerciseSetKey : Key
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
        /// Training day id
        /// </summary>
        public int TrainingDayId { get; set; }
        /// <summary>
        /// Id of training exercise
        /// </summary>
        public int TrainingExerciseId { get; set; }
        /// <summary>
        /// Id of set/Rep
        /// </summary>
        public int Id { get; set; }

        public override string GetCacheKey()
        {
            return string.Format("TrainingExerciseSetKey_{0}_{1}_{2}_{3}_{4}_{5}_{6}",
                UserId, Year.ToString(), WeekOfYear.ToString(), DayOfWeek.ToString(), 
                TrainingDayId.ToString(), TrainingExerciseId.ToString(), Id.ToString());
        }
    }

    public class TrainingExerciseSet : TrainingExerciseSetKey
    {
        /// <summary>
        /// Unit Type
        /// </summary>
        public TUnitType Unit
        {
            get;
            set;
        }
        /// <summary>
        /// Number of sets
        /// </summary>
        public int NumberOfSets { get; set; }
        /// <summary>
        /// Number of reps
        /// </summary>
        public int NumberOfReps { get; set; }
        /// <summary>
        /// Weight
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
		/// Modification Date
		/// </summary>
		public DateTime ModificationDate
        {
            get;
            set;
        }
    }

    public class TrainingExerciseSetCriteria : CriteriaField
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
        /// Training Day Id
        /// </summary>
        public IntegerCriteria TrainingDayId { get; set; }

        /// <summary>
        /// Training Exercise Id
        /// </summary>
        public IntegerCriteria TrainingExerciseId { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public IntegerCriteria Id { get; set; }

        public override string GetCacheKey()
        {
            return string.Format("TrainingExerciseSetCriteria_{0}_{1}_{2}_{3}_{4}_{5}_{6}",
                UserId == null ? "null" : UserId.GetCacheKey(),
                Year == null ? "null" : Year.GetCacheKey(),
                WeekOfYear == null ? "null" : WeekOfYear.GetCacheKey(),
                DayOfWeek == null ? "null" : DayOfWeek.GetCacheKey(),
                TrainingDayId == null ? "null" : TrainingDayId.GetCacheKey(),
                TrainingExerciseId == null ? "null" : TrainingExerciseId.GetCacheKey(),
                Id == null ? "null" : Id.GetCacheKey());
        }
    }
}
