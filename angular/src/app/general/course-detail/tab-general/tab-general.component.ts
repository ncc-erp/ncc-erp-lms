import { Component, OnInit, Input, Injector, Output, EventEmitter } from '@angular/core';
import { EditCourseDto, ICourseStatusDto, ICourseTypeDto } from '@app/models/courses-dto';
import { CourseSettingDto } from '@app/models/course-setting-dto';
import { AppComponentBase } from '@shared/app-component-base';
import { CoursesService } from '@app/services/systems-admin-services/courses.service';
import { CourseSettingService } from '@app/services/systems-admin-services/course.setting.service';
import { BaseService } from '@app/services/base-service/base.service';
import { LanguageDto } from '@app/models/language-dto';
import { map } from 'rxjs/operators';
import { UserExtralRoleService, CourseAdminsToCourseDto } from '@app/services/systems-admin-services/user.extra.role.service';
import { NumberIdNameDto } from '@app/models/common-dto';
import { GradeSchemeDto } from '@app/models/gradescheme-dto';
import { GradeSchemeService } from '@app/services/systems-admin-services/gradescheme.service';

@Component({
  selector: 'app-tab-general',
  templateUrl: './tab-general.component.html',
  styleUrls: ['./tab-general.component.scss']
})
export class TabGeneralComponent extends AppComponentBase implements OnInit {
  // @Input() courseId: string;
  @Input() courseInstanceId: string;
  @Input() courseId: string;
  @Input() dropdownList: any[];
  @Output() stateOut: EventEmitter<number> = new EventEmitter<number>();
  @Output() sourseOut: EventEmitter<number> = new EventEmitter<number>();

  course: EditCourseDto = new EditCourseDto();
  courseSetting = {} as CourseSettingDto;
  levels: ICourseStatusDto[] = [];
  types: ICourseTypeDto[] = [{ id: 0, displayName: 'Recur' }, { id: 1, displayName: 'Perpetual' }];
  languages: LanguageDto[] = [];
  courseStates: NumberIdNameDto[] = [{ id: 0, name: 'Draft' }, { id: 1, name: 'Publish' }];

  progress: number;
  uploadMessage: string;
  imgBase64Value: string;
  gradeSchemes: GradeSchemeDto[] = [];
  relatedImgBase64Value: string;
  selectedItems = [];
  dropdownSettings = {
    singleSelection: false,
    idField: 'userId',
    textField: 'userName',
    selectAllText: 'Select All',
    unSelectAllText: 'UnSelect All',
    itemsShowLimit: 10,
    allowSearchFilter: true
  };
  initTinymce = {
    height: 400
  }
  constructor(
    injector: Injector,
    private _courseService: CoursesService,
    private _courseSettingService: CourseSettingService,
    private _baseService: BaseService,
    private _userExtraRoleService: UserExtralRoleService,
    private _gradeSchemeService: GradeSchemeService,
  ) { super(injector); }

  ngOnInit() {
    this.selectedItems = this.dropdownList.filter(i => i.isSelected);
    this.getGrades(this.courseId);
    this.getCourse();
    this.getCourseInstance();
    this.getCourseLevels();
    this.getAllLanguages();
  }

  //#region tab course-detail
  getAllLanguages() {
    this._baseService._studentService.getAllLanguage().pipe(map(e => e.result)).subscribe(result => {
      this.languages = result;
    });
  }
  getCourseInstance() {
    this._courseSettingService.getById(this.courseInstanceId).subscribe(result => {
      if (result.result === null) {
        this.courseSetting = new CourseSettingDto();
        this.courseSetting.courseId = this.course.id;
      } else {
        this.courseSetting = result.result;
        this.courseSetting.startTime = this.getDateLocal(this.courseSetting.startTime);
        this.courseSetting.endTime = this.getDateLocal(this.courseSetting.endTime);
      }
    });
  }
  getCourse() {
    this._courseService.getByCourseInstanceId(this.courseInstanceId).subscribe(item => {
      this.course = item.result;
      this.stateOut.emit(this.course.state);
      this.sourseOut.emit(this.course.sourse);
    })
  }

  onCourseStateChange() {
    this.stateOut.emit(this.course.state);
  }
  getCourseLevels() {
    this._courseService.getAllStatus().subscribe(result => {
      this.levels = result.result.items;
    });
  }

  getImgBase64(value) {
    if (this.course.file) {
      this.imgBase64Value = value;
    }
  }

  getFileInfo(fileInfo: File) {
    if (fileInfo.type.includes('image')) {
      this.course.file = fileInfo;
    } else {
      this.notify.error('The format must be of image');
    }
  }

  getRelatedImgBase64(value) {
    if (this.course.relatedFile) {
      this.relatedImgBase64Value = value;
    }
  }

  getRelatedFileInfo(fileInfo: File) {
    if (fileInfo.type.includes('image')) {
      this.course.relatedFile = fileInfo;
    } else {
      this.notify.error('The format must be of image');
    }
  }

  saveCourse() {
    this._courseService.updateFormData(this.course).subscribe(() => {
      this.notify.info(this.l('SavedSuccessfully'));
    });
    this.saveCourseInstance();
  }

  saveCourseInstance() {
    !this.courseSetting.courseId && (this.courseSetting.courseId = this.course.id);
    this.courseSetting.startTime = this.getDateUTC(this.courseSetting.startTime);
    this.courseSetting.endTime = this.getDateUTC(this.courseSetting.endTime);

    this._courseSettingService.save(this.courseSetting).subscribe((result) => {
      result && result.result && (this.courseSetting = result.result);
      this.notify.info(this.l('SavedCourseSettingSuccessfully'));
    })
  }
  //#endregion

  //#region tab Instructor

  // dropdownList = [];
  // GetSelectCourseAdmins() {
  //   this._userExtraRoleService.GetCourseAdminsByCourseId(this.courseId).subscribe((result) => {
  //     console.log(result);
  //     this.dropdownList = result.result;
  //     this.selectedItems = this.dropdownList.filter(item => item.isSelected);
  //   });
  // }

  saveUserExtraRole() {
    const obj = new CourseAdminsToCourseDto();
    obj.courseId = this.courseId;
    obj.CourseAdminIds = [];
    obj.courseAdminNames = [];
    this.selectedItems.forEach(element => {

      obj.CourseAdminIds.push(element[this.dropdownSettings.idField]);
      obj.courseAdminNames.push(element[this.dropdownSettings.textField]);
    });
    this._userExtraRoleService.AddCourseAdminsToCourse(obj).subscribe(() => {
      this.notify.info(this.l('SaveSuccessfully'));
    })
  }
  show() {
  }

  onSaveSyllabusClick(course: EditCourseDto) {
    const courseTemp: any = { id: course.id, syllabus: course.syllabus }
    this._courseService.UpdateCourseSyllabus(courseTemp).subscribe((result) => {
      this.notify.info(this.l('SaveSuccessfully'));
    });
  }
  //#endregion
  getGrades(courseId: string) {
    this._gradeSchemeService.getGradesByCourseId(courseId).subscribe((result) => {
      this.gradeSchemes = result.result;
    });
  }
}
