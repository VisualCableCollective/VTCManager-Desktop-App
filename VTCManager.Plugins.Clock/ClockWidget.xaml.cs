using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;

namespace VTCManager.Plugins.Clock
{
    /// <summary>
    /// Interaktionslogik für ClockWidget.xaml
    /// </summary>
    public partial class ClockWidget : UserControl
    {
        private readonly Timer UpdateTimeLabelTimer;

        private readonly ClockViewModel viewModel;

        public ClockWidget()
        {
            InitializeComponent();

            viewModel = new ClockViewModel();
            DataContext = viewModel;

            UpdateTimeLabelTimer = new Timer(1000);
            UpdateTimeLabelTimer.Elapsed += new ElapsedEventHandler(UpdateTimeLabel);
            UpdateTimeLabelTimer.Start();
            UpdateTimeLabel(null, null);
        }

        private void UpdateTimeLabel(object sender, ElapsedEventArgs e)
        {
            DateTime TimeNow = DateTime.Now;
            viewModel.CurrentTimeWidgetHourString = TimeNow.ToString("HH");
            viewModel.CurrentTimeWidgetMinuteString = TimeNow.ToString("mm");

            if (viewModel.CurrentTimeWidgetMiddlePartVisibility == Visibility.Visible)
                viewModel.CurrentTimeWidgetMiddlePartVisibility = Visibility.Hidden;
            else
                viewModel.CurrentTimeWidgetMiddlePartVisibility = Visibility.Visible;
        }
    }
}
