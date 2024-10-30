import { AppConsts } from '../../../shared/AppConsts';
import { CreateAttachmentDto , AttachmentDto as EditDto } from '../../models/attachment-dto';
import { HttpClient, HttpHeaders, HttpParams, HttpEvent, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IResultObject } from '@app/models/common-dto';

const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });

@Injectable({
  providedIn: 'root'
})
export class StudentFileService {
  private NAME = 'StudentAssignmentFile';

  constructor(private $http: HttpClient) { }



  public delete(id: string): Observable<EditDto> {
    return this.$http.delete<EditDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Delete', {
      params: new HttpParams().set('Id', id)
    })
  }

  public update(item: EditDto): Observable<EditDto> {
    return this.$http.put<EditDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Update', item);
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
  public GetStudentAssignmentFiles(id: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('assignmentSettingId', id);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetStudentAssignmentFiles',
      {
        params: httpParam
      }
    );
  }

  public getFileByAssignmentSettingIdAndStudentId(assignmentSettingId: string, courseAssignedStudentId: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('assignmentSettingId', assignmentSettingId).set("courseAssignedStudentId", courseAssignedStudentId);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/getFileByAssignmentSettingIdAndStudentId',
      {
        params: httpParam
      }
    );
  }

}
