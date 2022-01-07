import Log from "@awesomeeng/awesome-log";

export class LogManager{
    static Init() {
        Log.init();
        Log.start();
    }
}