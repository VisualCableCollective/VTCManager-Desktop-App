using SCSSdkClient;
using SCSSdkClient.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTCManager.SDK.Facades
{
    public class TelemetryFacade
    {
        // Logging
        private const string LOGPREFIX = nameof(TelemetryFacade);

        // Telemetry
        private static SCSSdkTelemetry _telemetry;

        public TelemetryFacade()
        {
            _telemetry = new SCSSdkTelemetry();

            // set up event handlers
            _telemetry.Data += TelemetryDataReceived;
        }

        #region Telemetry Events
        private void TelemetryDataReceived(SCSTelemetry data, bool newTimestamp)
        {

        }
        #endregion
    }
}
