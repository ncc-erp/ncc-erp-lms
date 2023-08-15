import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { AbpSessionService } from '@abp/session/abp-session.service';
import { throwError } from 'rxjs';
import { IResultObject } from '@app/models/common-dto';
import { AppConsts } from '@shared/AppConsts';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {
  protected httpClient: HttpClient;

  constructor(private abpsession: AbpSessionService,  _httpClient:HttpClient) {
    this.httpClient = _httpClient
  }
  public handleError(error: any, api:string, payload?:any) {
  let userId = this.abpsession.userId;

    let errMessage = {
      userId: userId,
      message: error,
      api: api,
      payload: payload ? JSON.stringify(payload) : 'no payload'
    } as ErrorMessageDto
    if(typeof window['postSentryLog'] == 'function'){
      window['postSentryLog']("error when call api", errMessage)
    }
    return throwError(error);
}
}

export interface ErrorMessageDto{
  userId:any;
  api: string;
  payload: any;
  message: string;
}
