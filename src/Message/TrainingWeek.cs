using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Message
{
	public class TrainingWeekKey : INotifyPropertyChanged
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
				OnPropertyChanged("UserId");
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
				OnPropertyChanged("Year");
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
				OnPropertyChanged("WeekOfYear");
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
				OnPropertyChanged("WeekOfYearDescription");
			}
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
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
				OnPropertyChanged("UserHeight");
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
				OnPropertyChanged("UserWeight");
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
}
