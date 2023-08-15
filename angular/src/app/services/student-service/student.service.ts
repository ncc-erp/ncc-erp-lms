import { BookMarkDto } from './../../models/bookmark-dto';
import { AppConsts } from './../../../shared/AppConsts';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ResultDto, UserDto } from '@app/models/user-dto';
import { ResultDto as Result } from '@app/models/entity'
import { Observable, Subject } from 'rxjs';
import { IStatictisDto } from '@app/models/student-dto';
import { LanguageDto } from '@app/models/language-dto';
import { TimezoneDto } from '@app/models/timezone-dto';
import { CourseDetailDto } from '@app/models/courses-dto';
import { PagedResultResultDto, PagedRequestDto } from '@shared/paged-listing-component-base';
import { QAQuestionDto, QAQuestionAnswerDto, QAAnswerDto, QAAnswerInput } from '@app/models/qaquestion-dto';
import { ResponseResultModel, ResponseModel } from '@app/models/response.model';
import { FAQQuestionDto } from '@app/models/faqquestion-dto';
import { StudentCertificationDto } from '@app/models/studentcertification-dto';
import { IResultObject } from '@app/models/common-dto';
const headers = new HttpHeaders(
  ({ 'Content-Type': 'application/json' })
)
@Injectable({
  providedIn: 'root'
})
export class StudentService {
  private baseUrl: string = AppConsts.remoteServiceBaseUrl;
  users: Subject<UserDto> = new Subject<UserDto>();
  users$ = this.users.asObservable();
  languages: Subject<LanguageDto[]> = new Subject<LanguageDto[]>();
  languages$ = this.languages.asObservable();
  timeZone: Subject<TimezoneDto[]> = new Subject<TimezoneDto[]>();
  timeZone$ = this.timeZone.asObservable();
  isChanges: Subject<boolean> = new Subject<boolean>();
  isChanges$ = this.isChanges.asObservable();
  permissonName: Subject<string> = new Subject<string>();
  permissonName$ = this.permissonName.asObservable();
  isList: Subject<boolean> = new Subject<boolean>();
  isList$ = this.isList.asObservable();
  isNavbar: Subject<boolean> = new Subject<boolean>();
  isNavbar$ = this.isNavbar.asObservable();
  previousPage: Subject<string> = new Subject<string>();
  previousPage$ = this.previousPage.asObservable();
  view = false;
  constructor(private $http: HttpClient) { }
  setPreviousPage(page) {
    return this.previousPage.next(page);
  }
  setNavbar(navbar) {
    return this.isNavbar.next(navbar);
  }
  setList(list) {
    this.view = list
    return this.isList.next(list);
  }
  getList() {
    return this.view;
  }
  setUser(user) {
    return this.users.next(user);
  }
  setLanguage(language) {
    return this.languages.next(language);
  }

  setTimeZone(timeZone) {
    return this.timeZone.next(timeZone);
  }

  setPermissonName(permisson) {
    return this.permissonName.next(permisson);
  }

  isChange(check) {
    return this.isChanges.next(check);
  }
  getUserById(): Observable<ResultDto> {
    return this.$http.get<ResultDto>(this.baseUrl + '/api/services/app/Account/GetUserProfile');
  }

  getAllTimeZone() {
    return this.$http.get<Result<TimezoneDto>>(this.baseUrl + '/api/services/app/TimeZone/GetAll');
  }

  updateUser(user): Observable<ResultDto> {
    return this.$http.put<ResultDto>(this.baseUrl + '/api/services/app/Account/UpdateUserInfo', user);
  }

  getAllLanguage() {
    return this.$http.get<Result<LanguageDto[]>>(this.baseUrl + '/api/services/app/Languages/GetAll');
  }

  getTimeZone() {
    return this.$http.get<Result<LanguageDto>>(this.baseUrl + '/api/services/app/TimeZone/GetAll');
  }

  getAllCourse(request: PagedRequestDto) {
    return this.$http.post(this.baseUrl + '/api/services/app/Course/getAllCourse', request);
  }

  getCourseStatistic(): Observable<Result<IStatictisDto>> {
    return this.$http.get<Result<IStatictisDto>>(this.baseUrl + '/api/services/app/Course/GetCourseStatistic');
  }

  getProfileById(id): Observable<ResultDto> {
    const data: HttpParams = new HttpParams().set('Id', id);
    return this.$http.get<ResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/Course/GetProfileById', { params: data })
  }

  GetCourseByInstanceId(id): Observable<Result<CourseDetailDto>> {
    const params = new HttpParams().set('courseInstanceId', id);
    return this.$http.get<Result<CourseDetailDto>>(this.baseUrl + '/api/services/app/Course/GetCourseByInstanceId', { params })
  }
  public GetAnnoucementForStudentByCourseInstanceIdPagging(request: PagedRequestDto, courseInstanceId: string)
    : Observable<PagedResultResultDto> {
    return this.$http.post<PagedResultResultDto>(
      this.baseUrl + '/api/services/app/' + 'Annoucement'
      + '/GetAnnoucementForStudentByCourseInstanceIdPagging', { input: request, courseInstanceId: courseInstanceId });
  }

  CreateQAQuestion(qaQuestion: QAQuestionDto) {
    return this.$http.post<ResponseResultModel<QAQuestionAnswerDto>>
      (this.baseUrl + '/api/services/app/QAQuestion/CreateQAQuestion', qaQuestion);
  }

  CreateQAAnswer(qaAnswer: QAAnswerInput) {
    return this.$http.post<ResponseResultModel<QAAnswerDto>>
      (this.baseUrl + '/api/services/app/QAQuestion/CreateQAAnswer', qaAnswer);
  }


  UpdateQAQuestion(qaQuestion: QAQuestionDto) {
    return this.$http.post(this.baseUrl + '/api/services/app/QAQuestion/Update', qaQuestion);
  }

  IsFollowQAQuestion(questionId: string) {
    return this.$http.post<ResponseResultModel<any>>(this.baseUrl + '/api/services/app/QAQuestion/IsFollowQAQuestion', { id: questionId });
  }

  // AddUserFollowQA
  AddUserFollowQA(QuestionId: string) {
    return this.$http.post<ResponseResultModel<any>>
      (this.baseUrl + '/api/services/app/QAQuestion/AddUserFollowQA', { id: QuestionId });
  }

  // DeleteUserFollowQA
  DeleteUserFollowQA(questionId: string) {
    return this.$http.post<ResponseResultModel<any>>(this.baseUrl + '/api/services/app/QAQuestion/DeleteUserFollowQA', { id: questionId });
  }

  // DeleteQAAnswer
  DeleteQAAnswer(answerId: string) {
    return this.$http.post<ResponseResultModel<any>>(this.baseUrl + '/api/services/app/QAQuestion/DeleteQAAnswer', { id: answerId });
  }

  // GetQAQuestionAnswer
  public GetQAQuestionAnswer(
    request: PagedRequestDto,
    courseInstanceId: string,
    isFollower: boolean,
    isResponse: boolean,
    sort: string): Observable<PagedResultResultDto> {
    return this.$http.post<PagedResultResultDto>(
      this.baseUrl + '/api/services/app/QAQuestion/GetQAQuestionAnswer'
      , { input: request, courseInstanceId: courseInstanceId, isFollower: isFollower, isResponse: isResponse, sort: sort });
  }

  getAllQAAnswerByQAQuestionId(QAQuestionId: string): Observable<ResponseModel<any[]>> {
    return this.$http.get<ResponseModel<any[]>>
      (this.baseUrl + '/api/services/app/QAQuestion/GetQAAnswerByQAQuestionId?QAQuestionId=' + QAQuestionId);
  }

  GetAllFAQQuestionByCourseId(courseId: string, search: string): Observable<ResponseModel<FAQQuestionDto[]>> {
    const params: HttpParams = new HttpParams().set('Id', courseId).set('Content', search);
    return this.$http.get<ResponseModel<FAQQuestionDto[]>>
      (this.baseUrl + '/api/services/app/QAQuestion/GetsFAQQuestionByCourseId', { params });
  }

  public GetsDiscussionByCourseInstanceIdPagging(
    request: PagedRequestDto,
    courseInstanceId: string): Observable<PagedResultResultDto> {
    return this.$http.post<PagedResultResultDto>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/QAQuestion/GetsDiscussionByCourseInstanceIdPagging'
      , { input: request, courseInstanceId: courseInstanceId });
  }

  public StudentEnrollCourse(courseInstanceId: string) {
    return this.$http.post<any>
      (this.baseUrl + '/api/services/app/UserAssignedToCourses/StudentEnrollCourse', { courseInstanceId: courseInstanceId });
  }

  public StudentReEnrollCourse(courseInstanceId: string) {
    return this.$http.post<any>
      (this.baseUrl + '/api/services/app/UserAssignedToCourses/StudentReEnrollCourse', { courseInstanceId: courseInstanceId });
  }

  public StudentAcceptRejectInvitaionCourse(courseInstanceId: string, status: number) {
    return this.$http.post<any>
      (this.baseUrl + '/api/services/app/UserAssignedToCourses/StudentAcceptRejectInvitaionCourse',
        { courseInstanceId: courseInstanceId, status: status });
  }

  // GetsUserBookMark
  GetsUserBookMark(courseInstanceId: string): Observable<ResponseModel<BookMarkDto[]>> {
    return this.$http.get<ResponseModel<BookMarkDto[]>>
      (this.baseUrl + '/api/services/app/Page/GetsUserBookMark?courseInstanceId=' + courseInstanceId);
  }

  // CheckUserBookMark
  public CheckUserBookMark(courseInstanceId: string, PageId: string) {
    return this.$http.post<ResponseResultModel<boolean>>(this.baseUrl + '/api/services/app/Page/CheckUserBookMark',
      { courseInstanceId: courseInstanceId, pageId: PageId });
  }

  // CreateUserBookMark
  public CreateUserBookMark(courseInstanceId: string, PageId: string) {
    return this.$http.post<ResponseResultModel<boolean>>(this.baseUrl + '/api/services/app/Page/CreateUserBookMark',
      { courseInstanceId: courseInstanceId, pageId: PageId });
  }
  GetAllCertificationsByUser() {
    return this.$http.get<IResultObject>(this.baseUrl + '/api/services/app/Account/GetAllCertificationsByUser');
  }
  print(courseCertificationId: string): Observable<any> {
    return this.$http.get(`${this.baseUrl}/api/services/app/Account/PrintAsync?courseCertificationId=${courseCertificationId}`);
  }
  public getImage(url: string): Observable<Blob> {
    return this.$http.get(`${url}`, { responseType: 'blob' })
  }
  public GetStudentsProgressInCourse(courseInstanceId: string): Observable<any> {
    const httpParam = new HttpParams().set('courseInstanceId', courseInstanceId);
    return this.$http.get<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/Account/GetStudentsProgressInCourse',
      {
        params: httpParam
      });
  }
}
