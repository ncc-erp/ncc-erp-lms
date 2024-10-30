import { AppConsts } from '../../../shared/AppConsts';
import { CreateAssignmentDto as CreateDto, AssignmentDto as EditDto, IResultDto } from '../../models/assignments-dto';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IResultObject, ListResultDto } from '@app/models/common-dto';
import { PagedRequestDto } from '@shared/paged-listing-component-base';

const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });

@Injectable({
  providedIn: 'root'
})
export class AssignmentsService {
  private NAME = 'Assignment';

  constructor(private $http: HttpClient) { }



  public delete(id: string): Observable<EditDto> {
    return this.$http.delete<EditDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Delete', {
      params: new HttpParams().set('Id', id)
    })
  }

  public update(item: EditDto): Observable<EditDto> {
    return this.$http.put<EditDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Update', item);
  }

  public create(item: CreateDto): Observable<EditDto> {
    return this.$http.post<EditDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Create', item);
  }

  public getById(id: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('Id', id);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Get',
      {
        params: httpParam
      }
    );
  }

  public getByAssignmentSettingId(id: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('Id', id);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetByAssignmentSettingId',
      {
        params: httpParam
      }
    );
  }
  public GetAssignmentsByCourseId(courseId: string): Observable<IResultDto> {
    const httpParam = new HttpParams().set('courseId', courseId);
    return this.$http.get<IResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetAssignmentsByCourseId',
      {
        params: httpParam
      });
  }

  public GetSelectAssignmentsByCourseInstanceId(courseInstanceId: string): Observable<ListResultDto> {
    const httpParam = new HttpParams().set('courseInstanceId', courseInstanceId);
    return this.$http.get<ListResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetSelectAssignmentsByCourseInstanceId',
      {
        params: httpParam
      });
  }

  public GetAssignmentsByCourseIdPagging(request: PagedRequestDto, courseId: string, courseInstanceId: string): Observable<IResultDto> {
    return this.$http.post<IResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetAssignmentsByCourseIdPagging',
      { input: request, courseId: courseId, courseInstanceId: courseInstanceId });
  }

  public UpdateStudentAssignments(item: any[]): Observable<any> {
    return this.$http.put<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/UpdateStudentAssignmentScore', item);
  }
   public GetStudentAssignmentProgress(courseInstanceId: string): Observable<ListResultDto> {
    const httpParam = new HttpParams().set('courseInstanceId', courseInstanceId);
    return this.$http.get<ListResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetStudentAssignmentProgress',
      {
        params: httpParam
      });
  }
}
