import { AppConsts } from '../../../shared/AppConsts';
import { GradeSchemeElementDto as CreateDto, GradeSchemeElementDto as EditDto, IResultDto } from '../../models/gradescheme-dto';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IResultObject } from '@app/models/common-dto';
import { PagedRequestDto, PagedResultResultDto } from '@shared/paged-listing-component-base';

const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });

@Injectable({
  providedIn: 'root'
})
export class GradeSchemeElementService {
  private NAME = 'GradeSchemeElement';

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
  public getElementsByGradeId(gradeId: string): Observable<IResultDto> {
    const httpParam = new HttpParams().set('gradeId', gradeId);
    return this.$http.get<IResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetElementsByGradeId',
      {
        params: httpParam
      });
  }
  public getElementByGradeIdPagging(request: PagedRequestDto, gradeId: string): Observable<PagedResultResultDto> {
    return this.$http.post<PagedResultResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetElementByGradeIdPagging', { input: request, gradeId: gradeId });
  }

}
