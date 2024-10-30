import { AppConsts } from '../../../shared/AppConsts';
import { CreateModuleDto as CreateDto, ModuleDto as EditDto, IModulesPagesDto, IModulePagesDto } from '../../models/module-dto';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedRequestDto, PagedResultResultDto } from 'shared/paged-listing-component-base';
import { IResultObject } from '@app/models/common-dto';

const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });

@Injectable({
  providedIn: 'root'
})
export class ModulesService {
  private NAME: string = 'Module';

  constructor(private $http: HttpClient) { }


  public getAllByCourseId(courseId: string): Observable<PagedResultResultDto> {
    let httpParam = new HttpParams().set('courseId', courseId);
    return this.$http.get<PagedResultResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetAllByCourseId',
      {
        params: httpParam
      });
  }

  public getModulesPagesByCourseId(courseId: string): Observable<IResultObject> {
    let httpParam = new HttpParams().set('courseId', courseId);
    return this.$http.get<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetModulesPagesByCourseId',
      {
        params: httpParam
      });
  }

  public getModulesPagesByCourseInstanceId(courseInstanceId: string): Observable<IResultObject> {
    let httpParam = new HttpParams().set('courseInstanceId', courseInstanceId);
    return this.$http.get<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetModulesPagesByCourseInstanceId',
      {
        params: httpParam
      });
  }

    public GetModulesPagesForStudent(courseInstanceId: string): Observable<IResultObject> {
    let httpParam = new HttpParams().set('courseInstanceId', courseInstanceId);
    return this.$http.get<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetModulesPagesForStudent',
      {
        params: httpParam
      });
  }


  public delete(id: string): Observable<EditDto> {
    return this.$http.delete<EditDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Delete', {
      params: new HttpParams().set('Id', id)
    })
  }

  public update(item: EditDto): Observable<IResultObject> {
    return this.$http.put<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Update', item);
  }

  public create(item: CreateDto): Observable<IResultObject> {
    return this.$http.post<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Create', item);
  }

  public getById(id: string): Observable<IResultObject> {
    let httpParam = new HttpParams().set('Id', id);
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Get',
      {
        params: httpParam
      }
    );
  }

  public saveModulesPages(item: IModulesPagesDto) {
    return this.$http.post<IModulesPagesDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/SaveModulesPages', item);
  }

  public savePages(item: IModulePagesDto) {
    return this.$http.post<IModulePagesDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/SavePages', item);
  }

}
