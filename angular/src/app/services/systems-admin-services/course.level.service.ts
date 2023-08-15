import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
import { CouseLevelDto } from '@app/models/couse-level-dto';
import { Observable } from 'rxjs';
import { ResultDto } from '@app/models/entity';
import { IResultObject } from '@app/models/common-dto';
@Injectable({
  providedIn: 'root'
})
export class CourseLevelService {

  constructor(private $http: HttpClient) { }
  public getAll() {
    return this.$http.get<ResultDto<CouseLevelDto[]>>(AppConsts.remoteServiceBaseUrl + '/api/services/app/Course/GetAllCourseLevel');
  }
  public create(course: CouseLevelDto) : Observable<ResultDto<CouseLevelDto>>{
    return this.$http.post<ResultDto<CouseLevelDto>>(AppConsts.remoteServiceBaseUrl + '/api/services/app/Course/CreateCourseLevel', course);
  }
  public update(course: CouseLevelDto) {
    return this.$http.put(AppConsts.remoteServiceBaseUrl + '/api/services/app/Course/UpdateCourseLevel', course);
  }
  public delete(id: string) {
    return this.$http.delete(AppConsts.remoteServiceBaseUrl + '/api/services/app/Course/DeleteCourseLevel', {
      params: new HttpParams().set('Id',id)
    });
  }
  public CheckDeleteCourseLevel(id: string): Observable<any> {
    return this.$http.post<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/Course/CheckDeleteCourseLevel', { 'id': id })
  }
}
