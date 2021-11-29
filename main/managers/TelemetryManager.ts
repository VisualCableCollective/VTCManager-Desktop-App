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
        let active = true;
        if (data === null) {
            active = false;
        }

        TelemetryManager.appWindow.webContents.send("telemetry", {
            active,
            data
        });
    }
}