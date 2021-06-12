namespace VTCManager_Client
{
    /// <summary>
    /// General information about the application
    /// </summary>
    public static class VTCManager
    {
        public static readonly string AppName = "VTCManager";
        public static readonly string Author = "The VisualCable Collective";
        public static readonly string Version = "v1.4.0 Public Beta";
        public static readonly string DiscordClientID = "659036297561767948";

        #region UpdateInfo
        public static readonly string UpdatePublishedData = "June 12, 2021";

        //Changelog
        public static readonly string CLNewFeaturesList = "- New Hotkey: You can now open and close the sidebar with the \"ESC\" key.";
        public static readonly string CLAdditionalImprovementsList = "- The behavior of the sidebar when opening and closing and the visual display of the navigation items when navigating have been improved.\n- The image that is displayed in the dashboard when no game is running has been replaced with an image that matches the current summer season.\n- The download progress of the latest update is now displayed in the system tray.";
        public static readonly string CLSecurityAndBugFixesList = "- Fixed an issue where skipping the VCC intro animation resulted in the configuration file not being read.\n- Fixed an issue where the application was not brought to the foreground and could potentially crash.\n- Fixed an issue that incorrectly displayed error messages when there was no internet connection.\n- Fixed an issue that caused the application to crash if the configuration file was corrupted.";
        #endregion

        /// <summary>
        /// Can be changed in the config file or in the settings by the user
        /// </summary>
        public static bool DebugMode = false;

        /// <summary>
        /// Only true in development environment and can't be changed in the settings by the user
        /// </summary>
        public static bool DeveloperMode =>
#if DEBUG
                true;
#else
                false;
#endif


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

        // Crash Reports
        public static readonly string CrashReportReceiverEmail = "joschua.hass.sh@gmail.com";
    }
}