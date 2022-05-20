using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace VTCManager_Client.Controllers.API
{
    /// <summary>
    /// Contains alls the requests towards the VTCM HTTP API
    /// </summary>
    public static class VTCM_APIController
    {
        private static string Server_url = "https://api.vtcmanager.eu/api/";
        private static string UserData_url = Server_url + "user";
        private static string ServerStatus_url = Server_url + "status";
        private static string CreateNewJob_url = Server_url + "jobs";
        private static string DataUpdate_url = Server_url + "client/update";
        private static readonly string LogPrefix = "[VTCM_API] ";
        private static bool NewJobWaitForDataTimeOut_Elapsed = false;
        private static Timer DataUpdateTimer = new Timer(5000);

        //only executed by the loading screen
        public static Models.ControllerStatus Init()
        {
            if (VTCManager.UseLocalServer)
            {
                LogController.Write(LogPrefix + "Changing server url to localhost:8000", LogController.LogType.Warning);
                Server_url = "http://computer.local:8000/api/";
                //update urls
                UserData_url = Server_url + "user";
                ServerStatus_url = Server_url + "status";
                CreateNewJob_url = Server_url + "jobs";
                DataUpdate_url = Server_url + "client/update";
            }
            DataUpdateTimer.Elapsed += DataUpdateTimer_Elapsed;

            if (!IsServerOperational())
            {
                return Models.ControllerStatus.VTCMServerInoperational;
            }

            if (String.IsNullOrEmpty(AuthDataController.GetAPIToken()))
            {
                return Models.ControllerStatus.VTCMShowLogin;
            }
            else
            {
                // check if token is still valid
                if (!IsAuthTokenValid()) {
                    return Models.ControllerStatus.VTCMShowLoginTokenExpired;
                }
                else
                {
                    DataUpdateTimer.Start();
                    return Models.ControllerStatus.OK;
                }
            }
        }

        public static string GetUserData()
        {
            // check if token is still valid
            Dictionary<string, string> response = HTTPRequestController.Request(UserData_url, HTTPRequestController.Service.VTCManager);
            response.TryGetValue("response", out string response_string);
            return response_string.Trim();
        }

        public static bool IsAuthTokenValid()
        {
            string server_response = GetUserData();
            switch (server_response)
            {
                case "{\"message\":\"Unauthenticated.\"}":
                    return false;
                case "REQUEST_FAILED":
                    return false;
                default:
                    return true;
            }
        }

        public static void ActivateDataSync()
        {
            DataUpdateTimer.Start();
        }

        private static void DataUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(() =>
            {
                DataUpdateTimer.Stop();
                LogController.Write(LogPrefix + "Sending regular data update to the server...", LogController.LogType.Debug);
                Dictionary<string, string> postParameters = new Dictionary<string, string>();
                if (TelemetryController.TelemetryData.Game == SCSSdkClient.SCSGame.Unknown)
                {
                    postParameters.Add("game_running", "false");
                }
                else
                {
                    postParameters.Add("game_running", TelemetryController.TelemetryData.Game.ToString());

                    //general player data
                    postParameters.Add("PositionX", TelemetryController.TelemetryData.TruckValues.CurrentValues.PositionValue.Position.X.ToString());
                    postParameters.Add("PositionY", TelemetryController.TelemetryData.TruckValues.CurrentValues.PositionValue.Position.Y.ToString());
                    postParameters.Add("PositionZ", TelemetryController.TelemetryData.TruckValues.CurrentValues.PositionValue.Position.Z.ToString());
                    postParameters.Add("OrientationHeading", TelemetryController.TelemetryData.TruckValues.CurrentValues.PositionValue.Orientation.Heading.ToString());

                    //job data
                    if (TelemetryController.JobRunning)
                    {
                        //General data
                        postParameters.Add("JobID", StorageController.Config.User.Job.ID.ToString());
                        postParameters.Add("TrailersAttached", TelemetryController.TelemetryData.TrailerValues.Length.ToString());
                        postParameters.Add("CurrentIngameTime", TelemetryController.TelemetryData.CommonValues.GameTime.Date.ToString("yyyy-MM-dd HH:mm:ss"));

                        //current truck damage
                        postParameters.Add("current_truck_cabin_damage", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Cabin.ToString());
                        postParameters.Add("current_truck_chassis_damage", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Chassis.ToString());
                        postParameters.Add("current_truck_engine_damage", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Engine.ToString());
                        postParameters.Add("current_truck_transmission_damage", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Transmission.ToString());
                        postParameters.Add("current_truck_wheels_avg_damage", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.WheelsAvg.ToString());

                        //current trailer damage
                        postParameters.Add("current_trailer_avg_damage_chassis", TelemetryController.TrailerAvgDamageChassis.ToString());
                        postParameters.Add("current_trailer_avg_damage_wheels", TelemetryController.TrailerAvgDamageWheels.ToString());

                        //navigation data
                        postParameters.Add("CurrentSpeedKph", Math.Abs((int)TelemetryController.TelemetryData.TruckValues.CurrentValues.DashboardValues.Speed.Kph).ToString());
                        postParameters.Add("CurrentSpeedLimitKph", ((int)TelemetryController.TelemetryData.NavigationValues.SpeedLimit.Kph).ToString());
                        postParameters.Add("navigation_distance_remaining", TelemetryController.TelemetryData.NavigationValues.NavigationDistance.ToString());

                        DateTime NavTimeRemainingDT = new DateTime(TimeSpan.FromSeconds(TelemetryController.TelemetryData.NavigationValues.NavigationTime).Ticks);
                        postParameters.Add("navigation_time_remaining", NavTimeRemainingDT.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
                HTTPRequestController.Request(DataUpdate_url, HTTPRequestController.Service.VTCManager, "POST", postParameters);
                DataUpdateTimer.Start();
            });
        }

        public static bool IsServerOperational()
        {
            Dictionary<string, string> request_response_data = HTTPRequestController.Request(ServerStatus_url, HTTPRequestController.Service.VTCManager);
            request_response_data.TryGetValue("status_code", out string statuscode);
            if (statuscode != "200")
            {
                return false;
            }

            try
            {
                request_response_data.TryGetValue("response", out string response);
                Models.API.VTCManager.StatusResponse statusresponse = JsonConvert.DeserializeObject<Models.API.VTCManager.StatusResponse>(response);
                if (!statusresponse.DesktopClient.operational)
                {
                    return false;
                }
            }catch(Exception ex)
            {
                return false;
            }

            return true;
        }

        public static void NewJob()
        {
            Task.Run(() =>
            {
                LogController.Write(LogPrefix + "A new job is running. Waiting for data...", LogController.LogType.Debug);
                NewJobWaitForDataTimeOut_Elapsed = false;
                Timer WaitForDataTimeOut = new Timer(10000);
                WaitForDataTimeOut.Elapsed += NewJobWaitForDataTimeOut_ElapsedEvent;
                WaitForDataTimeOut.Start();
                while (String.IsNullOrWhiteSpace(TelemetryController.TelemetryData.JobValues.CargoValues.Id))
                {
                    if (NewJobWaitForDataTimeOut_Elapsed)
                    {
                        WaitForDataTimeOut.Stop();
                        WaitForDataTimeOut.Dispose();
                        NewJobWaitForDataTimeOut_Elapsed = false;
                        LogController.Write(LogPrefix + "Couldn't start the job. Received no data", LogController.LogType.Error);
                        return; //cancel function. show error to the user
                    }
                }
                WaitForDataTimeOut.Stop();
                WaitForDataTimeOut.Dispose();
                NewJobWaitForDataTimeOut_Elapsed = false;
                DiscordRPCController.SetPresence(DiscordRPCController.RPCStatus.TourRunning);
                LogController.Write(LogPrefix + "A new job is running. Sending data to the server...");
                Dictionary<string, string> postParameters = null;
                try {
                     postParameters = new Dictionary<string, string>
            {
                // Cargo Data
                { "cargo_id", TelemetryController.TelemetryData.JobValues.CargoValues.Id },
                { "cargo_name", TelemetryController.TelemetryData.JobValues.CargoValues.Name },
                { "cargo_mass", TelemetryController.TelemetryData.JobValues.CargoValues.Mass.ToString() },

                // Navigation Data
                { "planned_distance_km", TelemetryController.TelemetryData.JobValues.PlannedDistanceKm.ToString() },
                { "city_departure_id", TelemetryController.TelemetryData.JobValues.CitySourceId },
                { "city_departure_name", TelemetryController.TelemetryData.JobValues.CitySource },
                { "company_departure_id", TelemetryController.TelemetryData.JobValues.CompanySourceId },
                { "company_departure_name", TelemetryController.TelemetryData.JobValues.CompanySource },
                { "city_destination_id", TelemetryController.TelemetryData.JobValues.CityDestinationId },
                { "city_destination_name", TelemetryController.TelemetryData.JobValues.CityDestination },
                { "company_destination_id", TelemetryController.TelemetryData.JobValues.CompanyDestinationId },
                { "company_destination_name", TelemetryController.TelemetryData.JobValues.CompanyDestination },

                // Truck Data
                { "truck_model_id", TelemetryController.TelemetryData.TruckValues.ConstantsValues.Id },
                { "truck_model_name", TelemetryController.TelemetryData.TruckValues.ConstantsValues.Name },
                { "truck_model_manufacturer_id", TelemetryController.TelemetryData.TruckValues.ConstantsValues.BrandId },
                { "truck_model_manufacturer_name", TelemetryController.TelemetryData.TruckValues.ConstantsValues.Brand },
                { "truck_cabin_damage_at_start", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Cabin.ToString() },
                { "truck_chassis_damage_at_start", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Chassis.ToString() },
                { "truck_engine_damage_at_start", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Engine.ToString() },
                { "truck_transmission_damage_at_start", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Transmission.ToString() },
                { "truck_wheels_avg_damage_at_start", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.WheelsAvg.ToString() },

                // Trailer Data
                { "trailer_avg_damage_chassis_at_start", TelemetryController.TrailerAvgDamageChassis.ToString() },
                { "trailer_avg_damage_wheels_at_start", TelemetryController.TrailerAvgDamageWheels.ToString() },

                // Additional Data
                { "market_id", TelemetryController.TelemetryData.JobValues.Market.ToString() },
                { "special_job", TelemetryController.TelemetryData.JobValues.SpecialJob ? "1" : "0" },
                { "job_ingame_started", TelemetryController.TelemetryData.CommonValues.GameTime.Date.ToString("yyyy-MM-dd HH:mm:ss") },
                { "job_ingame_deadline", TelemetryController.TelemetryData.JobValues.DeliveryTime.Date.ToString("yyyy-MM-dd HH:mm:ss") },
                { "job_income", TelemetryController.TelemetryData.JobValues.Income.ToString() },
                { "language_code", StorageController.Config.GameLanguageCode },
            };
                }
                catch(Exception ex)
                {
                    LogController.Write(LogPrefix + "Couldn't start new job. An error occured while parsing the job data: " + ex.Message + "(Source: " + ex.Source + ")", LogController.LogType.Error);
                    MessageBox.Show("Couldn't start new job. An error occured while parsing the job data: " + ex.Message + "(Source: " + ex.Source + ")", "Error: Start new job", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Dictionary<string, string> response = HTTPRequestController.Request(CreateNewJob_url, HTTPRequestController.Service.VTCManager, "POST", postParameters);
                response.TryGetValue("status_code", out string status_code);
                LogController.Write("Request Status Code: " + status_code, LogController.LogType.Debug);
                response.TryGetValue("response", out string response_string);
                if (status_code != "200")
                {
                    LogController.Write(LogPrefix + "The server returned status code " + status_code + " and the response:\n" + response_string, LogController.LogType.Warning);
                    MessageBox.Show("The server returned status code " + status_code + " and the response:\n" + response_string, "Warning: Couldn't send job data to the server", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // ToDo: better warning handling
                    return;
                }

                uint job_id = 0;
                try
                {
                    job_id = Convert.ToUInt32(response_string);
                }
                catch (Exception ex)
                {
                    LogController.Write(LogPrefix + "Exception: " + ex.Message + "\nThe server returned status code " + status_code + " and the response:\n" + response_string, LogController.LogType.Warning);
                    MessageBox.Show("The server returned status code " + status_code + " and the response:\n" + response_string, "Warning: Couldn't convert response to uint", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if(StorageController.Config.User.Job != null)
                {
                    if (job_id == StorageController.Config.User.Job.ID && job_id != 0)
                    {
                        NotificationController.Notificate(NotificationController.NotificationType.JobStarted, "Job Continued");
                        return;
                    }
                }
                StorageController.Config.User.Job = new Models.Job
                {
                    ID = job_id
                };
                NotificationController.Notificate(NotificationController.NotificationType.JobStarted, "Job Started");
            });
        }

        private static void NewJobWaitForDataTimeOut_ElapsedEvent(object sender, ElapsedEventArgs e)
        {
            NewJobWaitForDataTimeOut_Elapsed = true;
        }

        public static void JobDelivered()
        {
            LogController.Write(LogPrefix + "The job has been delivered. Sending data to the server...");
            Dictionary<string, string> postParameters;
            try
            {
                postParameters = new Dictionary<string, string>
            {
                // Cargo and Job Data
                { "job_id", StorageController.Config.User.Job.ID.ToString() },
                { "remaining_delivery_time", TelemetryController.TelemetryData.JobValues.RemainingDeliveryTime.Date.ToString("yyyy-MM-dd HH:mm:ss") },
                { "remaining_distance", TelemetryController.TelemetryData.NavigationValues.NavigationDistance.ToString() },
                { "cargo_damage", TelemetryController.TelemetryData.JobValues.CargoValues.CargoDamage.ToString() },

                // Truck Data
                { "truck_cabin_damage_at_end", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Cabin.ToString() },
                { "truck_chassis_damage_at_end", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Chassis.ToString() },
                { "truck_engine_damage_at_end", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Engine.ToString() },
                { "truck_transmission_damage_at_end", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Transmission.ToString() },
                { "truck_wheels_avg_damage_at_end", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.WheelsAvg.ToString() },

                // Trailer Data
                { "trailer_avg_damage_chassis_at_end", TelemetryController.TrailerAvgDamageChassis.ToString() },
                { "trailer_avg_damage_wheels_at_end", TelemetryController.TrailerAvgDamageWheels.ToString() },
            };
            }
            catch(Exception ex)
            {
                LogController.Write(LogPrefix + "Couldn't finish current job. An error occured while parsing the job data: " + ex.Message + "(Source: " + ex.Source + ")", LogController.LogType.Error);
                MessageBox.Show("Couldn't finish current job. An error occured while parsing the job data: " + ex.Message + "(Source: " + ex.Source + ")", "Error: Finishing job", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Dictionary<string, string> response = HTTPRequestController.Request(Server_url + "jobs/" + StorageController.Config.User.Job.ID.ToString() + "/delivered", HTTPRequestController.Service.VTCManager, "POST", postParameters);
            response.TryGetValue("status_code", out string status_code);
            LogController.Write("Request Status Code: " + status_code, LogController.LogType.Debug);
            response.TryGetValue("response", out string response_string);
            if (status_code != "204")
            {
                LogController.Write(LogPrefix + "The server returned status code " + status_code + " and the response:\n" + response_string, LogController.LogType.Warning);
                MessageBox.Show("The server returned status code " + status_code + " and the response:\n" + response_string, "Warning: Couldn't send job data to the server", MessageBoxButton.OK, MessageBoxImage.Warning);
                // ToDo: better warning handling
            }
            StorageController.Config.User.Job.ID = 0;
            DiscordRPCController.SetPresence(DiscordRPCController.RPCStatus.FreeRoam);
        }

        public static void JobCancelled()
        {
            LogController.Write(LogPrefix + "The job has been cancelled. Sending data to the server...");

            Dictionary<string, string> postParameters = new Dictionary<string, string>
            {
                // Cargo and Job Data
                { "job_id", StorageController.Config.User.Job.ID.ToString() },
                { "remaining_delivery_time", TelemetryController.TelemetryData.JobValues.RemainingDeliveryTime.Date.ToString("yyyy-MM-dd HH:mm:ss") },
                { "remaining_distance", TelemetryController.TelemetryData.NavigationValues.NavigationDistance.ToString() },
                { "cargo_damage", TelemetryController.TelemetryData.JobValues.CargoValues.CargoDamage.ToString() },

                // Truck Data
                { "truck_cabin_damage_at_end", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Cabin.ToString() },
                { "truck_chassis_damage_at_end", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Chassis.ToString() },
                { "truck_engine_damage_at_end", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Engine.ToString() },
                { "truck_transmission_damage_at_end", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.Transmission.ToString() },
                { "truck_wheels_avg_damage_at_end", TelemetryController.TelemetryData.TruckValues.CurrentValues.DamageValues.WheelsAvg.ToString() },

                // Trailer Data
                { "trailer_avg_damage_chassis_at_end", TelemetryController.TrailerAvgDamageChassis.ToString() },
                { "trailer_avg_damage_wheels_at_end", TelemetryController.TrailerAvgDamageWheels.ToString() },
            };
            Dictionary<string, string> response = HTTPRequestController.Request(Server_url + "jobs/" + StorageController.Config.User.Job.ID.ToString() + "/cancelled", HTTPRequestController.Service.VTCManager, "POST", postParameters);
            response.TryGetValue("status_code", out string status_code);
            LogController.Write("Request Status Code: " + status_code, LogController.LogType.Debug);
            response.TryGetValue("response", out string response_string);
            if (status_code != "204")
            {
                LogController.Write(LogPrefix + "The server returned status code " + status_code + " and the response:\n" + response_string, LogController.LogType.Warning);
                MessageBox.Show("The server returned status code " + status_code + " and the response:\n" + response_string, "Warning: Couldn't send job data to the server", MessageBoxButton.OK, MessageBoxImage.Warning);
                // ToDo: better warning handling
            }
            StorageController.Config.User.Job.ID = 0;
            DiscordRPCController.SetPresence(DiscordRPCController.RPCStatus.FreeRoam);
        }
    }
}