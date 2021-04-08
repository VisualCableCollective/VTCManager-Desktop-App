using Microsoft.Win32;
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

namespace VTCManager_Client.UI.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        RegistryKey RegSubKeyAutoStart = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        public SettingsPage()
        {
            InitializeComponent();
            if (Controllers.StorageController.Config.DiscordRPC_Enabled)
                EnableDiscordRPC_CB.IsChecked = true;

            if (RegSubKeyAutoStart.GetValue("VTCManager") == null)
            {
                EnableAutoStart_CB.IsChecked = false;
            }
            else
            {
                EnableAutoStart_CB.IsChecked = true;
            }
        }

        private void EnableDiscordRPC_CB_Unchecked(object sender, RoutedEventArgs e)
        {
            Controllers.StorageController.Config.DiscordRPC_Enabled = false;
            Controllers.DiscordRPCController.ShutDown();
        }

        private void EnableDiscordRPC_CB_Checked(object sender, RoutedEventArgs e)
        {
            Controllers.StorageController.Config.DiscordRPC_Enabled = true;
            Controllers.DiscordRPCController.Init();
        }

        private void EnableAutoStart_CB_Checked(object sender, RoutedEventArgs e)
        {
            RegSubKeyAutoStart.SetValue("VTCManager", "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"" + " -silent");
        }

        private void EnableAutoStart_CB_Unchecked(object sender, RoutedEventArgs e)
        {
            RegSubKeyAutoStart.DeleteValue("VTCManager", false);
            Controllers.StorageController.Config.User_Disabled_Auto_Start = true;
        }
    }
}
