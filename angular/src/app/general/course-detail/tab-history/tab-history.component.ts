import { Component, OnInit, Input, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CoursesService } from '@app/services/systems-admin-services/courses.service';
import { CourseDto } from '@app/models/courses-dto';
import { ICourseInstance } from '@app/models/student-dto';

@Component({
  selector: 'app-tab-history',
  templateUrl: './tab-history.component.html',
  styleUrls: ['./tab-history.component.scss']
})
export class TabHistoryComponent extends AppComponentBase implements OnInit {
  @Input() courseInstanceId: string;

  courseInstances: ICourseInstance[] = [];
  constructor(
    injector: Injector,
    private _courseService: CoursesService,
  ) { super(injector); }

  ngOnInit() {
    this.getCourse();
  }
  getCourse() {
    this._courseService.getCourseInstancesHistory(this.courseInstanceId).subscribe((result) => {
      this.courseInstances = result.result;
    })
  }
}
