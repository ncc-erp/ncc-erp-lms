import { Component, OnInit, Injector, Input } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CourseGroupService } from '@app/services/systems-admin-services/course.group.service';
import { CourseGroupWithMemberDto, StudentCourseGroupListDto, StudentCourseGroupDto, CourseGroupDto } from '@app/models/course-group-dto';
import { DndDropEvent, DropEffect } from 'ngx-drag-drop';
import { MatSnackBar, MatDialog } from '@angular/material';
import { DialogCreateCourseGroupComponent } from './dialog-create-course-group/dialog-create-course-group.component';
import { DialogEditCourseGroupComponent } from './dialog-edit-course-group/dialog-edit-course-group.component';
import { IResultObject } from '@app/models/common-dto';

@Component({
  selector: 'app-tab-group',
  templateUrl: './tab-group.component.html',
  styleUrls: ['./tab-group.component.scss']
})
export class TabGroupComponent extends AppComponentBase implements OnInit {
  @Input() courseId: string;
  @Input() courseInstanceId: string;
  groupsStudents: CourseGroupWithMemberDto[];
  unAssignedStudents: StudentCourseGroupListDto[];
  private currentDragEffectMsg: string;
  constructor(
    injector: Injector,
    private snackBarService: MatSnackBar,
    private _courseGroupService: CourseGroupService,
    public dialog: MatDialog
  ) {
    super(injector);
  }

  ngOnInit() {
    this.reloadGroupStudents();
  }
  /*---- tab Group----*/

  getCourseGroupsWithStudents() {
    this._courseGroupService.getAllCourseGroupByCourse(this.courseInstanceId).subscribe((result) => {
      this.groupsStudents = result.result;
    })
  }

  getUnAssignedStudents() {
    this._courseGroupService.getUnAssignedStudents(this.courseInstanceId).subscribe((result) => {
      this.unAssignedStudents = result.result;
    })
  }

  reloadGroupStudents() {
    this.getCourseGroupsWithStudents();
    this.getUnAssignedStudents();
  }


  saveStudentCourseGroups() {
    const listToSave: StudentCourseGroupDto[] = [];
    this.groupsStudents.forEach(group => {
      const scg = new StudentCourseGroupDto();
      scg.courseGroupId = group.id;
      scg.assignedStudentIds = [];
      group.students.forEach(stu => {
        scg.assignedStudentIds.push(stu.assignedStudentId);
      })
      listToSave.push(scg);
    });
    this._courseGroupService.saveStudentCourseGroups(listToSave).subscribe(() => {
      this.notify.info('SavedSuccessfully');
    })
  }

  onCreatedCourseGroup(cgroup: CourseGroupDto) {
    const obj = {} as CourseGroupWithMemberDto;
    obj.groupName = cgroup.name;
    obj.id = cgroup.id;
    obj.students = [];
    this.groupsStudents.push(obj);
  }

  onEditedCourseGroup(cgroup: CourseGroupDto) {
    for (let i = 0; i < this.groupsStudents.length; i++) {
      if (this.groupsStudents[i].id === cgroup.id) {
        this.groupsStudents[i].groupName = cgroup.name;
        return;
      }
    }
  }

  createCourseGroup() {
    const dialogRef = this.dialog.open(DialogCreateCourseGroupComponent, {
      data: { courseInstanceId: this.courseInstanceId },
    });

    dialogRef.afterClosed().subscribe(group => {
      if (group) {
        this._courseGroupService.create(group)
          .subscribe((result: IResultObject) => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.onCreatedCourseGroup(result.result);
          });
      }
    });
    // this.createCourseGroupModal.show();
  }

  editCourseGroup(item: CourseGroupWithMemberDto) {
    const dialogRef = this.dialog.open(DialogEditCourseGroupComponent, {
      data: { courseInstanceId: this.courseInstanceId, group: item },
    });

    dialogRef.afterClosed().subscribe(group => {
      if (group) {
        this._courseGroupService.update(group)
          .subscribe((result: IResultObject) => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.onEditedCourseGroup(result.result);
          });
      }
    });
    // this.editCourseGroupModal.show(item);
  }

  deleteCourseGroup(item: CourseGroupWithMemberDto): void {
    abp.message.confirm(
      'Delete category ' + item.groupName + '?',
      (result: boolean) => {
        if (result) {
          this._courseGroupService.delete(item.id).subscribe(() => {
            abp.notify.info('Deleted group: ' + item.groupName);
            for (let i = 0; i < this.groupsStudents.length; i++) {
              if (this.groupsStudents[i].id === item.id) {
                this.groupsStudents.splice(i, 1);
                return;
              }
            }
          });
        }
      }
    );
  }

  /* ---end tab Group-----*/
  onDragStart(event: DragEvent) {
    this.currentDragEffectMsg = '';
    this.snackBarService.dismiss();
    this.snackBarService.open('Drag started!', undefined, { duration: 2000 });
  }

  onDragged(item: any, list: any[], effect: DropEffect) {
    if (effect === 'move') {
      const index = list.indexOf(item);
      list.splice(index, 1);
    }
  }


  onDrop(event: DndDropEvent, list?: any[]) {
    if (list
      && (event.dropEffect === 'copy'
        || event.dropEffect === 'move')) {

      let index = event.index;
      if (typeof index === 'undefined') {
        index = list.length;
      }
      list.splice(index, 0, event.data);
    }
  }

  onDragEnd(event: DragEvent) {
    this.snackBarService.dismiss();
    this.snackBarService.open(this.currentDragEffectMsg || `Drag ended!`, undefined, { duration: 2000 });
  }

}
