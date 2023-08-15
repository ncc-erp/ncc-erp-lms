import { AppConsts } from './../../../shared/AppConsts';
import { IResultObject } from './../../models/common-dto';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
const httpOptions = new HttpHeaders( { 'Content-Type': 'application/json' } );
@Injectable({
  providedIn: 'root'
})
export class CourseSettingService {
  constructor(private $http: HttpClient) {} 
  
  public getById ( courseInstanceId: string ): Observable<IResultObject> {
    let httpParam = new HttpParams().set( 'courseInstanceId', courseInstanceId );
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/CourseInstance/GetById',
      {
        params: httpParam
      }
    );
  }

  

  public save ( item: any ) : Observable<any>{
    return this.$http.post<any>( AppConsts.remoteServiceBaseUrl + '/api/services/app/CourseInstance/Save', item);
  }

}
