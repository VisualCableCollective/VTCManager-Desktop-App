using System.ComponentModel;
using System.Windows;
using System.Runtime.CompilerServices;

namespace VTCManager.Plugins.Clock
{
    internal class ClockViewModel : INotifyPropertyChanged
    {
        public Visibility CurrentTimeWidgetMiddlePartVisibility
        {
            get { return _CurrentTimeWidgetMiddlePartVisibility; }
            set
            {
                if (value == _CurrentTimeWidgetMiddlePartVisibility)
                    return;
                _CurrentTimeWidgetMiddlePartVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility _CurrentTimeWidgetMiddlePartVisibility;

        public string CurrentTimeWidgetHourString
        {
            get { return _CurrentTimeWidgetHourString; }
            set
            {
                if (value == _CurrentTimeWidgetHourString)
                    return;
                _CurrentTimeWidgetHourString = value;
                NotifyPropertyChanged();
            }
        }

        private string _CurrentTimeWidgetHourString;

        public string CurrentTimeWidgetMinuteString
        {
            get { return _CurrentTimeWidgetMinuteString; }
            set
            {
                if (value == _CurrentTimeWidgetMinuteString)
                    return;
                _CurrentTimeWidgetMinuteString = value;
                NotifyPropertyChanged();
            }
        }

        private string _CurrentTimeWidgetMinuteString;

        public event PropertyChangedEventHandler PropertyChanged;//CurrentTimeString

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
