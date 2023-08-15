import { AppConsts } from '../../../shared/AppConsts';
import { CreateAttachmentDto as CreateDto, AttachmentDto as EditDto } from '../../models/attachment-dto';
import { HttpClient, HttpHeaders, HttpParams, HttpEvent, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IResultObject } from '@app/models/common-dto';
import { IResultDto } from '@app/models/annoucement-dto';
import { PagedRequestDto, PagedResultResultDto } from '@shared/paged-listing-component-base';

const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });

@Injectable({
  providedIn: 'root'
})
export class AttachmentService {
  private NAME = 'Resource';

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
      formData.append(element, item[element]);
    });
    const uploadReq = new HttpRequest('POST',
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Create',
      formData,
      {
        reportProgress: true
      }
    );
    return this.$http.request(uploadReq);
  }
  public getByEntityId(id: string, type: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('EntityId', id)
    .set('EntityType', type);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/getByEntityId',
      {
        params: httpParam
      }
    );
  }
}
