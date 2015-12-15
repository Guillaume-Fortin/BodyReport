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
    }
}
