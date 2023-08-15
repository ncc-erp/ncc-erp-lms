import { Component, OnInit, Input, Injector, Output, EventEmitter } from '@angular/core';
import { QuestionDto } from '@app/models/question-dto';
import { AppComponentBase } from '@shared/app-component-base';
import { EQuestion, Common } from '@shared/AppEnums';
import { MatSnackBar } from '@angular/material';
import { DropEffect, DndDropEvent } from 'ngx-drag-drop';
import { StudentAnswerDto } from '@app/models/student-answer-dto';
import { StudentAnswerService } from '@app/services/student-service/student.answer.service';
import { QuizOptionDto } from '@app/models/quizzes-dto';
import { TestAttemptDto } from '@app/services/student-service/test.attempt.service';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient, HttpHeaders, HttpRequest } from '@angular/common/http';



@Component({
  selector: 'app-view-question',
  templateUrl: './view-question.component.html',
  styleUrls: ['./view-question.component.scss']
})
export class ViewQuestionComponent extends AppComponentBase implements OnInit {
  @Input() question: QuestionDto;
  @Input() testAttemptId: string;
  @Input() option: QuizOptionDto;
  @Input() isFinished:boolean
  // @Input() studentSubmittedAnswer: boolean = false;
  @Input() isAllSubmitted: boolean;
  @Input() isTeacherView?: boolean;
  @Input() index: number;
  // @Input() courseAssignedStudentId?: number;
  @Input() quizType: string; // quiz, quiz_final, survey
  @Output() studentAnswers: EventEmitter<StudentAnswerDto[]> = new EventEmitter();
  @Output() testAttemptChange: EventEmitter<TestAttemptDto> = new EventEmitter();
  isCorrect = false;
  public isShowMessage: boolean = false
  private currentDragEffectMsg: string;
  header: HttpHeaders = new HttpHeaders().append('Authorization', 'Bearer ' + abp.auth.getToken())
    .append('.AspNetCore.Culture', abp.utils.getCookieValue('Abp.Localization.CultureName') + '')
    .append('Abp.TenantId', abp.multiTenancy.getTenantIdCookie() + '');
  initTinymce = {
    height: 300,
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
          formData.append('Data', that.testAttemptId);
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
      const formData = new FormData(); formData.append('Data', this.question.questionQuizId);
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
  constructor(
    injector: Injector,
    private _studentAnswerService: StudentAnswerService,
    private snackBarService: MatSnackBar,
    private $http: HttpClient
  ) {
    super(injector)
  }

  ngOnDestroy(): void {
    if(!this.isFinished && !this.isTeacherView){
      this.saveCurrentAnswer()
    }
  }
  ngOnInit() {
    //•	Shuffle Answers
    window['enableReloadFunction'] = true;
    if (this.option.isShuffleAnswer) {
      this.shuffleAnswer();
    }

    if (this.question.type === EQuestion.Matching) {
      // matching -> init lAnswers
      if (this.question.lAnswers || this.question.lAnswers.length === 0) {
        this.question.lAnswers = [];
        if (this.question.answers && this.question.answers.length > 0) {
          const lAnswers = this.question.answers.map(item => item.lAnswer).filter((value, index, self) => self.indexOf(value) === index);
          this.question.lAnswers = [...lAnswers]
        }
      }
    } else if (this.question.type === EQuestion.Ranked) {
      this.question.viewType = EQuestion.ViewType_Select;
      // ranking - type select >> init question.
      this.question.sequenceOrders = this.question.answers.map(item => item.sequenceOrder)
        .filter((value, index, self) => self.indexOf(value) === index);
      // ranking - type drag drop
    }

    if (this.isTeacherView) {
      this.isCorrect = true;
      switch (this.question.type) {
        case EQuestion.MCQ:
          if (this.question.answers.find(s => (s.isSelected && !s.isCorrect) || (!s.isSelected && s.isCorrect)) != null) {
            this.isCorrect = false;
          }
          break;
        case EQuestion.FixedWord:
        case EQuestion.TrueFalse:
        case EQuestion.SCQ:
          if(this.question.selectedAnswerId == null){
            this.isCorrect=false
          }
          else{
            const item = this.question.answers.find(s => s.id === this.question.selectedAnswerId);
            if (item) {
              this.isCorrect = item.isCorrect;
            }
          }

          break;
        case EQuestion.Matching:
          let x = this.question.answers.find(s => s.matchTo != s.lAnswer);
          //console.log('Matching x=>', x);
          if (x && x != null) {
            this.isCorrect = false;
          }
          break;
        case EQuestion.OpenEnd:
          if (this.question.answerText === undefined || this.question.answerText.trim() === '') {
            this.isCorrect = false;
          }
          break;
        case EQuestion.Ranked:
          let rfind = this.question.answers.find(s => s.selectedSequenceOrder != s.sequenceOrder);
          //console.log('Ranked rfind=>', rfind);
          if (rfind) {
            this.isCorrect = false;
          }
          break;
        case EQuestion.MatrixTableQuestion:
      }
    }
    if (this.question.selectedAnswerId || this.question.answerText
      || (this.question.answers.some(answer => answer.isSelected || answer.selectedSequenceOrder || answer.matchTo))) {
      this.isShowMessage = true
    }
  }

  shuffleAnswer() {
    this.question.answers.sort(function () {
      return .5 - Math.random();
    });
  }

  show() {
    //console.log('question', this.question);
    //console.log('quizType => ', this.quizType)
    //console.log('option', this.option);
    // this.shuffleAnswer();
  }

  tips() {
    this.question.isShowedTip = true;
  }

  changeViewType() {
    if (this.question.viewType === EQuestion.ViewType_DragDrop) {
      this.question.viewType = EQuestion.ViewType_Select;
    } else {
      this.question.viewType = EQuestion.ViewType_DragDrop;
    }
  }

  onDragStart() {
    this.currentDragEffectMsg = "";
    this.snackBarService.dismiss();
    this.snackBarService.open('Drag started!', undefined, { duration: 2000 });
  }

  onDragged(item: any, list: any[], effect: DropEffect) {
    if (effect === 'move') {
      const index = list.indexOf(item);
      list.splice(index, 1);
    }
  }

  onDrop(event: DndDropEvent, list?: any[]) {
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

  onDragEnd() {
    this.snackBarService.dismiss();
    this.snackBarService.open(this.currentDragEffectMsg || `Drag ended!`, undefined, { duration: 2000 });
  }

  // teacherPoint: number;
  teacherSubmitPoint() {
    if (this.question.studentPoint > this.question.mark) {
      abp.message.error('Mark has to less than or equal \'' + this.question.mark + '\'!');
      return;
    }
    let item = {
      questionQuizId: this.question.questionQuizId,
      testAttempId: this.testAttemptId,
      mark: this.question.studentPoint,
      // courseAssignedStudentId: this.courseAssignedStudentId,
    } as TeacherStudentAnswerDto;
    this._studentAnswerService.TeacherTakeScoreOpenEndQuestion(item).subscribe(result => {
      this.notify.success('UpdateSuccessfully');
      this.testAttemptChange.emit(result.result);
    })
  }
  saveCurrentAnswer() {
    let studentAnswer = {
      questionId: this.question.id,
      testAttempId: this.testAttemptId,
      studentAsnwers: this.getAnswer(this.question)
    } as SaveAnswerDto
    if (studentAnswer.studentAsnwers && studentAnswer.studentAsnwers.length > 0 && !this.question.isSaved) {
      let message = `save answer for question ${this.index + 1}`
      let answerLog = [...studentAnswer.studentAsnwers]
      let args = {
        saveTime: this.formatTime(new Date()),
        question: this.question,
        studentAnswer: JSON.stringify(answerLog)
      }
      this.logStudentProcessToSentry(message, args);

      if (this.option.lookQuestionAfterAnswer) {
        abp.message.confirm(
          `Your question will be locked, do you want to save?`,
          "",
          (result: boolean) => {
            if (result) {
              this._studentAnswerService.CreateStudentAnswers(studentAnswer).subscribe(() => {
                abp.notify.success("answer saved")
                this.question.isDisable = true
                this.question.isSaved = true
                this.question.isAnswerChanged = false
              });
            }
          }
        )
      }
      else {
        this._studentAnswerService.CreateStudentAnswers(studentAnswer).subscribe(() => {
          abp.notify.success("answer saved")
          this.question.isSaved = true
          this.question.isAnswerChanged = false
        });
      }
    }
  }
  changeAnswer() {
    this.isShowMessage = true
    if (this.question.isSaved) {
      this.question.isAnswerChanged = true
    }
    this.question.isSaved = false
  }

  getAnswer(question: QuestionDto): StudentAnswerDto[] {
    const stuAnswers: StudentAnswerDto[] = [];
    switch (question.type) {
      case EQuestion.MCQ:
        question.answers.forEach(ans => {
          if (ans.isSelected) {
            let stuAns = new StudentAnswerDto();
            stuAns.answerId = ans.id;
            stuAnswers.push(stuAns);
          }
        })
        break;
      case EQuestion.FixedWord:
      case EQuestion.TrueFalse:
      case EQuestion.SCQ:
        if (question.selectedAnswerId) {
          let stuAns = new StudentAnswerDto();
          stuAns.answerId = question.selectedAnswerId;
          stuAnswers.push(stuAns);
        }
        break;
      case EQuestion.Matching:
        let lstAnswers = question.answers.filter(s => s.matchTo);
        lstAnswers.forEach(ans => {
          let stuAns = new StudentAnswerDto();
          stuAns.answerId = ans.id;
          stuAns.answerText = ans.matchTo;
          stuAnswers.push(stuAns);
        });
        break;

      case EQuestion.OpenEnd:
        if(question.answerText){
          let stuAns = new StudentAnswerDto();
          stuAns.answerText = question.answerText;
          stuAnswers.push(stuAns);
        }
        break;


      case EQuestion.Ranked:
        const rankedAnswers = question.answers.filter(s => s.selectedSequenceOrder != null);
        rankedAnswers.forEach(ans => {
          let stuAns = new StudentAnswerDto();
          stuAns.answerId = ans.id;
          stuAns.answerText = ans.selectedSequenceOrder + '';
          stuAnswers.push(stuAns);
        });

        break;
      case EQuestion.MatrixTableQuestion:

    }

    return stuAnswers;

  }

}


export class TeacherStudentAnswerDto {
  questionQuizId: string;
  testAttempId: string;
  // courseAssignedStudentId?: number;
  mark: number;
}
export class SaveAnswerDto {
  questionId: string;
  testAttempId: string;
  studentAsnwers: StudentAnswerDto[] = []

}
