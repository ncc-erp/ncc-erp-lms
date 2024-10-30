import { AppConsts } from '../../../shared/AppConsts';
import { CreateCategoryDto as CreateDto, CategoryDto as EditDto, IPagedResultDtoOfCategoryDto, IResult, IResultObject } from '../../models/categories-dto';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {PagedRequestDto, PagedResultResultDto} from 'shared/paged-listing-component-base';

const httpOptions = new HttpHeaders( { 'Content-Type': 'application/json' } );

@Injectable({
  providedIn: 'root'
})
export class CategoriesService {
  private NAME: string = 'Category';

  constructor(private $http: HttpClient) {}
  
  public getAll(
    skipCount: number | null | undefined,
    maxResultCount: number | null | undefined
  ) {
    let httpParams = new HttpParams()
      .set('SkipCount', skipCount + '')
      .set( 'MaxResultCount', maxResultCount + '' );

    return this.$http.get(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetAll',
      {
        headers: httpOptions,
        params: httpParams
      }
    );
  }

  public getAllPagging ( request: PagedRequestDto ): Observable<PagedResultResultDto> {
    return this.$http.post<PagedResultResultDto>( AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetAllPagging', request );
  }

  public getAllNotPagging () : Observable<IResult> {
    return this.$http.get<IResult>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetAllNotPagging');
  }

  public delete ( id: string ) : Observable<EditDto> {    
    return this.$http.delete<EditDto>( AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Delete', {
      params : new HttpParams().set( 'Id', id )
    })
  }

  public update ( item: EditDto ) : Observable<EditDto>{
    return this.$http.put<EditDto>( AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Update', item);
  }

  public create ( item: CreateDto ): Observable<EditDto> {
    return this.$http.post<EditDto>( AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Create', item );
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
}
