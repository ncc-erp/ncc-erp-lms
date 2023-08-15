import { ResponseModel } from '@app/models/response.model';
import { OptionUserDto } from './../../models/groups-dto';
import { AppConsts } from './../../../shared/AppConsts';

import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ReportInput, UserGroupLoginDto } from '@app/models/report-dto';
const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private baseUrl: string = AppConsts.remoteServiceBaseUrl;
  constructor(private $http: HttpClient) { }

  // GetsUserAsStudent
  public GetsUserOption(): Observable<ResponseModel<OptionUserDto[]>> {
    return this.$http.get<ResponseModel<OptionUserDto[]>>(this.baseUrl
      + `/api/services/app/Report/GetsUserOption`);
  }


  // GetsGroupUserLoginInfo
  public GetsGroupUserLoginInfo(input: ReportInput): Observable<ResponseModel<UserGroupLoginDto[]>> {
    const skipCount = ((input.CurrentPage - 1) * input.MaxResultCount);
    return this.$http.post<ResponseModel<UserGroupLoginDto[]>>(this.baseUrl
      + `/api/services/app/Report/GetsGroupUserLoginInfo`,
      {
        fromDate: input.FromDate, ToDate: input.ToDate, userId: input.UserId, sortDirection: input.SortDirection,
        searchText: input.SearchText, skipCount: skipCount, maxResultCount: input.MaxResultCount, sort: 'UserName'
      });
  }

  // GetsGroupUserActivitiesInfo
  public GetsGroupUserActivitiesInfo(input: ReportInput): Observable<ResponseModel<UserGroupLoginDto[]>> {
    const skipCount = ((input.CurrentPage - 1) * input.MaxResultCount);
    return this.$http.post<ResponseModel<UserGroupLoginDto[]>>(this.baseUrl
      + `/api/services/app/Report/GetsGroupUserActivitiesInfo`,
      {
        fromDate: input.FromDate, ToDate: input.ToDate, userId: input.UserId, sortDirection: input.SortDirection,
        searchText: input.SearchText, skipCount: skipCount, maxResultCount: input.MaxResultCount, sort: 'UserName'
      });
  }

  // GetsGroupStudentStatisticsInfo
  public GetsGroupStudentStatisticsInfo(input: ReportInput): Observable<ResponseModel<UserGroupLoginDto[]>> {
    const skipCount = ((input.CurrentPage - 1) * input.MaxResultCount);
    return this.$http.post<ResponseModel<UserGroupLoginDto[]>>(this.baseUrl
      + `/api/services/app/Report/GetsGroupStudentStatisticsInfo`,
      {
        fromDate: input.FromDate, ToDate: input.ToDate, userId: input.UserId, sortDirection: input.SortDirection,
        searchText: input.SearchText, skipCount: skipCount, maxResultCount: input.MaxResultCount, sort: 'UserName'
      });
  }

  // GetsGroupInstructorStatisticsInfo
  public GetsGroupInstructorStatisticsInfo(input: ReportInput): Observable<ResponseModel<UserGroupLoginDto[]>> {
    const skipCount = ((input.CurrentPage - 1) * input.MaxResultCount);
    return this.$http.post<ResponseModel<UserGroupLoginDto[]>>(this.baseUrl
      + `/api/services/app/Report/GetsGroupInstructorStatisticsInfo`,
      {
        fromDate: input.FromDate, ToDate: input.ToDate, userId: input.UserId, sortDirection: input.SortDirection,
        searchText: input.SearchText, skipCount: skipCount, maxResultCount: input.MaxResultCount, sort: 'UserName'
      });
  }

  // GetsGroupCourseStatisticsInfo
  public GetsGroupCourseStatisticsInfo(input: ReportInput): Observable<ResponseModel<UserGroupLoginDto[]>> {
    const skipCount = ((input.CurrentPage - 1) * input.MaxResultCount);
    return this.$http.post<ResponseModel<UserGroupLoginDto[]>>(this.baseUrl
      + `/api/services/app/Report/GetsGroupCourseStatisticsInfo`,
      {
        fromDate: input.FromDate, ToDate: input.ToDate, userId: input.UserId, sortDirection: input.SortDirection,
        searchText: input.SearchText, skipCount: skipCount, maxResultCount: input.MaxResultCount, sort: 'UserName'
      });
  }

  // GetsGroupCourseImportExportInfo
  public GetsGroupCourseImportExportInfo(input: ReportInput): Observable<ResponseModel<UserGroupLoginDto[]>> {
    const skipCount = ((input.CurrentPage - 1) * input.MaxResultCount);
    return this.$http.post<ResponseModel<UserGroupLoginDto[]>>(this.baseUrl
      + `/api/services/app/Report/GetsGroupCourseImportExportInfo`,
      {
        fromDate: input.FromDate, ToDate: input.ToDate, userId: input.UserId, sortDirection: input.SortDirection,
        searchText: input.SearchText, skipCount: skipCount, maxResultCount: input.MaxResultCount, sort: 'UserName'
      });
  }

  // CreateUserLogoutInfo
  public CreateUserLogoutInfo(): Observable<ResponseModel<boolean>> {
    return this.$http.post<ResponseModel<boolean>>(this.baseUrl
      + `/api/services/app/TrackingLog/CreateUserLogoutInfo`, {});
  }

  // CreateExportOfReportAuditLog
  public CreateExportOfReportAuditLog(actionName: string): Observable<ResponseModel<boolean>> {
    return this.$http.post<ResponseModel<boolean>>(this.baseUrl
      + `/api/services/app/Report/CreateExportOfReportAuditLog`, { actionName: actionName });
  }
}
