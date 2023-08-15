import { Component, OnInit, Input, Output, Injector, OnChanges, OnDestroy, EventEmitter, SimpleChanges } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { QuestionDto } from '@app/models/question-dto';
import { AssignmentDto } from '@app/models/assignments-dto';
import { AssignmentsService } from '@app/services/systems-admin-services/assignments.service';
import { EAssignment } from '@shared/AppEnums';

@Component({
  selector: 'app-student-assignment',
  templateUrl: './student-assignment.component.html',
  styleUrls: ['./student-assignment.component.scss']
})
export class StudentAssignmentComponent extends PagedListingComponentBase<QuestionDto> implements OnInit, OnChanges, OnDestroy {

  @Input() assignmentSettingId: string;

  @Output() status: EventEmitter<any> = new EventEmitter<any>();
  assignment = {} as AssignmentDto;
  submissiontypes: any[] = EAssignment.SubmissionType;

  constructor(
    injector: Injector,
    private _assignmentService: AssignmentsService
  ) { super(injector); }

  ngOnInit() {
  }
  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {

  }
  protected delete(entity: QuestionDto) {

  }
  ngOnChanges(changes: SimpleChanges): void {
    this.getAssignment(); 
  }
  ngOnDestroy() {
    this.assignmentSettingId = null;
  }
  getAssignment() {
    this._assignmentService.getByAssignmentSettingId(this.assignmentSettingId).subscribe(result => {
      this.assignment = result.result;
    });
  }
  getSubmissionType(type: number) {
    const result = this.submissiontypes.filter(i => i.id === type);
    if (result.length > 0) {
      return result[0].name;
    }
  }
}
