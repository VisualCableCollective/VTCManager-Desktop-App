import {VtcmApiClientConfig} from "./VtcmApiClientConfig";
import {SERVICE_STATUS_ROUTE, STORE_JOB_ROUTE} from "./constants/routes";
import {HttpRequestUtil} from "./utils/HttpRequestUtil";
import {ServiceStatusResponse} from "./models/responses/ServiceStatusResponse";
import {Storage} from "../../main/managers/StorageManager";
import {StoreJobRequest} from "./models/requests/StoreJobRequest";

export class VtcmApiClient {
    static Config;

    static Init(config = new VtcmApiClientConfig()) {
        this.Config = config;
        this.Config.BearerToken = Storage.get("UserBearerToken");
        HttpRequestUtil.Config = this.Config;
    }

    static async GetServiceStatus() {
        const response = await HttpRequestUtil.Request(SERVICE_STATUS_ROUTE);

        // happens if server is completely down
        if (response == null) {
            // fake response
            const fakeData = {
                WebApp: {
                    operational: false,
                },
                DesktopClient: {
                    operational: false,
                }
            }

            return fakeData;
        }

        return new ServiceStatusResponse(await response.json());
    }

    static JobStart(data) {
        const response = HttpRequestUtil.Request(STORE_JOB_ROUTE, data);
    }

    static SetBearerToken(token) {
        Storage.set("UserBearerToken", token);
        this.Config.BearerToken = token;
        HttpRequestUtil.Config = this.Config;
    }
}