using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Models
{
    /// <summary>
    /// Database table TrainingDay
    /// </summary>
    public class TrainingDayRow
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
        /// Begin hour
        /// </summary>
        public DateTime BeginHour { get; set; }
        /// <summary>
        /// End hour
        /// </summary>
        public DateTime EndHour { get; set; }
        
    }
}
