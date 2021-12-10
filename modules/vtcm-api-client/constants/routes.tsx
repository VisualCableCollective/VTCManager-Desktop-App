import {APIRoute, RequestMethod} from "../models/ApiRoute";

export const ROUTE_ID_REPLACE_PLACEHOLDER: string = '{id}';

export const SERVICE_STATUS_ROUTE: APIRoute = {
    route: 'status', method: RequestMethod.GET, requiresAuth: false, requiresID: false, requiresParentRoute: false, disableApiUrlPrefix: false,
};

export const STORE_JOB_ROUTE: APIRoute = {
    route: 'jobs', method: RequestMethod.POST, requiresAuth: true, requiresID: false, requiresParentRoute: false, disableApiUrlPrefix: false,
};

export const JOB_DELIVERED_ROUTE: APIRoute = {
    route: 'jobs/{id}/delivered', method: RequestMethod.POST, requiresAuth: true, requiresID: true, requiresParentRoute: false, disableApiUrlPrefix: false,
};

export const GET_SELF_USER_ROUTE: APIRoute = {
    route: 'user', method: RequestMethod.GET, requiresAuth: true, requiresID: false, requiresParentRoute: false, disableApiUrlPrefix: false,
};

