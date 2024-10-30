import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { BaseService } from '@app/services/base-service/base.service';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs/operators';
import { CourseDetailDto } from '@app/models/courses-dto';
import { AssignedStatus } from '@shared/AppEnums';

@Component({
  selector: 'app-course-detail',
  templateUrl: './course-detail.component.html',
  styleUrls: ['./course-detail.component.scss']
})
export class CourseDetailComponent extends AppComponentBase implements OnInit {
  courseDetail = { course: {}, creator: {} } as CourseDetailDto;
  courseInstanceId: string;
  courseId: string;

  constructor(private injector: Injector, private _base: BaseService, private router: ActivatedRoute) {
    super(injector);
  }
  ngOnInit() {
    this._base._studentService.setNavbar(true);
    this.router.params.subscribe(e => {
      this._base._studentService.GetCourseByInstanceId(e.id).pipe(map(m => m.result)).subscribe(s => {
        this.courseInstanceId = e.id;
        this.courseDetail = s;
        this.courseId = this.courseDetail.course.id;
      })
    })
  }

  onPrevious() {
    this._base._studentService.setPreviousPage('student');
  }

  enroll() {
    this._base._studentService.StudentEnrollCourse(this.courseInstanceId).subscribe(result => {
      this.notify.success("EnrollSuccessfully");
      this.courseDetail.status = result.result.status;
    })
  }

  reEnroll() {
    this._base._studentService.StudentReEnrollCourse(this.courseInstanceId).subscribe(result => {
      this.notify.success("EnrollSuccessfully");
      this.courseDetail.status = result.result.status;
    })
  }

  accept() {
    this._base._studentService.StudentAcceptRejectInvitaionCourse(this.courseInstanceId, AssignedStatus.Accepted).subscribe(result => {
      this.notify.success("AcceptSuccessfully");      
      this.courseDetail.status = result.result.status;      
    })
  }

  reject() {
    this._base._studentService.StudentAcceptRejectInvitaionCourse(this.courseInstanceId, AssignedStatus.Rejected).subscribe(result => {
      this.notify.success("RejectSuccessfully");
      this.courseDetail.status = result.result.status;
    })
  }
}
