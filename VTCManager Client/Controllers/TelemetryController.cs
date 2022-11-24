using SCSSdkClient;
using SCSSdkClient.Object;
using System;
using System.Windows;
using VTCManager.Logging;

namespace VTCManager_Client.Controllers
{
    public static class TelemetryController
    {
        private static SCSSdkTelemetry Telemetry;
        private static readonly string LogPrefix = "[TelemetryController] ";
        public static SCSTelemetry TelemetryData = new SCSTelemetry();
        public static float TrailerAvgDamageChassis = 0;
        public static float TrailerAvgDamageWheels = 0;
        public static bool isGamePaused = false;
        private static bool _InitDone = false;
        private static uint prevGameTimeValue = 0;
        private static DateTime prevGameTimeCheckTime;
        public static bool InitDone
        {
            get { return _InitDone; }
        }

        public static bool JobRunning = false;

        public static void Init()
        {
            // init telemetry
            Telemetry = new SCSSdkTelemetry();
            Telemetry.Data += DataReceived;
            Telemetry.JobStarted += JobStarted;

            Telemetry.JobCancelled += JobCancelled;
            Telemetry.JobDelivered += JobDelivered;
            Telemetry.Fined += Fined;
            Telemetry.Tollgate += Tollgate;
            Telemetry.Ferry += FerryUsed;
            Telemetry.Train += TrainUsed;
            Telemetry.RefuelStart += RefuelStarted;
            Telemetry.RefuelEnd += RefuelEnded;
            Telemetry.RefuelPayed += RefuelPayed;

            if (Telemetry.Error != null)
            {
                LogController.Write(LogPrefix + "The following telemetry error occured: " + Telemetry.Error.StackTrace, LogController.LogType.Warning);
                MessageBox.Show("The following telemetry error occured: " + Telemetry.Error.StackTrace, "Telemetry Controller", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            _InitDone = true;
        }

        public static void ShutDown()
        {
            if (!_InitDone)
                return;
            Telemetry.Dispose();
        }

        private static void RefuelPayed(object sender, EventArgs e)
        {

        }

        private static void RefuelEnded(object sender, EventArgs e)
        {

        }

        private static void RefuelStarted(object sender, EventArgs e)
        {

        }

        private static void TrainUsed(object sender, EventArgs e)
        {

        }

        private static void FerryUsed(object sender, EventArgs e)
        {

        }

        private static void Tollgate(object sender, EventArgs e)
        {

        }
        private static void Fined(object sender, EventArgs e)
        {

        }
        private static void JobDelivered(object sender, EventArgs e)
        {
            LogController.Write(LogPrefix + "Job has been delivered");
            JobRunning = false;
            API.VTCM_APIController.JobDelivered();
            NotificationController.Notificate(NotificationController.NotificationType.JobDelivered, "Job Delivered");
        }

        private static void JobCancelled(object sender, EventArgs e)
        {
            LogController.Write(LogPrefix + "Job has been cancelled");
            JobRunning = false;
            API.VTCM_APIController.JobCancelled();
            NotificationController.Notificate(NotificationController.NotificationType.JobCancelled, "Job Cancelled");
        }

        private static void JobStarted(object sender, EventArgs e)
        {
            LogController.Write(LogPrefix + "Job has been started");
            JobRunning = true;

            //we also run this here because when the JobStarted event is called, the game should be running and data in the log file should be available
            GameLogController.SetGameLanguageCodeInConfig();

            API.VTCM_APIController.NewJob();
        }

        private static void DataReceived(SCSTelemetry data, bool newTimestamp)
        {
            TelemetryData = data;
            CalculateAverageTrailerDamageValues();
            CheckIfGameIsPaused();
        }

        /// <summary>
        /// The Paused value in the TelemetryData object doesn't work, so we have to check this manually.
        /// </summary>
        private static void CheckIfGameIsPaused()
        {
            // we have to wait 5 seconds, because the GameTime value doesn't update very often
            DateTime currentTelemetryTime = DateTime.Now;
            if(prevGameTimeCheckTime != null)
            {
                TimeSpan difference = currentTelemetryTime - prevGameTimeCheckTime;
                if (difference.TotalSeconds < 5)
                    return;
            }

            if (prevGameTimeValue == TelemetryData.CommonValues.GameTime.Value)
            {
                isGamePaused = true;
            }
            else
            {
                isGamePaused = false;
            }
            prevGameTimeValue = TelemetryData.CommonValues.GameTime.Value;
            prevGameTimeCheckTime = currentTelemetryTime;
        }

        private static void CalculateAverageTrailerDamageValues()
        {
            float _TrailerAvgDamageChassis = 0;
            float _TrailerAvgDamageWheels = 0;
            foreach (SCSTelemetry.Trailer trailer in TelemetryData.TrailerValues)
            {
                _TrailerAvgDamageChassis += trailer.DamageValues.Chassis;
                _TrailerAvgDamageWheels += trailer.DamageValues.Wheels;
            }
            TrailerAvgDamageChassis = _TrailerAvgDamageChassis / TelemetryData.TrailerValues.Length;
            TrailerAvgDamageWheels = _TrailerAvgDamageWheels / TelemetryData.TrailerValues.Length;
        }

        /// <summary>
        /// Calculates the percentage to what extent the tour is completed.
        /// </summary>
        /// <returns><see cref="uint"/> between 0 and 100</returns>
        public static uint GetTourPercentageCompleted()
        {
            int percentageCompleted = 100 - (int)((((TelemetryData.NavigationValues.NavigationDistance / 1000) / TelemetryData.JobValues.PlannedDistanceKm) * 100));
            if (percentageCompleted > 100)
            {
                percentageCompleted = 100;
            }
            else if (percentageCompleted < 0)
            {
                percentageCompleted = 0;
            }

            return (uint)percentageCompleted;
        }
    }
}