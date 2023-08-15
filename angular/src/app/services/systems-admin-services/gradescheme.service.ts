import { AppConsts } from '../../../shared/AppConsts';
import { CreateGradeSchemeDto as CreateDto, GradeSchemeDto as EditDto } from '../../models/gradescheme-dto';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IResultObject } from '@app/models/common-dto';
import { IResultDto } from '@app/models/gradescheme-dto';
import { PagedRequestDto, PagedResultResultDto } from '@shared/paged-listing-component-base';

const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });

@Injectable({
  providedIn: 'root'
})
export class GradeSchemeService {
  private NAME = 'GradeScheme';

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
  public getGradesByCourseId(courseId: string): Observable<IResultDto> {
    const httpParam = new HttpParams().set('courseId', courseId);
    return this.$http.get<IResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetGradesByCourseId',
      {
        params: httpParam
      });
  }
  public getGradesByCourseIdPagging(request: PagedRequestDto, courseId: string): Observable<PagedResultResultDto> {
    return this.$http.post<PagedResultResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/getGradesByCourseIdPagging', { input: request, courseId: courseId });
  }

}
