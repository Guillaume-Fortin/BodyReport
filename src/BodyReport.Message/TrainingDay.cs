using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public class TrainingDayKey : Key
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

        public TrainingDayKey()
        {
        }

        public TrainingDayKey(TrainingDayKey key)
        {
            UserId = key.UserId;
            Year = key.Year;
            WeekOfYear = key.WeekOfYear;
            DayOfWeek = key.DayOfWeek;
            TrainingDayId = key.TrainingDayId;
        }

        public TrainingDayKey Clone()
        {
            var copy = new TrainingDayKey();
            copy.UserId = UserId;
            copy.Year = Year;
            copy.WeekOfYear = WeekOfYear;
            copy.DayOfWeek = DayOfWeek;
            copy.TrainingDayId = TrainingDayId;
            return copy;
        }

        /// <summary>
        /// Equals by key
        /// </summary>
        /// <returns></returns>
        public static bool IsEqualByKey(TrainingDayKey key1, TrainingDayKey key2)
        {
            return key1.UserId == key2.UserId && key1.Year == key2.Year && key1.WeekOfYear == key2.WeekOfYear &&
                   key1.DayOfWeek == key2.DayOfWeek && key1.TrainingDayId == key2.TrainingDayId;
        }

        public override string GetCacheKey()
        {
            return string.Format("TrainingDayKey_{0}_{1}_{2}_{3}_{4}",
                UserId, Year.ToString(), WeekOfYear.ToString(), DayOfWeek.ToString(), TrainingDayId.ToString());
        }
    }

    public class TrainingDay : TrainingDayKey
    {
        /// <summary>
        /// Heure de début
        /// </summary>
        public DateTime BeginHour { get; set; }
        /// <summary>
        /// Heure de fin
        /// </summary>
        public DateTime EndHour { get; set; }
        /// <summary>
		/// Modification Date
		/// </summary>
		public DateTime ModificationDate
        {
            get;
            set;
        }
        /// <summary>
        /// Unit Type
        /// </summary>
        public TUnitType Unit
        {
            get;
            set;
        }
        /// <summary>
        /// Training journal day exercises
        /// </summary>
        public List<TrainingExercise> TrainingExercises { get; set; }

        /// <summary>
        /// Version number of object for internal use
        /// 0: initial value
        /// 1: unit type
        /// </summary>
        [DefaultValue(0)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)] // populate default value attribute if not present
        public int ObjectVersionNumber { get; set; } = 1;

        public TrainingDay()
        { }

        public TrainingDay(TrainingDayKey key) : base(key)
        {
        }
        
        new public TrainingDay Clone()
        {
            var copy = new TrainingDay(this);
            copy.BeginHour = BeginHour;
            copy.EndHour = EndHour;
            copy.ModificationDate = ModificationDate;
            copy.Unit = Unit;
            if (TrainingExercises != null)
            {
                copy.TrainingExercises = new List<TrainingExercise>();
                foreach (var trainingExercise in TrainingExercises)
                {
                    if (trainingExercise == null)
                        copy.TrainingExercises.Add(null);
                    else
                        copy.TrainingExercises.Add(trainingExercise.Clone());
                }
            }
            return copy;
        }
    }

    public class TrainingDayCriteria : CriteriaField
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
        /// Day Of Year
        /// </summary>
        public IntegerCriteria TrainingDayId { get; set; }

        public override string GetCacheKey()
        {
            return string.Format("TrainingDayCriteria_{0}_{1}_{2}_{3}_{4}",
                UserId == null ? "null" : UserId.GetCacheKey(),
                Year == null ? "null" : Year.GetCacheKey(),
                WeekOfYear == null ? "null" : WeekOfYear.GetCacheKey(),
                DayOfWeek == null ? "null" : DayOfWeek.GetCacheKey(),
                TrainingDayId == null ? "null" : TrainingDayId.GetCacheKey());
        }
    }

    public class TrainingDayScenario : Key
    {
        public bool ManageExercise { get; set; } = true;

        public TrainingDayScenario()
        {
        }

        public override string GetCacheKey()
        {
            return string.Format("TrainingDayScenario_{0}",
                ManageExercise ? "1" : "0");
        }
    }
}
