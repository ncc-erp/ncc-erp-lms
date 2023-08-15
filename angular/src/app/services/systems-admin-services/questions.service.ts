import { AppConsts } from '../../../shared/AppConsts';
import { CreateQuestionDto as CreateDto, QuestionDto as EditDto } from '../../models/question-dto';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedRequestDto, PagedResultResultDto } from 'shared/paged-listing-component-base';
import { IResultObject, ListResultDto } from '@app/models/common-dto';
import { EQuestion } from '@shared/AppEnums';

const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });

@Injectable({
  providedIn: 'root'
})
export class QuestionsService {
  private NAME = 'Question';

  constructor(private $http: HttpClient) { }
  public getQuestionsByCourseId(courseId: string, request: PagedRequestDto): Observable<PagedResultResultDto> {
    return this.$http.post<PagedResultResultDto>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetQuestionsByCourseIdPagging',
      { courseId: courseId, input: request, quizId: '' });
  }

  // public getQuestionsByCourseId ( courseId: string, request: PagedRequestDto ): Observable<PagedResultResultDto> {
  //   let httpParam = new HttpParams().set('courseId', courseId );
  //   return this.$http.post<PagedResultResultDto>(
  //    AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetQuestionsByCourseId',
  //     {
  //       params: httpParam
  //     } );
  // }

  public getQuestionsByModuleId(moduleId: string): Observable<PagedResultResultDto> {
    const httpParam = new HttpParams().set('moduleId', moduleId);
    return this.$http.get<PagedResultResultDto>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetQuestionsByModuleId',
      {
        params: httpParam
      });
  }

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
  public GetQuestionsFromPool(courseId: string): Observable<PagedResultResultDto> {
    const httpParam = new HttpParams().set('courseId', courseId);
    return this.$http.get<PagedResultResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetQuestionsFromPool',
      {
        params: httpParam
      });
  }
  public ImportQuestionToModule(item: EditDto): Observable<EditDto> {
    return this.$http.post<EditDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/ImportQuestionToModule', item);
  }

  public getQuestionTypeName(id: number): string {
    for (let i = 0; i < EQuestion.QUESTION_TYPES.length; i++) {
      if (EQuestion.QUESTION_TYPES[i].id === id) {
        return EQuestion.QUESTION_TYPES[i].brifName;
      }
    }
    return id.toString();
  }
  public GetQuestionsByQuizIdPagging(quizId: string, request: PagedRequestDto): Observable<PagedResultResultDto> {
    return this.$http.post<PagedResultResultDto>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetQuestionsByQuizIdPagging',
      { input: request, quizId: quizId, courseId: '' });
  }

  public GetStudentAnswersPagging(testAttemptId: string, request: PagedRequestDto): Observable<PagedResultResultDto> {
    return this.$http.post<PagedResultResultDto>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/StudentAnswer/GetStudentAnswersPagging',
      { input: request, testAttempId: testAttemptId });
  }

  public GetStudentAnswersNotPagging(testAttemptId: string): Observable<ListResultDto> {
    const httpParam = new HttpParams().set('testAttemptId', testAttemptId);
    return this.$http.get<ListResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/StudentAnswer/GetStudentAnswersNotPagging',
      {
        params: httpParam
      });
  }

  public GetQuestionsByQuizIdNotPagging(quizId: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('quizId', quizId);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetQuestionsByQuizIdNotPagging',
      {
        params: httpParam
      });
  }

  public GetQuestionsByQuizSettingIdNotPagging(quizSettingId: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('quizSettingId', quizSettingId);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetQuestionsByQuizSettingIdNotPagging',
      {
        params: httpParam
      });
  }

  public GetQuestionsByTestAttemptIdNotPagging(testAttemptId: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('testAttemptId', testAttemptId);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetQuestionsByTestAttemptIdNotPagging',
      {
        params: httpParam
      });
  }

  public GetQuestionsPool(courseId: string, quizId: string, request: PagedRequestDto): Observable<IResultObject> {
    return this.$http.post<PagedResultResultDto>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetQuestionsPool',
      { input: request, quizId: quizId, courseId: courseId });
  }
  public LinkQuestion(quizId: string, questionId: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('quizId', quizId)
      .set('questionId', questionId);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/LinkQuestion',
      {
        params: httpParam
      });
  }
  public RemoveLink(quizId: string, questionId: string): Observable<EditDto> {
    return this.$http.delete<EditDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/RemoveLink', {
      params: new HttpParams().set('quizId', quizId).set('questionId', questionId)
    })
  }

  //Lưu vị trí thay đổi câu hỏi
  public SaveIndexQuestion(input: any): Observable<EditDto> {
    return this.$http.post<EditDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/SaveIndexQuestion', input);
  }

}
