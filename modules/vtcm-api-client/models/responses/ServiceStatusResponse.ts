export class ServiceStatusResponse {
    WebApp: ServiceStatus;
    DesktopClient: ServiceStatus;

    constructor(data) {
        this.WebApp = new ServiceStatus(data.WebApp.operational);
        this.DesktopClient = new ServiceStatus(data.DesktopClient.operational);
    }
}

export class ServiceStatus {
    operational: boolean;

    constructor(operational) {
        this.operational = operational;
    }
}