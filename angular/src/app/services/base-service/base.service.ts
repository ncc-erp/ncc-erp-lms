import { AppConsts } from '../../../shared/AppConsts';
import { IResult, IResultObject } from '../../models/common-dto';
import { HttpClient, HttpHeaders, HttpParams, HttpEvent, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedRequestDto, PagedResultResultDto } from 'shared/paged-listing-component-base';
import { AccountServiceProxy } from '@shared/service-proxies/service-proxies';
import { StudentService } from '../student-service/student.service';
import { ModulesService } from '../systems-admin-services/modules.service';
import { PagesService } from '../systems-admin-services/pages.service';
import { UserStatusService } from '../systems-admin-services/user.status.service';
import { CoursesService } from '../systems-admin-services/courses.service';
import { CourseLevelService } from '../systems-admin-services/course.level.service';
import { SettingService } from '../systems-admin-services/setting.service';

const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });

@Injectable({
  providedIn: 'root'
})
export abstract class BaseService {
  public NAME = '';
  public _accountService: AccountServiceProxy;
  get accountService() {
    return this._accountService = this.inject.get(AccountServiceProxy);
  }
  public studentService: StudentService;
  get _studentService() {
    return this.studentService = this.inject.get(StudentService);
  }
  public moduleService: ModulesService;
  get _moduleService() {
    return this.moduleService = this.inject.get(ModulesService);
  }
  public pageService: PagesService;
  get _pageService() {
    return this.pageService = this.inject.get(PagesService);
  }
  public userStatusService: UserStatusService
  get _userStatusService() {
    return this.userStatusService = this.inject.get(UserStatusService);
  }
  public courseService: CoursesService;
  get _courseService() {
    return this.courseService = this.inject.get(CoursesService);
  }
  public courseLevelService: CourseLevelService;
  get _courseLevelService() {
    return this.courseLevelService = this.inject.get(CourseLevelService);
  }
  public settingService: SettingService;
  get _settingService(): SettingService {
    return this.settingService = this.inject.get(SettingService);
  }
  constructor(protected $http: HttpClient, public inject: Injector) { }
  init(name: string) {
    this.NAME = name;
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
    return this.$http.post
    <PagedResultResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetAllPagging', request);
  }


  public delete(id: string): Observable<any> {
    return this.$http.delete<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Delete', {
      params: new HttpParams().set('Id', id)
    })
  }

  public update(item: any): Observable<any> {
    return this.$http.put<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Update', item);
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

    // return this.$http.post<EditDto>( AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Create', item, {reportProgress: true} );
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

    // return this.$http.post<EditDto>( AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Create', item, {reportProgress: true} );
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
}
