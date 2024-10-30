import { AppConsts } from '@shared/AppConsts';
import { Component, OnInit, Input, Injector, Output, EventEmitter, OnDestroy } from '@angular/core';
import { StudentService } from '@app/services/student-service/student.service';
import { QAQuestionAnswerDto, QAAnswerDto, QAAnswerInput } from '@app/models/qaquestion-dto';
import { AppComponentBase } from '@shared/app-component-base';
import { AppSessionService } from '@shared/session/app-session.service';
import * as signalR from '@aspnet/signalr';

@Component({
  selector: 'app-tab-qa-detail',
  templateUrl: './tab-qa-detail.component.html',
  styleUrls: ['./tab-qa.component.scss']
})
export class TabQADetailComponent extends AppComponentBase implements OnInit, OnDestroy {

  @Input() courseInstanceId: string;
  @Input() courseId: string;
  @Input() qaQuestion: QAQuestionAnswerDto;
  @Output() isPageDetailQALoad: EventEmitter<boolean> = new EventEmitter<boolean>();

  title: string;
  content: string;
  answer = '';
  answerAdded = false;
  userName = '?';
  currentUserId: number;
  private hubConnection: signalR.HubConnection;

  constructor(injector: Injector,
    private _studentService: StudentService,
    private _userService: AppSessionService,

  ) {
    super(injector);
    this.startConnection();
    this.addTransferQAQuestiontDataListener();
  }

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(AppConsts.remoteServiceBaseUrl + '/signalr-qna')
      .build();
    this.hubConnection.start();
  }

  private addTransferQAQuestiontDataListener = () => {
    // Add QA question
    this.hubConnection.on('qaquestion', (data) => {
      const temp: QAAnswerDto = data;
      if (temp.pId) { // qaquestion_child
        for (let i = 0; i < this.qaQuestion.qaAnswers.length; i++) {
          if (this.qaQuestion.qaAnswers[i].id === temp.pId) {
            if (!this.qaQuestion.qaAnswers[i].answers) {
              this.qaQuestion.qaAnswers[i].answers = [];
            }
            this.qaQuestion.qaAnswers[i].answers.push(temp);
            this.qaQuestion.qaAnswers[i].numberAnswer++;
            return;
          }
        }
      } else { // qaquestion_parent
        for (let i = 0; i < this.qaQuestion.qaAnswers.length; i++) {
          if (this.qaQuestion.qaAnswers[i].id === temp.id) {
            return;
          }
        }
        this.qaQuestion.qaAnswers.push(temp);
      }
    });
    // Delete QAQuestion
    this.hubConnection.on('delete_qaanswer', (data) => {
      const quanwerId: string = data;
      if (!this.qaQuestion.qaAnswers) {
        return;
      }
      for (let i = 0; i < this.qaQuestion.qaAnswers.length; i++) {
        if (this.qaQuestion.qaAnswers[i].id === quanwerId) {
          this.qaQuestion.qaAnswers.splice(i, 1);
          return;
        } else {
          if (this.qaQuestion.qaAnswers[i].answers) {
            for (let j = 0; j < this.qaQuestion.qaAnswers[i].answers.length; j++) {
              if (this.qaQuestion.qaAnswers[i].answers[j].id === quanwerId) {
                this.qaQuestion.qaAnswers[i].answers.splice(j, 1);
                this.qaQuestion.qaAnswers[i].numberAnswer--;
                return;
              }
            }
          }
        }
      }
    });
  }


  ngOnInit() {
    this.onLoadAnswers(this.qaQuestion.id);
    this.currentUserId = this._userService.userId;
    this.userName = this._userService.user.name + ' ' + this._userService.user.surname;
  }

  onLoadAnswers(QuestionId: string) {
    if (QuestionId) {
      this._studentService.getAllQAAnswerByQAQuestionId(QuestionId)
        .subscribe(data => {
          this.qaQuestion.qaAnswers = data.result.items;
        }, err => this.isPageDetailQALoad.emit(this.answerAdded));
    } else {
      // Close this page
      this.isPageDetailQALoad.emit(this.answerAdded);
    }

  }

  onAddAnswerClick(): void {
    if (this.answer == null || this.answer.trim() === '') {
      abp.notify.info('DataNotSave');
      return;
    }
    const tempQAAnswerDto: QAAnswerInput = new QAAnswerInput();
    tempQAAnswerDto.questionId = this.qaQuestion.id;
    tempQAAnswerDto.content = this.answer;
    tempQAAnswerDto.replyUserName = this.qaQuestion.fullName;
    this._studentService.CreateQAAnswer(tempQAAnswerDto)
      .subscribe(data => {
        if (data.success) {
          this.answerAdded = true;
          // this.qaQuestion.qaAnswers.push(data.result);
          this.answer = '';
          this.notify.info(this.l('SaveSuccessfully'));
        } else {
          abp.notify.info('DataNotSave');
        }
      });
  }

  loadQAQuestions(): void {
    this.isPageDetailQALoad.emit(this.answerAdded);
  }

  onUn_Follow_Responses_Click(item: any): void {
    this._studentService.DeleteUserFollowQA(item.id)
      .subscribe(data => {
        if (data.success) {
          item.isFollow = false;
        }
      });
  }
  onFollow_Responses_Click(item: any): void {
    this._studentService.AddUserFollowQA(item.id)
      .subscribe(data => {
        if (data.success) {
          item.isFollow = true;
        }
      });
  }

  onReplyAnswer_Click(item: QAAnswerDto) {
    item.new_content = '<a>@' + item.fullName + '</a>:&nbsp;';
  }

  onReplySubAnswer_Click(item: QAAnswerDto, index: number) {
    item.new_content = '<a>@' + item.answers[index].fullName + '</a>:&nbsp;';
  }

  onAddReplyAnswer_Click(qaAnswer: QAAnswerDto, qaQuestionId: string) {
    // item.qaAnswers.push(item.a)
    const temp: QAAnswerInput = new QAAnswerInput();
    temp.questionId = qaQuestionId;
    temp.content = qaAnswer.new_content;
    temp.responseParentId = qaAnswer.id;
    temp.replyUserName = qaAnswer.fullName;
    // temp.content = item.qaAnswers.answer
    this._studentService.CreateQAAnswer(temp)
      .subscribe(data => {
        if (data.success) {
          qaAnswer.numberAnswer++;
          // qaAnswer.answers.push(data.result);
          // Reset
          qaAnswer.new_content = '';
          qaAnswer.isShowAnswer = true;
          qaAnswer.isEdit = false;
        }
      });
  }

  onDeleteAnswer_Click(item: QAAnswerDto, i: number, qaQuestion: QAAnswerDto[]) {
    abp.message.confirm(
      '',
      (result: boolean) => {
        if (result) {
          this._studentService.DeleteQAAnswer(item.id)
            .subscribe(data => {
              if (data.success) {
                qaQuestion.splice(i, 1);
                abp.notify.success(this.l('DeleteSuccessfully'));
              }
            });
        }
      }
    );
  }

  onDeleteSubAnswer_Click(item: QAAnswerDto, index: number) {
    abp.message.confirm(
      '',
      (result: boolean) => {
        if (result) {
          this._studentService.DeleteQAAnswer(item.answers[index].id)
            .subscribe(data => {
              if (data.success) {
                item.answers.splice(index, 1);
                item.numberAnswer--;
                abp.notify.success(this.l('DeleteSuccessfully'));
              }
            });
        }
      }
    );
  }
  ngOnDestroy() {
    this.hubConnection.stop();
  }
}
