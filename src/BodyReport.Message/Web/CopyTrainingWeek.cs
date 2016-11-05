using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message.Web
{
    public class CopyTrainingWeek : NotifyPropertyChanged
    {
        /// <summary>
        /// User Id
        /// </summary>
		private string _userId;
        public string UserId
        {
            get { return _userId; }
            set
            {
                if (_userId != value)
                {
                    _userId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Origin Year
        /// </summary>
		private int _originYear;
        public int OriginYear
        {
            get { return _originYear; }
            set
            {
                if (_originYear != value)
                {
                    _originYear = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Origin Week Of Year
        /// </summary>
		private int _originWeekOfYear;
        public int OriginWeekOfYear
        {
            get { return _originWeekOfYear; }
            set
            {
                if (_originWeekOfYear != value)
                {
                    _originWeekOfYear = value;
                    OnPropertyChanged();
                }
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
                if (_year != value)
                {
                    _year = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Week Of Year
        /// </summary>
		private int _weekOfYear;
        public int WeekOfYear
        {
            get { return _weekOfYear; }
            set
            {
                if (_weekOfYear != value)
                {
                    _weekOfYear = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
