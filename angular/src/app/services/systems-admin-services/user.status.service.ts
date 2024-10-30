import { AppConsts } from './../../../shared/AppConsts';
import { UserStatusDto, PagedResultDto, Result, ResultObject } from './../../models/user-status-dto';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ResultDto } from '@app/models/entity';
const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });
@Injectable({
  providedIn: 'root'
})
export class UserStatusService {
  constructor(private $http: HttpClient) { }
  public getAll(
    skipCount: number | null | undefined,
    maxResultCount: number | null | undefined
  ) {
    let httpParams = new HttpParams()
      .set('SkipCount', skipCount + '')
      .set('MaxResultCount', maxResultCount + '');

    return this.$http.get(
      AppConsts.remoteServiceBaseUrl + `/api/services/app/UserStatus/GetAll`,
      {
        headers: httpOptions,
        params: httpParams
      }
    );
  }

  public getAllNotPagging(): Observable<Result> {
    return this.$http.get<Result>(AppConsts.remoteServiceBaseUrl + `/api/services/app/UserStatus/GetAllNotPagging`);
  }

  public delete(id: string): Observable<UserStatusDto> {
    let httpParams = new HttpParams().set('Id', id);
    return this.$http.delete<UserStatusDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/UserStatus/Delete', {
      params: httpParams
    })
  }

  public CheckDelete(id: string): Observable<any> {
    return this.$http.post<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/UserStatus/CheckDelete', { 'id': id })
  }

  public update(obj: UserStatusDto): Observable<UserStatusDto> {
    return this.$http.put<UserStatusDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/UserStatus/Update', obj);
  }

  public create(obj: UserStatusDto): Observable<ResultDto<UserStatusDto>> {
    return this.$http.post<ResultDto<UserStatusDto>>(AppConsts.remoteServiceBaseUrl + '/api/services/app/UserStatus/Create', obj);
  }

  public getById(id: string): Observable<ResultObject> {
    let httpParam = new HttpParams().set('Id', id);
    return this.$http.get<ResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/UserStatus/Get',
      {
        params: httpParam
      }
    );
  }
}
