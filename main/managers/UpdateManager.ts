const { app, autoUpdater } = require('electron')
import Log from "@awesomeeng/awesome-log";

export class UpdateManager {
    static IsUpdating = false;

    static async Init() {
        const url = `https://hazel-sage-zeta.vercel.app/update/${process.platform}/${app.getVersion()}`;

        autoUpdater.setFeedURL({ url })

        autoUpdater.on("update-available", () => {
            UpdateManager.IsUpdating = true;
            Log.info("Update is available");
        });

        /*autoUpdater.on("update-downloaded", (e: UpdateInfo) => {
            app.relaunch();
            app.quit();
        });*/

        if (process.env.NODE_ENV === 'production') {
            await autoUpdater.checkForUpdates();
        }
    }
}