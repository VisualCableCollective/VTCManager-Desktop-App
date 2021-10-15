import Telemetry, {TelemetryData} from "trucksim-telemetry";
import Log from "@awesomeeng/awesome-log";
import {BrowserWindow} from "electron";

export class TelemetryManager {
    static telemetry;
    static readonly UpdateInterval = 50;
    static appWindow : BrowserWindow;

    static Init(mainWindow: BrowserWindow) {
        this.appWindow = mainWindow;
        this.telemetry = Telemetry();
        this.telemetry.watch({interval: this.UpdateInterval}, this.UpdateReceived);
        Log.info("[Telemetry] Listening to telemetry updates with an interval of " + this.UpdateInterval);
    }

    static UpdateReceived(data: TelemetryData) {
        Log.debug("[Telemetry] Sent data!");
        this.appWindow.webContents.send("telemetry", data);
    }
}