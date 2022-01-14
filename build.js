var electronInstaller = require('electron-winstaller');
const fs = require('fs');

fs.copyFileSync("./LICENSE", "./dist/win-unpacked/LICENSE");

resultPromise = electronInstaller.createWindowsInstaller({
    appDirectory: './dist/win-unpacked',
    outputDirectory: './dist/installer',
    authors: 'The VisualCable Collective',
    exe: 'VTCManager.exe'
});

resultPromise.then(() => console.log("It worked!"), (e) => console.log(`No dice: ${e.message}`));