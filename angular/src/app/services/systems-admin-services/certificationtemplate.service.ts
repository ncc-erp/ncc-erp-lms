import { AppConsts } from '../../../shared/AppConsts';
import { CreateCertificationTemplateDto as CreateDto, CertificationTemplateDto as EditDto } from '../../models/certificationtemplate-dto';
import { HttpClient, HttpHeaders, HttpParams, HttpEvent, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IResultObject } from '@app/models/common-dto';
import { IResultDto } from '@app/models/certificationtemplate-dto';
import { PagedRequestDto, PagedResultResultDto } from '@shared/paged-listing-component-base';

const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });

@Injectable({
  providedIn: 'root'
})
export class CertificationTemplateService {
  private NAME = 'CertificationTemplate';

  constructor(private $http: HttpClient) { }



  public delete(id: string): Observable<EditDto> {
    return this.$http.delete<EditDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Delete', {
      params: new HttpParams().set('Id', id)
    })
  }

  public update(item: EditDto): Observable<EditDto> {
    return this.$http.put<EditDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Update', item);
  }

  // public create(item: CreateDto): Observable<EditDto> {
  //   return this.$http.post<EditDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Create', item);
  // }


  public getById(id: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('Id', id);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Get',
      {
        params: httpParam
      }
    );
  }
  public createFormData(item: any): Observable<HttpEvent<any>> {
    const formData = new FormData();
    const propertyNames = Object.getOwnPropertyNames(item);
    propertyNames.forEach(element => {
      if (item[element] != null) {
        formData.append(element, item[element]);
      }
    });
    let action = '/Create';
    let request = 'POST';
    if (item.id) {
      action = '/Update';
      request = 'PUT';
    }
    const uploadReq = new HttpRequest(request,
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + action,
      formData,
      {
        reportProgress: true
      }
    );
    return this.$http.request(uploadReq);
  }
  public getByCourseId(id: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('CourseId', id);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/getByCourseId',
      {
        params: httpParam
      }
    );
  }

  public getImage(url: string): Observable<Blob> {
    return this.$http.get(`${url}`, { responseType: 'blob' })
  }
}
