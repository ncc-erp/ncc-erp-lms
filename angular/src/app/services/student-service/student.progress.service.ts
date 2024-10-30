import { AppConsts } from '../../../shared/AppConsts';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ListResultDto, IResultObject } from '@app/models/common-dto';

@Injectable({
    providedIn: 'root'
})
export class StudentProgressService {
    public showMess = true
    private NAME = 'StudentProgress';

    constructor(private $http: HttpClient) { }


    public update(item: any): Observable<IResultObject> {
        return this.$http.put<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Update', item);
    }

    public create(item: any): Observable<IResultObject> {
        return this.$http.post<IResultObject>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/Create', item);
    }

    public GetStudentProgressesByCourseInstanceId(courseInstanceId: string): Observable<ListResultDto> {
        const httpParam = new HttpParams().set('courseInstanceId', courseInstanceId);
        return this.$http.get<ListResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/GetStudentProgressesByCourseInstanceId',
            {
                params: httpParam
            });
    }
    public CreateUserAttendanceCertification(courseInstanceId: string): Observable<ListResultDto> {
        // return this.$http.post<ListResultDto>(
        //     AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/CreateUserAttendanceCertification',
        //     {
        //         courseInstanceId: courseInstanceId
        //     });
        const httpParam = new HttpParams().set('courseInstanceId', courseInstanceId);
        return this.$http.get<ListResultDto>(
      AppConsts.remoteServiceBaseUrl + '/api/services/app/' + this.NAME + '/CreateUserAttendanceCertification',
       { params: httpParam});
    }
}
