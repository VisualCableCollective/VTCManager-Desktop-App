import { autoUpdater } from "electron-updater";

export class UpdateManager {
    static IsUpdating = false;

    static async Init() {
        const log = require("electron-log");
        log.transports.file.level = "debug";
        autoUpdater.logger = log;

        // result is null in dev environment
        let result = await autoUpdater.checkForUpdatesAndNotify(); // ToDO: investigate result of update check
        if (result) {
            console.log("got: " + result);
            this.IsUpdating = true;
        }
    }
}