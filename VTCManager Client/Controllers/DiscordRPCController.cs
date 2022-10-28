using DiscordRPC;
using DiscordRPC.Logging;
using System;
using System.Timers;

namespace VTCManager_Client.Controllers
{
    public static class DiscordRPCController
    {
        private static DiscordRpcClient DiscordRPCClient;
        private static Timer UpdateRPCTimer = new Timer(5000);
        private static readonly string LogPrefix = "[DiscordRPCController] ";
        private static bool InitDone = false;
        private static String DefaultSmallImageText = AppInfo.AppName + " " + AppInfo.Version;
        private static readonly String PauseSmallImage = "pause-icon";
        private static Timestamps CurrentUsedTS;
        public static RPCStatus CurrentRPCStatus
        {
            get
            {
                return _CurrentRPCStatus;
            }
        }
        private static RPCStatus _CurrentRPCStatus;
        public enum RPCStatus
        {
            LoadingApp,
            IDLE,
            TourRunning,
            FreeRoam,
            NoPresence
        }
        public static void Init()
        {
            if (InitDone)
                return;
            DiscordRPCClient = new DiscordRpcClient(AppInfo.DiscordClientID)
            {
                Logger = new ConsoleLogger() { Level = LogLevel.Warning }
            };
            DiscordRPCClient.OnReady += (sender, e) =>
            {
                LogController.Write("[Discord] Received Ready from user " + e.User.Username);
            };
            DiscordRPCClient.OnPresenceUpdate += (sender, e) =>
            {
                LogController.Write("[Discord] Received presence update", LogController.LogType.Debug);
            };
            DiscordRPCClient.Initialize();

            UpdateRPCTimer.Elapsed += UpdateRPCTimer_Elapsed;

            InitDone = true;
            SetPresence(RPCStatus.LoadingApp);
        }

        private static void UpdateRPCTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RichPresence RPC = new RichPresence
            {
                Assets = new Assets()
                {
                    LargeImageKey = "big-image",
                    SmallImageKey = "vtcmanager_logo",
                    SmallImageText = DefaultSmallImageText,
                },
                Buttons = new Button[]
                {
                    new Button() {Label = "Get VTCManager", Url = "https://vtcmanager.eu/"}
                }
            };
            switch (CurrentRPCStatus)
            {
                case RPCStatus.TourRunning:
                    RPC.Details = "Delivering " + TelemetryController.TelemetryData.JobValues.CargoValues.Name + " (" + ((int)TelemetryController.TelemetryData.JobValues.CargoValues.Mass) / 1000 + "t)";
                    RPC.State = TelemetryController.TelemetryData.JobValues.CitySource + " -> " + TelemetryController.TelemetryData.JobValues.CityDestination + " (" + TelemetryController.GetTourPercentageCompleted() + "% completed)";
                    break;
                case RPCStatus.FreeRoam:
                    RPC.Details = "Free as the wind.";
                    break;
                default:
                    return;
            }

            /*LogController.Write(LogPrefix + "isGamePaused: " + TelemetryController.isGamePaused, LogController.LogType.Debug);
            if (TelemetryController.isGamePaused)
            {
                RPC.Assets.SmallImageKey = PauseSmallImage;
                RPC.Assets.LargeImageText = "Resting in the " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Brand + " " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Name;
            }
            else
            {
                RPC.Assets.LargeImageText = "Driving in the " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Brand + " " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Name + " | " + (uint)TelemetryController.TelemetryData.TruckValues.CurrentValues.DashboardValues.Speed.Kph + " km/h";
            }*/
            RPC.Assets.LargeImageText = "Driving in the " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Brand + " " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Name + " | " + (uint)TelemetryController.TelemetryData.TruckValues.CurrentValues.DashboardValues.Speed.Kph + " km/h";

            RPC.Timestamps = new Timestamps()
            {
                Start = CurrentUsedTS.Start,
            };
            DiscordRPCClient.SetPresence(RPC);
            LogController.Write(LogPrefix + "Auto Updated RPC: Current RPC is " + CurrentRPCStatus.ToString(), LogController.LogType.Debug);
        }

        public static void ShutDown()
        {
            if (!InitDone)
                return;
            UpdateRPCTimer.Stop();
            DiscordRPCClient.Dispose();
            InitDone = false;
        }

        public static void SetPresence(RPCStatus rpc_status)
        {
            if (!InitDone)
                return;
            RichPresence rpc = new RichPresence();
            switch (rpc_status)
            {
                case RPCStatus.LoadingApp:
                    rpc.Details = "Launching VTCManager...";
                    rpc.Assets = new Assets()
                    {
                        LargeImageKey = "big-image",
                        SmallImageKey = "vtcmanager_logo",
                        SmallImageText = DefaultSmallImageText,
                    };
                    break;
                case RPCStatus.TourRunning:
                    rpc.Details = "Delivering " + TelemetryController.TelemetryData.JobValues.CargoValues.Name + " (" + ((int)TelemetryController.TelemetryData.JobValues.CargoValues.Mass) / 1000 + "t)";
                    rpc.State = TelemetryController.TelemetryData.JobValues.CitySource + " -> " + TelemetryController.TelemetryData.JobValues.CityDestination + " (" + TelemetryController.GetTourPercentageCompleted() + "% completed)";
                    rpc.Assets = new Assets()
                    {
                        LargeImageKey = "big-image",
                        LargeImageText = "Driving " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Brand + " " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Name + " | " + (uint)TelemetryController.TelemetryData.TruckValues.CurrentValues.DashboardValues.Speed.Kph + " km/h",
                        SmallImageKey = "vtcmanager_logo",
                        SmallImageText = DefaultSmallImageText,
                    };
                    CurrentUsedTS = Timestamps.Now;
                    rpc.Timestamps = new Timestamps()
                    {
                        Start = CurrentUsedTS.Start,
                    };
                    UpdateRPCTimer.Start();
                    break;
                case RPCStatus.FreeRoam:
                    rpc.Details = "Free as the wind.";
                    rpc.Assets = new Assets()
                    {
                        LargeImageKey = "big-image",
                        LargeImageText = "Driving " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Brand + " " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Name + " | " + (uint)TelemetryController.TelemetryData.TruckValues.CurrentValues.DashboardValues.Speed.Kph + " km/h",
                        SmallImageKey = "vtcmanager_logo",
                        SmallImageText = DefaultSmallImageText,
                    };
                    CurrentUsedTS = Timestamps.Now;
                    rpc.Timestamps = new Timestamps()
                    {
                        Start = CurrentUsedTS.Start,
                    };
                    UpdateRPCTimer.Start();
                    break;
                case RPCStatus.IDLE:
                    rpc.Details = "No Game Running.";
                    CurrentUsedTS = Timestamps.Now;
                    rpc.Timestamps = new Timestamps()
                    {
                        Start = CurrentUsedTS.Start,
                    };
                    rpc.Assets = new Assets()
                    {
                        LargeImageKey = "big-image",
                        SmallImageKey = "vtcmanager_logo",
                        SmallImageText = DefaultSmallImageText,
                    };
                    UpdateRPCTimer.Stop();
                    break;
                default:
                    return;
            }

            rpc.Buttons = new Button[]
            {
                new Button() {Label = "Get VTCManager", Url = "https://vtcmanager.eu/"}
            };

            _CurrentRPCStatus = rpc_status;
            DiscordRPCClient.SetPresence(rpc);
            LogController.Write(LogPrefix + "Updated RPC", LogController.LogType.Debug);
        }
    }
}