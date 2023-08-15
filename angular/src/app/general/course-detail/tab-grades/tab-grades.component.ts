import { Component, OnInit, Input, Injector, ɵConsole } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { GradeSchemeDto, CreateGradeSchemeDto as EditDto, GradeSchemeElementDto } from '@app/models/gradescheme-dto';
import { Router } from '@angular/router';
import { GradeSchemeService } from '@app/services/systems-admin-services/gradescheme.service';
import { PagedRequestDto, PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { GradeSchemeElementService } from '@app/services/systems-admin-services/gradeschemeelement.service';
import { EGradeScheme } from '@shared/AppEnums';

@Component({
  selector: 'app-tab-grades',
  templateUrl: './tab-grades.component.html',
  styleUrls: ['./tab-grades.component.scss']
})
export class TabGradesComponent extends PagedListingComponentBase<EditDto>  {
  @Input() courseInstanceId: string;
  @Input() courseId: string;

  GradeListPanel = true;
  ElementPanel = true;
  addNewElement = false;
  courseGrades: GradeSchemeDto[] = [];
  gradescheme = {} as EditDto;
  gradeElements: GradeSchemeElementDto[] = [];
  gradeElement = {} as GradeSchemeElementDto;
  lowcompare: any[] = EGradeScheme.LowGradeSchemeCompareOperation;
  highcompare: any[] = EGradeScheme.HighGradeSchemeCompareOperation;

  constructor(
    injector: Injector,
    private router: Router,
    private _gradeService: GradeSchemeService,
    private _gradeElementService: GradeSchemeElementService,

  ) {
    super(injector);
  }
  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this._gradeService
      .getGradesByCourseIdPagging(request, this.courseId)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: any) => {
        this.courseGrades = result.result.items;
        this.showPaging(result.result, pageNumber);
      });
  }
  protected delete(item: EditDto): void {
    abp.message.confirm(
      'Delete category \'' + item.title + '\'?',
      (result: boolean) => {
        if (result) {
          this._gradeService.delete(item.id).subscribe(() => {
            abp.notify.info('Deleted category: ' + item.title);
            this.refresh();
          });
        }
      }
    );
  }
  newGrade() {
    this.gradescheme = {} as EditDto;
    this.gradeElements = [];
    this.GradeListPanel = false;
  }
  editGrade(item: EditDto) {
    this.gradescheme = item;
    this.GradeListPanel = false;
    this.getElementsByGrade(this.gradescheme.id);
  }
  changeGradePanel() {
    this.GradeListPanel = !this.GradeListPanel;
  }
  changeElementPanel() {
    this.ElementPanel = !this.ElementPanel;
  }
  getGradesByCourseId(courseId: string) {
    this.courseGrades = [];
    this._gradeService.getGradesByCourseId(courseId).subscribe((result: any) => {
      this.courseGrades = result;
    })
  }
  saveGrade() {
    this.gradescheme.elementList = this.gradeElements;
    if (this.gradescheme.id == null) {
      this.gradescheme.courseId = this.courseId;
      this.gradescheme.status = 0;
      this.gradescheme.elementList = this.gradeElements;
      this._gradeService.create(this.gradescheme).subscribe((result: any) => {
        this.refresh();
        this.changeGradePanel();
      })
    } else {
      this._gradeService.update(this.gradescheme).subscribe((result: any) => {
        this.refresh();
        this.changeGradePanel();
      })
    }
  }
  newElement() {
    this.gradeElement = {} as GradeSchemeElementDto;
    this.gradeElement.lowCompareOperation = 2;
    this.gradeElement.highCompareOpertion = 0;
    this.addNewElement = true;
  }
  saveNewElement() {
    if (this.gradeElement.name === undefined || this.gradeElement.name === '') {
      abp.notify.warn('Please insert name');
      return;
    }
    if (this.gradeElement.lowRange === undefined || this.gradeElement.lowRange === null) {
      abp.notify.warn('Please insert low range');
      return;
    }
    if (this.gradeElement.highRange === undefined || this.gradeElement.highRange === null) {
      abp.notify.warn('Please insert high range');
      return;
    }
    if (Number(this.gradeElement.highRange) < Number(this.gradeElement.lowRange)) {
      abp.notify.warn('High range must bigger than low range');
      return;
    }
    if (Number(this.gradeElement.highRange) > 100 || Number(this.gradeElement.lowRange) > 100) {
      abp.notify.warn('Range must smaller than 100');
      return;
    }
    this.gradeElement.gradeSchemeId = this.gradescheme.id;
    this.gradeElement.editable = false;
    this.gradeElement.rowIndex = this.gradeElements.length;
    this.gradeElements.push(this.gradeElement);
    this.addNewElement = false;
    this.gradeElement = {} as GradeSchemeElementDto;
  }
  cancelNewElement() {
    this.addNewElement = false;
    this.gradeElement = {} as GradeSchemeElementDto;
  }
  editElement(item: GradeSchemeElementDto) {
    for (let i = 0; i < this.gradeElements.length; i++) {
      this.gradeElements[i].editable = false;
    }
    const index = this.gradeElements.indexOf(item);
    this.gradeElements[index].editable = true;
  }
  deleteElement(item: GradeSchemeElementDto) {
    const index = this.gradeElements.indexOf(item);
    this.gradeElements.splice(index, 1);
  }
  saveElement(item: GradeSchemeElementDto) {
    const index = this.gradeElements.indexOf(item);
    this.gradeElements[index].editable = false;
  }
  getElementsByGrade(gradeSchemeId: string) {
    this.gradeElements = [];
    this._gradeElementService.getElementsByGradeId(gradeSchemeId).subscribe((result: any) => {
      this.gradeElements = result.result;
    })
  }
  getOperationById(id: number) {
    switch (id) {
      case 0: {
        return EGradeScheme.HighGradeSchemeCompareOperation[0].name;
      }
      case 1: {
        return EGradeScheme.HighGradeSchemeCompareOperation[1].name;
      }
      case 2: {
        return EGradeScheme.LowGradeSchemeCompareOperation[0].name;
      }
      case 3: {
        return EGradeScheme.LowGradeSchemeCompareOperation[1].name;
      }
      default: {
        break;
      }
    }
  }
}
