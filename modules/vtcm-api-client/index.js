import {VtcmApiClientConfig} from "./VtcmApiClientConfig";
import {SERVICE_STATUS_ROUTE} from "./constants/routes";
import {HttpRequestUtil} from "./utils/HttpRequestUtil";
import {ServiceStatusResponse} from "./models/responses/ServiceStatusResponse";
import {Storage} from "../../main/managers/StorageManager";

export class VtcmApiClient {
    static Config;

    static Init(config = new VtcmApiClientConfig()) {
        this.Config = config;
        this.Config.BearerToken = Storage.get("UserBearerToken");
        HttpRequestUtil.Config = this.Config;
    }

    static async GetServiceStatus() {
        const response = await HttpRequestUtil.Request(SERVICE_STATUS_ROUTE);
        return new ServiceStatusResponse(await response.json());
    }

    static SetBearerToken(token) {
        Storage.set("UserBearerToken", token);
        this.Config.BearerToken = token;
        HttpRequestUtil.Config = this.Config;
    }
}