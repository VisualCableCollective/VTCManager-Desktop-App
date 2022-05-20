﻿namespace VTCManager_Client
{
    /// <summary>
    /// General information about the application
    /// </summary>
    public static class VTCManager
    {
        public static readonly string AppName = "VTCManager";
        public static readonly string Author = "The VisualCable Collective";
        public static readonly string Version = "v1.4.1 Public Beta";
        public static readonly string DiscordClientID = "659036297561767948";

        #region UpdateInfo
        public static readonly string UpdatePublishedData = "October 29, 2021";

        //Changelog
        public static readonly string CLNewFeaturesList = "";
        public static readonly string CLAdditionalImprovementsList = "- The edge rounding and the size of the widgets have been adjusted.\n- The design of the button to open the sidebar has been adjusted when hovering with a mouse over the button.";
        public static readonly string CLSecurityAndBugFixesList = "- Fixed an issue that led to a faulty plugin installation and the application crashing during the plugin installation.";
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