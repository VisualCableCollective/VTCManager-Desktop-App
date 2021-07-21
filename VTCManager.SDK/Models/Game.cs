using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCSSdkClient.Object;

namespace VTCManager.SDK.Models
{
    /// <summary>
    /// Represents an instance of Euro Truck Simulator 2 or American Truck Simulator.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// The current version of the running game.
        /// </summary>
        public Version GameVersion { get; }

        /// <summary>
        /// The current type of game that is currently running.
        /// </summary>
        public GameType Type { get; }

        /// <summary>
        /// The current date and time in the game.
        /// </summary>
        public DateTime InGameTime { get; }

        public Game(SCSTelemetry telemetryData)
        {
            GameVersion = new Version((int)telemetryData.GameVersion.Major, (int)telemetryData.GameVersion.Minor);

            switch (telemetryData.Game)
            {
                case SCSSdkClient.SCSGame.Ats:
                    Type = GameType.ATS;
                    break;
                case SCSSdkClient.SCSGame.Ets2:
                    Type = GameType.ETS2;
                    break;
                case SCSSdkClient.SCSGame.Unknown:
                    Type = GameType.None;
                    break;
            }

            InGameTime = telemetryData.CommonValues.GameTime.Date;
        }
    }
}
