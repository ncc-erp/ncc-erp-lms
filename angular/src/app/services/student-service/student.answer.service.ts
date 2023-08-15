import { AppConsts } from '../../../shared/AppConsts';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ListResultDto, IResultObject } from '@app/models/common-dto';
import { ErrorHandlerService } from '../base-service/errorHandler.service';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class StudentAnswerService {
  private NAME = 'StudentAnswer';

  constructor(private $http: HttpClient, private errorHandler:ErrorHandlerService) { }
 

  public update(item: any): Observable<any> {
    let api = '/api/services/app/' + this.NAME + '/Update'
    return this.$http.put<IResultObject>(AppConsts.remoteServiceBaseUrl + api, item)
    .pipe(catchError(err => {
      this.errorHandler.handleError(err, api, item);
      throw err
    }));
  }

  public create(item: any): Observable<any> {
    let api = '/api/services/app/' + this.NAME + '/Create'
    return this.$http.post<IResultObject>(AppConsts.remoteServiceBaseUrl + api, item)
    .pipe(catchError(err => {
      this.errorHandler.handleError(err, api, item);
      throw err
    }));
  }

  public CreateStudentAnswers(item: any): Observable<any> {
    let api =  '/api/services/app/' + this.NAME + '/Save'
    return this.$http.post<any>(AppConsts.remoteServiceBaseUrl + api, item)
    .pipe(catchError(err => {
      this.errorHandler.handleError(err, api, item);
      throw err
    }));
  }

  public GetCorrectAnswersByQuestionId(questionId: string): Observable<ListResultDto> {
    const httpParam = new HttpParams().set('questionId', questionId);
    return this.$http.get<ListResultDto>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetCorrectAnswersByQuestionId',
      {
        params: httpParam
      }
    );
  }
  public GetCorrectAnswersByQuizId(quizId: string): Observable<ListResultDto> {
    const httpParam = new HttpParams().set('quizId', quizId);
    return this.$http.get<ListResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetCorrectAnswersByQuizId',
      {
        params: httpParam
      });
  }

 public TeacherTakeScoreOpenEndQuestion(item: any): Observable<IResultObject> {
    return this.$http.post<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/TeacherTakeScoreOpenEndQuestion', item);
  }

}
