export class ResponseModel<T> {
    result: ResultItems<T>;
    targetUrl: string;
    success: boolean;
    error: string;
    unAuthorizedRequest: boolean;
    __abp: boolean;
}

export interface ResultItems<T> {
    totalCount: number;
    items: T;
}

export class ResponseResultModel<T> {
    result: T;
    targetUrl: string;
    success: boolean;
    error: string;
    unAuthorizedRequest: boolean;
    __abp: boolean;
}
