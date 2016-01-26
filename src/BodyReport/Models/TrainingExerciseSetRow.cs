using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Models
{
    /// <summary>
    /// Database table TrainingExerciseSet
    /// </summary>
    public class TrainingExerciseSetRow
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
        /// Unit Type
        /// </summary>
        public int Unit
        {
            get;
            set;
        }
    }
}
