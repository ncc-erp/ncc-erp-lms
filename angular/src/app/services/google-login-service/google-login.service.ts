import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AppConsts } from "@shared/AppConsts";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class GoogleLoginService {
    private NAME = 'TokenAuth';
    private baseUrl = AppConsts.remoteServiceBaseUrl + '/api/' + this.NAME;
    constructor(private $http: HttpClient) { }
    googleAuthenticate(googleToken: string, tenancyName: string): Observable<any> {
        return this.$http.post(`${this.baseUrl}/GoogleAuthenticate`, { googleToken: googleToken, tenancyName: tenancyName });
    }

}