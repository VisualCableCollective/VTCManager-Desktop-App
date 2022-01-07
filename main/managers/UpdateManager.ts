import { NsisUpdater  } from "electron-updater";

export class UpdateManager {
    static IsUpdating = false;

    static async Init() {
        const appUpdater = new NsisUpdater({
            provider: 'generic',
            url: 'https://hazel-sage-zeta.vercel.app'
        });

        const log = require("electron-log");
        log.transports.file.level = "debug";
        appUpdater.logger = log;

        // result is null in dev environment
        let result = await appUpdater.checkForUpdatesAndNotify(); // ToDO: investigate result of update check
        if (result) {
            console.log("got: " + result);
            this.IsUpdating = true;
        }
    }
}