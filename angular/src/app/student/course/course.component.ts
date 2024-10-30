import { Component, OnInit, Injector } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CoursesService } from '@app/services/systems-admin-services/courses.service';
import { EditCourseDto, CourseDetailDto } from '@app/models/courses-dto';
import { AppComponentBase } from '@shared/app-component-base';
import { StudentService } from '@app/services/student-service/student.service';
import { BaseService } from '@app/services/base-service/base.service';
import { map } from 'rxjs/operators';
import { CourseSettingFeatualValueDto } from '@app/models/course-setting-dto';
import { AssignedStatus } from '@shared/AppEnums';

@Component({
  selector: 'app-course',
  templateUrl: './course.component.html',
  styleUrls: ['./course.component.scss']
})
export class CourseComponent extends AppComponentBase implements OnInit {

  courseInstanceId: string;
  courseId: string;
  tabIndex: number = 0;
  tabCourseSession = 'tabStudentCourseSession';

  courseDetail = { course: {} } as CourseDetailDto;

  courseSetting: CourseSettingFeatualValueDto = new CourseSettingFeatualValueDto();
  canAccess = false;

  constructor(
    injector: Injector,
    private route: ActivatedRoute,
    private _base: BaseService,
    private _courseService: CoursesService,
  ) { super(injector) }

  ngOnInit() {
    this.courseInstanceId = this.route.snapshot.paramMap.get('id');
    this.getCourseByInstanceId();
  }

  onPrevious() {
    this._base._studentService.setPreviousPage('student');
  }

  getCourseByInstanceId() {
    this._base._studentService.GetCourseByInstanceId(this.courseInstanceId).pipe(map(m => m.result)).subscribe(s => {
      this.courseDetail = s;
      this.canAccess = this.courseDetail.canStart && this.courseDetail.status == 2;
      this.courseId = this.courseDetail.course.id;
      this.courseDetail.completedPercent = this.courseDetail.totalPage > 0 ? this.courseDetail.nPageCompleted * 100 / this.courseDetail.totalPage : 0;
      // Get value of course setting
      this.getCourseSettingFeatualValue();
    });

  }

  private getCourseSettingFeatualValue() {
    // Get value of CourseSettingFeatualValue from database
    this._courseService.GetCourseLMSSettingValue(this.courseId)
      .subscribe(items => {
        items.result.items.forEach(item => {
          switch (item.key) {
            case 'Show_recent_annoucements_as_first_page':
              this.courseSetting.Show_recent_annoucements_as_first_page = item.value === 'true' ? true : false;
              if (this.courseSetting.Show_recent_annoucements_as_first_page) {
                this.tabIndex = 1;
              }
              break;
            case 'Number_of_annoucements_shown_on_homepage':
              this.courseSetting.Number_of_annoucements_shown_on_homepage = +item.value;
              break;

            case 'Allow_students_create_disscussion_on_QA':
              this.courseSetting.Allow_students_create_disscussion_on_QA = item.value === 'true' ? true : false;
              break;

            case 'Grades_Summary_List_students_by_name':
              this.courseSetting.Grades_Summary_List_students_by_name = item.value === 'true' ? true : false;
              break;

            case 'Allow_completed_students_to_re_enroll':
              this.courseSetting.Allow_completed_students_to_re_enroll = item.value === 'true' ? true : false;
              break;
          }
          if (this.getSession(this.tabCourseSession) !== null) {
            this.tabIndex = this.getSession(this.tabCourseSession);
          }
        });
      });
  }
  onTabChanged(event) {
    this.setSession(this.tabCourseSession, event.index);
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

  logStudentProcess(){
    let message = `Start the Course ${this.courseDetail.course.name}`
    let args = {
      timeStart: this.formatTime(new Date()),
      // courseDetail: JSON.stringify(this.courseDetail)
    }
    this.logStudentProcessToSentry(message, args);
  }
}
