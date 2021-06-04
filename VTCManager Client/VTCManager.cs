namespace VTCManager_Client
{
    /// <summary>
    /// General information about the app
    /// </summary>
    public static class VTCManager
    {
        public static string AppName = "VTCManager";
        public static string Author = "The VisualCable Collective";
        public static string Version = "v1.4.0 Public Beta";
        public static string DiscordClientID = "659036297561767948";

        #region UpdateInfo
        public static string UpdatePublishedData = "March 28, 2021";
        //Changelog
        public static string CLNewFeaturesList = "";
        public static string CLAdditionalImprovementsList = "- The behavior of the sidebar when opening and closing and the visual display of the navigation items when navigating have been improved.\n- The image that is displayed in the dashboard when no game is running has been replaced with an image that matches the current summer season.";
        public static string CLSecurityAndBugFixesList = "- Fixed an issue where skipping the VCC intro animation resulted in the configuration file not being read.\n- Fixed an issue where the application was not brought to the foreground and could potentially crash.";
        #endregion

        /// <summary>
        /// Can be changed in the config file or in the settings by the user
        /// </summary>
        public static bool DebugMode = false;

        /// <summary>
        /// Only true in development environment
        /// </summary>
        public static bool DeveloperMode //can't be changed in the settings by the user
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Use local VTCManager/VCC Server
        /// </summary>
        public static bool UseLocalServer = false;

        /// <summary>
        /// Returns the type of the active main window
        /// </summary>
        public static Models.Enums.AppWindow CurrentWindow = Models.Enums.AppWindow.LoadingWindow;

        public static bool SilentAutoStartMode = false;

        // VTCM API

        //WS
        public static readonly string VTCMAPI_WSHost = "api.vtcmanager.eu:6001";
        public static readonly string VTCMAPI_WSAuthEndpointURL = "https://api.vtcmanager.eu/broadcasting/auth";
        public static readonly string VTCMAPI_WSAppKey = "vtcmapitestingpusherkey";
    }
}