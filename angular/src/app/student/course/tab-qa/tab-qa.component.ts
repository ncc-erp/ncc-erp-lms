import { AppConsts } from '@shared/AppConsts';
import { Component, OnInit, Input, Injector } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto, PagedResultResultDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { StudentService } from '@app/services/student-service/student.service';
import { QAQuestionAnswerDto, QAQuestionDto } from '@app/models/qaquestion-dto';
import { CourseSettingFeatualValueDto } from '@app/models/course-setting-dto';
import { FAQQuestionDto } from '@app/models/faqquestion-dto';
import * as signalR from '@aspnet/signalr';

@Component({
  selector: 'app-tab-qa',
  templateUrl: './tab-qa.component.html',
  styleUrls: ['./tab-qa.component.scss']
})
export class TabQAComponent extends PagedListingComponentBase<any> implements OnInit {

  @Input() courseInstanceId: string;
  @Input() courseId: string;
  @Input() courseSetting: CourseSettingFeatualValueDto;

  isEdit: boolean = false;
  title: string;
  content: string;
  isFollower: boolean;
  isResponse: boolean;
  sortDirection = 'date_asc';
  isPageDetailQA: boolean = false;
  qaQuestion: QAQuestionAnswerDto = new QAQuestionAnswerDto();
  qaQuestions: QAQuestionAnswerDto[] = [];
  faqQuestions: FAQQuestionDto[] = [];
  private hubConnection: signalR.HubConnection;

  constructor(injector: Injector,
    private _service: StudentService,
  ) {
    super(injector);
    this.startConnection();
    this.addTransferQAQuestiontDataListener();
  }

  ngOnInit() {
    this.getDataPage(1);
    this.getFAQQuestion();
  }

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(AppConsts.remoteServiceBaseUrl + '/signalr-qna')
      .build();
    this.hubConnection.start();
  }

  private addTransferQAQuestiontDataListener = () => {
    // Add QA question
    this.hubConnection.on('CreateQAQuestion', (data) => {
      const temp: QAQuestionAnswerDto = data;
      for (let i = 0; i < this.qaQuestions.length; i++) {
        if (this.qaQuestions[i].id === temp.id) {
          return;
        }
      }
      this.qaQuestions.unshift(data);
    });
  }


  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this._service
      .GetQAQuestionAnswer(request, this.courseInstanceId, this.isFollower, this.isResponse, this.sortDirection)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: PagedResultResultDto) => {
        this.qaQuestions = result.result.items;
        this.showPaging(result.result, pageNumber);
      });
  }

  protected delete(item: any): void {

  }

  getFAQQuestion() {
    if (this.courseId === null) {
      return;
    }
    this._service.GetAllFAQQuestionByCourseId(this.courseId, '')
      .subscribe(data => {
        if (data.success) {
          this.faqQuestions = data.result.items;
        } else {
          this.notify.info(this.l('ERROR_500'));
        }
      })
  }

  onGetDetailAnswer(qaQuestionSelect: QAQuestionAnswerDto) {
    this.qaQuestion = qaQuestionSelect;
    this.isPageDetailQA = true;
  }

  onSaveQnAClick() {
    if (this.title == null || this.title.trim() === '') {
      abp.notify.error('Title is required!');
      return;
    }

    const qaQuestionTemp: QAQuestionDto = new QAQuestionDto();
    qaQuestionTemp.title = this.title;
    qaQuestionTemp.content = this.content;
    qaQuestionTemp.courseInstanceId = this.courseInstanceId;
    this._service.CreateQAQuestion(qaQuestionTemp)
      .subscribe(data => {
        if (data.success) {
          this.isEdit = false;
          this.notify.info(this.l('SaveSuccessfully'));
          this.title = null;
          this.content = null;
        } else {
          this.notify.error(this.l('DataNotSave'));
        }
      })
  }


  loadQAQuestions(isLoad: boolean): void {
    this.isPageDetailQA = false
    if (isLoad) {
      this.getDataPage(this.pageNumber);
    }
  }

}
