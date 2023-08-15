import { Component, OnInit, Injector } from '@angular/core';
import { HttpHeaders, HttpRequest, HttpClient, HttpErrorResponse } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/app-component-base';
import { AssignmentsService } from '@app/services/systems-admin-services/assignments.service';
import { ActivatedRoute } from '@angular/router';
import { AssignmentDto as EditDto, CreateAssignmentDto as CreateDto, AssignmentSettingsDto } from '@app/models/assignments-dto';
import { EAssignment } from '@shared/AppEnums';
import { CoursesService } from '@app/services/systems-admin-services/courses.service';

@Component({
  selector: 'app-assignments',
  templateUrl: './assignments.component.html',
  styleUrls: ['./assignments.component.scss']
})
export class AssignmentsComponent extends AppComponentBase implements OnInit {

  httpRequests: any;
  header: HttpHeaders = new HttpHeaders().append('Authorization', 'Bearer ' + abp.auth.getToken())
    .append('.AspNetCore.Culture', abp.utils.getCookieValue('Abp.Localization.CultureName') + '')
    .append('Abp.TenantId', abp.multiTenancy.getTenantIdCookie() + '');
  initTinymce = {
    height: 400,
    plugins: AppConsts.Tinymceplugins,
    toolbar1: AppConsts.Tinymcetoolbar,
    font_formats: AppConsts.TinymceFont,
    image_advtab: true,
    images_upload_credentials: true,
    file_picker_types: 'file image media',
    file_picker_callback: (callback, value, meta) => {
      const input = document.createElement('input');
      if (meta.filetype === 'image') {
        input.setAttribute('type', 'file');
        input.setAttribute('accept', 'image/*');
      }
      if (meta.filetype === 'media') {
        input.setAttribute('type', 'file');
        input.setAttribute('accept', 'audio/*,video/*');
      }
      input.click();
      var files: File;
      var that = this;
      input.onchange = function (e: any) {
        var fileType: string = e.path[0].files[0].type;
        if (fileType.includes('video') && meta.filetype === 'media'
          || fileType.includes('audio') && meta.filetype === 'media'
          || fileType.includes('image') && meta.filetype === 'image') {
          const formData = new FormData();
          // formData.append('Data',that.page.courseId);
          formData.append('UploadType', '0')
          formData.append('File', e.path[0].files[0]);
          abp.ajax({
            url: AppConsts.remoteServiceBaseUrl + '/api/services/app/UploadService/UploadFile',
            method: 'POST',
            headers: {
              Authorization: 'Bearer ' + abp.auth.getToken(),
              '.AspNetCore.Culture': abp.utils.getCookieValue('Abp.Localization.CultureName'),
              'Abp.TenantId': abp.multiTenancy.getTenantIdCookie(),
            },
            processData: false,
            contentType: false,
            data: formData
          }).done(result => {
            const data: any = result;
            const link = that.getImageServerPath(data.serverPath);
            callback(link, { title: data.fileName });
          });
        } else {
          abp.notify.error(`This is not format file`);
        }
      }

    },
    images_upload_handler: (blobInfo, success, failure) => {
      const formData = new FormData();
      // formData.append('Authorization' , 'Bearer ' + abp.auth.getToken());
      // formData.append('.AspNetCore.Culture', abp.utils.getCookieValue("Abp.Localization.CultureName"));
      // formData.append('Abp.TenantId', abp.multiTenancy.getTenantIdCookie()+'');
      // formData.append('Data', this.page.courseId);
      formData.append('UploadType', '0');
      formData.append('File', blobInfo.blob(), blobInfo.filename());
      const httpRequest = new HttpRequest('POST', AppConsts.remoteServiceBaseUrl + '/api/services/app/UploadService/UploadFile', formData, {
        headers: this.header
      });
      this.$http.request(httpRequest).subscribe((e) => {
        const data: any = e;
        const link = this.getImageServerPath(data.body.result.serverPath);
        success(link);
      });
    }
  }

  assignment = { settings: {} } as CreateDto;
  assignmentId: string;
  courseId: string;
  courseInstanceId: string;
  isnewAssignment = true;
  displaygrade: any[] = EAssignment.DisplayGradeType;
  submissiontypes: any[] = EAssignment.SubmissionType;
  assignmentsettings = {} as AssignmentSettingsDto;
  dropdownSettings = {};
  dropdownListGroup = [];
  selectedGroupItems = [];

  constructor(
    injector: Injector,
    private route: ActivatedRoute,
    private _service: AssignmentsService,
    private _courseService: CoursesService,
    private $http: HttpClient
  ) {
    super(injector);
  }
  ngOnInit() {
    this.route.data.subscribe((data: any) => {
      const res = data.courseGroups;
      const result = res.filter(i => i.selected === true);
      this.dropdownListGroup = data.courseGroups;
      this.selectedGroupItems = result;
    });
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'groupName',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 30,
      allowSearchFilter: true
    };
    const mode = this.route.snapshot.paramMap.get('mode');
    switch (mode) {
      case 'add': {
        this.courseInstanceId = this.route.snapshot.paramMap.get('id');
        this.getCourseData(this.courseInstanceId);
        break;
      }
      case 'edit': {
        this.assignmentId = this.route.snapshot.paramMap.get('id');
        this.isnewAssignment = false;
        this.getDetailData(this.assignmentId);
        break;
      }
      default: {
        break;
      }
    }
  }
  getDetailData(id: string) {
    this._service.getById(id).subscribe(item => {
      this.assignment = {} as EditDto;
      this.assignment = item.result;
      this.assignmentId = item.result.id;
      this.courseId = item.result.courseId;
      this.courseInstanceId = item.result.courseInstanceId;
    })
  }
  save() {
    if (this.courseId == null) {
      this.notify.info('');
      return;
    }
    this.assignment.groupsAssingedAssignment = this.selectedGroupItems.map(({ id }) => id);
    this.assignment.settings.courseInstanceId = this.courseInstanceId;
    if (this.isnewAssignment) {
      this.assignment.courseId = this.courseId;
      this._service.create(this.assignment).subscribe((result: any) => {
        this.assignment.id = result.result.id;
        this.isnewAssignment = false;
        this.assignment.settings = result.result.settings;
        this.notify.info('SaveSuccessfully!');
      })
    } else {
      this._service.update(this.assignment).subscribe((result) => {
        this.notify.info('UpdatedSuccessfully!');
      })
    }
  }
  delete() {
    if (!this.isnewAssignment) {
      this._service.delete(this.assignment.id).subscribe((result) => {
        this.notify.info('SaveSuccessfully!');
      })
    }
  }
  getCourseData(id: string) {
    this._courseService.getByCourseInstanceId(id).subscribe(item => {
      this.courseId = item.result.id;
    })
  }
  /* Group dropdown*/
  onItemSelect(item: any) {
  }
  onSelectAll(items: any) {
  }
  onItemDeSelect(item: any) {
  }
  /* End Group dropdown*/

}
