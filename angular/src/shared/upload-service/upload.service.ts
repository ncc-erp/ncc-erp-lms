import { Router } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';
import { Injectable} from '@angular/core';
import { Observable } from 'rxjs';
import { HttpRequest, HttpClient, HttpEventType } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};
@Injectable({
  providedIn: 'root'
})
export class UploadService {
  // imageSrc: string;
  // public progress: number;
  // public message: string;
  
  constructor(private http: HttpClient, private router: Router) {}

  public upload(url, paramters: Map<string, string>, file: File) : Observable<any> {
    const formData = new FormData();
    paramters.forEach((key: string, value: string) => {
      formData.append(value, key);
    });
    formData.append('file', file);
    const uploadReq = new HttpRequest(
      'POST', url, formData,
      {
        reportProgress: true
      }
    );
    return this.http.request(uploadReq);
  }

  public uploadImageFile(url,file: File) : Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    const uploadReq = new HttpRequest(
      'POST',AppConsts.remoteServiceBaseUrl + url, formData,
      {
        reportProgress: true
      }
    );
    return this.http.request(uploadReq);
  }
}
