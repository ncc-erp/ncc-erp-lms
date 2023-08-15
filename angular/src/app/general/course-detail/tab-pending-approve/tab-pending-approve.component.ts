import { Component, OnInit, Injector, Input } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto, PagedResultResultDto } from '@shared/paged-listing-component-base';
import { CoursesService, RequestAssignedStudentCourseDto } from '@app/services/systems-admin-services/courses.service';
import { AssignedStatus } from '@shared/AppEnums';
import { finalize } from 'rxjs/operators';



@Component({
  selector: 'app-tab-pending-approve',
  templateUrl: './tab-pending-approve.component.html',
  styleUrls: ['./tab-pending-approve.component.scss']
})
export class TabPendingApproveComponent extends PagedListingComponentBase<StudentDto> {

  @Input() courseInstanceId: string;

  students: StudentDto[] = [];

  constructor(
    injector: Injector,
    private courseService: CoursesService
  ) { super(injector) }


  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this.courseService
      .getAssignedStudentByCourseAndStatus({ request: request, courseInstanceId: this.courseInstanceId, status: AssignedStatus.PendingApproved })
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: PagedResultResultDto) => {
        this.students = result.result.items;
        this.showPaging(result.result, pageNumber);
      });
  }


  protected delete(entity: StudentDto) {

  }

  accept(id: string, userName: string) {
    this.courseService.UpdateCourseAssignedStudentStatus({ id: id, status: AssignedStatus.Accepted, userName: userName })
      .subscribe(result => {
        this.notify.success("AcceptSuccessfully");
        let index = this.students.findIndex(x => x.studentAssignedCourseId === id);
        this.students.splice(index, 1);
      })
  }

  reject(id: string, userName: string) {
    this.courseService.UpdateCourseAssignedStudentStatus({ id: id, status: AssignedStatus.Rejected, userName: userName })
      .subscribe(result => {
        this.notify.success("RejectSuccessfully");
        let index = this.students.findIndex(x => x.studentAssignedCourseId === id);
        this.students.splice(index, 1);
      })
  }

}

export class StudentDto {
  studentAssignedCourseId: string;
  studentId: string;
  studentName: string;
  userName: string;
  roleName: string;
  lastActivity: string;
  totalActivity: string
  enrollCount: number;
}
