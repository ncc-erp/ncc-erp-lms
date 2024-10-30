import { Component, OnInit, Injector, Input, OnChanges, SimpleChanges } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { AttachmentDto, CreateAttachmentDto } from '@app/models/attachment-dto';
import { AttachmentService } from '@app/services/systems-admin-services/attachment.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { finalize } from 'rxjs/operators';
import { StudentFileService } from '@app/services/systems-admin-services/studentfile.service';
// import { saveAs } from 'file-saver';

@Component({
  selector: 'app-file-attachments',
  templateUrl: './file-attachments.component.html',
  styleUrls: ['./file-attachments.component.scss']
})
export class FileAttachmentsComponent extends AppComponentBase implements OnInit, OnChanges {
  @Input() entityId: string;
  @Input() entityType: string;
  @Input() courseGroupId: string;

  addNewAttachment = false;
  fileupload = {} as CreateAttachmentDto;
  attachments: AttachmentDto[] = [];
  saving = false;
  constructor(
    injector: Injector,
    private _service: AttachmentService,
    private _fileservice: StudentFileService,
    private $http: HttpClient

  ) { super(injector); }

  ngOnInit() {
    // this.getAttachment();
  }
  ngOnChanges(changes: SimpleChanges): void {
    this.getAttachment();
  }
  getAttachment() {

    switch (this.entityType) {
      case 'student-assignment': {
        this._fileservice.GetStudentAssignmentFiles(this.entityId).subscribe((result: any) => {
          this.attachments = result.result;
        })
        break;
      }
      default: {
        this._service.getByEntityId(this.entityId, this.entityType).subscribe((result: any) => {
          this.attachments = result.result;
        })
        break;
      }
    }

  }
  newAttachment() {
    this.fileupload = {} as CreateAttachmentDto;
    this.addNewAttachment = true;
  }
  cancelAttachment() {
    this.fileupload = {} as CreateAttachmentDto;
    this.addNewAttachment = false;
  }
  getFileInfo(fileInfo: File) {
    this.fileupload.file = fileInfo;
  }
  uploadFile() {
    this.saving = true;
    switch (this.entityType) {
      case 'student-assignment': {
        this.fileupload.assignmentSettingId = this.entityId;
        if (this.courseGroupId) {
          this.fileupload.courseGroupId = this.courseGroupId;
        }
        this._fileservice.createFormData(this.fileupload)
          .pipe(finalize(() => { this.saving = false; }))
          .subscribe(event => {
            this.getAttachment();
            this.fileupload = {} as CreateAttachmentDto;
            this.addNewAttachment = false;
          });
        break;
      }
      default: {
        this.fileupload.entityId = this.entityId;
        this.fileupload.entityType = this.entityType;
        this._service.createFormData(this.fileupload)
          .pipe(finalize(() => { this.saving = false; }))
          .subscribe(event => {
            this.getAttachment();
            this.fileupload = {} as CreateAttachmentDto;
            this.addNewAttachment = false;
          });
        break;
      }
    }
  }
  cancelUpload() {
    this.fileupload = {} as CreateAttachmentDto;
  }
  downloadAttachment(item: AttachmentDto) {
    const filePath = this.getImageServerPath(item.filePath);
    window.open(filePath);
  }
  deleteAttachment(item: AttachmentDto) {
    abp.message.confirm(
      'Delete file \'' + item.fileName + '\'?',
      (result: boolean) => {
        if (result) {
          switch (this.entityType) {
            case 'student-assignment': {
              this._fileservice.delete(item.id).subscribe(() => {
                abp.notify.info('Deleted file: ' + item.fileName);
                this.getAttachment();
              });
              break;
            }
            default: {
              this._service.delete(item.id).subscribe(() => {
                abp.notify.info('Deleted file: ' + item.fileName);
                this.getAttachment();
              });
              break;
            }
          }
        }
      }
    );
  }
}
