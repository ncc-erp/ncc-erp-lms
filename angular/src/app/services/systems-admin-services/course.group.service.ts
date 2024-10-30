import { AppConsts } from '../../../shared/AppConsts';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { StudentCourseGroupDto, CourseGroupDto } from '@app/models/course-group-dto';
import { IResultObject, ListResultDto } from '@app/models/common-dto';

const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });

@Injectable({
  providedIn: 'root'
})

export class CourseGroupService {
  constructor(private $http: HttpClient) { }

  public getUnAssignedStudents(courseInstanceId: string): Observable<ListResultDto> {
    return this.$http.get<ListResultDto>(AppConsts.remoteServiceBaseUrl + `/api/services/app/CourseGroup/GetUnAssignedStudents`, { params: new HttpParams().set('courseInstanceId', courseInstanceId) });
  }

  public getAllCourseGroupByCourse(courseInstanceId: string): Observable<ListResultDto> {
    return this.$http.get<ListResultDto>(AppConsts.remoteServiceBaseUrl + `/api/services/app/CourseGroup/GetAllCourseGroupByCourse`, { params: new HttpParams().set('courseInstanceId', courseInstanceId) });
  }

  public saveStudentCourseGroups(obj: StudentCourseGroupDto[]) {
    return this.$http.post<StudentCourseGroupDto[]>(AppConsts.remoteServiceBaseUrl + '/api/services/app/CourseGroup/SaveStudentCourseGroups', obj);
  }

   public update ( item: any ) : Observable<any>{
    return this.$http.put<any>( AppConsts.remoteServiceBaseUrl + '/api/services/app/CourseGroup/Update', item);
  }

  public create ( item: any ): Observable<any> {
    return this.$http.post<any>( AppConsts.remoteServiceBaseUrl + '/api/services/app/CourseGroup/Create', item );
  }

  public delete ( id: string ) : Observable<any>{    
    return this.$http.delete<any>( AppConsts.remoteServiceBaseUrl + '/api/services/app/CourseGroup/Delete', {
      params : new HttpParams().set( 'Id', id )
    })
  }
  public getAllCourseGroupByCourseId(courseInstanceId: string): Observable<IResultObject> {
    return this.$http.get<IResultObject>(AppConsts.remoteServiceBaseUrl + `/api/services/app/CourseGroup/GetAllCourseGroupByCourse`,
     { params: new HttpParams().set('courseInstanceId', courseInstanceId) });
  }
    public getCourseGroupsByCourseId(courseInstanceId: string): Observable<IResultObject> {
    return this.$http.get<IResultObject>(AppConsts.remoteServiceBaseUrl + `/api/services/app/CourseGroup/GetCourseGroupsByCourseId`,
     { params: new HttpParams().set('courseInstanceId', courseInstanceId) });
  }
   public getAllCourseGroupByQuizId(quizId: string): Observable<IResultObject> {
    return this.$http.get<IResultObject>(AppConsts.remoteServiceBaseUrl + `/api/services/app/CourseGroup/GetAllCourseGroupByQuiz`,
     { params: new HttpParams().set('quizId', quizId) });
  }
   public getAllCourseGroupByAssignmentId(assignmentId: string): Observable<IResultObject> {
    return this.$http.get<IResultObject>(AppConsts.remoteServiceBaseUrl + `/api/services/app/CourseGroup/GetAllCourseGroupByAssignment`,
     { params: new HttpParams().set('assignmentId', assignmentId) });
  }
}
