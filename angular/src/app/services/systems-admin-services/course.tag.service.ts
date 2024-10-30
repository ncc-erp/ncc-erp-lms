import { AppConsts } from '../../../shared/AppConsts';
import { IResult } from '../../models/common-dto';
import { ITagsToCourseDto} from 'app/models/course-tag-dto';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
const httpOptions = new HttpHeaders( { 'Content-Type': 'application/json' } );
@Injectable({
  providedIn: 'root'
})
export class CourseTagService {
  constructor(private $http: HttpClient) {} 

   public getTagsByCourseInstanceId (courseInstanceId: string) : Observable<IResult> {  
      return this.$http.get<IResult>(AppConsts.remoteServiceBaseUrl + `/api/services/app/CourseCategory/GetTagsByCourseInstanceId`, {params: new HttpParams().set('courseInstanceId', courseInstanceId)});
  }

  public addTagsToCourse( obj: ITagsToCourseDto) {    
    return this.$http.post<ITagsToCourseDto> (AppConsts.remoteServiceBaseUrl + '/api/services/app/CourseCategory/AddTagsToCourse', obj);
  }

}
