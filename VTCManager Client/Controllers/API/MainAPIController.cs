using System.Collections.Generic;
using VTCManager_Client.Models;

namespace VTCManager_Client.Controllers.API
{
    public static class MainAPIController
    {
        public static readonly string LogPrefix = "[MainAPIController] ";
        public static List<ControllerStatus> APIInitStatusList = new List<ControllerStatus>();
        public static List<ControllerStatus> Init()
        {
            LogController.Write(LogPrefix + "Booting API Controllers...", LogController.LogType.Debug);

            //VTCM WS
            ControllerStatus VTCM_ws_init_result = VTCM_WSController.Init();
            LogController.Write(LogPrefix + "VTCM WS returned: " + VTCM_ws_init_result.ToString());
            if (VTCM_ws_init_result != ControllerStatus.OK)
            {
                APIInitStatusList.Add(VTCM_ws_init_result);
                return APIInitStatusList;
            }

            //VTCM API
            ControllerStatus VTCM_api_init_result = VTCM_APIController.Init();
            LogController.Write(LogPrefix + "VTCMAPI returned: " + VTCM_api_init_result.ToString());
            if (VTCM_api_init_result != ControllerStatus.OK)
            {
                APIInitStatusList.Add(VTCM_api_init_result);
                return APIInitStatusList;
            }

            LogController.Write(LogPrefix + "Init finished.", LogController.LogType.Debug);
            return APIInitStatusList;
        }

        public static void ShutDown()
        {
            LogController.Write(LogPrefix + "Shutting down API Controllers...", LogController.LogType.Debug);

            VTCM_WSController.ShutDown();

            LogController.Write(LogPrefix + "Shutdown of API Controllers finished.", LogController.LogType.Debug);
        }
    }
}