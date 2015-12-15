using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Models
{
    /// <summary>
    /// Database table Muscle
    /// </summary>
    public class MuscleRow
    {
        /// <summary>
        /// Muscular Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Muscular group Id
        /// </summary>
        public int MuscularGroupId { get; set; }
    }
}
