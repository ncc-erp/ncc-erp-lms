import { AppConsts } from '../../../shared/AppConsts';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserDto } from '@shared/service-proxies/service-proxies';
import { ResponseModel } from '@app/models/response.model';

const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private NAME: string = 'User';

  constructor(private http: HttpClient) { }
  
  getUsersByTenantId(tenantId, skipCount, maxResultCount, searchText): Observable<ResponseModel<UserDto[]>> {
    const httpParam = new HttpParams()
      .set('tenantId', tenantId).set('skipCount', skipCount).set('maxResultCount', maxResultCount).set("searchText", searchText);
    return this.http.get<ResponseModel<UserDto[]>>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/User/GetUsersByTenantIdAsync',
      { params: httpParam }
    );
  }


}
