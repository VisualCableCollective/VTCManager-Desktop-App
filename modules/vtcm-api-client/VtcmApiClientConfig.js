import {Environment} from "../../enums/Environment";

export class VtcmApiClientConfig {
    BearerToken = '';

    OAuthClientId = 0;

    OAuthClientSecret = '';

    Environment = Environment.Production;

    ServerUrl = "https://alpha-api.vtcmanager.eu/api/";

    LogPrefix = "[VtcmAPIClient] ";

    Debug = true;

    constructor(environment = Environment.Production, customServerUrl = "http://localhost:8000/api/") {
        if (environment === Environment.Development) {
            this.ServerUrl = customServerUrl;
        }
    }
}