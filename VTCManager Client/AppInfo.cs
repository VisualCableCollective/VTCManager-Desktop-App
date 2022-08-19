namespace VTCManager_Client
{
    /// <summary>
    /// General information about the application
    /// </summary>
    public static class AppInfo
    {
        public static readonly string AppName = "VTCManager";
        public static readonly string Author = "The VisualCable Collective";
        public static readonly string VersionId = "1.5.3";
        public static readonly string Version = $"v{VersionId} Public Beta";
        public static readonly string DiscordClientID = "659036297561767948";

        #region UpdateInfo
        public static readonly string UpdatePublishedDate = "August 16, 2022";

        //Changelog
        public static readonly string CLNewFeaturesList = "";
        public static readonly string CLAdditionalImprovementsList = "- The design of the time in the dashboard has been improved.";
        public static readonly string CLSecurityAndBugFixesList = "- Fixed an issue that sometimes caused the error window to not be displayed properly (and the application to crash).\n- Fixed an issue that caused an error when closing a popup/modal.\n- Fixed an issue that caused incorrect behavior when canceling a job.";
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

        public static bool EnableWebsockets = false;

        // VTCM API
        //WS
        public static readonly string VTCMAPI_WSHost = "api.vtcmanager.eu:6001";
        public static readonly string VTCMAPI_WSAuthEndpointURL = "https://api.vtcmanager.eu/broadcasting/auth";
        public static readonly string VTCMAPI_WSAppKey = "vtcmapitestingpusherkey";

        // Crash Reports
        public static readonly string CrashReportReceiverEmail = "joschua.hass.sh@gmail.com";
    }
}