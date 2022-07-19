namespace VTCManager_Client
{
    /// <summary>
    /// General information about the application
    /// </summary>
    public static class VTCManager
    {
        public static readonly string AppName = "VTCManager";
        public static readonly string Author = "The VisualCable Collective";
        public static readonly string VersionId = "1.5.0";
        public static readonly string Version = $"v{VersionId} Public Beta";
        public static readonly string DiscordClientID = "659036297561767948";

        #region UpdateInfo
        public static readonly string UpdatePublishedData = "July 19, 2022";

        //Changelog
        public static readonly string CLNewFeaturesList = "- The game plugin installation can now be started by pressing a button in the settings.";
        public static readonly string CLAdditionalImprovementsList = "- The automatic plugin installation is now running once after each update and not after each start of the client as before.";
        public static readonly string CLSecurityAndBugFixesList = "- Fixed a bug that unintentionally focused certain UI elements.\n- Security patches, bug fixes and performance improvements.";
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