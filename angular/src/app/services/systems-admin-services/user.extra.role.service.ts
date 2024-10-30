import { AppConsts } from '../../../shared/AppConsts';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IResultObject } from '@app/models/common-dto';

@Injectable({
    providedIn: 'root'
})
export class UserExtralRoleService {
    constructor(private $http: HttpClient) { }

    public GetCourseAdminsByCourseId(courseId: string): Observable<IResultObject> {
        return this.$http.get<IResultObject>(AppConsts.remoteServiceBaseUrl + `/api/services/app/UserExtraRole/GetCourseAdminsByCourseId`, { params: new HttpParams().set('courseId', courseId) });
    }

    public AddCourseAdminsToCourse(obj: CourseAdminsToCourseDto) {
        //console.log('obj', obj);

        return this.$http.post<CourseAdminsToCourseDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/UserExtraRole/AddCourseAdminsToCourse', obj);
    }


}

export class CourseAdminsToCourseDto {
    courseId: string;
    CourseAdminIds: string[];
    courseAdminNames: string[];
}
