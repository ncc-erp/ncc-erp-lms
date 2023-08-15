import { FAQQuestionAdminDto } from './../../models/faqquestion-dto';
import { AppConsts } from '../../../shared/AppConsts';
import { IResult, IResultObject } from '../../models/common-dto';
import { HttpClient, HttpHeaders, HttpParams, HttpEvent, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedRequestDto, PagedResultResultDto } from 'shared/paged-listing-component-base';
import { EditCourseDto } from '@app/models/courses-dto';
import { CourseSettingFeatualModel } from '@app/models/course-setting-dto';
import { ResponseResultModel, ResponseModel } from '@app/models/response.model';
import { FAQQuestionDto } from '@app/models/faqquestion-dto';
const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });
@Injectable({
  providedIn: 'root'
})
export class CoursesService {
  private NAME = 'Course';

  constructor(protected $http: HttpClient) { }
  init() {
  }

  public getAll(
    skipCount: number | null | undefined,
    maxResultCount: number | null | undefined
  ) {
    const httpParams = new HttpParams()
      .set('SkipCount', skipCount + '')
      .set('MaxResultCount', maxResultCount + '');

    return this.$http.get(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetAll',
      {
        headers: httpOptions,
        params: httpParams
      }
    );
  }

  public getAllPagging(request: PagedRequestDto): Observable<PagedResultResultDto> {
    return this.$http.post<PagedResultResultDto>
    (AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetAllPagging', request);
  }

  public GetAllCourseNotPagging(): Observable<IResultObject> {
    return this.$http.get<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetAllCourseNotPagging');
  }

  public getInvitationStudentByCourse(request: InvitationCourseRequestDto): Observable<PagedResultResultDto> {
    return this.$http.post<PagedResultResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetInvitationStudentByCourse', request);
  }

  public getAssignedStudentByCourseAndStatus(request: RequestAssignedStudentCourseDto): Observable<PagedResultResultDto> {
    return this.$http.post<PagedResultResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetAssignedStudentByCourseAndStatus', request);
  }

  public delete(id: string): Observable<any> {
    return this.$http.delete<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Delete', {
      params: new HttpParams().set('Id', id)
    })
  }
  public deleteCourse(id: string): Observable<any> {
    return this.$http.delete<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/DeleteCourse', {
      params: new HttpParams().set('Id', id)
    })
  }
  public CheckCourseInfo(id: string): Observable<any> {
    return this.$http.get<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/CheckCourseInfo', {
      params: new HttpParams().set('Id', id)
    })
  }
  public update(item: any): Observable<any> {
    return this.$http.put<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Update', item);
  }

  public UpdateCourseSyllabus(course: any): Observable<EditCourseDto> {
    return this.$http.put<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/UpdateCourseSyllabus', course);
  }

  // UpdateFAQQuestionSequenceOrder
  public UpdateFAQQuestionSequenceOrder(items: any): Observable<ResponseModel<any>> {
    return this.$http.put<ResponseModel<any>>(AppConsts.remoteServiceBaseUrl +
      '/api/services/app/QAQuestion/UpdateFAQQuestionSequenceOrder', items);
  }

  public create(item: any): Observable<HttpEvent<any>> {
    return this.$http.post<any>
    (AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Create', item, { reportProgress: true });
  }


  public createFormData(item: any): Observable<HttpEvent<any>> {
    const formData = new FormData();
    const propertyNames = Object.getOwnPropertyNames(item);
    propertyNames.forEach(element => {
      formData.append(element, item[element]);
    });
    const uploadReq = new HttpRequest('POST',
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Create',
      formData,
      {
        reportProgress: true
      }
    );
    return this.$http.request(uploadReq);

    // return this.$http.post<EditDto>
    // ( AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Create', item, {reportProgress: true} );
  }
  // Upload file only
  uploadFile(url, file: File) {
    const formData = new FormData();
    formData.append('file', file);
    const uploadReq = new HttpRequest('POST',
      AppConsts.remoteServiceBaseUrl + url,
      formData,
      {
        reportProgress: true
      }
    );
    return this.$http.request(uploadReq);
  }

  public updateFormData(item: any): Observable<HttpEvent<any>> {
    const formData = new FormData();
    const propertyNames = Object.getOwnPropertyNames(item);
    propertyNames.forEach(element => {
      if (item[element] != null) {
        formData.append(element, item[element]);
      }
    });
    const uploadReq = new HttpRequest('PUT',
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Update',
      formData,
      {
        reportProgress: true
      }
    );
    return this.$http.request(uploadReq);

    // return this.$http.post<EditDto>
    // ( AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Create', item, {reportProgress: true} );
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

  public getByCourseInstanceId(id: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('Id', id);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetByCourseInstanceId',
      {
        params: httpParam
      }
    );
  }

  public GetScormCourseAndCourseAssignedStudent(courseInstanceId: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('courseInstanceId', courseInstanceId);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetScormCourseAndCourseAssignedStudent',
      {
        params: httpParam
      }
    );
  }

  public getAllStatus(): Observable<IResult> {
    return this.$http.get<IResult>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetAllStatus');
  }

  public save(item: any): Observable<HttpEvent<any>> {
    const formData = new FormData();
    const propertyNames = Object.getOwnPropertyNames(item);
    propertyNames.forEach(element => {
      formData.append(element, item[element]);
    });
    const uploadReq = new HttpRequest('PUT',
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/UpdateDetail',
      formData,
      {
        reportProgress: true
      }
    );
    return this.$http.request(uploadReq);
  }

  public getUnAssingedStudents(courseInstanceId: string, request: PagedRequestDto): Observable<PagedResultResultDto> {
    return this.$http.post<PagedResultResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME
      + '/GetUnAssingedStudents', { courseInstanceId: courseInstanceId, request: request });
  }


  public updateCourseSettingFeatual(CourseSettingFeatual: CourseSettingFeatualModel[]):
    Observable<ResponseResultModel<CourseSettingFeatualModel[]>> {
    return this.$http.post<ResponseResultModel<CourseSettingFeatualModel[]>>(AppConsts.remoteServiceBaseUrl + '/api/services/app/'
      + this.NAME + '/CreateOrUpdateCourseLMSSetting', CourseSettingFeatual);
  }
  //
  public completedCourse(courseAssignedStudentId: string): Observable<any> {
    return this.$http.post<any>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/CompletedCourse',
       new HttpParams().set('courseAssignedStudentId', courseAssignedStudentId));
  }
  //

  public GetCourseLMSSettingValue(courseId: string): Observable<ResponseModel<CourseSettingFeatualModel[]>> {
    return this.$http.get<ResponseModel<CourseSettingFeatualModel[]>>(AppConsts.remoteServiceBaseUrl + '/api/services/app/'
      + this.NAME + '/GetCourseLMSSettingValue?courseId=' + courseId);
  }

  public GetAllCourseFAQ(): Observable<ResponseModel<FAQQuestionAdminDto[]>> {
    return this.$http.get<ResponseModel<FAQQuestionAdminDto[]>>(AppConsts.remoteServiceBaseUrl + '/api/services/app/'
      + this.NAME + '/GetAllCourseFAQ');
  }


  public CreateFAQQuestion(faqQuestion: FAQQuestionDto): Observable<ResponseResultModel<FAQQuestionDto>> {
    return this.$http.post<ResponseResultModel<FAQQuestionDto>>(AppConsts.remoteServiceBaseUrl +
      '/api/services/app/QAQuestion/CreateFAQQuestion', faqQuestion);
  }

  public UpdateFAQQuestion(faqQuestion: FAQQuestionDto): Observable<ResponseModel<FAQQuestionDto>> {
    return this.$http.put<ResponseModel<FAQQuestionDto>>(AppConsts.remoteServiceBaseUrl +
      '/api/services/app/QAQuestion/UpdateFAQQuestion', faqQuestion);
  }

  DeleteQAQuestion(questionId: string) {
    const params = new HttpParams().set('id', questionId);
    return this.$http.delete<ResponseResultModel<any>>(AppConsts.remoteServiceBaseUrl +
      '/api/services/app/QAQuestion/DeleteQAQuestion', { params });
  }

  // DeleteFAQAnswer
  DeleteFAQQuestion(id: string) {
    return this.$http.post<ResponseResultModel<any>>(AppConsts.remoteServiceBaseUrl +
      '/api/services/app/QAQuestion/DeleteFAQQuestion', { id: id });
  }
  /**
   * Get Accepted courses for students->courses->tab courses
   * tien 2019-01-30
   */
  public GetAcceptedCourses(): Observable<IResultObject> {
    return this.$http.get<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetAcceptedCourses');
  }

  public GetAcceptedCoursesForCalendar(): Observable<IResultObject> {
    return this.$http.get<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetAcceptedCoursesForCalendar');
  }

  public PostAcceptedCourses(data): Observable<IResultObject> {
    return this.$http.post<IResultObject>
    (AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/CreateOrUpdateCourseColor', data)
  }

  /**
  * student->courses->tab archived
  * tien 2019-02-01
  */
  public GetArchivedCourses(): Observable<IResultObject> {
    return this.$http.get<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetArchivedCourses');
  }

  /**
* student->courses->tab invitation
* tien 2019-02-01
*/
  public GetInvitedCourses(): Observable<IResultObject> {
    return this.$http.get<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetInvitedCourses');
  }

  public GetCompletedCourses(): Observable<IResultObject> {
    return this.$http.get<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetCompletedCourses');
  }

  public UpdateCourseAssignedStudentStatus(input: UpdateCourseAssignedStudentStatusDto): Observable<any> {
    return this.$http.put<any>
    (AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/UpdateCourseAssignedStudentStatus', input);
  }

  public GetCourseStatisticsPaging(request: PagedRequestDto): Observable<any> {
    return this.$http.post(AppConsts.remoteServiceBaseUrl+`/api/services/app/${this.NAME}/GetCourseStatistics`, request)
  }
  public GetStudentSurvey(id: string): Observable<any> {
    return this.$http.get<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetStudentSurvey', {
      params: new HttpParams().set('Id', id)
    })
  }
  public RePublishCourse(courseInstanceId: string): Observable<any> {
    return this.$http.post<any>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/RePublishCourse',
      new HttpParams().set('courseInstanceId', courseInstanceId)
    );
  }
  public getCourseInstancesHistory(courseInstanceId: string): Observable<IResultObject> {
    const httpParam = new HttpParams().set('courseInstanceId', courseInstanceId);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/getCourseInstancesHistory',
      {
        params: httpParam
      }
    );
  }
}
export class InvitationCourseRequestDto {
  courseInstanceId: string;
  request: PagedRequestDto;
}

export class RequestAssignedStudentCourseDto {
  courseInstanceId: string;
  status: number;
  request: PagedRequestDto;
}

export class UpdateCourseAssignedStudentStatusDto {
  id: string;
  status: number;
  userName: string;
}
