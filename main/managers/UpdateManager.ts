import { autoUpdater } from "electron-updater";
import { app } from "electron";

export class UpdateManager {
    static IsUpdating = false;

    static async Init() {
        const log = require("electron-log");
        log.transports.file.level = "debug";
        autoUpdater.logger = log;

        // redirect to custom server
        const server = "https://hazel-sage-zeta.vercel.app";
        const url = `${server}/update/${process.platform}/${app.getVersion()}`

        autoUpdater.setFeedURL({
            acl: undefined,
            channel: undefined,
            channels: undefined,
            component: undefined,
            distribution: undefined,
            encryption: undefined,
            endpoint: undefined,
            host: undefined,
            package: undefined,
            path: undefined,
            platform: undefined,
            private: undefined,
            protocol: undefined,
            provider: undefined,
            publishAutoUpdate: false,
            publisherName: undefined,
            releaseType: undefined,
            requestHeaders: undefined,
            storageClass: undefined,
            updateProvider: undefined,
            updaterCacheDirName: undefined,
            useMultipleRangeRequest: false,
            user: undefined,
            vPrefixedTagName: false,
            url: url })

        // result is null in dev environment
        let result = await autoUpdater.checkForUpdatesAndNotify(); // ToDO: investigate result of update check
        if (result) {
            console.log("got: " + result);
            this.IsUpdating = true;
        }
    }
}