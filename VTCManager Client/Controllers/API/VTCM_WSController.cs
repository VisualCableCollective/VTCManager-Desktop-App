using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PusherClient;

namespace VTCManager_Client.Controllers.API
{
    public static class VTCM_WSController
    {
        public static Pusher client = null;
        private static readonly string LogPrefix = "[" + nameof(VTCM_WSController) + "] ";
        private static bool InitDone = false;
        public static Models.ControllerStatus Init()
        {
            client = new Pusher(VTCManager.VTCMAPI_WSAppKey, new PusherOptions()
            {
                Host = VTCManager.VTCMAPI_WSHost,
                Authorizer = new HttpAuthorizer(VTCManager.VTCMAPI_WSAuthEndpointURL, AuthDataController.GetAPIToken())
            });
            client.ConnectionStateChanged += Client_ConnectionStateChanged;
            client.Connected += Client_Connected;
            client.Disconnected += Client_Disconnected;
            client.Error += Client_Error;

            if (!VTCManager.EnableWebsockets)
                return Models.ControllerStatus.OK;

            ConnectionState connectionState;

            // if an exception occurs, it usually means that we can't connect to the server
            try
            {
                connectionState = client.ConnectAsync().Result;
            }catch(AggregateException ex)
            {
                return Models.ControllerStatus.VTCMServerInoperational;
            }

            if (connectionState == ConnectionState.ConnectionFailed)
                return Models.ControllerStatus.VTCMServerInoperational;

            InitDone = true;

            return Models.ControllerStatus.OK;
        }

        #region Client Events
        private static void Client_Connected(object sender)
        {
            LogController.Write(LogPrefix + "Connected");
            if(ControllerManager.MainWindow != null)
                ControllerManager.MainWindow.dashPage.maindash.UpdateConnectionStatus(Models.ConnectionState.Connected);
        }

        private static void Client_ConnectionStateChanged(object sender, ConnectionState state)
        {
            LogController.Write(LogPrefix + "Connection state changed to " + state.ToString());
        }

        private static void Client_Disconnected(object sender)
        {
            LogController.Write(LogPrefix + "Disconnected");
            if (ControllerManager.MainWindow != null)
                if(ControllerManager.MainWindow.dashPage != null)
                    ControllerManager.MainWindow.dashPage.maindash.UpdateConnectionStatus(Models.ConnectionState.Disconnected);
        }

        private static void Client_Error(object sender, PusherException error)
        {
            LogController.Write(LogPrefix + "An error (PusherCode: " + error.PusherCode.ToString() + ") occured. Message: " + error.Message, LogController.LogType.Error);
        }
        #endregion

        public static void ShutDown()
        {
            if (!InitDone)
                return;

            client.DisconnectAsync();
            client = null;
            InitDone = false;
        }
    }
}
