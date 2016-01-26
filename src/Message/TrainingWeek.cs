using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Message
{
    public class TrainingWeekKey
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
    }

    public class TrainingWeek : TrainingWeekKey
    {
        /// <summary>
        /// User Height
        /// </summary>
        public double UserHeight { get; set; }
        /// <summary>
        /// User Weight
        /// </summary>
        public double UserWeight { get; set; }
        /// <summary>
        /// User Weight
        /// </summary>
        public List<TrainingDay> TrainingDays { get; set; }
    }

    public class TrainingWeekCriteria : CriteriaField
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
    }
}
