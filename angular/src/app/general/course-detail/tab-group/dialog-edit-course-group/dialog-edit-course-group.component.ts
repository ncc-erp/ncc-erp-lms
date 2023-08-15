import { Component, OnInit, Inject, Injector } from '@angular/core';
import { CourseGroupDto } from '@app/models/course-group-dto';
import { MAT_DIALOG_DATA } from '@angular/material';
import { CourseGroupService } from '@app/services/systems-admin-services/course.group.service';
import { IResultObject } from '@app/models/common-dto';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-dialog-edit-course-group',
  templateUrl: './dialog-edit-course-group.component.html',
  styleUrls: ['./dialog-edit-course-group.component.scss']
})
export class DialogEditCourseGroupComponent extends AppComponentBase implements OnInit {
  
  active: boolean = true;
  saving: boolean = false;
  group = {} as CourseGroupDto;
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    injector: Injector,
    private _service: CourseGroupService) { super(injector); }

  ngOnInit() {
    this.group.id = this.data.group.id;
    this.group.name = this.data.group.groupName;
    this.group.courseInstanceId = this.data.courseInstanceId;
  }

  save(): void {
    this.saving = true;    
    this._service.update(this.group)
      .pipe(finalize(() => { this.saving = false; }))
      .subscribe((result: IResultObject) => {
        this.notify.info(this.l('SavedSuccessfully'));
         this.group = result.result;        
      });    
  }

}
