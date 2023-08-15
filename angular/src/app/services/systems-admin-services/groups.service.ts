import { AppConsts } from './../../../shared/AppConsts';
import { GroupsDto, PagedResultDtoOfGroupDto, Result, ResultObject } from './../../models/groups-dto';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });
@Injectable({
  providedIn: 'root'
})
export class GroupsService {
  constructor(private $http: HttpClient) { }
  public getAllGroups(
    skipCount: number | null | undefined,
    maxResultCount: number | null | undefined
  ) {
    let httpParams = new HttpParams()
      .set('SkipCount', skipCount + '')
      .set('MaxResultCount', maxResultCount + '');

    return this.$http.get(
      AppConsts.remoteServiceBaseUrl + `/api/services/app/Group/GetAll`,
      {
        headers: httpOptions,
        params: httpParams
      }
    );
  }

  public getAllNotPaging(): Observable<Result> {
    return this.$http.get<Result>(AppConsts.remoteServiceBaseUrl + `/api/services/app/Group/GetAllGroups`);
  }

  public delete(id: string): Observable<GroupsDto> {
    let httpParams = new HttpParams().set('Id', id);
    return this.$http.delete<GroupsDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/Group/Delete', {
      params: httpParams
    })
  }

  public update(group: GroupsDto): Observable<GroupsDto> {
    return this.$http.put<GroupsDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/Group/Update', group);
  }

  public create(group: GroupsDto): Observable<GroupsDto> {
    return this.$http.post<GroupsDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/Group/Create', group);
  }

  public getById(id: string): Observable<ResultObject> {
    let httpParam = new HttpParams().set('Id', id);
    return this.$http.get<ResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/Group/Get',
      {
        params: httpParam
      }
    );
  }
}
