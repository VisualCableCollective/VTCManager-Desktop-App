using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace VTCManager_Client.UI.Views.Models
{
    public class MainDashboardViewModel : INotifyPropertyChanged
    {
        public ImageSource BackgroundImageSource
        {
            get => _backgroundImageSource;
            set
            {
                if (value == _backgroundImageSource)
                    return;
                _backgroundImageSource = value;
                NotifyPropertyChanged();
            }
        }
        private ImageSource _backgroundImageSource;

        public uint WidgetsCornerRadius
        {
            get { return 20; }
        }

        public String SpeedLimitString
        {
            get { return _SpeedLimitString; }
            set
            {
                if (value == _SpeedLimitString)
                    return;
                _SpeedLimitString = value;
                NotifyPropertyChanged();
            }
        }
        private string _SpeedLimitString;

        public String CurrentSpeedString
        {
            get { return _CurrentSpeedString; }
            set
            {
                if (value == _CurrentSpeedString)
                    return;
                _CurrentSpeedString = value;
                NotifyPropertyChanged();
            }
        }
        private string _CurrentTruckModelString;

        public String CurrentTruckModelString
        {
            get { return _CurrentTruckModelString; }
            set
            {
                if (value == _CurrentTruckModelString)
                    return;
                _CurrentTruckModelString = value;
                NotifyPropertyChanged();
            }
        }
        private string _CurrentSpeedString;

        public String CurrentCargoString
        {
            get { return _CurrentCargoString; }
            set
            {
                if (value == _CurrentCargoString)
                    return;
                _CurrentCargoString = value;
                NotifyPropertyChanged();
            }
        }
        private string _CurrentCargoString;

        public String CurrentDestinationString
        {
            get { return _CurrentDestinationString; }
            set
            {
                if (value == _CurrentDestinationString)
                    return;
                _CurrentDestinationString = value;
                NotifyPropertyChanged();
            }
        }
        private string _CurrentDestinationString;

        public String ArrivalTimeString
        {
            get { return _ArrivalTimeString; }
            set
            {
                if (value == _ArrivalTimeString)
                    return;
                _ArrivalTimeString = value;
                NotifyPropertyChanged();
            }
        }
        private string _ArrivalTimeString;

        public String ArrivalTimeLeftString
        {
            get { return _ArrivalTimeLeftString; }
            set
            {
                if (value == _ArrivalTimeLeftString)
                    return;
                _ArrivalTimeLeftString = value;
                NotifyPropertyChanged();
            }
        }
        private string _ArrivalTimeLeftString;

        public String ArrivalDistanceLeftString
        {
            get { return _ArrivalDistanceLeftString; }
            set
            {
                if (value == _ArrivalDistanceLeftString)
                    return;
                _ArrivalDistanceLeftString = value;
                NotifyPropertyChanged();
            }
        }
        private string _ArrivalDistanceLeftString;

        public Visibility SpeedLimitSignVisibility
        {
            get { return _SpeedLimitSignVisibility; }
            set
            {
                if (value == _SpeedLimitSignVisibility)
                    return;
                _SpeedLimitSignVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility _SpeedLimitSignVisibility;

        public Visibility TruckVisibility
        {
            get { return _TruckVisibility; }
            set
            {
                if (value == _TruckVisibility)
                    return;
                _TruckVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility _TruckVisibility;

        public Visibility CargoVisibility
        {
            get { return _CargoVisibility; }
            set
            {
                if (value == _CargoVisibility)
                    return;
                _CargoVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility _CargoVisibility;

        public Visibility DestinationVisibility
        {
            get { return _DestinationVisibility; }
            set
            {
                if (value == _DestinationVisibility)
                    return;
                _DestinationVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility _DestinationVisibility;

        public Visibility NavigationInfoPanelVisibility
        {
            get { return _NavigationInfoPanelVisibility; }
            set
            {
                if (value == _NavigationInfoPanelVisibility)
                    return;
                _NavigationInfoPanelVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility _NavigationInfoPanelVisibility;

        public ImageSource TruckImageSource
        {
            get { return _TruckImageSource; }
            set
            {
                if (value == _TruckImageSource)
                    return;
                _TruckImageSource = value;
                NotifyPropertyChanged();
            }
        }

        private ImageSource _TruckImageSource;

        public String CurrentConnectionStateString
        {
            get { return _CurrentConnectionStateString; }
            set
            {
                if (value == _CurrentConnectionStateString)
                    return;
                _CurrentConnectionStateString = value;
                NotifyPropertyChanged();
            }
        }

        private String _CurrentConnectionStateString;

        public Brush CurrentConnectionStateColor
        {
            get { return _CurrentConnectionStateColor; }
            set
            {
                if (value == _CurrentConnectionStateColor)
                    return;
                _CurrentConnectionStateColor = value;
                NotifyPropertyChanged();
            }
        }

        private Brush _CurrentConnectionStateColor;

        public event PropertyChangedEventHandler PropertyChanged;//CurrentTimeString

        protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}