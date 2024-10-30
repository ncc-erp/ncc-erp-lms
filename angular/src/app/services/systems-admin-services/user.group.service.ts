import { ResponseResultModel, ResponseModel } from '@app/models/response.model';
import { GroupsDto, GroupIncludeUserDto, GroupStudentDto, OptionUserDto } from './../../models/groups-dto';
import { AppConsts } from './../../../shared/AppConsts';
import {
  UserGroupDto, CreateUserGroupDto,
  IGroupsToUserDto, UsersToGroupDto, ResultObject, UsersByGroupIdResult, GroupsByUserIdResult
} from './../../models/user-group-dto';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });
@Injectable({
  providedIn: 'root'
})
export class UserGroupService {
  private baseUrl: string = AppConsts.remoteServiceBaseUrl;
  constructor(private $http: HttpClient) { }
  public getAll(
    skipCount: number | null | undefined,
    maxResultCount: number | null | undefined
  ) {
    let httpParams = new HttpParams()
      .set('SkipCount', skipCount + '')
      .set('MaxResultCount', maxResultCount + '');

    return this.$http.get(
      this.baseUrl + `/api/services/app/UserGroup/GetAll`,
      {
        headers: httpOptions,
        params: httpParams
      }
    );
  }

  public GetAllAsync(
    skipCount: number | null | undefined,
    maxResultCount: number | null | undefined
  ) {
    let httpParams = new HttpParams()
      .set('SkipCount', skipCount + '')
      .set('MaxResultCount', maxResultCount + '');

    return this.$http.get(
      this.baseUrl + `/api/services/app/UserGroup/GetAllAsync`,
      {
        headers: httpOptions,
        params: httpParams
      }
    );
  }

  public getGroupsByUserIdNotPaging(userId: number): Observable<GroupsByUserIdResult> {
    return this.$http.get<GroupsByUserIdResult>(this.baseUrl + `/api/services/app/UserGroup/getGroupsByUserId`, { params: new HttpParams().set('userId', userId.toString()) });
  }

  public getUsersByGroupIdNotPaging(groupId: string): Observable<UsersByGroupIdResult> {
    return this.$http.get<UsersByGroupIdResult>(this.baseUrl + `/api/services/app/UserGroup/getUsersByGroupId`, { params: new HttpParams().set('userId', groupId) });
  }

  public delete(id: string): Observable<UserGroupDto> {
    let httpParams = new HttpParams().set('Id', id);
    return this.$http.delete<UserGroupDto>(this.baseUrl + '/api/services/app/UserGroup/Delete', {
      params: httpParams
    })
  }

  public create(obj: CreateUserGroupDto): Observable<CreateUserGroupDto> {
    return this.$http.post<CreateUserGroupDto>(this.baseUrl + '/api/services/app/UserGroup/Create', obj);
  }

  public getById(id: string): Observable<ResultObject> {
    let httpParam = new HttpParams().set('Id', id);
    return this.$http.get<ResultObject>(
      this.baseUrl + '/api/services/app/UserGroup/Get',
      {
        params: httpParam
      }
    );
  }

  public addGroupsToUser(obj: IGroupsToUserDto) {
    return this.$http.post<IGroupsToUserDto>(this.baseUrl + '/api/services/app/UserGroup/AddGroupsToUserAsync', obj);
  }

  public addUsersToGroup(obj: UsersToGroupDto) {
    return this.$http.post<UsersToGroupDto>(this.baseUrl + '/api/services/app/UserGroup/AddUsersToGroupAsync', obj);
  }

  // User group tab

  // UpdateGroup
  public UpdateGroup(group: GroupsDto): Observable<ResponseModel<boolean>> {
    return this.$http.put<ResponseModel<boolean>>(this.baseUrl + '/api/services/app/UserGroup/UpdateGroup', group);
  }

  // CreateGroup
  public CreateGroup(group: GroupsDto): Observable<ResponseResultModel<string>> {
    //console.log('group create', group);

    return this.$http.post<ResponseResultModel<string>>(this.baseUrl + '/api/services/app/UserGroup/CreateGroup', group);
  }

  // DeleteGroup
  public DeleteGroup(GroupId: string): Observable<ResponseResultModel<string>> {
    return this.$http.delete<ResponseResultModel<string>>(this.baseUrl + '/api/services/app/UserGroup/DeleteGroup', {
      params: new HttpParams().set('Id', GroupId)
    });
  }

  // CreateUserGroup
  public CreateUserGroup(userGroup: GroupStudentDto): Observable<ResponseModel<string>> {
    return this.$http.post<ResponseModel<string>>(this.baseUrl + '/api/services/app/UserGroup/CreateUserGroup', userGroup);
  }

  // DeleteUserGroup
  public DeleteUserGroup(userGroup: GroupStudentDto): Observable<ResponseModel<string>> {
    return this.$http.post<ResponseModel<string>>(this.baseUrl + '/api/services/app/UserGroup/DeleteUserGroup', userGroup);
  }

  // UpdateUserGroup
  public UpdateUserGroup(userGroup: GroupStudentDto): Observable<ResponseModel<string>> {
    return this.$http.put<ResponseModel<string>>(this.baseUrl + '/api/services/app/UserGroup/UpdateUserGroup', userGroup);
  }

  // GetsGroupIncludeUser
  public GetsGroupIncludeUser(): Observable<ResponseModel<GroupIncludeUserDto[]>> {
    return this.$http.get<ResponseModel<GroupIncludeUserDto[]>>(this.baseUrl
      + `/api/services/app/UserGroup/GetsGroupIncludeUser`);
  }

  // GetsUserAsStudent
  public GetsUserAsStudent(): Observable<ResponseModel<GroupStudentDto[]>> {
    return this.$http.get<ResponseModel<GroupStudentDto[]>>(this.baseUrl
      + `/api/services/app/UserGroup/GetsUserAsStudent`);
  }


}
