import {APIRoute, RequestMethod} from "../models/ApiRoute";

export const ROUTE_ID_REPLACE_PLACEHOLDER: string = '{id}';

export const SERVICE_STATUS_ROUTE: APIRoute = {
    route: 'status', method: RequestMethod.GET, requiresAuth: false, requiresID: false, requiresParentRoute: false, disableApiUrlPrefix: false,
};

