using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Message
{
    public class TrainingExerciseKey
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
        /// Id of training exercise
        /// </summary>
        public int Id { get; set; }
    }

    public class TrainingExercise : TrainingExerciseKey
    {
        /// <summary>
        /// Id of body exercise
        /// </summary>
        public int BodyExerciseId { get; set; }
        /// <summary>
        /// Rest time (second)
        /// </summary>
        public int RestTime { get; set; }

        public List<TrainingExerciseSet> TrainingExerciseSets { get; set; }
    }

    public class TrainingExerciseCriteria : CriteriaField
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
        public IntegerCriteria Id { get; set; }

        /// <summary>
        /// Body Exercise Id
        /// </summary>
        public IntegerCriteria BodyExerciseId { get; set; }
    }
}
