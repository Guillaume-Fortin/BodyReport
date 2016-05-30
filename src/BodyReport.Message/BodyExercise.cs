using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public class BodyExerciseKey
    {
        /// <summary>
        /// Exercise Id
        /// </summary>
        public int Id { get; set; } = 0;
    }

    public class BodyExercise : BodyExerciseKey
    {
        /// <summary>
        /// Exercise Name
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Exercise Picture name
        /// </summary>
        public string ImageName { get; set; } = string.Empty;
        /// <summary>
        /// Muscle Id
        /// </summary>
        public int MuscleId { get; set; } = 0;
    }

    public class BodyExerciseCriteria : CriteriaField
    {
        /// <summary>
        /// Exercise Id
        /// </summary>
        public IntegerCriteria Id { get; set; }

        /// <summary>
        /// Exercise Name
        /// </summary>
        public StringCriteria Name { get; set; }

        /// <summary>
        /// Muscle Id
        /// </summary>
        public IntegerCriteria MuscleId { get; set; }
    }
}
