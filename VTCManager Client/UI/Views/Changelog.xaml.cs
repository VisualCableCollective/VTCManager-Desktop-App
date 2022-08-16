using System.Windows.Controls;
using System.Windows.Input;

namespace VTCManager_Client.UI.Views
{
    /// <summary>
    /// Interaktionslogik für Changelog.xaml
    /// </summary>
    public partial class Changelog : Page
    {
        public Changelog()
        {
            InitializeComponent();
            ChangelogTitle.Text += AppInfo.Version;
            UpdatePublishedLabel.Content += AppInfo.UpdatePublishedData;
            if (!string.IsNullOrWhiteSpace(AppInfo.CLNewFeaturesList))
            {
                NewFeaturesList.Text = AppInfo.CLNewFeaturesList;
            }
            else
            {
                NewFeaturesTitle.Visibility = System.Windows.Visibility.Collapsed;
                NewFeaturesList.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (!string.IsNullOrWhiteSpace(AppInfo.CLAdditionalImprovementsList))
            {
                AdditionalImprovementsList.Text = AppInfo.CLAdditionalImprovementsList;
            }
            else
            {
                AdditionalImprovementsTitle.Visibility = System.Windows.Visibility.Collapsed;
                AdditionalImprovementsList.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (!string.IsNullOrWhiteSpace(AppInfo.CLSecurityAndBugFixesList))
            {
                BugAndSecurityFixesList.Text = AppInfo.CLSecurityAndBugFixesList;
            }
            else
            {
                BugAndSecurityFixesTitle.Visibility = System.Windows.Visibility.Collapsed;
                BugAndSecurityFixesList.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void Confirmed_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Controllers.StorageController.Config.last_version_used = AppInfo.Version;
            Controllers.ModalController.CloseCurrentModal();
        }
    }
}