import { AppConsts } from '../../../shared/AppConsts';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IResultObject } from '@app/models/common-dto';

@Injectable({
  providedIn: 'root'
})
export class ScormService {
  private NAME = 'Scorm';

  constructor(private $http: HttpClient) { }

  public CreateStudentScorm(item: any): Observable<IResultObject> {
    return this.$http.post<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/CreateStudentScorm', item);
  }

  public UpdateStudentScorm(item: any): Observable<IResultObject> {
    return this.$http.put<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/UpdateStudentScorm', item);
  }

  public GetStudentScorms(courseAssignedStudentId: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('courseAssignedStudentId', courseAssignedStudentId);
    return this.$http.get<IResultObject>
    (AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetStudentScorms', { params: httpParam })
  }

  public GetCourseNavigation(courseAssignedStudentId: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('courseAssignedStudentId', courseAssignedStudentId);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetCourseNavigation',
      {
        params: httpParam
      }
    );
  }
  public GetScormStatistics(courseInstanceId: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('courseInstanceId', courseInstanceId);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetScormStatistics',
      {
        params: httpParam
      }
    );
  }
  public GetStudentQuizProgress(courseInstanceId: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('courseInstanceId', courseInstanceId);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetStudentQuizProgress',
      {
        params: httpParam
      }
    );
  }
  public CreateStudentProgressScorm(item: any): Observable<IResultObject> {
    return this.$http.post<IResultObject>
    (AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/CreateStudentProgressScorm', item);
  }

  public CreateScormTestAttempt(item: any): Observable<IResultObject> {
    return this.$http.post<IResultObject>
    (AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/CreateScormTestAttempt', item);
  }

  public CompletedCourseScorm12(courseAssignedStudentId: any): Observable<IResultObject> {
    const item = {id: courseAssignedStudentId};
    return this.$http.post<IResultObject>
    (AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/CompletedCourseScorm12', item);
  }

}

