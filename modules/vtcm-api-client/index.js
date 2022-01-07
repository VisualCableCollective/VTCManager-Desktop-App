import {VtcmApiClientConfig} from "./VtcmApiClientConfig";
import {GET_SELF_USER_ROUTE, JOB_DELIVERED_ROUTE, SERVICE_STATUS_ROUTE, STORE_JOB_ROUTE} from "./constants/routes";
import {HttpRequestUtil} from "./utils/HttpRequestUtil";
import {ServiceStatusResponse} from "./models/responses/ServiceStatusResponse";
import {Storage} from "../../main/managers/StorageManager";
import {StoreJobRequest} from "./models/requests/StoreJobRequest";

export const CHECK_AUTH_RESULT = Object.freeze({"SUCCESS":1, "UNAUTHORIZED":2, "NO_LICENSE_KEY":3})

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

    static async JobStart(data) {
        const response = await HttpRequestUtil.Request(STORE_JOB_ROUTE, data);
        console.log("JobStarted Response: " + JSON.stringify(response));

        if (response.status !== 200) {
            return {success: false};
        }

        return response.json();
    }

    static async CheckAuthentication() {
        if (!VtcmApiClient.Config.BearerToken) {
            return CHECK_AUTH_RESULT.UNAUTHORIZED;
        }

        const response = await HttpRequestUtil.Request(GET_SELF_USER_ROUTE);
        console.log("response: " + JSON.stringify(response));

        if (response.status !== 200) {
            if (response.status === 403) {
                return CHECK_AUTH_RESULT.NO_LICENSE_KEY;
            }

            return CHECK_AUTH_RESULT.UNAUTHORIZED;
        }

        return CHECK_AUTH_RESULT.SUCCESS;
    }

    static async JobDelivered(data) {
        const route = JOB_DELIVERED_ROUTE;
        route.ID = data.JobId;
        const response = await HttpRequestUtil.Request(route, data.GetPostData());
        console.log("JobDelivered Response: " + response.body);

        if (response.status !== 204) {
            return {success: false};
        }

        return {success: true};
    }

    static SetBearerToken(token) {
        Storage.set("UserBearerToken", token);
        this.Config.BearerToken = token;
        HttpRequestUtil.Config = this.Config;
    }
}