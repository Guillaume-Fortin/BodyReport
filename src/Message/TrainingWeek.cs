using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Message
{
	public class TrainingWeekKey : NotifyPropertyChanged
    {
        /// <summary>
        /// UserId
        /// </summary>
		private string _userId;
		public string UserId
		{
			get { return _userId; }
			set 
			{ 
				_userId = value;
				OnPropertyChanged();
			}
		}

        /// <summary>
        /// Year
        /// </summary>
		private int _year;
		public int Year
		{
			get { return _year; }
			set 
			{ 
				_year = value;
				OnPropertyChanged();
			}
		}

        /// <summary>
        /// Week of year
        /// </summary>
		private int _weekOfYear;

		public int WeekOfYear
		{
			get { return _weekOfYear; }
			set 
			{ 
				_weekOfYear = value;
				OnPropertyChanged();
			}
		}
    }

    public class TrainingWeek : TrainingWeekKey
    {
        /// <summary>
        /// Unit Type
        /// </summary>
        public TUnitType Unit
        {
            get;
            set;
        }
        /// <summary>
        /// User Height
        /// </summary>
		private double _userHeight;
		public double UserHeight
		{
			get { return _userHeight; }
			set 
			{ 
				_userHeight = value;
				OnPropertyChanged();
			}
		}
        /// <summary>
        /// User Weight
        /// </summary>
		private double _userWeight;
		public double UserWeight
		{
			get { return _userWeight; }
			set 
			{ 
				_userWeight = value;
				OnPropertyChanged();
			}
		}
        
        /// <summary>
        /// Week of year description
        /// </summary>
        private string _weekOfYearDescription;

        public string WeekOfYearDescription
        {
            get { return _weekOfYearDescription; }
            set
            {
                _weekOfYearDescription = value;
                OnPropertyChanged();
            }
        }

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

    public class TrainingWeekScenario
    {
        public bool ManageTrainingDay { get; set; } = true;

        public TrainingDayScenario TrainingDayScenario { get; set; }

        public TrainingWeekScenario()
        {
        }
    }
}
