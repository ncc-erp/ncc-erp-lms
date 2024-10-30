import { Component, OnInit, Inject, Injector, Input } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MAT_DIALOG_DATA } from '@angular/material';
import { CourseGroupDto } from '@app/models/course-group-dto';
import { CourseGroupService } from '@app/services/systems-admin-services/course.group.service';
import { finalize } from 'rxjs/operators';
import { IResultObject } from '@app/models/common-dto';

@Component({
  selector: 'app-dialog-create-course-group',
  templateUrl: './dialog-create-course-group.component.html',
  styleUrls: ['./dialog-create-course-group.component.scss']
})
export class DialogCreateCourseGroupComponent extends AppComponentBase implements OnInit {

  active: boolean = true;
  saving: boolean = false;
  group = {} as CourseGroupDto;
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    injector: Injector,
    private _service: CourseGroupService) { super(injector); }

  ngOnInit() {
    this.group.courseInstanceId = this.data.courseInstanceId;
  }

  save(): void {
    this.saving = true;
    this.group.courseInstanceId = this.data.courseInstanceId;
    this._service.create(this.group)
      .pipe(finalize(() => { this.saving = false; }))
      .subscribe((result: IResultObject) => {
        this.notify.info(this.l('SavedSuccessfully'));
         this.group = result.result;
      },
      error =>{
        console.log("error");
        this.group = null;
      }
      );
  }

}
