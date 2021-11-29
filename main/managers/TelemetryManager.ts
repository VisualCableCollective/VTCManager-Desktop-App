import Telemetry, {TelemetryData} from "trucksim-telemetry";
import Log from "@awesomeeng/awesome-log";
import {BrowserWindow} from "electron";

export class TelemetryManager {
    static telemetry;
    static readonly UpdateInterval = 50;
    static appWindow : BrowserWindow;

    static Init(mainWindow: BrowserWindow) {
        TelemetryManager.appWindow = mainWindow;
        TelemetryManager.telemetry = Telemetry();
        TelemetryManager.UpdateReceived(null);
        TelemetryManager.telemetry.watch({interval: TelemetryManager.UpdateInterval}, TelemetryManager.UpdateReceived);
        Log.info("[Telemetry] Listening to telemetry updates with an interval of " + TelemetryManager.UpdateInterval);
    }

    static UpdateReceived(data: TelemetryData) {
        if (data === null) {
            return;
        }

        if (data.truck) {
            if (data.truck.model) {
                Log.debug("[Telemetry] Sent data!" + data.truck.model.name);
            }
        }
        TelemetryManager.appWindow.webContents.send("telemetry", data);
    }
}