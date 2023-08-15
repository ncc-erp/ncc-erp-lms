import { Component, OnInit, Injector, Input } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { AssignmentDto as EditDto, AssignmentDto, AssignmentSettingsDto } from '@app/models/assignments-dto';
import { Router } from '@angular/router';
import { AssignmentsService } from '@app/services/systems-admin-services/assignments.service';
import { PagedRequestDto, PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { CourseGroupDto } from '@app/models/course-group-dto';
import { CourseGroupService } from '@app/services/systems-admin-services/course.group.service';
import { EAssignment } from '@shared/AppEnums';

@Component({
  selector: 'app-tab-assignment',
  templateUrl: './tab-assignment.component.html',
  styleUrls: ['./tab-assignment.component.scss']
})
export class TabAssignmentComponent extends PagedListingComponentBase<EditDto> {
  @Input() courseId: string;
  @Input() courseInstanceId: string;

  AssignmentListPanel = true;
  assignments: EditDto[] = [];
  assignmentItem = {} as EditDto;
  dropdownSettings = {
    singleSelection: false,
    idField: 'id',
    textField: 'groupName',
    selectAllText: 'Select All',
    unSelectAllText: 'UnSelect All',
    itemsShowLimit: 30,
    allowSearchFilter: true
  };
  dropdownListSources = [];
  dropdownListGroups = [];
  selectedGroupItems = [];
  displaygrade: any[] = EAssignment.DisplayGradeType;
  submissiontypes: any[] = EAssignment.SubmissionType;
  isAllowNotify = false;

  constructor(
    injector: Injector,
    private router: Router,
    private _assignmentService: AssignmentsService,
    private _courseGroupService: CourseGroupService,

  ) { super(injector); }


  getAsssignmentByCourseId() {
    this.assignments = [];
    this._assignmentService.GetAssignmentsByCourseId(this.courseId).subscribe((result) => {
      this.assignments = result.result;
    })
  }
  protected deleteAssignment(item: EditDto): void {
    abp.message.confirm(
      'Delete Assignment \'' + item.title + '\'?',
      (result: boolean) => {
        if (result) {
          this._assignmentService.delete(item.id).subscribe(() => {
            abp.notify.info('Deleted Assignment: ' + item.title);
            this.getAsssignmentByCourseId();
          });
        }
      }
    );
  }
  addNewAssignment() {
    // this.router.navigateByUrl('app/assignment/add/' + this.courseInstanceId);
    this.AssignmentListPanel = false;
    this.assignmentItem = {
      settings: { numberOfDueDays: 0, point: 1 } as AssignmentSettingsDto,
      displayGrade: 0,
      submissionType: 0
    } as AssignmentDto;
    this.dropdownListGroups = this.dropdownListSources.filter(r => r.isEveryOne);
    this.selectedGroupItems = this.dropdownListSources.filter(r => r.isEveryOne);
    this.onItemSelect(this.selectedGroupItems);
  }
  changePanel() {
    this.AssignmentListPanel = !this.AssignmentListPanel;
  }
  editAssignment(item: EditDto) {
    this.selectedGroupItems = [];
    this.AssignmentListPanel = false;
    this.assignmentItem = item;
    if (item.isGroupAssignment) {
      this.dropdownListGroups = this.dropdownListSources.filter(r => !r.isEveryOne);
    } else {
      this.dropdownListGroups = this.dropdownListSources.filter(r => r.isEveryOne);
    }
    if (this.assignmentItem.groupsAssingedAssignment != null) {
      this.selectedGroupItems = this.dropdownListGroups.filter(x => this.assignmentItem.groupsAssingedAssignment.indexOf(x.id) > -1);
    }
    this.onItemSelect(this.selectedGroupItems);
  }

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this.getcousegroupbyCourseId(this.courseInstanceId);
    this._assignmentService
      .GetAssignmentsByCourseIdPagging(request, this.courseId, this.courseInstanceId)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: any) => {
        this.assignments = result.result.items;
        this.showPaging(result.result, pageNumber);
      });
  }
  protected delete(item: EditDto): void {
    abp.message.confirm(
      'Delete assignment \'' + item.title + '\'?',
      (result: boolean) => {
        if (result) {
          this._assignmentService.delete(item.id).subscribe(() => {
            abp.notify.info('Deleted category: ' + item.title);
            this.refresh();
          });
        }
      }
    );
  }

  save() {
    this.assignmentItem.status = 0;
    this.saveData(false);
  }
  saveAndPublish() {
    this.assignmentItem.status = 1;
    const notify = this.isAllowNotify;
    this.saveData(notify);
  }
  saveData(notify: boolean) {
    this.assignmentItem.allowNotify = notify;
    this.assignmentItem.groupsAssingedAssignment = this.selectedGroupItems.map(({ id }) => id);
    if (this.assignmentItem.id == null) {
      this.assignmentItem.courseId = this.courseId;
      this.assignmentItem.settings.courseInstanceId = this.courseInstanceId;
      this._assignmentService.create(this.assignmentItem).subscribe((result: any) => {
        this.refresh();
        this.changePanel();
      })
    } else {
      this._assignmentService.update(this.assignmentItem).subscribe((result: any) => {
        this.refresh();
        this.changePanel();
      })
    }
  }
  backToList() {
    this.assignmentItem = {} as EditDto;
    this.refresh();
    this.AssignmentListPanel = true;
  }
  /* Group dropdown*/
  getcousegroupbyCourseId(courseInstanceId: string) {
    this._courseGroupService.getCourseGroupsByCourseId(courseInstanceId).subscribe((result) => {
      this.dropdownListGroups = result.result;
      this.dropdownListSources = result.result;
    })
  }
  onItemSelect(item: any) {
  }
  /* End Group dropdown*/
  onGroupAssignChanged() {
    abp.message.confirm(
      'This change may cause your data to be lost, are you sure want to continue ? ',
      (result: boolean) => {
        if (result) {
          this.selectedGroupItems = [];
          if (this.assignmentItem.isGroupAssignment) {
            this.dropdownListGroups = this.dropdownListSources.filter(r => !r.isEveryOne);
          } else {
            this.dropdownListGroups = this.dropdownListSources.filter(r => r.isEveryOne);
            this.selectedGroupItems = this.dropdownListSources.filter(r => r.isEveryOne);
          }
        } else {
          this.assignmentItem.isGroupAssignment = !this.assignmentItem.isGroupAssignment;
        }
      }
    );
  }
}
