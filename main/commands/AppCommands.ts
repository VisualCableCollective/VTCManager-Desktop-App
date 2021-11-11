import {app, ipcMain} from "electron";
import {LogManager} from "../managers/LogManager";
import {UpdateManager} from "../managers/UpdateManager";
import {TelemetryManager} from "../managers/TelemetryManager";
import {VtcmApiClientConfig} from "../../modules/vtcm-api-client/VtcmApiClientConfig";
import {Environment} from "../../enums/Environment";
import {mainWindow} from "../background";
import {VtcmApiClient} from "../../modules/vtcm-api-client";
import {createWindow} from "../helpers";

export class AppCommands {
    static Init() {
        ipcMain.on("init-app", async (event, args) => {
            LogManager.Init();

            mainWindow.webContents.send("loading-status-update", {message: "Looking for updates..."});
            await UpdateManager.Init();
            if(UpdateManager.IsUpdating) {
                return;
            }

            TelemetryManager.Init(mainWindow);
            VtcmApiClient.Init(new VtcmApiClientConfig(Environment.Development));
            mainWindow.webContents.send("loading-status-update", {message: "Connecting to the VTCManager services..."});

            let response = await VtcmApiClient.GetServiceStatus();
            if (!response.DesktopClient.operational) {
                mainWindow.webContents.send("show-loading-screen-error", {errorCode: "SERVICES_NOT_AVAILABLE"});
                return;
            }

            mainWindow.webContents.send("show-login");
            mainWindow.webContents.send("loading-status-update", {message: "Logging in..."});
            if (!VtcmApiClient.Config.BearerToken) {
                await this.openLoginPopup();
            }
            event.reply("init-finished");
        });

        ipcMain.on("quit-app", async (event, args) => {
            app.quit();
        });
    }

    static async openLoginPopup() {

        let loginWindow = await createWindow("login", {show: false});

        // reset storage so we don't get instant login if user already signed in -> prompt user to reauthenticate
        await loginWindow.webContents.session.clearStorageData();

        // no close, you have to authenticate :)
        loginWindow.on("close", (e) => {
            e.preventDefault();
        })

        loginWindow.webContents.on("did-finish-load", async () => {
            loginWindow.show();

            const currentPage = loginWindow.webContents.getURL();

            if (currentPage.includes("localhost:8000/auth/vcc/desktop-client/callback")) {

                const pageContent = JSON.parse(await loginWindow.webContents.executeJavaScript('document.documentElement.innerText;'));

                if (pageContent.message !== "OK") {
                    await this.openLoginPopup();
                    return;
                }

                loginWindow.destroy();
                VtcmApiClient.SetBearerToken(pageContent.token);
            }
        });
        await loginWindow.loadURL("http://localhost:8000/auth/vcc/desktop-client/redirect");
    }
}