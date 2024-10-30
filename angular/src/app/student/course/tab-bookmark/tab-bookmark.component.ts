import { AppComponentBase } from '@shared/app-component-base';
import { BookMarkDto } from './../../../models/bookmark-dto';
import { Component, OnInit, Input, Injector } from '@angular/core';
import { CourseSettingFeatualValueDto } from '@app/models/course-setting-dto';
import { StudentService } from '@app/services/student-service/student.service';

@Component({
  selector: 'app-tab-bookmark',
  templateUrl: './tab-bookmark.component.html',
  styleUrls: ['./tab-bookmark.component.scss']
})
export class TabBookmarkComponent extends AppComponentBase implements OnInit {
  @Input() courseInstanceId: string;
  @Input() courseId: string;
  @Input() courseSetting: CourseSettingFeatualValueDto;

  public bookMarks: BookMarkDto[];
  constructor(injector: Injector,
    private _studentService: StudentService,
  ) {
    super(injector);
  }

  ngOnInit() {
    this.getsBookMars();
  }
  getsBookMars() {
    this._studentService.GetsUserBookMark(this.courseInstanceId)
      .subscribe(data => {
        //console.log('data', data);
        if (data.success) {
          this.bookMarks = data.result.items;
        }
      });
  }

  onModule_Click(item: BookMarkDto) {
    //console.log('onModule_Click', item);

  }
  onPage_Click(item: BookMarkDto) {
    //console.log('onPage_Click', item);
  }
  onView_Click(item: BookMarkDto) {
    //console.log('onView_Click', item);
  }

}
