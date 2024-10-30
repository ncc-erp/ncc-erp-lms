import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { BaseService } from '@app/services/base-service/base.service';
import { map, count } from 'rxjs/operators';
import { UserStatusDto } from '@app/models/user-status-dto';
import { CompareOperation } from '@shared/AppEnums';
import { e } from '@angular/core/src/render3';
import { DateAdapter } from '@angular/material';
import { CouseLevelDto } from '@app/models/couse-level-dto';

@Component({
  selector: 'app-setting',
  templateUrl: './setting.component.html',
  styleUrls: ['./setting.component.scss']
})
export class SettingComponent extends AppComponentBase implements OnInit {
  studentLevel: FormGroup;
  courseLevel: FormGroup;
  levelArray: FormArray;
  courseLevelArray: FormArray;
  userStatus: UserStatusDto[] = [];
  courseLevelDto: CouseLevelDto[] = [];
  get getFormArray() {
    return <FormArray>this.studentLevel.get('levelForm');
  }
  get getCourseFormArray() {
    return <FormArray>this.courseLevel.get('levelForm');
  }
  constructor(injector: Injector, private fb: FormBuilder, private _base: BaseService) {
    super(injector);
  }

  compareOperations = [
    {
      id: CompareOperation.LessEqual,
      value: '<='
    },
    {
      id: CompareOperation.LessThan,
      value: '<'
    },
    {
      id: CompareOperation.GreaterEqual,
      value: '>='
    },
    {
      id: CompareOperation.GreaterThan,
      value: '>'
    },
    {
      id: CompareOperation.Equal,
      value: '='
    },
  ];

  studentCompareOperations = [
    {
      id: CompareOperation.GreaterEqual,
      value: '>='
    },
    {
      id: CompareOperation.GreaterThan,
      value: '>'
    }
  ];


  ngOnInit() {
    this.studentLevel = this.fb.group(
      {
        levelForm: this.fb.array([])
      }
    );
    this.courseLevel = this.fb.group({
      levelForm: this.fb.array([])
    });
    this.getStudentLevel();
    this.getCourseLevel();
  }

  /*
  ********************************
    Student Level
  ********************************
  */
  getStudentLevel() {
    this._base._userStatusService.getAllNotPagging().pipe(map(m => m.result.items)).subscribe(s => {
      this.userStatus = s;

      for (const item of s) {
        const newItem = this.addLevelForm();
        const lowCompare = item.lowCompareOperation;
        const requiredStudent = item.requiredNumber;
        let value = this.onSetValueOfRequiredLevel(lowCompare, requiredStudent);
        newItem.patchValue({
          id: item.id,
          displayName: item.displayName,
          level: item.level,
          lowCompareOperation: item.lowCompareOperation,
          requiredNumberOld: item.requiredNumber,
          requiredNumber: value,
          isEdit: false
        });
        this.getFormArray.push(newItem);
      }
    });
  }


  onDelete(i) {
    this._base._userStatusService.CheckDelete(this.getFormArray.at(i).value.id).subscribe(countItem => {
      if (countItem.result > 0) {
        abp.message.confirm(
          'Student Level: <b style="color: red">' + this.getFormArray.at(i).value.level + ' - '
          + this.getFormArray.at(i).value.displayName + ' is using for ' + countItem.result + ' student. </b>', 'Are you sure delete?',
          (result: boolean) => {
            if (result) {
              this.deleteUserStatus(i);
            }
          }, true
        );
      } else {
        abp.message.confirm(
          'Student Level: <b> ' + this.getFormArray.at(i).value.level + ' - '
          + this.getFormArray.at(i).value.displayName + '</b>', 'Are you sure delete?',
          (result: boolean) => {
            if (result) {
              this.deleteUserStatus(i);
            }
          }, true
        );
      }
    });
  }

  private deleteUserStatus(i: number) {
    this._base._userStatusService.delete(this.getFormArray.at(i).value.id).subscribe(ok => {
      this.getFormArray.removeAt(i);
      abp.notify.success('Delete Success !')
    });
  }

  onHidden(i): boolean {
    return this.getFormArray.at(i).get('isEdit').value;
  }

  onCancel(i) {
    let id = this.getFormArray.at(i).get('id').value;
    let value = '';
    if (id == '00000000-0000-0000-0000-000000000000') {
      this.getFormArray.removeAt(i);
    }
    else {
      const lowCompare = this.getFormArray.at(i).get('lowCompareOperation').value;
      const requiredStudent = this.getFormArray.at(i).get('requiredNumberOld').value;
      let user = this.userStatus.find(s => s.id == id);
      this.getFormArray.at(i).patchValue({
        displayName: user.displayName,
        level: user.level,
        lowCompareOperation: user.lowCompareOperation,
      });
      let value = this.onSetValueOfRequiredLevel(lowCompare, requiredStudent);
      if (requiredStudent != 0) {
        this.getFormArray.at(i).get('requiredNumber').setValue(value);
      }
      this.getFormArray.at(i).get('isEdit').setValue(false);
    }
  }

  onSave(i) {
    const data = this.getFormArray.at(i).value;
    if (data.id != '00000000-0000-0000-0000-000000000000') {
      this._base._userStatusService.update(data).subscribe(() => {
        abp.notify.success('Save Success');
        const lowCompare = this.getFormArray.at(i).get('lowCompareOperation').value;
        const requiredStudent = this.getFormArray.at(i).get('requiredNumber').value;
        let value = this.onSetValueOfRequiredLevel(lowCompare, requiredStudent);
        this.studentLevel = this.fb.group(
          {
            levelForm: this.fb.array([])
          }
        );
        this.getStudentLevel();
      })
    }
    else {
      this._base._userStatusService.create(data).pipe(map(m => m.result)).subscribe((e) => {
        abp.notify.success('Save Success');
        const lowCompare = this.getFormArray.at(i).get('lowCompareOperation').value;
        const requiredStudent = this.getFormArray.at(i).get('requiredNumber').value;
        let value = this.onSetValueOfRequiredLevel(lowCompare, requiredStudent);
        this.studentLevel = this.fb.group(
          {
            levelForm: this.fb.array([])
          }
        );
        this.getStudentLevel();
      })
    }
  }

  onEnableEdit(i) {
    let required = this.getFormArray.at(i).get('requiredNumber').value;
    if (required != 0) {
      this.getFormArray.at(i).get('requiredNumber').setValue(this.getFormArray.at(i).get('requiredNumberOld').value);
    }
    this.getFormArray.at(i).get('isEdit').setValue(true);
  }


  addLevelForm(): FormGroup {
    return this.fb.group({
      id: ['00000000-0000-0000-0000-000000000000'],
      isEdit: true,
      level: ['', Validators.required],
      displayName: ['', Validators.required],
      requiredNumberOld: [0],
      requiredNumber: ['', Validators.required],
      lowCompareOperation: [0, Validators.required]
    });
  }

  addLevel() {
    this.levelArray = this.studentLevel.get('levelForm') as FormArray;
    this.levelArray.push(this.addLevelForm());
  }

  /*
  ********************************
    COURSE LEVEL
  *******************************
   */
  addCourseLevelForm(): FormGroup {
    return this.fb.group({
      id: ['00000000-0000-0000-0000-000000000000'],
      isEdit: true,
      level: ['', Validators.required],
      displayName: ['', Validators.required],
      requiredStudentLevelOld: [0],
      requiredStudentLevel: ['', Validators.required],
      lowCompareOperation: [0, Validators.required]
    });
  }

  addCourseLevel() {
    this.courseLevelArray = this.courseLevel.get('levelForm') as FormArray;
    this.courseLevelArray.push(this.addCourseLevelForm());
  }

  getCourseLevel() {
    this._base._courseLevelService.getAll().pipe(map(m => m.result)).subscribe(s => {
      this.courseLevelDto = s;
      //console.log('data', s);
      let value = '';
      for (const item of s) {
        const newItem = this.addCourseLevelForm();
        const lowCompare = item.lowCompareOperation;
        const requiredStudent = item.requiredStudentLevel;
        let value = this.onSetValueOfRequiredLevel(lowCompare, requiredStudent);
        newItem.patchValue({
          id: item.id,
          displayName: item.displayName,
          level: item.level,
          isEdit: false,
          requiredStudentLevel: value,
          requiredStudentLevelOld: item.requiredStudentLevel,
          lowCompareOperation: item.lowCompareOperation
        });
        this.getCourseFormArray.push(newItem);
      }
    })
  }

  onDeleteCourse(i) {
    this._base._courseLevelService.CheckDeleteCourseLevel(this.getCourseFormArray.at(i).value.id).subscribe(countItem => {
      if (countItem.result > 0) {
        abp.message.confirm(
          'Course Level: <b style="color: red">' + this.getCourseFormArray.at(i).value.level + ' - '
          + this.getCourseFormArray.at(i).value.displayName + ' is using for '
          + countItem.result + ' course. </b>', 'Are you sure delete?',
          (result: boolean) => {
            if (result) {
              this.deleteCourseLevel(i);
            }
          }, true
        );
      } else {
        abp.message.confirm(
          'Course Level: <b> ' + this.getCourseFormArray.at(i).value.level + ' - '
          + this.getCourseFormArray.at(i).value.displayName + '</b>', 'Are you sure delete?',
          (result: boolean) => {
            if (result) {
              this.deleteCourseLevel(i);
            }
          }, true
        );
      }
    });
  }
  private deleteCourseLevel(i: number) {
    this._base._courseLevelService.delete(this.getCourseFormArray.at(i).value.id).subscribe(() => {
      this.getCourseFormArray.removeAt(i);
      abp.notify.success('Delete Success !')
    });
  }
  onHiddenCourse(i): boolean {
    return this.getCourseFormArray.at(i).get('isEdit').value;
  }

  onCancelCourse(i) {
    let id = this.getCourseFormArray.at(i).get('id').value;
    let value = '';
    if (id == '00000000-0000-0000-0000-000000000000') {
      this.getCourseFormArray.removeAt(i);
    }
    else {
      const lowCompare = this.getCourseFormArray.at(i).get('lowCompareOperation').value;
      const requiredStudent = this.getCourseFormArray.at(i).get('requiredStudentLevelOld').value;
      let value = this.onSetValueOfRequiredLevel(lowCompare, requiredStudent);
      let course = this.courseLevelDto.find(s => s.id == id);
      if (course) {
        this.getCourseFormArray.at(i).patchValue({
          displayName: course.displayName,
          level: course.level,
          lowCompareOperation: course.lowCompareOperation
        });
      }
      if (requiredStudent != 0) {
        this.getCourseFormArray.at(i).get('requiredStudentLevel').setValue(value);
      }
      this.getCourseFormArray.at(i).get('isEdit').setValue(false);
    }
  }

  onSaveCourse(i) {
    const data = this.getCourseFormArray.at(i).value;
    let item: string = data.requiredStudentLevel;
    if (data.id != '00000000-0000-0000-0000-000000000000') {
      this._base._courseLevelService.update(data).subscribe(() => {
        abp.notify.success('Save Success');
        const lowCompare = this.getCourseFormArray.at(i).get('lowCompareOperation').value;
        const requiredStudent = this.getCourseFormArray.at(i).get('requiredStudentLevel').value;
        let value = this.onSetValueOfRequiredLevel(lowCompare, requiredStudent);
        this.courseLevel = this.fb.group(
          {
            levelForm: this.fb.array([])
          }
        );
        this.getCourseLevel();
      })
    }
    else {
      this._base._courseLevelService.create(data).pipe(map(m => m.result)).subscribe((e) => {
        abp.notify.success('Save Success');
        const lowCompare = data.lowCompareOperation;
        const requiredStudent = data.requiredStudentLevel;
        let value = this.onSetValueOfRequiredLevel(lowCompare, requiredStudent);
        this.courseLevel = this.fb.group(
          {
            levelForm: this.fb.array([])
          }
        );
        this.getCourseLevel();
      })
    }
  }

  onSetValueOfRequiredLevel(lowCompareOperation, requiredStudentLevel): string {
    let value = ''
    if (lowCompareOperation == CompareOperation.LessEqual && requiredStudentLevel != null) {
      return value = '<= ' + requiredStudentLevel;
    }
    else if (lowCompareOperation == CompareOperation.LessThan && requiredStudentLevel != null) {
      return value = '< ' + requiredStudentLevel;
    }
    else if (lowCompareOperation == CompareOperation.GreaterEqual && requiredStudentLevel != null) {
      return value = '>= ' + requiredStudentLevel;
    }
    else if (lowCompareOperation == CompareOperation.GreaterThan && requiredStudentLevel != null) {
      return value = '> ' + requiredStudentLevel;
    }
    else if (lowCompareOperation == CompareOperation.Equal && requiredStudentLevel != null) {
      return value = '= ' + requiredStudentLevel;
    }
    else {
      return value = '';
    }
  }

  onEnableEditCourse(i) {
    let required = this.getCourseFormArray.at(i).get('requiredStudentLevelOld').value;
    if (required != 0) {
      this.getCourseFormArray.at(i).get('requiredStudentLevel').setValue(this.getCourseFormArray.at(i).get('requiredStudentLevelOld').value);
    }
    this.getCourseFormArray.at(i).get('isEdit').setValue(true);
  }
}
