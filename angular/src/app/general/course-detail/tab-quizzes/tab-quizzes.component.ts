import { Component, OnInit, Injector, Input } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { QuizzesService } from '@app/services/systems-admin-services/quizzes.service';
import { QuizDto, QuizSettingsDto } from '@app/models/quizzes-dto';
import { EQuiz, EQuestion, QuizType } from '@shared/AppEnums';
import { Router } from '@angular/router';
import { QuestionsService } from '@app/services/systems-admin-services/questions.service';
import { PagedRequestDto } from '@shared/paged-listing-component-base';
import { QuestionDto as EditQuestionDto, CreateQuestionDto, QuestionPoolDto } from '@app/models/question-dto';
import { CourseGroupService } from '@app/services/systems-admin-services/course.group.service';
import { AnswerDto } from '@app/models/answer-dto';
import { finalize } from 'rxjs/operators';
import { CourseSettingService } from '@app/services/systems-admin-services/course.setting.service';
import { DropEffect, DndDropEvent } from 'ngx-drag-drop';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient, HttpHeaders, HttpRequest } from '@angular/common/http';

@Component({
  selector: 'app-tab-quizzes',
  templateUrl: './tab-quizzes.component.html',
  styleUrls: ['./tab-quizzes.component.scss']
})
export class TabQuizzesComponent extends AppComponentBase implements OnInit {
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
  @Input() courseId: string;
  @Input() courseInstanceId: string;
  ListPanel = true;
  assignmentQuizzes: QuizDto[] = [];
  surveyQuizzes: QuizDto[] = [];
  quiz: QuizDto = {} as QuizDto;

  dropdownSettings = {
    singleSelection: false,
    idField: 'id',
    textField: 'groupName',
    selectAllText: 'Select All',
    unSelectAllText: 'UnSelect All',
    itemsShowLimit: 30,
    allowSearchFilter: true
  };
  dropdownListGroups = [];
  selectedGroupItems = [];
  quiztypes: any[] = EQuiz.QUIZ_TYPES;
  quizscoretokeeptypes: any[] = EQuiz.QUIZ_SCORETOKEEPTYPES;
  studentreponsetypes: any[] = EQuiz.QUIZ_STUDENTRESPONSETYPES;
  questions: EditQuestionDto[] = [];
  question = {} as EditQuestionDto;
  cquestion = {} as CreateQuestionDto;
  isTimeLimit = false;
  isallowAttemptsBox = false;
  isallowAttempts = false;
  studentSeeTheirQuiz = false;
  questionList = true;
  currentAnswer = { rAnswer: '', isCorrect: false } as AnswerDto;
  saving = false;
  isEditAnswer = false;
  buttonName = 'Add';
  oldQuestionType: number;
  types: any[] = EQuestion.QUESTION_TYPES;
  groups: any[] = EQuestion.QUESTION_CATES;
  oldEditingQuestion: EditQuestionDto;
  isAllowNotify = false;
  questionPool = false;
  questionspool: QuestionPoolDto[] = [];
  addoreditQuestion = false;
  isBtnSave = false;
  avaiableFromDate: Date;
  avaiableToDate: Date;
  avaiableFromDate_old: Date | string;
  avaiableToDate_old: Date | string;
  trueFalseValue = true;
  constructor(
    injector: Injector,
    private router: Router,
    private _quizzesService: QuizzesService,
    private _questionService: QuestionsService,
    private _courseGroupService: CourseGroupService,
    private _courseSettingService: CourseSettingService,
    private $http: HttpClient
  ) { super(injector); }

  ngOnInit() {
    this.quiz.settings = new QuizSettingsDto;
    this.getQuizzesByCourseId(this.courseId);
    this.getcousegroupbyCourseId(this.courseInstanceId);
  }
  getQuizzesByCourseId(courseId: string) {
    this.assignmentQuizzes = [];
    this.surveyQuizzes = [];
    this._quizzesService.getQuizzesByCourseId(courseId).subscribe((result) => {
      let assignquiz = 0;
      for (let i = 0; i < EQuiz.QUIZ_TYPES.length; i++) {
        if (EQuiz.QUIZ_TYPES[i].name === 'Quiz') {
          assignquiz = EQuiz.QUIZ_TYPES[i].id;
        }
      }
      result.result.forEach(element => {
        if (element.type === assignquiz) {
          this.assignmentQuizzes.push(element);
        } else {
          this.surveyQuizzes.push(element);
        }
      });
      // this.onChangeApplyTime();
    })
  }
  protected deleteQuiz(item: QuizDto): void {
    abp.message.confirm(
      'Delete quiz \'' + item.title + '\'?',
      (result: boolean) => {
        if (result) {
          this._quizzesService.delete(item.id).subscribe(() => {
            abp.notify.info('Deleted quiz: ' + item.title);
            this.getQuizzesByCourseId(this.courseId);
          });
        }
      }
    );
  }
  addNewQuiz() {
    this.quiz = {
      settings: { noOfDueDays: 0, point: 1, totalNumberQuestion: 0, applySameStartEndTimeAsCourse: true, } as QuizSettingsDto,
      type: 0,
      // point: 1,
      isShuffleAnswer: true,
      timeLimit: 5,
      scoreKeepType: 0,
      allowAttempts: 2,
      showOneQuestionAtATime: true,
      lookQuestionAfterAnswer: false,
      responseType: '2',
    } as QuizDto;
    this.isallowAttemptsBox = false;
    this.questions = [];
    this.selectedGroupItems = this.dropdownListGroups.filter(r => r.isEveryOne);
    this.onItemSelect(this.selectedGroupItems);
    this.ListPanel = false;
    this.setDateOfCourseInstance();
  }
  editQuiz(item: any) {
    this.selectedGroupItems = [];
    this.quiz = item;
    this.quiz.settings = {} as QuizSettingsDto;
    this.getQuestionByQuiz(this.quiz.id);
    this.getQuizData(this.quiz.id);
    this.isTimeLimit = this.quiz.timeLimit > 0;
    this.isallowAttemptsBox = this.quiz.allowAttempts > 0;
    this.isallowAttempts = this.quiz.allowAttempts > 0;
    this.studentSeeTheirQuiz = (Number(this.quiz.responseType) !== 10);
    this.quiz.responseType = String(this.quiz.responseType);
    this.ListPanel = false;

  }
  getQuestionByQuiz(quizId: string) {
    this._questionService.GetQuestionsByQuizIdNotPagging(this.quiz.id).subscribe((result) => {
      this.questions = result.result;
      // tslint:disable-next-line:no-unused-expression
      this.questions && this.questions.forEach(e => {
        e.typeName = this._questionService.getQuestionTypeName(e.type);
      });
    });
  }


  onTimeLimitChange() {
    if (this.isTimeLimit) {
      this.quiz.timeLimit = 5;
    } else {
      this.quiz.timeLimit = null;
    }
  }
  onAllowAttemptBoxChange() {
    if (this.isallowAttemptsBox) {
      this.isallowAttempts = true;
      this.quiz.allowAttempts = 2;
      this.quiz.scoreKeepType = 0;
    } else {
      this.isallowAttempts = false;
      this.quiz.allowAttempts = null;
      this.quiz.scoreKeepType = 0;
    }
  }

  onAllowAttemptChange() {
    this.quiz.allowAttempts = !this.isallowAttempts ? 2 : null;
  }

  backToList() {
    this.getQuizzesByCourseId(this.courseId);
    this.ListPanel = true;
    this.questionPool = false;
  }
  saveData(notify: boolean) {
    if (!this.isTimeLimit) {
      this.quiz.timeLimit = null;
    }
    if (!this.isallowAttempts || !this.isallowAttemptsBox) {
      this.quiz.allowAttempts = null;
    }
    if (!this.studentSeeTheirQuiz) {
      this.quiz.responseType = '10';
    }
    if (!this.quiz.showOneQuestionAtATime) {
      this.quiz.lookQuestionAfterAnswer = false;
    }
    this.quiz.allowNotify = notify;
    this.quiz.groupsAssingedQuiz = this.selectedGroupItems.map(({ id }) => id);
    this.quiz.courseInstanceId = this.courseInstanceId;

    this.quiz.settings.startTimeUtc = this.getDateUTC(this.quiz.settings.startTimeUtc);
    this.quiz.settings.endTimeUtc = this.getDateUTC(this.quiz.settings.endTimeUtc);
    if (!this.quiz.settings.applySameStartEndTimeAsCourse) {
      this.getOldDateQuizSetting();
    }

    if (this.quiz.type === QuizType.Survey) {
      this.quiz.allowAttempts = null;
    }
    if (this.quiz.id == null) {
      this.quiz.courseId = this.courseId;
      this.quiz.settings.courseInstanceId = this.courseInstanceId;
      this._quizzesService.create(this.quiz).subscribe((result: any) => {
        this.quiz.id = result.result.id;
        this.quiz.settings = result.result.settings;
        this.notify.info('SaveSuccessfully!');
        this.getQuizzesByCourseId(this.courseId);
        // this.ListPanel = true;
      })
    } else {
      this._quizzesService.update(this.quiz).subscribe((result) => {
        this.notify.info('UpdatedSuccessfully!');
        this.getQuizzesByCourseId(this.courseId);
        this.ListPanel = false;
      })
    }
    this.onChangeApplyTime();
  }
  save() {
    this.quiz.status = 0;
    this.saveData(false);
  }
  saveAndPublish() {
    this.quiz.status = 1;
    this.saveData(this.isAllowNotify);
  }
  cancel() {
    this.getQuizzesByCourseId(this.courseId);
    this.ListPanel = true;
  }
  show() {
    this.log(this.quiz);
  }
  getQuizData(quizId: string) {
    this._quizzesService.getQuizData(quizId, this.courseInstanceId).subscribe((result: any) => {
      this.quiz.settings = result.result.settings;
      this.getOldDateQuizSetting();
      this.quiz.groupsAssingedQuiz = result.result.groupsAssingedQuiz;
      if (this.quiz.groupsAssingedQuiz != null) {
        this.selectedGroupItems = this.dropdownListGroups.filter(x => this.quiz.groupsAssingedQuiz.indexOf(x.id) > -1);
      }
      this.onItemSelect(this.selectedGroupItems);

      this.setDateOfCourseInstance();
    })
  }

  private getOldDateQuizSetting() {
    this.avaiableFromDate_old = this.quiz.settings.startTimeUtc;
    this.avaiableToDate_old = this.quiz.settings.endTimeUtc;
  }
  /* Group dropdown*/
  getcousegroupbyCourseId(courseInstanceId: string) {
    this._courseGroupService.getCourseGroupsByCourseId(courseInstanceId).subscribe((result) => {
      this.dropdownListGroups = result.result;
    })
  }
  onItemSelect(item: any) {
  }
  /* End Group dropdown*/

  /* Questions */
  createQuestion() {
    if (this.quiz.id == null) {
      abp.notify.info('You must save quiz first');
      return
    }
    this.question = { mark: 1 } as EditQuestionDto;
    this.prepareControl();
    this.question.isEditing = true;
    this.question.isExpanded = true;
    this.question.answers = [];
    this.questions.unshift(this.question);
    this.addoreditQuestion = true;
    // this.questionList = false;
  }
  editQuestion(item: EditQuestionDto) {
    if (this.checkEditingQuestion(item)) {
      abp.message.confirm(
        'You are editing another question. Are you sure to cancel edit this question?',
        (result: boolean) => {
          if (result) {
            this.cancelEditingOtherQuestion();
            this.question = item;
            this.oldQuestionType = item.type;
            this.prepareControl();
            item.isExpanded = true;
            // item.isEditing = true;
            this.oldEditingQuestion = {} as EditQuestionDto;
            this.mapQuestion(item, this.oldEditingQuestion);
            this.oldEditingQuestion.isEditing = false;
            this.addoreditQuestion = true;
            item.isEditing = true;
          } else {
            return;
          }
        }
      );
    } else {
      // item.isEditing = true;
      this.addoreditQuestion = true;
      this.oldEditingQuestion = {} as EditQuestionDto;
      this.mapQuestion(item, this.oldEditingQuestion);
      this.oldEditingQuestion.isEditing = false;
      this.question = item;
      item.isEditing = true;
    }

  }
  prepareControl() {
    this.question.group = this.quiz.type;
    // this.groups.forEach(item => {
    //   if (item.id === this.quiz.type) {
    //     this.types = item.types;
    //   }
    // })
  }
  removeLink(item: EditQuestionDto) {
    abp.message.confirm(
      'Remove this question?',
      (result: boolean) => {
        if (result) {
          this._questionService.RemoveLink(this.quiz.id, item.id).subscribe(() => {
            abp.notify.info('Remove question: ' + item.title);
            this.getQuestionByQuiz(this.quiz.id);
          });
        }
      }
    );
  }
  deleteQuestion(item: EditQuestionDto) {
    abp.message.confirm(
      'Delete this question?',
      (result: boolean) => {
        if (result) {
          this._questionService.delete(item.id).subscribe(() => {
            abp.notify.info('Deleted question: ' + item.title);
            this.getQuestionByQuiz(this.quiz.id);
          });
        }
      }
    );
  }
  cancelEditQuestion() {
    // this.questionList = true;
    this.question.isEditing = false;
    this.question.isExpanded = true;
    this.getQuestionByQuiz(this.quiz.id);
    this.addoreditQuestion = false;
  }

  showTrueFalseValue() {
  }

  saveQuestion(question: EditQuestionDto) {
    this.updateQuestionTrueFalse(question);
    this.saving = true;
    if (question.id == null) {
      this.cquestion = {
        title: question.title,
        description: question.description,
        mark: question.mark,
        nWord: question.nWord,
        type: question.type,
        group: question.group,
        courseId: this.courseId,
        quizId: this.quiz.id,
        answers: question.answers
      } as CreateQuestionDto;
      this._questionService.create(this.cquestion)
        .pipe(finalize(() => { this.saving = false; }))
        .subscribe(() => {
          this.notify.info(this.l('SavedSuccessfully'));
          question.isEditing = false;
          // this.oldEditingQuestion = question;
          this.getQuestionByQuiz(this.quiz.id);
          this.addoreditQuestion = false;
          this.oldEditingQuestion = null;
        });
    } else {
      this._questionService.update(question)
        .pipe(finalize(() => { this.saving = false; }))
        .subscribe(() => {
          this.notify.info(this.l('UpdatedSuccessfully'));
          question.isEditing = false;
          // this.oldEditingQuestion = question;
          this.getQuestionByQuiz(this.quiz.id);
          this.addoreditQuestion = false;
          this.oldEditingQuestion = null;
        });
    }
  }

  updateQuestionTrueFalse(question: EditQuestionDto) {
    if (question.type === EQuestion.TrueFalse) {
      question.answers = [];
      const answerTrue = new AnswerDto();
      answerTrue.isCorrect = this.trueFalseValue;
      answerTrue.rAnswer = 'True';
      question.answers.push(answerTrue);
      const answerFalse = new AnswerDto();
      answerFalse.isCorrect = !this.trueFalseValue;
      answerFalse.rAnswer = 'False';
      question.answers.push(answerFalse);
    }
  }
  addOrEditAnswer(question: EditQuestionDto) {
    if (!this.currentAnswer.rAnswer || this.currentAnswer.rAnswer.trim() === '') {
      return;
    }
    if (!this.isEditAnswer) {
      question.answers.push(this.currentAnswer);
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

  deleteAnswer(question: EditQuestionDto, item: AnswerDto) {
    question.answers.splice(question.answers.indexOf(item), 1);
  }
  onQuestionTypeChange(value: any) {
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
  checkEditingQuestion(item: EditQuestionDto): boolean {
    if (this.oldEditingQuestion == null ||
      this.oldEditingQuestion.id == null ||
      this.questions == null ||
      this.questions.length === 0 ||
      this.oldEditingQuestion.id === item.id) {
      return false;
    }
    for (let i = 0; i < this.questions.length; i++) {
      const question = this.questions[i];
      if (question.id === this.oldEditingQuestion.id) {

        if (question.title !== this.oldEditingQuestion.title
          || question.mark !== this.oldEditingQuestion.mark
          || question.nWord !== this.oldEditingQuestion.nWord
        ) {
          return true;
        }
        if (question.answers.length !== this.oldEditingQuestion.answers.length) {
          return true;
        }
        for (let j = 0; j < this.oldEditingQuestion.answers.length; j++) {
          if (question.answers[j].rAnswer !== this.oldEditingQuestion.answers[j].rAnswer
            || question.answers[j].lAnswer !== this.oldEditingQuestion.answers[j].lAnswer
            || question.answers[j].isCorrect !== this.oldEditingQuestion.answers[j].isCorrect
            || question.answers[j].sequenceOrder !== this.oldEditingQuestion.answers[j].sequenceOrder
          ) {
            return true;
          }
        }
        question.isEditing = false;
        question.isExpanded = false;
        return false;

      }
    }
    return false;
  }
  mapQuestion(sourse: EditQuestionDto, dest: EditQuestionDto) {
    const propertyNames = Object.getOwnPropertyNames(sourse);
    propertyNames.forEach(element => {
      if (sourse[element] != null) {
        dest[element] = sourse[element];
      }
    });
    dest.answers = [];
    if (sourse.answers && sourse.answers.length > 0) {
      sourse.answers.forEach(ans => {
        dest.answers.push(ans);
      })
    }
  }
  cancelEditingOtherQuestion() {
    if (this.oldEditingQuestion && this.oldEditingQuestion.id) {
      if (this.questions && this.questions.length > 0) {
        for (let i = 0; i < this.questions.length; i++) {
          if (this.questions[i].id === this.oldEditingQuestion.id) {
            this.mapQuestion(this.oldEditingQuestion, this.questions[i]);
            this.questions[i].isEditing = false;
            this.questions[i].isExpanded = false;
            return;
          }
        }
      }
    }

  }
  viewPool() {
    this.questionPool = true;
    this.getQuestionPool();
  }
  getQuestionPool() {
    // Todo: fix paging
    const request = {
      filterItems: [],
      maxResultCount: 100,
      searchText: '',
      skipCount: 0,
    } as PagedRequestDto;
    this._questionService.GetQuestionsPool(this.courseId, this.quiz.id, request).subscribe((result) => {
      this.questionspool = result.result.items;
      // tslint:disable-next-line:no-unused-expression
      this.questionspool && this.questionspool.forEach(e => {
        e.typeName = this._questionService.getQuestionTypeName(e.type);
        if (this.questions.filter(q => q.id === e.id).length > 0) {
          e.linkable = false;
        } else {
          e.linkable = true;
        }
      });
    });
  }
  cloneQuestion(item: EditQuestionDto) {
    const clonequestion = item;
    this.question = item;
    clonequestion.group = this.quiz.type;
    clonequestion.answers = item.answers;
    clonequestion.id = null;
    this.prepareControl();
    clonequestion.isEditing = true;
    clonequestion.isExpanded = true;
    this.questions.unshift(clonequestion);
    this.questionPool = false;

  }
  linkQuestion(item: EditQuestionDto) {
    this._questionService.LinkQuestion(this.quiz.id, item.id).subscribe((result) => {
      this.notify.info(this.l('UpdatedSuccessfully'));
      this.getQuestionByQuiz(this.quiz.id);
      this.questionPool = false;
    });
  }
  cancelPool() {
    this.questionPool = false;
  }
  /*End question */
  setDateOfCourseInstance() {
    this._courseSettingService.getById(this.courseInstanceId).subscribe(data => {
      this.avaiableFromDate = data.result.startTime;
      this.avaiableToDate = data.result.endTime;
      this.onChangeApplyTime();
    });
  }

  onChangeApplyTime() {
    if (this.quiz.settings.applySameStartEndTimeAsCourse) {
      this.quiz.settings.startTimeUtc = this.getDateLocal(this.avaiableFromDate);
      this.quiz.settings.endTimeUtc = this.getDateLocal(this.avaiableToDate);
    } else {
      this.quiz.settings.startTimeUtc = this.getDateLocal(this.avaiableFromDate_old);
      this.quiz.settings.endTimeUtc = this.getDateLocal(this.avaiableToDate_old);
    }
  }


  onDragged(item: any, list: any[], effect: DropEffect) {
    if (effect === 'move') {
      this.log('move');
      const index = list.indexOf(item);
      list.splice(index, 1);
      this.isBtnSave = true;
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
      this.isBtnSave = true;
    }
  }

  // thay đổi vị trí question
  SaveIndexQuestion() {
    const listChange = [];
    let index = 0;
    this.questions.forEach(x => {
      const listChangedto = {
        id: x.questionQuizId,
        index: index++
      }
      listChange.push(listChangedto);
    })
    const input = {
      listChange: listChange,
      quizId: this.quiz.id
    }
    this._questionService.SaveIndexQuestion(input).subscribe(data => {
      this.notify.info('SaveSuccessful');
      this.getQuestionByQuiz('');
      this.isBtnSave = false;
    })
  }
}
