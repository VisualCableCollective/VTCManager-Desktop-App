using System;

namespace VTCManager.SDK
{
    public static class VTCManagerClient
    {
        // Facades
        private static Facades.TelemetryFacade _telemetryFacade;
        private static Facades.DiscordRPCFacade _discordRPCFacade;

        // Models
        /// <summary>
        /// Contains all game-related data.
        /// </summary>
        public static Models.Game Game { get; set; }

        public static void Initalize(Models.Config config)
        {
            _telemetryFacade = new Facades.TelemetryFacade();

            if (config.EnableDiscordRPCFeature)
            {
                _discordRPCFacade = new Facades.DiscordRPCFacade();
            }
        }
    }
}
