import { Component, OnInit, Input, Injector } from '@angular/core';
import { StudentCourseDto } from '../courses.component';
import { AppConsts } from '@shared/AppConsts';
import { AssignedStatus } from '@shared/AppEnums';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-student-course-view-item',
  templateUrl: './student-course-view-item.component.html',
  styleUrls: ['./student-course-view-item.component.scss']
})
export class StudentCourseViewItemComponent extends AppComponentBase implements OnInit {
  @Input() item: StudentCourseDto;
  courseInstanceId = "";

  constructor(injecter: Injector) {
    super(injecter)
  }

  ngOnInit() {
    this.courseInstanceId = this.item.courseInstanceId;
    if (this.item.state == AssignedStatus.Accepted){
      this.item.link = "/app/student/course-detail/" + this.item.courseInstanceId;
    }else{
      this.item.link = "/app/student/course/" + this.item.courseInstanceId;
    }
  }

   public getImageServerPath(imageCover: string) {
    return AppConsts.remoteServiceBaseUrl + '/' + imageCover;
  }  
  logStudentProcess(){
    let message = `select course ${this.item.name}`
    let args = {
      courseName: this.item.name,
      courseId: this.item.courseId,
      viewTime: this.formatTime(new Date())
    }
    this.logStudentProcessToSentry(message, args);
  }
}
