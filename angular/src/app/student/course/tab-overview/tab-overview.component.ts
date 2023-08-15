import { Component, OnInit, Input, Injector } from '@angular/core';
import { CourseDetailDto, EditCourseDto, IUserDto } from '@app/models/courses-dto';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-tab-overview',
  templateUrl: './tab-overview.component.html',
  styleUrls: ['./tab-overview.component.scss']
})
export class TabOverviewComponent extends AppComponentBase implements OnInit {
  @Input() courseDetail = { creator: {} } as CourseDetailDto;

  // course = {} as EditCourseDto;


  constructor(
    injector: Injector
  ) { super(injector) }

  ngOnInit() {
    // this.course = this.courseDetail.course;
    // this.courseDetail.creator = {} as IUserDto;
  }

}
