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
            ChangelogTitle.Content += VTCManager.Version;
            UpdatePublishedLabel.Content += VTCManager.UpdatePublishedData;
            if (!string.IsNullOrWhiteSpace(VTCManager.CLNewFeaturesList))
            {
                NewFeaturesList.Text = VTCManager.CLNewFeaturesList;
            }
            else
            {
                NewFeaturesTitle.Visibility = System.Windows.Visibility.Collapsed;
                NewFeaturesList.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (!string.IsNullOrWhiteSpace(VTCManager.CLAdditionalImprovementsList))
            {
                AdditionalImprovementsList.Text = VTCManager.CLAdditionalImprovementsList;
            }
            else
            {
                AdditionalImprovementsTitle.Visibility = System.Windows.Visibility.Collapsed;
                AdditionalImprovementsList.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (!string.IsNullOrWhiteSpace(VTCManager.CLSecurityAndBugFixesList))
            {
                BugAndSecurityFixesList.Text = VTCManager.CLSecurityAndBugFixesList;
            }
            else
            {
                BugAndSecurityFixesTitle.Visibility = System.Windows.Visibility.Collapsed;
                BugAndSecurityFixesList.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void Confirmed_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Controllers.StorageController.Config.last_version_used = VTCManager.Version;
            Controllers.ModalController.CloseCurrentModal();
        }
    }
}