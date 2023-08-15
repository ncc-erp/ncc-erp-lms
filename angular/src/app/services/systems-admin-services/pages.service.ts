import { AppConsts } from '../../../shared/AppConsts';
import { CreatePageDto as CreateDto, PageDto as EditDto } from '../../models/pages-dto';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResultResultDto} from 'shared/paged-listing-component-base';
import { IResultObject } from '@app/models/common-dto';
import { CreateBookMarkDto } from '@app/models/bookmark-dto';

const httpOptions = new HttpHeaders( { 'Content-Type': 'application/json' } );

@Injectable({
  providedIn: 'root'
})
export class PagesService {
  private NAME: string = 'Page';

  constructor(private $http: HttpClient) {}


  public getAllByCourseId ( courseId: string ): Observable<PagedResultResultDto> {
    let httpParam = new HttpParams().set( 'courseId', courseId );
    return this.$http.get<PagedResultResultDto>( AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetAllByCourseId',
      {
        params: httpParam
      } );
  }

  public delete ( id: string ) : Observable<EditDto> {
    return this.$http.delete<EditDto>( AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Delete', {
      params : new HttpParams().set( 'Id', id )
    })
  }

  public update ( item: EditDto ) : Observable<IResultObject>{
    return this.$http.put<IResultObject>( AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Update', item);
  }

  public create ( item: CreateDto ): Observable<IResultObject> {
    return this.$http.post<IResultObject>( AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Create', item );
  }

  public bookmarkPage ( item: CreateBookMarkDto ): Observable<IResultObject> {
    return this.$http.post<IResultObject>( AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/BookmarkPage', item );
  }

   public unBookmarkPage ( pageId: string ): Observable<IResultObject> {
    return this.$http.delete<IResultObject>( AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/UnBookmarkPage', {
      params : new HttpParams().set( 'Id', pageId )
    })
  }


  public getById ( id: string ): Observable<IResultObject> {
    let httpParam = new HttpParams().set( 'Id', id );
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Get',
      {
        params: httpParam
      }
    );
  }

  public getForStudentById ( id: string ): Observable<IResultObject> {
    let httpParam = new HttpParams().set( 'Id', id );
    return this.$http.get<IResultObject>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetForStudent',
      {
        params: httpParam
      }
    );
  }

  public getPagesByModuleId(id: string): Observable<any> {
    let httpParam = new HttpParams().set('moduleId', id);
    return this.$http.get<any>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/getPagesByModuleId',
      {
        params: httpParam
      }
    );
  }
}

