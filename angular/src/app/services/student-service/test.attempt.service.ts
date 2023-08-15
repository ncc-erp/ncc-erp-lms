import { AppConsts } from '../../../shared/AppConsts';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { StudentAnswerDto } from '@app/models/student-answer-dto';
import { QuestionDto } from '@app/models/question-dto';
import { ErrorHandlerService } from '../base-service/errorHandler.service';
import { catchError } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class TestAttemptService {
    private NAME = 'TestAttempt';

    constructor(private $http: HttpClient,  private errorHandler:ErrorHandlerService) { }

    public create(item: TestAttemptDto): Observable<any> {
        let api = '/api/services/app/' + this.NAME + '/Create'
        return this.$http.post<any>(AppConsts.remoteServiceBaseUrl + api, item)
        .pipe(catchError(err => {
            this.errorHandler.handleError(err, api, item);
            throw err
          }));
    }

    public update(item: TestAttemptDto): Observable<any> {
        let api = '/api/services/app/' + this.NAME + '/Update'
        return this.$http.put<any>(AppConsts.remoteServiceBaseUrl + api, item)
        .pipe(catchError(err => {
            this.errorHandler.handleError(err, api, item);
            throw err
          }));
    }

    public ProcessScore(item: TestAttemptDto): Observable<any> {
        let api = '/api/services/app/' + this.NAME + '/ProcessScore'
        return this.$http.post<any>(AppConsts.remoteServiceBaseUrl + api, item)
        .pipe(catchError(err => {
            this.errorHandler.handleError(err, api, item);
            throw err
          }));;
    }

    public GetTestAttempt(quizSettingId: string, status: number): Observable<any> {
        let api = '/api/services/app/' + this.NAME + '/GetTestAttempt'
        const httpParam = new HttpParams().set('quizSettingId', quizSettingId).set('status', status.toString());
        return this.$http.get<any>
        (AppConsts.remoteServiceBaseUrl + api, {params: httpParam}).pipe(catchError(err => {
            this.errorHandler.handleError(err, api, httpParam);
            throw err
          }));;
    }

    public GetStudentTestAttempts(studentId: number, quizSettingId: string): Observable<any> {
        let api = '/api/services/app/' + this.NAME + '/GetStudentTestAttempts'
        const httpParam = new HttpParams().set('quizSettingId', quizSettingId).set('studentId', studentId.toString());
        return this.$http.get<any>
        (AppConsts.remoteServiceBaseUrl + api, {params: httpParam}).pipe(catchError(err => {
            this.errorHandler.handleError(err, api, httpParam);
            throw err
          }));;
    }
}

export class TestAttemptDto {
    id: string;
    status: number;
    quizSettingId: string;
    score: number;
    timeRemaining?: number;
    studentAnswers: StudentAnswerDto[];
    questions: QuestionDto[];
    lastModificationTime: string;

    isViewing: boolean;
    type: number; // PageType:
    maxScore: number;

}
