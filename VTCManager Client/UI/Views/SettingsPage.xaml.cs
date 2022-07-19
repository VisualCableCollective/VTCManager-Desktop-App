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
using VTCManager_Client.Controllers;

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

            InstallPlugins_Button.Click += InstallPlugins_Button_Click;
        }

        private void InstallPlugins_Button_Click(object sender, RoutedEventArgs e)
        {
            StorageController.Config.ETS_Plugin_Installation_Tried = false;
            StorageController.Config.ATS_Plugin_Installation_Tried = false;

            PluginInstaller.Install();

            string mbText;
            var mbImage = MessageBoxImage.Information;

            if (StorageController.Config.ETS_Plugin_Installed || StorageController.Config.ATS_Plugin_Installed)
            {
                mbText = "Successfully installed plugins for the following games: ";

                if (StorageController.Config.ETS_Plugin_Installed)
                {
                    mbText += "Euro Truck Simulator 2";
                }

                if (StorageController.Config.ATS_Plugin_Installed)
                {
                    if (mbText.Contains("Euro Truck"))
                    {
                        mbText += " & American Truck Simulator";
                    }
                    else
                    {
                        mbText += "American TruckSimulator";
                    }
                }
            } 
            else
            {
                mbText = "The plugin installation was not successful because no Euro Truck Simulator 2 or American Truck Simulator installation could be found.";
                mbImage = MessageBoxImage.Error;
            }

            MessageBox.Show(mbText, "VTCManager: Plugin Installation", MessageBoxButton.OK, mbImage);
        }

        private void EnableDiscordRPC_CB_Unchecked(object sender, RoutedEventArgs e)
        {
            StorageController.Config.DiscordRPC_Enabled = false;
            DiscordRPCController.ShutDown();
        }

        private void EnableDiscordRPC_CB_Checked(object sender, RoutedEventArgs e)
        {
            StorageController.Config.DiscordRPC_Enabled = true;
            DiscordRPCController.Init();
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
