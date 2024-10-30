import { Component, OnInit, Injector, Input } from '@angular/core';
import { ModulesService } from '@app/services/systems-admin-services/modules.service';
import { PagesService } from '@app/services/systems-admin-services/pages.service';
import { AppComponentBase } from '@shared/app-component-base';
import { CModuleDto, ModuleDto, CPageDto } from '@app/models/module-dto';
import { PageDto, PageLinkExamDto } from '@app/models/pages-dto';
import { MatSnackBar } from '@angular/material';
import { DropEffect, DndDropEvent } from 'ngx-drag-drop';
import { MatDialog } from '@angular/material';
import { DialogCreateModuleComponent } from './dialog-create-module/dialog-create-module.component';
import { DialogEditModuleComponent } from './dialog-edit-module/dialog-edit-module.component';
import { HttpClient, HttpHeaders, HttpRequest } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
import { QuizzesService } from '@app/services/systems-admin-services/quizzes.service';
import { AssignmentsService } from '@app/services/systems-admin-services/assignments.service';
import { PageLinkExamType, QuizType } from '@shared/AppEnums';



@Component({
  selector: 'app-tab-modules',
  templateUrl: './tab-modules.component.html',
  styleUrls: ['./tab-modules.component.scss']
})
export class TabModulesComponent extends AppComponentBase implements OnInit {
// tslint:disable-next-line: member-ordering
httpRequests: any;
// tslint:disable-next-line: member-ordering
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
    // tslint:disable-next-line: triple-equals
    if (meta.filetype == 'image') {
      input.setAttribute('type', 'file');
      input.setAttribute('accept', 'image/*');
    }
    if (meta.filetype === 'media') {
      input.setAttribute('type', 'file');
      input.setAttribute('accept', 'audio/*,video/*');
    }
    input.click();
    const that = this;
    input.onchange = function (e: any) {
      const fileType: string = e.path[0].files[0].type;
      if (fileType.includes('video') && meta.filetype === 'media'
        || fileType.includes('audio') && meta.filetype === 'media'
        || fileType.includes('image') && meta.filetype === 'image') {
        const formData = new FormData();
        formData.append('Data', that.courseId);
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
  images_upload_handler: (blobInfo, success) => {
    const formData = new FormData(); formData.append('Data', this.courseId);
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
  @Input() courseInstanceId: string;
  @Input() courseId: string;
  page = {} as PageDto;
  isNoPageContent = false;
  isShowPage = false;
  assignments: SelectAssignmentDto[] = [];
  quizzes: SelectQuizDto[] = [];
  surveys: SelectQuizDto[] = [];
  selectedLinkType: string;
  private currentDragEffectMsg: string;
  // selectedLinkId: string;
  modulesPages: CModuleDto[] = [];
  linkTypes = [
    { id: '', name: '---' },
    { id: PageLinkExamType.Quiz, name: PageLinkExamType.Quiz.toUpperCase() },
    { id: PageLinkExamType.QuizFinal, name: 'Final Quiz' },
    { id: PageLinkExamType.Survey, name: PageLinkExamType.Survey.toUpperCase() },
    { id: PageLinkExamType.Assignment, name: PageLinkExamType.Assignment.toUpperCase() }
    ];
  selectedLinks = []; // for selected quizzes and selected assignments
  dropdownSettings = {
    singleSelection: false,
    idField: 'id',
    textField: 'title',
    selectAllText: 'Select All',
    unSelectAllText: 'UnSelect All',
    itemsShowLimit: 10,
    allowSearchFilter: true
  };
  constructor(injector: Injector,
    private _moduleService: ModulesService,
    private _pageService: PagesService,
    private _quizService: QuizzesService,
    private _assignmentService: AssignmentsService,
    private snackBarService: MatSnackBar,
    public dialog: MatDialog,
    private $http: HttpClient
  ) { super(injector); }

  ngOnInit() {
    this.getModulesPagesByCourseId();
    this.getAllAssignments();
    this.getAllQuizzes();
  }


  /* modulesPages - drag -drop */
  saveModulesPages() {
    // set sequenceOrder for module and pages
    if (!this.modulesPages && this.modulesPages.length === 0) {
      this.notify.info('No Modules to save');
      return;
    }
    for (let i = 0; i < this.modulesPages.length; i++) {
      this.modulesPages[i].sequenceOrder = i;
      if (this.modulesPages[i].pages && (this.modulesPages[i].pages.length > 0)) {
        for (let j = 0; j < this.modulesPages[i].pages.length; j++) {
          this.modulesPages[i].pages[j].sequenceOrder = j;
        }
      }
    }
    // save
    this._moduleService.saveModulesPages({ courseId: this.courseId, modules: this.modulesPages }).subscribe(() => {
      this.notify.info('SaveModulesPagesSuccessful');
      this.getModulesPagesByCourseId();
    })

  }

  getModulesPagesByCourseId() {
    this.modulesPages = [];
    this._moduleService.getModulesPagesByCourseId(this.courseId).subscribe((result) => {
      this.modulesPages = result.result;
    })
  }

  onDragStart(event: DragEvent) {
    this.currentDragEffectMsg = '';
    this.snackBarService.dismiss();
    this.snackBarService.open('Drag started!', undefined, { duration: 2000 });
  }

  onDragged(item: any, list: any[], effect: DropEffect) {
    if (effect === 'move') {
      this.log('move');
      const index = list.indexOf(item);
      list.splice(index, 1);
    }
  }


  onDrop(event: DndDropEvent, list?: any[]) {
    this.log('onDrop ');
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

  /* modulesPages - drag -drop end */
  /*module*/
  protected deleteModule(item: ModuleDto): void {
    abp.message.confirm(
      'Delete module ' + item.name + '?',
      (result: boolean) => {
        if (result) {
          abp.notify.info('Deleted module: ' + item.name);
          this._moduleService.delete(item.id).subscribe(() => {
            for (let i = 0; i < this.modulesPages.length; i++) {
              if (this.modulesPages[i].id === item.id) {
                this.modulesPages.splice(i, 1);
                return;
              }
            }
          });
        }
      }
    );
  }

  createModule(): void {
    const dialogRef = this.dialog.open(DialogCreateModuleComponent);
    dialogRef.afterClosed().subscribe(module => {
      if (module) {
        module.courseId = this.courseId;
        this._moduleService.create(module)
          .subscribe((result) => {
            const m = new CModuleDto();
            m.id = result.result.id;
            m.name = result.result.name;
            m.sequenceOrder = result.result.sequenceOrder;
            m.pages = [];
            this.modulesPages.push(m);
            this.notify.info(this.l('SavedSuccessfully'));
            return;
          });
      }
    });
    // this.createModal.show();
  }

  editModule(id: string): void {
    const dialogRef = this.dialog.open(DialogEditModuleComponent, {
      data: { moduleId: id },
    });
    dialogRef.afterClosed().subscribe(module => {
      if (module) {
        this._moduleService.update(module).subscribe(result => {
          this.notify.success('SaveSuccessfully');
          for (let i = 0; i < this.modulesPages.length; i++) {
            if (this.modulesPages[i].id === result.result.id) {
              this.modulesPages[i].name = result.result.name;
              return;
            }
          }
        });
      }
    });

  }
  /* end module */
  /* page */
  getAllQuizzes() {
    this._quizService.GetSelectQuizzesByCourseInstanceId(this.courseInstanceId).subscribe(result => {
      this.quizzes = result.result.filter(s => s.type === QuizType.Quiz);
      this.surveys = result.result.filter(s => s.type === QuizType.Survey);
    })
  }

  getAllAssignments() {
    this._assignmentService.GetSelectAssignmentsByCourseInstanceId(this.courseInstanceId).subscribe(result => {
      this.assignments = result.result;
    })
  }


  createPage(moduleId: string): void {
    this.isShowPage = true;
    this.page = {} as PageDto;
    this.page.moduleId = moduleId;
    this.selectedLinkType = '';
    this.selectedLinks = [];
  }

  editPage(p: CPageDto) {
    this._pageService.getById(p.id).subscribe(result => {
      this.page = result.result;
      this.selectedLinks = [];
      if (this.page.links && this.page.links.length > 0) {
        this.selectedLinkType = this.page.links[0].linkType;
        const seletedLinkIds = this.page.links.map(x => x.linkId);
        if (this.selectedLinkType === PageLinkExamType.Quiz || this.selectedLinkType === PageLinkExamType.QuizFinal) {
          this.selectedLinks = this.quizzes.filter(q => seletedLinkIds.indexOf(q.id) > -1);
        } else if (this.selectedLinkType === PageLinkExamType.Assignment) {
          this.selectedLinks = this.assignments.filter(q => seletedLinkIds.indexOf(q.id) > -1);
        } else if (this.selectedLinkType === PageLinkExamType.Survey) {
          this.selectedLinks = this.surveys.filter(q => seletedLinkIds.indexOf(q.id) > -1);
        }
        this.isNoPageContent = true;
      } else {
        this.isNoPageContent = false;
      }
      this.isShowPage = true;
    })
  }

  onLinkTypeChange() {
    this.selectedLinks = [];
    if (this.page.links && this.page.links.length > 0) {
      const seletedLinkIds = this.page.links.map(x => x.linkId);
      if (this.selectedLinkType === PageLinkExamType.Assignment) {
        this.selectedLinks = this.assignments.filter(q => seletedLinkIds.indexOf(q.id) > -1);
      } else if (this.selectedLinkType === PageLinkExamType.Survey) {
        this.selectedLinks = this.surveys.filter(q => seletedLinkIds.indexOf(q.id) > -1);
      } else if (this.selectedLinkType !== '') {
        this.selectedLinks = this.quizzes.filter(q => seletedLinkIds.indexOf(q.id) > -1);
      }
    }
  }

  onQuizSelect() {
  }

  onAssignmentSelect() {
  }

  cancel() {
    this.isShowPage = false;
    this.page = {} as PageDto;
  }

  savePage(page: PageDto) {
    this.isShowPage = false;
    this.page.links = [];
    if (this.selectedLinks.length > 0) {
      this.selectedLinks.forEach(q => {
        const ple = new PageLinkExamDto();
        ple.linkType = this.selectedLinkType;
        ple.linkId = q.id;
        this.page.links.push(ple);
      })
    }
    // if (this.selectedLinkType != '') {
    //   let ple = new PageLinkExamDto();
    //   ple.linkType = this.selectedLinkType;
    //   ple.linkId = this.selectedLinkId;
    //   if (this.selectedLinkId != ''){
    //     this.page.links.push(ple);
    //   }
    // }

    if (page.id !== undefined) {
      this._pageService.update(page).subscribe((result) => {
        this.notify.info('Savesuccessfully!');
        for (let m = 0; m < this.modulesPages.length; m++) {
          if (this.modulesPages[m].id === result.result.moduleId) {
            for (let i = 0; i < this.modulesPages[m].pages.length; i++) {
              if (this.modulesPages[m].pages[i].id === result.result.id) {
                this.modulesPages[m].pages[i].name = result.result.name;
                this.modulesPages[m].pages[i].sequenceOrder = result.result.sequenceOrder
                return;
              }
            }
          }
        }
      })
    } else {
      this._pageService.create(page).subscribe((result) => {
        this.notify.info('Savesuccessfully!');
        for (let m = 0; m < this.modulesPages.length; m++) {
          if (this.modulesPages[m].id === result.result.moduleId) {
            const p = new CPageDto();
            p.id = result.result.id;
            p.name = result.result.name;
            p.sequenceOrder = result.result.sequenceOrder;
            this.modulesPages[m].pages.push(p);
            return;
          }
        }
      })
    }

  }

  protected deletePage(item: PageDto): void {
    abp.message.confirm(
      'Delete page ' + item.name + '?',
      (result: boolean) => {
        if (result) {
          this._pageService.delete(item.id).subscribe(() => {
            abp.notify.info('Deleted page: ' + item.name);
            for (let m = 0; m < this.modulesPages.length; m++) {
              if (this.modulesPages[m].pages && this.modulesPages[m].pages.length > 0) {
                for (let p = 0; p < this.modulesPages[m].pages.length; p++) {
                  if (this.modulesPages[m].pages[p].id === item.id) {
                    this.modulesPages[m].pages.splice(p, 1);
                    return;
                  }
                }
              }
            }
          });
        }
      }
    );
  }
  /* end page */

}

export class SelectQuizDto {
  id: string;
  title: string;
}

export class SelectAssignmentDto {
  id: string;
  title: string;
}
