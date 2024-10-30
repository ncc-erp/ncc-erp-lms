import { AppConsts } from '../../../shared/AppConsts';
import { UserGroupDto, CreateUserGroupDto, IGroupsToUserDto, UsersToGroupDto, ResultObject, UsersByGroupIdResult, GroupsByUserIdResult } from '../../models/user-group-dto';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResultResultDto, PagedResultDto } from '@shared/paged-listing-component-base';
import { ListResultDto } from '@app/models/common-dto';
// import { GroupsToCourseDto } from '@app/models/course-group-dto';
const httpOptions = new HttpHeaders({ 'Content-Type': 'application/json' });
@Injectable({
    providedIn: 'root'
})
export class UserAssignedToCourseService {
    constructor(private $http: HttpClient) { }

    public getGroupsAssignedToCourse(courseInstanceId: string): Observable<ListResultDto> {
        return this.$http.get<ListResultDto>(AppConsts.remoteServiceBaseUrl + `/api/services/app/UserAssignedToCourses/GetGroupsAssignedToCourse`, { params: new HttpParams().set('courseInstanceId', courseInstanceId) });
    }

    public addGroupsToCourse(obj: GroupsToCourseDto) {
        return this.$http.post<GroupsToCourseDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/UserAssignedToCourses/AddGroupsToCourse', obj);
    }

    public getStudentAssignedToCourse(courseInstanceId: string): Observable<ListResultDto> {
        return this.$http.get<ListResultDto>(AppConsts.remoteServiceBaseUrl + `/api/services/app/UserAssignedToCourses/GetGroupsAssignedToCourse`, { params: new HttpParams().set('courseInstanceId', courseInstanceId) });
    }

    public addStudentsToCourse(obj: StudentsToCourseDto) {
        return this.$http.post<StudentsToCourseDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/UserAssignedToCourses/AddStudentsToCourse', obj);
    }
    public unInviteToCourse(CourseInstanceId: string, StudentId: number) {
        return this.$http.get<StudentsToCourseDto>(AppConsts.remoteServiceBaseUrl + `/api/services/app/UserAssignedToCourses/UnInviteToCourse?CourseInstanceId=${CourseInstanceId}&&StudentId=${StudentId}`);
    }

}

export class GroupsToCourseDto {
    courseInstanceId: string;
    groups: string[];
}

export class CourseAssignedStudentDto {
    id: string;
    studentId: number;
    status: number;
    courseInstanceId: string;
}

export class CreateUpdateCourseAssignedStudentDto {
    // id: string;
    studentId: number;
    // status: number;
    // courseInstanceId: string;
    // assignedSource: number;
}

export class StudentsToCourseDto {
    courseInstanceId: string;
    // students: CreateUpdateCourseAssignedStudentDto[];
    students: number[];
    studentNames: string[];
}

