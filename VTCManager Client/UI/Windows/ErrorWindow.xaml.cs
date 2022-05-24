using System;
using System.Windows;
using System.Windows.Input;

namespace VTCManager_Client.Windows
{
    public partial class ErrorWindow : Window
    {
        public ErrorWindow(string Title = null, string Details = "", bool ShutDown = true)
        {
            Controllers.LogController.Write("hi new erro win: " + Title);
            InitializeComponent();
            if (Title != null)
                TitleTB.Text += ": " + Title;
            DetailsTB.Text = Details;
        }

        private void TopBar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Controllers.ControllerManager.ShutDown();
            Environment.Exit(-1);
        }

        private void WindowCloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Controllers.ControllerManager.ShutDown();
            Environment.Exit(-1);
        }
    }
}
