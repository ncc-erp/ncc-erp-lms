import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { CreateCourseDto, ICourseStatusDto, ICourseTypeDto } from 'app/models/courses-dto';
import { CoursesService } from 'app/services/systems-admin-services/courses.service';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';
import { HttpEventType } from '@angular/common/http';
import { CourseSourse } from '@shared/AppEnums';

@Component({
  selector: 'create-course-modal',
  templateUrl: './create-course.component.html'
})
export class CreateCourseComponent extends AppComponentBase {

  @ViewChild('createModal') modal: ModalDirective;
  @ViewChild('modalContent') modalContent: ElementRef;
  @ViewChild('courseName') courseName: ElementRef;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;
  course = { sourse: CourseSourse.NCC } as CreateCourseDto;
  levels: ICourseStatusDto[] = [];
  types: ICourseTypeDto[] = [{ id: 0, displayName: 'Recur' }, { id: 1, displayName: 'Perpetual' }];
  progress: number;
  uploadMessage: string;
  file: File;
  imgBase64Value: string;

  courseSourses = [
    { id: 0, name: 'NCC' },
    { id: 1, name: 'SCORM' }];

  isLoadingFileUpload = false;
  isFileUploadNotSupport = false;
  sourseVersion: number;
  constructor(
    injector: Injector,
    private _service: CoursesService
  ) {
    super(injector);
    // _service.NAME = 'Course';
  }

  show(): void {
    this.course.sourse = CourseSourse.NCC;
    this.file = null;
    this.imgBase64Value = null;
    this.getCourseLevels();
  }

  getData() : void{
    this.course.sourse = CourseSourse.NCC;
    this.file = null;
    this.imgBase64Value = null;
  }

  onShown(): void {
    $.AdminBSB.input.activate($(this.modalContent.nativeElement));
  }

  save(): void {
    if (this.course.sourse === 1) {
      if (!this.course.fileSCORM) {
        abp.notify.info(this.l('Please select the attached file!'));
        return;
      }
      if (this.isFileUploadNotSupport) {
        abp.notify.error(this.l('The file is not supported!'));
        return;
      }
      this.course.sourse = this.sourseVersion;
    }

    this.saving = true;
    //   this.course.type = 0;
    this._service.createFormData(this.course)
      .pipe(finalize(() => { this.saving = false; }))
      //   .subscribe(() => {
      //       this.notify.info(this.l('SavedSuccessfully'));
      //       this.close();
      //       this.modalSave.emit(null);
      //   });
      .subscribe(
        event => {
          if (event.type === HttpEventType.UploadProgress){
            return (this.progress = Math.round((100 * event.loaded) / event.total));
          } else if (event.type === HttpEventType.Response) {
            this.uploadMessage = event.body.toString();
            this.notify.info(this.l('SavedSuccessfully'));
            this.close();
            this.modalSave.emit(null);
            this.course = {} as CreateCourseDto;
          }
        });
  }
  close(): void {
    this.active = false;
    this.modal.hide();
    this.course.name = null;
    this.course.description = null;
    this.course.file = null;
  }

  getImgBase64(value) {
    if (this.course.file) {
      this.imgBase64Value = value;
    }
  }

  getFileInfo(fileInfo: File) {
    if (fileInfo.type.includes('image')) {
      //   this.file = content;
      this.course.file = fileInfo;
    } else {
      this.notify.error('The format must be of image');
    }
  }

  getCourseLevels() {
    this._service.getAllStatus().subscribe(result => {
      this.levels = result.result.items;
      this.levels && this.levels.length > 0 && (this.course.levelId = this.levels[0].id);
      this.course.type = this.types[0].id;
      this.active = true;
      this.modal.show();
    });
  }

  getFileInfoSCORM(fileInfo: File) {
    if (!fileInfo.name.toLowerCase().endsWith('zip')) {
      this.isFileUploadNotSupport = true;
      this.course.fileSCORM = null;
      return;
    }
    this.course.fileSCORM = fileInfo;

    const url = '/api/services/app/Course/ScormUpload';
    this.isLoadingFileUpload = true;
    this._service.uploadFile(url, fileInfo)
      .subscribe(event => {
        const temp: any = event;
        this.course.name = temp.body.result.title;
        this.courseName.nativeElement.focus();
        this.sourseVersion = temp.body.result.sourseVersion;
        this.isLoadingFileUpload = false;
        this.isFileUploadNotSupport = false;
      }, error => {
        this.isLoadingFileUpload = false;
        this.isFileUploadNotSupport = true;
        this.course.name = null;
        abp.notify.error(error.error.error.details);
      })
  }

  onSelectSCORMChange() {
    this.course.fileSCORM = null;
    this.course.name = '';
  }

  // courseSourses = [
  //   {id: 0, name: 'RMA'},
  //   {id: 1, name: 'SCORM 2004'},
  //   {id: 2, name: 'SCORM 1.2'}
  //   ];
}
