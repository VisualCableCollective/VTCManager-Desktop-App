import {app, ipcMain, dialog} from "electron";
import {LogManager} from "../managers/LogManager";
import {UpdateManager} from "../managers/UpdateManager";
import {TelemetryManager} from "../managers/TelemetryManager";
import {VtcmApiClientConfig} from "../../modules/vtcm-api-client/VtcmApiClientConfig";
import {Environment} from "../../enums/Environment";
import {mainWindow} from "../background";
import {VtcmApiClient} from "../../modules/vtcm-api-client";
import {createWindow} from "../helpers";
import { IpcMainEvent } from "electron/main";

export class AppCommands {
    static Init() {
        // set up ipc events
        ipcMain.on("init-app", this.InitApp);
        ipcMain.on("quit-app", this.QuitApp);
    }

    private static async InitApp(event: IpcMainEvent, args) {
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

        mainWindow.webContents.send("loading-status-update", {message: "Logging in..."});

        if (!VtcmApiClient.Config.BearerToken) {
            await AppCommands.openLoginPopup();
            return;
        }

        event.reply("init-finished");
    }

    private static async QuitApp(event: IpcMainEvent, args) {
        app.quit();
    }

    private static async openLoginPopup() {

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

                    loginWindow.destroy();

                    if (pageContent.error === "USER_NOT_REGISTERED") {
                        dialog.showMessageBoxSync({
                            message: "You have to create an account online before signing in!",
                            title: "Login Error"
                        });
                    }

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