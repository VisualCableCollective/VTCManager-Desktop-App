namespace VTCManager_Client.Models
{
    public enum ControllerStatus
    {
        //                  GENERAL

        ///<summary>A fatal error occurred.</summary>
        OK,

        ///<summary>A fatal error occurred.</summary>
        FatalError,

        ///<summary>A fatal error occurred and a detailed message is available in the InitErrorMessage string of the controller.</summary>
        FatalErrorIEM,

        /// <summary>
        /// The controller initialization has already been done
        /// </summary>
        InitAlreadyDone,

        /// <summary>
        /// The controller initialization didn't finish or wasn't started yet
        /// </summary>
        InitNotDone,


        //                  APIs

        //  VTCManager API

        ///<summary>The connection to the VTCManager server could not be established.</summary>
        VTCMServerInoperational,

        ///<summary>The user hasn't connected his VTCManager account yet.</summary>
        VTCMShowLogin,

        ///<summary>The VTCManager API key in the storage is invalid or expired.</summary>
        VTCMShowLoginTokenExpired,
    }

    public enum ConnectionState
    {
        Connected,
        Disconnected,
    }
}
