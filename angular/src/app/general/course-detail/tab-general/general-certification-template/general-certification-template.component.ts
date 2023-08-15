import { Component, OnInit, Input, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CertificationTemplateService } from '@app/services/systems-admin-services/certificationtemplate.service';
import { CertificationTemplateDto, CreateCertificationTemplateDto } from '@app/models/certificationtemplate-dto';
import { finalize } from 'rxjs/operators';
import { HttpHeaders, HttpRequest, HttpClient } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
import { ETemplate } from '@shared/AppEnums';

@Component({
  selector: 'app-general-certification-template',
  templateUrl: './general-certification-template.component.html',
  styleUrls: ['./general-certification-template.component.scss']
})
export class GeneralCertificationTemplateComponent extends AppComponentBase implements OnInit {
  @Input() courseId: string;

  attendanceTemplates: CertificationTemplateDto[] = [];
  completionTemplates: CertificationTemplateDto[] = [];
  preview = {} as CertificationTemplateDto;
  oldtemplate = {} as CertificationTemplateDto;
  file = {} as File;
  imgBase64Value: string;
  adding = false;
  saving = false;
  httpRequests: any;
  viewWidth: number;
  viewHeight: number;
  tempType: any[] = ETemplate.TemplateOrientation;

  header: HttpHeaders = new HttpHeaders().append('Authorization', 'Bearer ' + abp.auth.getToken())
    .append('.AspNetCore.Culture', abp.utils.getCookieValue('Abp.Localization.CultureName') + '')
    .append('Abp.TenantId', abp.multiTenancy.getTenantIdCookie() + '');

  constructor(
    injector: Injector,
    private _service: CertificationTemplateService,
    private $http: HttpClient
  ) { super(injector); }

  ngOnInit() {
    this.getTemplates();
  }
  getTemplates() {
    this.attendanceTemplates = [];
    this.completionTemplates = [];
    this._service.getByCourseId(this.courseId).subscribe((result) => {
      result.result.forEach(temp => {
        if (temp.certificationType === 0) {
          this.attendanceTemplates.push(temp);
        } else {
          this.completionTemplates.push(temp);
        }
      });
    })
  }
  getImgBase64(value) {
    if (this.file) {
      this.imgBase64Value = value;
    }
  }

  getFileInfo(fileInfo: File, item: CertificationTemplateDto) {
    if (fileInfo.type.includes('image')) {
      item.file = fileInfo;
    } else {
      this.notify.error('The format must be of image');
    }
  }
  addNewAttendance() {
    const newtemp = { isEdit: true, courseId: this.courseId, certificationType: 0 } as CertificationTemplateDto;
    this.CloseAddOrEdit(newtemp);
    this.imgBase64Value = null;
    this.adding = true;
    this.attendanceTemplates.unshift(newtemp);
  }
  addNewCompletion() {
    const newtemp = { isEdit: true, courseId: this.courseId, certificationType: 1 } as CertificationTemplateDto;
    this.CloseAddOrEdit(newtemp);
    this.imgBase64Value = null;
    this.adding = true;
    this.completionTemplates.unshift(newtemp);
  }
  saveTemplate(item: CertificationTemplateDto) {
    this.saving = true;
    this._service.createFormData(item)
      .pipe(finalize(() => { this.saving = false; }))
      .subscribe(event => {
        this.getTemplates();
        this.adding = false;
        this.oldtemplate = {} as CertificationTemplateDto;
      });
  }
  cancelTemplate(item: CertificationTemplateDto) {
    this.CloseAddOrEdit(item);
    this.oldtemplate = {} as CertificationTemplateDto;
  }
  editTemp(item: CertificationTemplateDto) {
    if (this.adding || this.oldtemplate.id) {
      abp.message.confirm(
        'Are you sure want to close current editing template?',
        (result: boolean) => {
          if (result) {
            this.CloseAddOrEdit(item);
            this.getbase(item.background);
            item.isEdit = true;
          }
        }
      );
    } else {
      this.getbase(item.background);
      this.CloseAddOrEdit(item);
      item.isEdit = true;
    }
  }
  deleteTemp(item: CertificationTemplateDto): void {
    abp.message.confirm(
      'Delete template \'' + item.name + '\'?',
      (result: boolean) => {
        if (result) {
          this._service.delete(item.id).subscribe(() => {
            abp.notify.info('Deleted template: ' + item.name);
            this.getTemplates();
          });
        }
      }
    );
  }
  CloseAddOrEdit(item: CertificationTemplateDto) {
    if (this.adding) {
      if (this.oldtemplate.certificationType === 0) {
        this.attendanceTemplates.shift();
      } else {
        this.completionTemplates.shift();
      }
      this.adding = false;
    }
    this.CloseEditingTemp();
    this.oldtemplate = {
      id: item.id,
      isActive: item.isActive,
      courseId: item.courseId,
      background: item.background,
      orientation: item.orientation,
      name: item.name,
      content: item.content,
      certificationType: item.certificationType,
      isEdit: false,
      isView: false,
    } as CertificationTemplateDto;
  }
  CloseEditingTemp() {
    if (this.oldtemplate.id) {
      if (this.oldtemplate.certificationType === 0) {
        const currentItem = this.attendanceTemplates.find(x => x.id === this.oldtemplate.id);
        const index = this.attendanceTemplates.indexOf(currentItem);
        this.attendanceTemplates[index] = this.oldtemplate;
        this.attendanceTemplates[index].isEdit = false;
      } else {
        const currentItem = this.completionTemplates.find(x => x.id === this.oldtemplate.id);
        const index = this.completionTemplates.indexOf(currentItem);
        this.completionTemplates[index] = this.oldtemplate;
        this.completionTemplates[index].isEdit = false;
      }
    }
  }


  getbase(imageCover: string) {
    const fullpath = this.getImageServerPath(imageCover);
    this._service.getImage(fullpath).subscribe(result => {
      const reader = new FileReader();
      reader.readAsDataURL(result);
      reader.onload = event => {
        // called once readAsDataURL is completed. Preview Image
        const item: any = event;
        this.imgBase64Value = item.target.result;
      };
    })
  }
  viewTemp(item: CertificationTemplateDto) {

    if (item.isView) {
      item.isView = false;
      return;
    }
    item.isView = true;
    if (item.orientation === 0) {
      this.viewWidth = 297;
      this.viewHeight = 210;
    } else {
      this.viewWidth = 210;
      this.viewHeight = 297;
    }
  }
  onOrientationChange(value: any) {
    if (value === 0) {
      this.viewWidth = 297;
      this.viewHeight = 210;
    } else {
      this.viewWidth = 210;
      this.viewHeight = 297;
    }
  }
  // tslint:disable-next-line:member-ordering
  initTinymce = {
    height: 240,
    plugins: AppConsts.Tinymceplugins,
    toolbar1: AppConsts.Tinymcetoolbar,
    font_formats: AppConsts.TinymceFont,

    image_advtab: true,
    images_upload_credentials: true,
    file_picker_types: 'image ',
    fontsize_formats: '8pt 10pt 12pt 14pt 16pt 18pt 20pt 24pt 28pt 32pt 36pt',
    // file_picker_callback: (callback, value, meta) => {
    //   const input = document.createElement('input');
    //   if (meta.filetype === 'image') {
    //     input.setAttribute('type', 'file');
    //     input.setAttribute('accept', 'image/*');
    //   }
    //   if (meta.filetype === 'media') {
    //     input.setAttribute('type', 'file');
    //     input.setAttribute('accept', 'audio/*,video/*');
    //   }
    //   input.click();
    //   var files: File;
    //   var that = this;
    //   input.onchange = function (e: any) {
    //     const fileType: string = e.path[0].files[0].type;
    //     if (fileType.includes('video') && meta.filetype === 'media'
    //       || fileType.includes('audio') && meta.filetype === 'media'
    //       || fileType.includes('image') && meta.filetype === 'image') {
    //       const formData = new FormData();
    //       formData.append('Data', that.courseId);
    //       formData.append('UploadType', '0')
    //       formData.append('File', e.path[0].files[0]);
    //       callback('link', { title: 'title'});
    //       abp.ajax({
    //         url: AppConsts.remoteServiceBaseUrl + '/api/services/app/UploadService/UploadFile',
    //         method: 'POST',
    //         headers: {
    //           Authorization: 'Bearer ' + abp.auth.getToken(),
    //           '.AspNetCore.Culture': abp.utils.getCookieValue('Abp.Localization.CultureName'),
    //           'Abp.TenantId': abp.multiTenancy.getTenantIdCookie(),
    //         },
    //         processData: false,
    //         contentType: false,
    //         data: formData
    //       }).done(result => {
    //         const data: any = result;
    //         const link = that.getImageServerPath(data.serverPath);
    //         callback(link, { title: data.fileName });
    //       });
    //     } else {
    //       abp.notify.error(`This is not format file`);
    //     }
    //   }

    // },
    // images_upload_handler: (blobInfo, success, failure) => {
    //   const formData = new FormData();
    //   formData.append('Data', this.courseId);
    //   formData.append('UploadType', '0');
    //   formData.append('File', blobInfo.blob(), blobInfo.filename());
    //   const httpRequest = new HttpRequest('POST', AppConsts.remoteServiceBaseUrl + '/api/services/app/UploadService/UploadFile', formData, {
    //     headers: this.header
    //   });
    //   this.$http.request(httpRequest).subscribe((e) => {
    //     const data: any = e;
    //     const link = this.getImageServerPath(data.body.result.serverPath);
    //     success(link);
    //   });
    // },
    images_upload_handler: function (blobInfo, success, failure) {
      success('data:' + blobInfo.blob().type + ';base64,' + blobInfo.base64());
    },
    setup: function (editor) {
      editor.addButton('customkey', {
        type: 'menubutton',
        text: 'Insert Token',
        icon: false,
        menu: [
          {
            text: 'User Name', icon: false, onclick: function () {
              editor.execCommand('mceInsertContent', false, '[_UserName_]');
              return false;
            }
          },
          {
            text: 'Completed Date', icon: false, onclick: function () {
              editor.execCommand('mceInsertContent', false, '[_CompletedDate_]');
              return false;
            }
          },
          {
            text: 'Grade Scheme Level', icon: false, onclick: function () {
              editor.execCommand('mceInsertContent', false, '[_GradeSchemeLevel_]');
              return false;
            }
          },
           {
            text: 'Total Score', icon: false, onclick: function () {
              editor.execCommand('mceInsertContent', false, '[_TotalScore_]');
              return false;
            }
          },
           {
            text: 'Student Score', icon: false, onclick: function () {
              editor.execCommand('mceInsertContent', false, '[_StudentScore_]');
              return false;
            }
          },
           {
            text: 'Student Score Percent', icon: false, onclick: function () {
              editor.execCommand('mceInsertContent', false, '[_StudentScorePercent_]');
              return false;
            }
          }
        ]
      });

    }
  }
}
