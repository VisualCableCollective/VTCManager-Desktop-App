using DiscordRPC;
using DiscordRPC.Logging;
using System;
using System.Timers;
using VTCManager.SDK.Models.Discord;
using VTCManager.SDK.Logging;

namespace VTCManager.SDK.Facades
{
    public class DiscordRPCFacade : IDisposable
    {
        private readonly DiscordRpcClient _discordRPCClient;

        private readonly Timer _updateRPCTimer = new(5000);

        // Logging
        private readonly string LOGPREFIX = nameof(DiscordRPCFacade);

        // RPC Config
        private readonly string DefaultSmallImageText = "AppName Version";
        private readonly string PauseSmallImage = "pause-icon";

        private Timestamps CurrentUsedTS;

        public static RpcStatus CurrentRpcStatus { get; private set; }

        public DiscordRPCFacade()
        {
            _discordRPCClient = new DiscordRpcClient("DiscordRpcClientId")
            {
                Logger = new ConsoleLogger() { Level = LogLevel.Warning }
            };

            // Set up event handling
            _discordRPCClient.OnReady += discordRPCClient_OnReady;
            _discordRPCClient.OnPresenceUpdate += discordRPCClient_OnPresenceUpdate;

            _discordRPCClient.Initialize();

            _updateRPCTimer.Elapsed += _updateRPCTimer_Elapsed;

            SetPresence(RpcStatus.LoadingApp);
        }

        #region Events
        private void discordRPCClient_OnPresenceUpdate(object sender, DiscordRPC.Message.PresenceMessage args)
        {
            Log.Debug("[Discord] Received presence update", "Discord");
        }

        private void discordRPCClient_OnReady(object sender, DiscordRPC.Message.ReadyMessage args)
        {
            Log.Info("Received Ready from user " + args.User.Username, "Discord");
        }
        #endregion

        private void _updateRPCTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RichPresence RPC = new()
            {
                Assets = new Assets()
                {
                    LargeImageKey = "big-image",
                    SmallImageKey = "vtcmanager_logo",
                    SmallImageText = DefaultSmallImageText,
                }
            };
            switch (CurrentRpcStatus)
            {
                case RpcStatus.TourRunning:
                    //RPC.Details = "Delivering " + TelemetryController.TelemetryData.JobValues.CargoValues.Name + " (" + ((int)TelemetryController.TelemetryData.JobValues.CargoValues.Mass) / 1000 + "t)";
                    //RPC.State = TelemetryController.TelemetryData.JobValues.CitySource + " -> " + TelemetryController.TelemetryData.JobValues.CityDestination + " (" + TelemetryController.GetTourPercentageCompleted() + "% completed)";
                    break;
                case RpcStatus.FreeRoam:
                    RPC.Details = "Free as the wind.";
                    break;
                default:
                    return;
            }

            //RPC.Assets.LargeImageText = "Driving in the " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Brand + " " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Name + " | " + (uint)TelemetryController.TelemetryData.TruckValues.CurrentValues.DashboardValues.Speed.Kph + " km/h";

            RPC.Timestamps = new Timestamps()
            {
                Start = CurrentUsedTS.Start,
            };
            _discordRPCClient.SetPresence(RPC);
            Log.Debug("Auto updated RPC: Current RPC is " + CurrentRpcStatus.ToString(), LOGPREFIX);
        }

        public void SetPresence(RpcStatus rpcStatus)
        {
            RichPresence rpc = new();
            switch (rpcStatus)
            {
                case RpcStatus.LoadingApp:
                    rpc.Details = "Launching VTCManager...";
                    rpc.Assets = new Assets()
                    {
                        LargeImageKey = "big-image",
                        SmallImageKey = "vtcmanager_logo",
                        SmallImageText = DefaultSmallImageText,
                    };
                    break;
                case RpcStatus.TourRunning:
                    //rpc.Details = "Delivering " + TelemetryController.TelemetryData.JobValues.CargoValues.Name + " (" + ((int)TelemetryController.TelemetryData.JobValues.CargoValues.Mass) / 1000 + "t)";
                    //rpc.State = TelemetryController.TelemetryData.JobValues.CitySource + " -> " + TelemetryController.TelemetryData.JobValues.CityDestination + " (" + TelemetryController.GetTourPercentageCompleted() + "% completed)";
                    rpc.Assets = new Assets()
                    {
                        LargeImageKey = "big-image",
                        //LargeImageText = "Driving " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Brand + " " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Name + " | " + (uint)TelemetryController.TelemetryData.TruckValues.CurrentValues.DashboardValues.Speed.Kph + " km/h",
                        SmallImageKey = "vtcmanager_logo",
                        SmallImageText = DefaultSmallImageText,
                    };
                    CurrentUsedTS = Timestamps.Now;
                    rpc.Timestamps = new Timestamps()
                    {
                        Start = CurrentUsedTS.Start,
                    };
                    _updateRPCTimer.Start();
                    break;
                case RpcStatus.FreeRoam:
                    rpc.Details = "Free as the wind.";
                    rpc.Assets = new Assets()
                    {
                        LargeImageKey = "big-image",
                        //LargeImageText = "Driving " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Brand + " " + TelemetryController.TelemetryData.TruckValues.ConstantsValues.Name + " | " + (uint)TelemetryController.TelemetryData.TruckValues.CurrentValues.DashboardValues.Speed.Kph + " km/h",
                        SmallImageKey = "vtcmanager_logo",
                        SmallImageText = DefaultSmallImageText,
                    };
                    CurrentUsedTS = Timestamps.Now;
                    rpc.Timestamps = new Timestamps()
                    {
                        Start = CurrentUsedTS.Start,
                    };
                    _updateRPCTimer.Start();
                    break;
                case RpcStatus.IDLE:
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
                    _updateRPCTimer.Stop();
                    break;
                default:
                    return;
            }
            CurrentRpcStatus = rpcStatus;
            _discordRPCClient.SetPresence(rpc);
            Log.Debug("Updated RPC", LOGPREFIX);
        }

        public void Dispose()
        {
            _updateRPCTimer.Stop();
            _discordRPCClient.Dispose();
        }
    }
}
