import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ICourseDashboard } from '@app/models/student-dto';
import { AppConsts } from '@shared/AppConsts';
import { BaseService } from '@app/services/base-service/base.service';
import { AssignedStatus } from '@shared/AppEnums';
import { CoursesService } from '@app/services/systems-admin-services/courses.service';

@Component({
  selector: 'app-view-course-item',
  templateUrl: './view-course-item.component.html',
  styleUrls: ['./view-course-item.component.scss']
})
export class ViewCourseItemComponent implements OnInit {
  @Input() item: ICourseDashboard;
  @Input() soure: string;
  @Input() isArchived: boolean;

  @Output() republishResult: EventEmitter<any> = new EventEmitter<any>();
  list = false;
  constructor(
    private _base: BaseService,
    private _courseService: CoursesService
  ) { }

  ngOnInit() {
    this.list = this._base._studentService.getList();
    this._base._studentService.isList$.subscribe((e) => {
      this.list = e;
    });
    if (this.soure === 'student') {
      // if (this.item.status === AssignedStatus.Accepted && this.item.isLearning) {
        this.item.link = '/app/student/course/' + this.item.id;
      // } else {
      //   this.item.link = '/app/student/course-detail/' + this.item.id;
      // }
    } else {
      this.item.link = '/app/course/' + this.item.id;
    }
  }



  public getImageServerPath(imageCover: string) {
    return AppConsts.remoteServiceBaseUrl + '/' + imageCover;
  }
  rePublishedCourse(item: ICourseDashboard) {
    this._courseService.RePublishCourse(item.id).subscribe((result) => {
      this.republishResult.emit(true);
    });
  }
  delteCourse(item: ICourseDashboard) {
    this._courseService.CheckCourseInfo(item.id).subscribe((result) => {
      var message = 'Are you sure want to delete course \'' + item.name + '\'?';
      if (result.result.alreadyStart) {
        message = 'This change may cause your data to be lost. Are you sure want to delete course \'' + item.name + '\'?';
      }
      abp.message.confirm(
        message,
        (result: boolean) => {
          if (result) {
            this._courseService.deleteCourse(item.id).subscribe((result) => {
              this.republishResult.emit(true);
            });
          }
        }
      );
    });
  }
}
