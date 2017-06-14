using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Models
{
    /// <summary>
    /// Database table BodyExercise
    /// </summary>
    public class BodyExerciseRow
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Muscle Id
        /// </summary>
        public int MuscleId { get; set; }

        /// <summary>
        /// Exercise Category Type
        /// </summary>
        public int? ExerciseCategoryType { get; set; }

        /// <summary>
        /// Exercise Unit Type
        /// </summary>
        public int? ExerciseUnitType { get; set; }
    }
}
