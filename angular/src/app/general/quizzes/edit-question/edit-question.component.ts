import { Component, OnInit, ViewChild, ElementRef, Output, EventEmitter, Injector, Input } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { ModalDirective } from 'ngx-bootstrap';
import { QuestionDto as EditDto } from '@app/models/question-dto';
import { QuestionsService } from '@app/services/systems-admin-services/questions.service';
import { finalize } from 'rxjs/operators';
import { AnswerDto } from '@app/models/answer-dto';
import { EQuestion } from '@shared/AppEnums';
import { HttpHeaders, HttpRequest, HttpClient } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';

@Component({
    selector: 'edit-question-modal',
    templateUrl: './edit-question.component.html',
    styleUrls: ['./edit-question.component.scss']
})
export class EditQuestionComponent extends AppComponentBase {
    @Input() courseId: string;
    @ViewChild('editQuestionModal') modal: ModalDirective;
    @ViewChild('modalContent') modalContent: ElementRef;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    oldQuestionType: number;
    question = {} as EditDto;
    types: any[] = [];
    groups: any[] = EQuestion.QUESTION_CATES;
    currentAnswer = { rAnswer: '', isCorrect: false } as AnswerDto;
    isEditAnswer = false;
    buttonName = 'Add';
    httpRequests: any;
    header: HttpHeaders = new HttpHeaders().append('Authorization', 'Bearer ' + abp.auth.getToken())
        .append('.AspNetCore.Culture', abp.utils.getCookieValue('Abp.Localization.CultureName') + '')
        .append('Abp.TenantId', abp.multiTenancy.getTenantIdCookie() + '');
    initTinymce = {
        height: 200,
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
        images_upload_handler: (blobInfo, success, failure) => {
            const formData = new FormData();
            // formData.append('Authorization' , 'Bearer ' + abp.auth.getToken());
            // formData.append('.AspNetCore.Culture', abp.utils.getCookieValue("Abp.Localization.CultureName"));
            // formData.append('Abp.TenantId', abp.multiTenancy.getTenantIdCookie()+'');
            formData.append('Data', this.courseId);
            formData.append('UploadType', '0');
            formData.append('File', blobInfo.blob(), blobInfo.filename());
            const httpRequest
            = new HttpRequest('POST', AppConsts.remoteServiceBaseUrl + '/api/services/app/UploadService/UploadFile', formData, {
                headers: this.header
            });
            this.$http.request(httpRequest).subscribe((e) => {
                const data: any = e;
                const link = this.getImageServerPath(data.body.result.serverPath);
                success(link);
            });
        }
    }
    constructor(
        injector: Injector,
        private _service: QuestionsService,
        private $http: HttpClient
    ) {
        super(injector);
    }

    show(id: string): void {
        this._service.getById(id).subscribe(result => {
            this.question = result.result;

            this.groups.forEach(item => {
                if (item.id === this.question.group) {
                    this.types = item.types;
                }
            })
            this.active = true;
            this.modal.show();
        });
    }

    onShown(): void {
        $.AdminBSB.input.activate($(this.modalContent.nativeElement));
    }

    onQuestionCateChange(value) {
        this.question.group = value;
        if (this.question.group == null || this.question.group === undefined) {
            this.types = [];
        } else {
            this.groups.forEach(item => {
                if (item.id === value) {
                    this.types = item.types;
                    return;
                }
            })
        }
    }

    onQuestionTypeChange(value) {
        if (this.question.answers.length === 0) {
            this.oldQuestionType = value;
            return;
        } else {
            abp.message.confirm(
                'Change question type will clear all current answers. Are you sure to change type?',
                (result: boolean) => {
                    if (result) {
                        this.question.answers = [];
                    } else {
                        this.question.type = this.oldQuestionType;
                    }
                }
            );
        }
    }

    addOrEditAnswer() {
        if (!this.currentAnswer.rAnswer || this.currentAnswer.rAnswer.trim() === '') {
            return;
        }
        if (!this.isEditAnswer) {
            this.question.answers.push(this.currentAnswer);
        }
        this.currentAnswer = new AnswerDto();
        this.currentAnswer.rAnswer = '';
        this.isEditAnswer = false;
        this.buttonName = 'Add';
    }

    editAnswer(item: AnswerDto) {
        this.currentAnswer = item;
        this.isEditAnswer = true;
        this.buttonName = 'Update';
    }

    deleteAnswer(item: AnswerDto) {
        this.question.answers.splice(this.question.answers.indexOf(item), 1);
    }

    save(): void {
        this.saving = true;
        this._service.update(this.question)
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


