namespace VTCManager_Client
{
    /// <summary>
    /// General information about the app
    /// </summary>
    public static class VTCManager
    {
        public static string AppName = "VTCManager";
        public static string Author = "The VisualCable Collective";
        public static string Version = "v1.3.0 Public Beta";
        public static string DiscordClientID = "659036297561767948";

        #region UpdateInfo
        public static string UpdatePublishedData = "March 28, 2021";
        //Changelog
        public static string CLNewFeaturesList = "- VTCManager now starts automatically in the background, so you don't have to worry anymore if you forgot to start VTCManager (enabled by default)\n- Introducing the tray icon: When VTCManager is running, you will see an icon in the lower right corner of the taskbar";
        public static string CLAdditionalImprovementsList = "- The animation when opening and closing the navigation menu has been improved\n- Opening the application multiple times was fixed to address possible stability issues\n- The startup speed has been improved\n- New Discord RPC image";
        public static string CLSecurityAndBugFixesList = "- Fixed an issue where required application files weren't copied after a minor update";
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