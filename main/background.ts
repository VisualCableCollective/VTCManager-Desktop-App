import { app } from 'electron';
import serve from 'electron-serve';
import { createWindow } from './helpers';
import {TelemetryManager} from "./managers/TelemetryManager";
import {LogManager} from "./managers/LogManager";
import {VtcmApiClient} from "../modules/vtcm-api-client";
import {VtcmApiClientConfig} from "../modules/vtcm-api-client/VtcmApiClientConfig";
import {Environment} from "../enums/Environment";
import {BrowserWindow, ipcMain} from "electron";
import {Storage} from "./managers/StorageManager";
import {UpdateManager} from "./managers/UpdateManager";
import {AppCommands} from "./commands/AppCommands";

export var mainWindow: BrowserWindow;

const isProd: boolean = process.env.NODE_ENV === 'production';

if (isProd) {
  serve({ directory: 'app' });
} else {
  app.setPath('userData', `${app.getPath('userData')} (development)`);
}

(async () => {
  await app.whenReady();

  mainWindow = createWindow('main', {
    width: 1000,
    height: 600,
  });

  if (isProd) {
    await mainWindow.loadURL('app://./home.html');
  } else {
    const port = process.argv[2];
    await mainWindow.loadURL(`http://localhost:${port}/home`);
    mainWindow.webContents.openDevTools();
  }

  AppCommands.Init();
})();

app.on('window-all-closed', () => {
  app.quit();
});
