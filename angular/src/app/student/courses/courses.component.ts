import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CoursesService } from '@app/services/systems-admin-services/courses.service';
import { CourseState } from '@shared/AppEnums';

@Component({
  selector: 'app-courses',
  templateUrl: './courses.component.html',
  styleUrls: ['./courses.component.scss']
})
export class CoursesComponent extends AppComponentBase implements OnInit {
  // list courses that
  courses: StudentCourseDto[] = [];//list courses: accepted and not archived
  archivedCourses: StudentCourseDto[] = [];//list courses: in Group or assign and archived
  invitedCourses: StudentCourseDto[] = []; //list courses: in Group or assign and not archived
  completedCourses: StudentCourseDto[] = [];

  constructor(
    injector: Injector,
    private _courseService: CoursesService
  ) { super(injector) }

  ngOnInit() {
    this.getAcceptedCourses();
    this.getArchivedCourses();
    this.getInvitedCourses();
    this.getCompletedCourses();
  }
  
  getAcceptedCourses(){
    this._courseService.GetAcceptedCourses().subscribe(result => {
      this.courses = result.result;
    })
  }

  getArchivedCourses(){
    this._courseService.GetArchivedCourses().subscribe(result => {
      this.archivedCourses = result.result;
    })
  }

  getInvitedCourses(){
    this._courseService.GetInvitedCourses().subscribe(result => {
      this.invitedCourses = result.result;
    })
  }

  getCompletedCourses(){
    this._courseService.GetCompletedCourses().subscribe(result => {
      this.completedCourses = result.result;
    })
  }

}

export class StudentCourseDto{
  courseId: string;
  courseInstanceId: string;
  name: string;
  relatedInformation: string;
  imageCover: string;
  completedPercent: number;
  state: number;
  link: string;
}