export interface APIRoute {
    route: string,
        method: RequestMethod,
        requiresAuth: boolean,
        requiresID: boolean,
        requiresParentRoute: boolean,
        parentRoute?: APIRoute,
        ID?: string | number,
        disableApiUrlPrefix: boolean,
}

export enum RequestMethod {
    GET,
    POST,
}