import { Component, OnInit, Input, ViewChild, ElementRef, Output, Injector, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { UserAssignedToCourseService, StudentsToCourseDto } from '@app/services/systems-admin-services/user.assigned.to.course.service';
import { PagedListingComponentBase, PagedRequestDto, PagedResultResultDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { InputFilterDto } from '@shared/filter/filter.component';
import { CoursesService } from '@app/services/systems-admin-services/courses.service';

@Component({
  selector: 'create-student-modal',
  templateUrl: './create-student.component.html',
  styleUrls: ['./create-student.component.scss']
})
export class CreateStudentComponent extends PagedListingComponentBase<any> implements OnInit {
  @Input() courseInstanceId: string;
  @ViewChild('createStudentModal') modal: ModalDirective;
  @ViewChild('modalContent') modalContent: ElementRef;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  active = false;
  saving = false;
  students: SelectableStudentDto[] = [];
  checkedAll = false;

  dropdownList = [];
  selectedItems = [];
  dropdownSettings = {
    singleSelection: false,
    idField: 'id',
    textField: 'displayName',
    selectAllText: 'Select All',
    unSelectAllText: 'UnSelect All',
    itemsShowLimit: 30,
    allowSearchFilter: true
  };
  public readonly FILTER_CONFIG: InputFilterDto[] = [
    { propertyName: 'DisplayName', comparisions: [0, 6, 7, 8] }];


  constructor(
    injector: Injector,
    private _service: UserAssignedToCourseService,
    private _courseService: CoursesService
  ) {
    super(injector);
  }

  ngOnInit() {
  }

  onCheckedAllChange() {
    this.students.forEach(element => {
      element.checked = this.checkedAll
    });

  }

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this._courseService
      .getUnAssingedStudents(this.courseInstanceId, request)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: PagedResultResultDto) => {
        this.students = result.result.items;
        this.showPaging(result.result, pageNumber);
        this.active = true;
        this.modal.show();
      });
  }

  protected delete(item: any): void {
  }


  show(): void {
    this.refresh();
    // if (this.students && this.students.length > 0){
    //   this.students.forEach(stu => {
    //     stu.checked = false;
    //   })
    // }
  }

  onShown(): void {
    $.AdminBSB.input.activate($(this.modalContent.nativeElement));
  }

  save(): void {
    this.saving = true;
    const studentToCourse = new StudentsToCourseDto();
    studentToCourse.students = [];
    studentToCourse.studentNames = [];
    studentToCourse.courseInstanceId = this.courseInstanceId;
    this.students.forEach(stu => {

      if (stu.checked) {
        // let item = new CreateUpdateCourseAssignedStudentDto();
        // item.courseInstanceId = this.courseInstanceId;
        // item.studentId = stu.id;
        // item.status = AssignedStatus.Invited;
        // item.assignedSource = AssignedSource.Direct;
        studentToCourse.students.push(stu.id);
        studentToCourse.studentNames.push(stu.name);
      }

    })

    this._service.addStudentsToCourse(studentToCourse)
      .pipe(finalize(() => { this.saving = false; }))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }



}

export class SelectableStudentDto {
  id: number;
  displayName: string;
  checked: boolean;
  name: string;
}
