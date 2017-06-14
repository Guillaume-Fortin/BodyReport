using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Models
{
    /// <summary>
    /// Database table TrainingExercise
    /// </summary>
    public class TrainingExerciseRow
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
        public int Id { get; set; }
        /// <summary>
        /// Id of body exercise
        /// </summary>
        public int BodyExerciseId { get; set; }
        /// <summary>
        /// Rest time (second)
        /// </summary>
        public int RestTime { get; set; }
        /// <summary>
        /// Eccentric Contraction Tempo (second)
        /// </summary>
        public int? EccentricContractionTempo { get; set; }
        /// <summary>
        /// Stretch Position Tempo (second)
        /// </summary>
        public int? StretchPositionTempo { get; set; }
        /// <summary>
        /// Concentric Contraction Tempo (second)
        /// </summary>
        public int? ConcentricContractionTempo { get; set; }
        /// <summary>
        /// Contracted Position Tempo (second)
        /// </summary>
        public int? ContractedPositionTempo { get; set; }
        /// <summary>
        /// Modification Date
        /// </summary>
        public DateTime ModificationDate
        {
            get;
            set;
        }
        /// <summary>
        /// Exercise Unit Type
        /// </summary>
        public int? ExerciseUnitType { get; set; }
    }
}
