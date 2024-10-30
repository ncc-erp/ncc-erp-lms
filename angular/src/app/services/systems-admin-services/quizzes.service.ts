import { AppConsts } from '../../../shared/AppConsts';
import { CreateQuizDto as CreateDto, QuizDto as EditDto, QuizDto } from '../../models/quizzes-dto';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IResultObject } from '@app/models/common-dto';
import { IResultDto } from '@app/models/quizzes-dto';

const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });

@Injectable({
  providedIn: 'root'
})
export class QuizzesService {
  private NAME = 'Quiz';

  constructor(private $http: HttpClient) { }



  public delete(id: string): Observable<EditDto> {
    return this.$http.delete<EditDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Delete', {
      params: new HttpParams().set('Id', id)
    })
  }

  public update(item: EditDto): Observable<EditDto> {
    return this.$http.put<EditDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Update', item);
  }

  public create(item: CreateDto): Observable<EditDto> {
    return this.$http.post<EditDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Create', item);
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
  public getQuizzesByCourseId(courseId: string): Observable<IResultDto> {
    const httpParam = new HttpParams().set('courseId', courseId);
    return this.$http.get<IResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetQuizzesByCourseId',
      {
        params: httpParam
      });
  }

  public GetSelectQuizzesByCourseInstanceId(courseInstanceId: string): Observable<IResultDto> {
    const httpParam = new HttpParams().set('courseInstanceId', courseInstanceId);
    return this.$http.get<IResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetSelectQuizzesByCourseInstanceId',
      {
        params: httpParam
      });
  }

  public getQuizData(quizId: string, courseInstanceId: string): Observable<any> {
    const httpParam = new HttpParams()
      .set('quizId', quizId)
      .set('courseInstanceId', courseInstanceId);
    return this.$http.get<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetQuizDataById',
      {
        params: httpParam
      });
  }

  public GetQuizOptions(quizId: string, courseInstanceId: string): Observable<any> {
    const httpParam = new HttpParams()
      .set('quizId', quizId)
      .set('courseInstanceId', courseInstanceId);
    return this.$http.get<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetQuizOptions',
      {
        params: httpParam
      });
  }

  public GetQuizOptionsAndTestAttemps(quizSettingId: string, courseInstanceId: string, quizType: string): Observable<any> {
    const httpParam = new HttpParams()
      .set('quizSettingId', quizSettingId)
      .set('courseInstanceId', courseInstanceId)
      .set('quizType', quizType);
    return this.$http.get<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetQuizOptionsAndTestAttemps',
      {
        params: httpParam
      });
  }
  public GetStudentQuizProgress(courseInstanceId: string): Observable<any> {
    const httpParam = new HttpParams().set('courseInstanceId', courseInstanceId);
    return this.$http.get<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetStudentQuizProgress',
      {
        params: httpParam
      });
  }
}
