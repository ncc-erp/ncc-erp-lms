import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { QuizDto as EditDto, CreateQuizDto as CreateDto, IQuizDto, QuizSettingsDto } from '@app/models/quizzes-dto';
import { QuizzesService } from '@app/services/systems-admin-services/quizzes.service';
import { HttpHeaders, HttpRequest, HttpClient, HttpErrorResponse } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
import { EQuiz } from '@shared/AppEnums';
import { ActivatedRoute } from '@angular/router';
import { CoursesService } from '@app/services/systems-admin-services/courses.service';
import { PagedRequestDto, PagedListingComponentBase, PagedResultResultDto } from '@shared/paged-listing-component-base';
import { QuestionsService } from '@app/services/systems-admin-services/questions.service';
import { QuestionDto as EditQuestionDto } from '@app/models/question-dto';
import { finalize, mapTo } from 'rxjs/operators';
import { InputFilterDto } from '@shared/filter/filter.component';
import { AnswerDto } from '@app/models/answer-dto';
import { GroupsService } from '@app/services/systems-admin-services/groups.service';
import { GroupsDto, Result as GroupResult } from '@app/models/groups-dto';
import { CreateQuestionComponent } from './create-question/create-question.component';
import { EditQuestionComponent } from './edit-question/edit-question.component';
import { CourseGroupDto, CourseGroupWithMemberDto } from '@app/models/course-group-dto';
@Component({
  selector: 'app-quizzes',
  templateUrl: './quizzes.component.html',
  styleUrls: ['./quizzes.component.scss']
})
export class QuizzesComponent extends PagedListingComponentBase<EditDto> implements OnInit {

  public readonly FILTER_CONFIG: InputFilterDto[] = [
    { propertyName: 'Title', comparisions: [0, 6, 7, 8] }];

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
  @ViewChild('createQuestionModal') createQuestionModal: CreateQuestionComponent;
  @ViewChild('editQuestionModal') editQuestionModal: EditQuestionComponent;
  courseId: string;
  quizId: string;
  courseInstanceId: string;
  isnewQuiz = true;
  oldQuizType: number;
  isEditQuiz = false;
  isallowAttempts = false;
  isTimeLimit = false;
  isallowAttemptsBox = true;
  studentSeeTheirQuiz = true;
  quiztypes: any[] = EQuiz.QUIZ_TYPES;
  quizscoretokeeptypes: any[] = EQuiz.QUIZ_SCORETOKEEPTYPES;
  studentreponsetypes: any[] = EQuiz.QUIZ_STUDENTRESPONSETYPES;
  questions: EditQuestionDto[] = [];
  quiz = {} as CreateDto;
  quizsettings = {} as QuizSettingsDto;
  searchText: string;
  dropdownSettings = {};
  dropdownListGroup = [];
  selectedGroupItems = [];
  courseGroups = {} as CourseGroupWithMemberDto;
  oldEditingQuestion: EditQuestionDto;
  saving = false;
  currentAnswer = { rAnswer: '', isCorrect: false } as AnswerDto;
  isEditAnswer = false;
  buttonName = 'Add';
  constructor(
    injector: Injector,
    private route: ActivatedRoute,
    private _service: QuizzesService,
    private _courseService: CoursesService,
    private _questionService: QuestionsService,

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
    const mode = this.route.snapshot.paramMap.get('mode');
    switch (mode) {
      case 'add': {
        this.courseInstanceId = this.route.snapshot.paramMap.get('id');
        this.getCourseData(this.courseInstanceId);
        break;
      }
      case 'edit': {
        this.quizId = this.route.snapshot.paramMap.get('id');
        this.isnewQuiz = false;
        this.getQuiz(this.quizId);
        break;
      }
      default: {
        break;
      }
    }
    this.refresh();
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'groupName',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 30,
      allowSearchFilter: true
    };
  }

  /* Group dropdown*/
  onItemSelect(item: any) {
  }
  onSelectAll(items: any) {
  }
  onItemDeSelect(item: any) {
  }
  /* End Group dropdown*/

  getCourseData(id: string) {
    this._courseService.getByCourseInstanceId(id).subscribe(item => {
      this.courseId = item.result.id;
    })
  }
  getQuiz(id: string) {
    this._service.getById(id).subscribe(item => {
      this.quiz = {} as EditDto;
      this.quiz = item.result;
      this.quizId = item.result.id;
      this.courseId = item.result.courseId;
      this.isallowAttempts = item.result.allowAttempts > 0;
      this.isTimeLimit = item.result.timeLimit > 0;
      this.quiz.responseType = String(item.result.responseType);
      this.courseInstanceId = item.result.courseInstanceId;
      this.quizsettings = item.result.settings;
    })
  }
  save() {
    this.quiz.settings = this.quizsettings;
    this.quiz.groupsAssingedQuiz = this.selectedGroupItems.map(({ id }) => id);
    if (this.isnewQuiz) {
      this.quiz.courseId = this.courseId;
      this.quiz.settings.courseInstanceId = this.courseInstanceId;
      this._service.create(this.quiz).subscribe((result: any) => {
        this.quizId = result.result.id;
        this.isnewQuiz = false;
        this.quizsettings = result.result.settings;
        this.notify.info('SaveSuccessfully!');
      })
    } else {
      this.quiz.id = this.quizId;
      this._service.update(this.quiz).subscribe((result) => {
        this.notify.info('UpdatedSuccessfully!');
      })
    }
  }
  delete() {
    if (!this.isnewQuiz) {
      this.quiz.courseId = this.courseId;
      this._service.delete(this.quizId).subscribe((result) => {
        this.notify.info('SaveSuccessfully!');
      })
    }
  }

  /* Questions */
  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this._questionService
      .GetQuestionsByQuizIdPagging(this.quizId, request)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: PagedResultResultDto) => {
        this.questions = result.result.items;
        // tslint:disable-next-line:no-unused-expression
        this.questions && this.questions.forEach(e => {
          e.typeName = this._questionService.getQuestionTypeName(e.type);
          // e.answers = [{ rAnswer: 'A. tron', lAnswer: '', isCorrect: true, questionId: e.id, sequenceOrder: 0, id: '' },
          // { rAnswer: 'A. vuong', lAnswer: '', isCorrect: false, questionId: e.id, sequenceOrder: 0, id: '' }];
        })
        this.showPaging(result.result, pageNumber);
      });
  }

  protected deleteQuestion(item: EditQuestionDto): void {
    abp.message.confirm(
      'Delete question \'' + item.title + '\'?',
      (result: boolean) => {
        if (result) {
          this._questionService.delete(item.id).subscribe(() => {
            abp.notify.info('Deleted question: ' + item.title);
            this.refresh();
          });
        }
      }
    );
  }

  createQuestion() {
    if (!this.isnewQuiz) {
      this.createQuestionModal.show();
    } else {
      abp.notify.info('You must save quiz first');
    }
  }

  editQuestion(id: string) {
    this.editQuestionModal.show(id);
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

  checkEditingQuestion(): boolean {
    if (this.oldEditingQuestion == null || this.oldEditingQuestion.id == null || this.questions == null || this.questions.length === 0) {
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
        for (let i = 0; i < this.oldEditingQuestion.answers.length; i++) {
          if (question.answers[i].rAnswer !== this.oldEditingQuestion.answers[i].rAnswer
            || question.answers[i].lAnswer !== this.oldEditingQuestion.answers[i].lAnswer
            || question.answers[i].isCorrect !== this.oldEditingQuestion.answers[i].isCorrect
            || question.answers[i].sequenceOrder !== this.oldEditingQuestion.answers[i].sequenceOrder
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

  editQuestion1(question: EditQuestionDto) {
    if (this.checkEditingQuestion()) {
      abp.message.confirm(
        'You are editing another question. Are you sure to cancel edit this question?',
        (result: boolean) => {
          if (result) {
            this.cancelEditingOtherQuestion();
            question.isEditing = true;

            this.oldEditingQuestion = new EditQuestionDto();
            this.mapQuestion(question, this.oldEditingQuestion);
            this.oldEditingQuestion.isEditing = false;
          } else {
            return;
          }
        }
      );
    } else {
      question.isEditing = true;

      this.oldEditingQuestion = new EditQuestionDto();
      this.mapQuestion(question, this.oldEditingQuestion);
      this.oldEditingQuestion.isEditing = false;
    }

    // question.isExpanded = false;
  }
  saveQuestion(question: EditQuestionDto) {
    this.saving = true;
    this._questionService.update(question)
      .pipe(finalize(() => { this.saving = false; }))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        question.isEditing = false;
        this.oldEditingQuestion = question;
      });
  }

  cancelEditQuestion(question: EditQuestionDto) {
    const propertyNames = Object.getOwnPropertyNames(this.oldEditingQuestion);
    propertyNames.forEach(element => {
      if (question[element] != null) {
        question[element] = this.oldEditingQuestion[element];
      }
    });
    question.answers = [];
    if (this.oldEditingQuestion.answers && this.oldEditingQuestion.answers.length > 0) {
      this.oldEditingQuestion.answers.forEach(ans => {
        question.answers.push(ans);
      })
    }
    question.isEditing = false;
    question.isExpanded = true;
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
  /* Questions end*/
}
