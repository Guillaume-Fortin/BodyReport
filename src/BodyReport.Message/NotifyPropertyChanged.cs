using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public class KeyNotifyPropertyChanged : Key, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region binding

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            // PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        public override string GetCacheKey()
        {
            return string.Empty;
        }
    }

    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region binding

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            // PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
