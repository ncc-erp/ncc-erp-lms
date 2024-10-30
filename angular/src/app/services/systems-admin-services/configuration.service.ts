import { AppConsts } from '../../../shared/AppConsts';
import { IResultObject } from '../../models/common-dto';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IStorageLocationDto } from '@app/models/configuration-dto';
const httpOptions = new HttpHeaders( { 'Content-Type': 'application/json' } );
@Injectable({
  providedIn: 'root'
})
export class ConfigurationService {
  constructor(private $http: HttpClient) {} 

   public getConfiguration () : Observable<IResultObject> {  
      return this.$http.get<IResultObject>(AppConsts.remoteServiceBaseUrl + `/api/services/app/Configuration/GetConfig`);
  }

  public changeDirectionLocation( obj: IStorageLocationDto) {    
    return this.$http.post<IStorageLocationDto> (AppConsts.remoteServiceBaseUrl + '/api/services/app/Configuration/ChangeDirectionLocation', obj);
  }

}
