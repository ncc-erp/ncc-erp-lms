import { StudentProgressService } from '@app/services/student-service/student.progress.service';
import { Component, OnInit, Injector, Input, OnDestroy, ViewChild, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { TestAttemptDto, TestAttemptService } from '@app/services/student-service/test.attempt.service';
import { TestAttemptStatus, EQuestion, Common, QuizScoreKeepType, PageLinkExamType, PageType } from '@shared/AppEnums';
import { LocalDataDto, QuestionDto } from '@app/models/question-dto';
import { QuizOptionDto } from '@app/models/quizzes-dto';
import { QuizzesService } from '@app/services/systems-admin-services/quizzes.service';
import { QuestionsService } from '@app/services/systems-admin-services/questions.service';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { takeUntil } from 'rxjs/operators';
import { StudentAnswerDto } from '@app/models/student-answer-dto';
import { StudentAnswerService } from '@app/services/student-service/student.answer.service';
import { AnswerDto } from '@app/models/answer-dto';
import { CountdownComponent } from 'ngx-countdown';
import { CountdownConfig } from 'ngx-countdown/src/countdown.config';
import { NavigationStart, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { MatDialog } from '@angular/material';
import { PopupSubmitAnswerComponent } from '../popup-submit-answer/popup-submit-answer.component';import { SaveAnswerDto } from '@shared/view-question/view-question.component';
;

@Component({
  selector: 'app-quiz',
  templateUrl: './quiz.component.html',
  styleUrls: ['./quiz.component.scss']
})
export class QuizComponent extends PagedListingComponentBase<QuestionDto> implements OnInit, OnChanges, OnDestroy {
  // export class QuizComponent extends AppComponentBase implements OnInit {
  @Input() courseInstanceId: string;
  @Input() quizSettingId: string;
  @Input() quizType: string; // quiz, quiz_final or survey
  @Input() pageId: string; // cần lưu lại pageId để direct đến đúng page đang làm
  quizOption = {} as QuizOptionDto;
  questions: QuestionDto[] = [];
  quizId: string;
  displayQuestions: QuestionDto[] = []; // questions that are display
  @Output() status: EventEmitter<any> = new EventEmitter<any>();
  destroy: Subject<boolean> = new Subject();
  studentDidAnswers: StudentAnswerDto[] = [];
  isAllSubmitted = false;
  highestAttempt = {} as TestAttemptDto;
  avarageScore = 0;
  completedAttempts: TestAttemptDto[] = [];
  isViewCountDown = false;
  @ViewChild(CountdownComponent) counter: CountdownComponent;
  @ViewChild("question") currentQuestion
  isSpecialStopCounting = false; //  dấu được phép lưu bài quiz vào localStorage
  timeLeftAfterReload = null;
  selectedItem = 0;
  localName = '';
  dataFromLocal: LocalDataDto = {} as LocalDataDto
  config = { template: '$!h!:$!m!:$!s!', demand: true } as CountdownConfig;
  checkValidSubmit = true;
  isDisplayQuesionsSubmitted = false;
  isDidTakeScore = false;
  isFinished = false;

  constructor(
    injector: Injector,
    private _testAttemptService: TestAttemptService,
    private _quizService: QuizzesService,
    private _questionService: QuestionsService,
    private _studentAnswerService: StudentAnswerService,
    private _router: Router,
    private dialog: MatDialog,
    public studentProcessService: StudentProgressService
  ) {
    super(injector);
    window['enableReloadFunction'] = true;
    this._router.events.pipe(takeUntil(this.destroy)).subscribe(res => {
      if (res instanceof NavigationStart && window['enableReloadFunction']
        && this.displayQuestions.length && !this.isDidTakeScore) { // event click browser back button
        this.isSpecialStopCounting = true;
        this.pauseCouting();
      }
    })
  }

  ngOnInit() {
    const paramThis = this;
    window.addEventListener('beforeunload', function (e) {   // confirm diaLog khi ấn thoát
      if (window['enableReloadFunction'] && paramThis.displayQuestions.length && !paramThis.isDidTakeScore) {
        const confirmationMessage = 'Bạn đang làm bài, bạn có chắc chắn thoát?';
        (e || window.event).returnValue = confirmationMessage;
        paramThis.isSpecialStopCounting = true;
        paramThis.pauseCouting();
        return confirmationMessage;
      }
    });
  }

  getDataFromLocalStorage(quizId: string) {
    this.dataFromLocal = this.getLocal(this.localName) as LocalDataDto;
    if (this.dataFromLocal && this.dataFromLocal.quizId == quizId) {
      this.timeLeftAfterReload = this.dataFromLocal.timeLeft - (+(new Date().getTime() / 1000).toFixed(0)) + this.dataFromLocal.createdTime;
      if (this.timeLeftAfterReload <= 0 || this.appSession.user.id !== this.dataFromLocal.userId) {
        this.deleteLocal(this.localName);
        this.timeLeftAfterReload = 0;
      }
    } else {
      this.timeLeftAfterReload = null;
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.localName = this.appSession.user.id + "";
    if (changes && changes.quizSettingId) {
      this.getDataFromLocalStorage(changes.quizSettingId.currentValue);
      if (changes.quizSettingId.currentValue !== changes.quizSettingId.previousValue) {
        this.getOption(changes.quizSettingId.currentValue, this.courseInstanceId);
      }
    }
  }

  getOption(quizSettingId: string, courseInstanceId: string) {
    this._quizService.GetQuizOptionsAndTestAttemps(quizSettingId, courseInstanceId, this.quizType).subscribe(result => {
      this.quizOption = result.result;
      if (!result.result.isExpired) {
        this.quizId = this.quizOption.id;
        this.processOption();
        let localData = this.getLocal(this.localName) as LocalDataDto
        if (localData && result.result.testingAttempt.timeRemaining) {
          localData.timeLeft = result.result.testingAttempt.timeRemaining
          this.setLocal(this.localName, localData)
        }
      }
    });
  }

  processOption() {
    this.isViewCountDown = false;
    this.isDidTakeScore = false;
    this.isAllSubmitted = false;
    // •	Allow Multiple Attempts
    // allowAttempts <  testAttempts.length -> can do quiz again
    // esle => completed quiz
    this.completedAttempts
      = this.quizOption.testAttempts && this.quizOption.testAttempts.filter(x => x.status === TestAttemptStatus.Marking);
    this.quizOption.allowAttempts = this.quizOption.allowAttempts ? this.quizOption.allowAttempts : 1;
    if (this.completedAttempts && this.completedAttempts.length > 0) {
      // already done allowAttempts
      // this.isDisplayCompletedAttempts = true;
      if (this.quizOption.scoreKeepType === QuizScoreKeepType.Hightest) {
        this.getHighestTestAttempt(this.completedAttempts);
      } else {
        // calculate avarage score
        this.caculateAvarageScore(this.completedAttempts);
      }
    }
    if (this.completedAttempts.length >= this.quizOption.allowAttempts) {
      return;
    }

    if (this.quizOption.testingAttempt && this.quizOption.testingAttempt.status === TestAttemptStatus.Testing) {
      this.status.emit(TestAttemptStatus.Testing);
      this.questions = this.quizOption.testingAttempt.questions;
      if (this.timeLeftAfterReload) { // trả lại các đáp án của từng câu hỏi đã được lưu
        this.restoreQuestionAnswers(this.questions, this.dataFromLocal.questions)
      }
      this.processQuestions();
      if (!this.quizOption.testingAttempt.timeRemaining) {
        return;
      }

      if (this.quizOption.testingAttempt.timeRemaining > 0) {
        this.config.leftTime = this.quizOption.testingAttempt.timeRemaining;
        this.isViewCountDown = true;
        if (this.counter) {
          this.counter.begin();
          this.isSpecialStopCounting = true; // lưu lại vào localStorage bài thi hiện tại
          this.pauseCouting()
        } else {
          this.config.demand = false;
        }

      } else if (this.timeLeftAfterReload) {
        this.config.leftTime = this.timeLeftAfterReload;
        this.isViewCountDown = true;
        if (this.counter) {
          this.counter.begin();
        } else {
          this.config.demand = false;
        }
      } else {
        this.config.leftTime = 0;
        this.isViewCountDown = true;
        if (this.counter) {
          this.counter.stop();
        }
        this.takeScore();
      }
    }
  }

  caculateAvarageScore(completedAttempts: TestAttemptDto[]) {
    this.avarageScore = 0;
    completedAttempts.forEach(attempt => {
      this.avarageScore += attempt.score;
    })
    this.avarageScore = Math.round(this.avarageScore / this.completedAttempts.length * 100) / 100;
  }

  getHighestTestAttempt(completedAttempts: TestAttemptDto[]) {
    if (completedAttempts && completedAttempts.length > 0) {
      this.highestAttempt = completedAttempts.sort((a: TestAttemptDto, b: TestAttemptDto) => b.score - a.score)[0];
    } else {
      this.highestAttempt = {} as TestAttemptDto;
    }

  }

  sendMessage() {
    this.studentProcessService.showMess = false
  }

  startDoQuiz() {
    let message = `Start do quiz`
    let args = {
      quizInfo: this.quizOption,
      startTime: this.formatTime(new Date())
    }
    this.logStudentProcessToSentry(message, args);


    this.isFinished = false
    this.sendMessage();
    const item = {} as TestAttemptDto;
    item.quizSettingId = this.quizOption.settings.id;
    item.status = TestAttemptStatus.Testing;
    this._testAttemptService.create(item).subscribe(result => {
      this.quizOption.testingAttempt = result.result;
      this.questions = result.result.questions;
      this.processQuestions();
      if (this.quizOption.timeLimit && this.quizOption.timeLimit > 0) {
        this.quizOption.testingAttempt.timeRemaining = this.quizOption.timeLimit;
        this.initCountDown(this.quizOption.timeLimit);
        this.isViewCountDown = true;
      } else {
        this.isViewCountDown = false;
      }
      // this.isStarted = true;
      this.status.emit(TestAttemptStatus.Testing);

    })
  }

  getEvents($event) {
    if ($event && $event.action === 'pause') {
      if (this.isSpecialStopCounting) {
        this.generateLocalStorageData(($event.left || 0) / 1000);
      }
    }
  }

  restoreQuestionAnswers(questions: QuestionDto[], questionsRestored: QuestionDto[]) {
    questions.forEach(q => {
      let qRestored = questionsRestored.find(item => q.id === item.id);
      if (qRestored) {
        switch (q.type) {
          case EQuestion.MCQ:
            let selectedArr = qRestored.answers.map(item => {
              return item.isSelected ? item.id : '';
            })
            q.answers.forEach(item => {
              item.isSelected = selectedArr.some(i => i === item.id)
            })
            return;
          case EQuestion.SCQ:
          case EQuestion.FixedWord:
          case EQuestion.TrueFalse:
            q.selectedAnswerId = qRestored.selectedAnswerId;
            return;
          case EQuestion.Ranked:
            q.answers.forEach(item => {
              item.selectedSequenceOrder = (qRestored.answers.find(it => it.id == item.id) || {} as AnswerDto).selectedSequenceOrder
            })
            return;
          case EQuestion.OpenEnd:
            q.answerText = qRestored.answerText;
            return;
          case EQuestion.Matching:
            q.answers.forEach(item => {
              item.matchTo = (qRestored.answers.find(it => it.id == item.id) || {} as AnswerDto).matchTo
            })
            return;
          case EQuestion.MatrixTableQuestion:
        }
      }
    })
  }

  pauseCouting() {
    if (this.counter) {
      this.counter.pause();
      setTimeout(() => {
        this.counter.resume();
      }, 0);
    } else {
      this.generateLocalStorageData(1000);
    }
  }

  generateLocalStorageData(timeLeft: number) {
    let data = {
      createdTime: +(new Date().getTime() / 1000).toFixed(0),
      questions: this.questions,
      quizId: this.quizSettingId,
      timeLeft: timeLeft,
      userId: this.appSession.user.id,
      questionIdOnScreen: this.displayQuestions.map(item => {
        return item.id;
      }),
      pageId: this.pageId
    } as LocalDataDto;
    this.setLocal(this.localName, data);
    this.isSpecialStopCounting = false;
  }

  initCountDown(timeLimit: number) {
    if (timeLimit && timeLimit > 0) {
      this.config.leftTime = timeLimit * 60;
      if (this.counter) {
        this.counter.begin();
      } else {
        this.config.demand = false;
      }

    }
  }


  onStart() {
  }


   onFinished() {
    if (this.counter === undefined) {
      return;
    }
    this.saveAndFinish()
  }
  // end countdown module


  getQuestions(quizId: string) {
    this._questionService.GetQuestionsByQuizIdNotPagging(quizId).subscribe(result => {
      this.questions = result.result;
      this.pageSize = this.quizOption.showOneQuestionAtATime ? 1 : this.questions.length;
      this.processQuestions();
    })
  }

  processQuestions() {
    this.pageSize = this.quizOption.showOneQuestionAtATime ? 1 : this.questions.length;
    this.tipCorrectAnswers(this.questions);
    if (this.quizOption.testingAttempt && this.quizOption.testingAttempt.studentAnswers) {
      this.bindSudentAnswerToQuestion(this.questions, this.quizOption.testingAttempt.studentAnswers);
    }
    this.lockQuestionsAfterAnswer(this.questions);
    this.isAllSubmitted = this.isAllQuestionsSubmitted(this.questions);
    this.refresh();
  }


  tipCorrectAnswers(questions: QuestionDto[]) {
    if (this.quizOption.responseType === '10') {
      return;
    }
    questions.forEach(question => {
      switch (question.type) {
        case EQuestion.MCQ:
        case EQuestion.FixedWord:
        case EQuestion.TrueFalse:
        case EQuestion.SCQ:
          question.tipAnswers = question.answers.filter(x => x.isCorrect).map(x => x.rAnswer);
          break;
        case EQuestion.Matching:
          question.lAnswers = [];
          if (question.answers && question.answers.length > 0) {
            const lAnswers = question.answers.map(item => item.lAnswer).filter((value, index, self) => self.indexOf(value) === index);
            question.lAnswers = [...lAnswers]
          }
          question.tipAnswers = question.answers.map(x => x.rAnswer + ' ->' + question.lAnswers.find(y => y === x.lAnswer))
          break;

        case EQuestion.OpenEnd:
          break;
        case EQuestion.Ranked:
          question.tipAnswers
            = question.answers.sort((a: AnswerDto, b: AnswerDto) => a.sequenceOrder - b.sequenceOrder).map(x => x.rAnswer);
          break;
        case EQuestion.MatrixTableQuestion:

      }
    })
  }


  isAllQuestionsSubmitted(questions: QuestionDto[]) {
    return questions && questions.filter(x => x.submitted).length === questions.length;
  }

  bindSudentAnswerToQuestion(questions: QuestionDto[], studentDidAnswers: StudentAnswerDto[]) {
    if (!questions || questions.length === 0 || !studentDidAnswers || studentDidAnswers.length === 0) {
      return;
    }
    questions.forEach(question => {

      switch (question.type) {
        case EQuestion.MCQ:
          question.answers.forEach(ans => {
            ans.isSelected = studentDidAnswers.find(x => x.answerId === ans.id && x.questionId === question.id) != null;
          })
          question.submitted = question.answers.filter(x => x.isSelected).length > 0;
          break;
        case EQuestion.FixedWord:
        case EQuestion.TrueFalse:
        case EQuestion.SCQ:
          const item1 = studentDidAnswers.find(x => x.questionId === question.id);
          if (item1) {
            question.selectedAnswerId = item1.answerId;
            question.submitted = true;
          }
          break;
        case EQuestion.Matching:
          question.answers.forEach(ans => {
            const item2 = studentDidAnswers.find(x => x.answerId === ans.id && x.questionId === question.id);
            if (item2) {
              ans.matchTo = item2.answerText;
              question.submitted = true;
            }
          })
          break;
        case EQuestion.OpenEnd:
          const item3 = studentDidAnswers.find(x => x.questionId === question.id);
          if (item3) {
            question.answerText = item3.answerText;
            question.submitted = true;
          }
          break;
        case EQuestion.Ranked:
          question.answers.forEach(ans => {
            const item4 = studentDidAnswers.find(x => x.questionId === question.id && x.answerId === ans.id);
            if (item4) {
              ans.selectedSequenceOrder = Number.parseInt(item4.answerText);
              question.submitted = true;
            }
          })

          break;
        case EQuestion.MatrixTableQuestion:

      }
    })

  }

  lockQuestionsAfterAnswer(questions: QuestionDto[]) {
    if (this.quizOption.lookQuestionAfterAnswer) {
      questions.forEach(question => {
        if(question.selectedAnswerId || question.answerText
          || (question.answers.some(answer => answer.isSelected || answer.selectedSequenceOrder || answer.matchTo)))
        question.isDisable = question.submitted;
      })
    }
  }

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this.displayQuestions = this.questions.slice((this.pageNumber - 1) * this.pageSize, this.pageNumber * this.pageSize);

    
    this.showPaging({ items: this.displayQuestions, totalCount: this.questions.length }, pageNumber);
    this.isDisplayQuesionsSubmitted = this.isAllQuestionsSubmitted(this.displayQuestions);
    this.isAllSubmitted = this.isAllQuestionsSubmitted(this.questions);
    if(this.quizOption.testingAttempt.studentAnswers && this.quizOption.testingAttempt.studentAnswers.length > 0){
      this.quizOption.testingAttempt.studentAnswers.forEach(answer => {
        if(answer.answerId || answer.answerText ){
          let questionMatchAnswer =  this.questions.find(question=>question.id == answer.questionId)
          if(!questionMatchAnswer.isAnswerChanged){
            questionMatchAnswer.isSaved = true
          }
        }
      })
    }
   
  }

  onQuestionClick(index) {
    this.pageNumber = Math.floor(index / this.pageSize) + 1;
    this.selectedItem = index;
    this.refresh();
  }


  nextPage() {
    this.pageNumber++;
    this.selectedItem++;
    this.refresh();
  }

  previousPage() {
      this.pageNumber--;
      this.selectedItem--;
      this.refresh();
  }

  
  

  filterAnswered(question) {
    let item: any;
    switch (question.type) {
      case EQuestion.MCQ:
        question.answers.forEach(element => {
          if (element.isSelected === true) { item = question; }
        });
        break;
      case EQuestion.SCQ:
        if (question.selectedAnswerId) { item = question; } break;
      case EQuestion.TrueFalse:
        if (question.selectedAnswerId) { item = question; } break;
      case EQuestion.FixedWord:
        if (question.selectedAnswerId) { item = question; } break;
      case EQuestion.OpenEnd: // 2
        if (question.answerText) { item = question; } break;
      case EQuestion.Ranked: // 4
        question.answers.forEach(element => {
          if (element.selectedSequenceOrder) { item = question; }
        });
        break;
      case EQuestion.Matching: // 5
        question.answers.forEach(element => {
          if (element.matchTo) { item = question; }
        });
        break;
    }
    return item;
  }


  submit() {
    const listAfterCheck = [];
    let listmarked = [];
    this.questions.forEach(el => {
      listAfterCheck.push(this.filterAnswered(el));
    });

    listmarked = listAfterCheck.filter(el => el !== undefined);
    this.checkValidSubmit = true;
    const studentAnswers = [];

    listmarked.forEach(q => {
      const ans = this.getAnswer(q);
      ans.forEach(a => {
        studentAnswers.push(a);
      })
    })

    const dialogRef = this.dialog.open(PopupSubmitAnswerComponent, {
      data: this.questions,
      width: "500px"
    });
    dialogRef.afterClosed().subscribe(res => {
      if (res == true) {
        let message = `finish Quiz`
        let args = {
          studentAnswer: [studentAnswers]
        }
        this.logStudentProcessToSentry(message, args);
        this.saveAndFinish()
        this.isFinished = true
      }
    });

  }
  saveCurrentAnswer() {
    const listAfterCheck = [];
    let listmarked = [];
    this.questions.forEach(el => {
      listAfterCheck.push(this.filterAnswered(el));
    });
    let studentAnswers = [];
    listmarked = listAfterCheck.filter(el => el !== undefined);
    listmarked.forEach(q => {
      const ans = this.getAnswer(q);
      ans.forEach(a => {
        studentAnswers.push(a);
      })
    })
  }

  save() {
    const listAfterCheck = [];
    let listmarked = [];
    this.questions.forEach(el => {
      listAfterCheck.push(this.filterAnswered(el));
    });
    let studentAnswers = [];
    listmarked = listAfterCheck.filter(el => el !== undefined);
    listmarked.forEach(q => {
      const ans = this.getAnswer(q);
      ans.forEach(a => {
        studentAnswers.push(a);
      })
    })
    abp.message.success("Save completed")
    let message = `Save answer`
    let args = {
      studentAnswer: studentAnswers,
      saveTime: this.formatTime(new Date())
    }
    this.logStudentProcessToSentry(message, args);

    this._studentAnswerService.CreateStudentAnswers(studentAnswers).subscribe(() => { });
  }

  finishQuiz() {
    this.isDisplayQuesionsSubmitted = true;
    abp.message.success("Submit success")
    this.isAllSubmitted = true;
    this.isFinished = true
    if (this.quizOption.testingAttempt.status == TestAttemptStatus.Testing) {
      this.takeScore();
    }
    this.status.emit(TestAttemptStatus.Marking);
  }

  takeScore() {
    if (this.counter) {
      this.counter.stop();
    }
    this.deleteLocal(this.localName);
    if (this.quizType === PageLinkExamType.Quiz) {
      this.quizOption.testingAttempt.type = PageType.Quiz;
    } else if (this.quizType === PageLinkExamType.QuizFinal) {
      this.quizOption.testingAttempt.type = PageType.QuizFinal;
    } else if (this.quizType === PageLinkExamType.Survey) {
      this.quizOption.testingAttempt.type = PageType.Survey;
    }
    this._testAttemptService.ProcessScore(this.quizOption.testingAttempt).subscribe(result => {
      this.quizOption.testingAttempt = result.result;
      this.quizOption.testAttempts.push(result.result);
      this.completedAttempts.push(result.result);
      if (this.quizType === PageLinkExamType.Survey) {
        this.status.emit(TestAttemptStatus.Marking);
        this.isDidTakeScore = true;
        return;
      }
      if (this.quizOption.scoreKeepType === QuizScoreKeepType.Hightest) {
        this.getHighestTestAttempt(this.completedAttempts);
      } else {
        this.caculateAvarageScore(this.completedAttempts);
      }

      this.status.emit(TestAttemptStatus.Marking);

      this.isDidTakeScore = true;
      this.questions.forEach(qs => qs.isDisable = true)

    })

  }
  saveAndFinish(){
      if(!this.currentQuestion.question.isSaved){
        let studentAnswer = {
          questionId: this.currentQuestion.question.id,
          testAttempId: this.currentQuestion.testAttemptId,
          studentAsnwers: this.getAnswer(this.currentQuestion.question)
        } as SaveAnswerDto
        this._studentAnswerService.CreateStudentAnswers(studentAnswer).subscribe(() => {
          let message = `time out and auto finish the quiz`
          this.logStudentProcessToSentry(message, null);
          this.finishQuiz()
        });
      }
      else{
        this.finishQuiz()
      }

  }


  getAnswer(question: QuestionDto): StudentAnswerDto[] {
    const stuAnswers: StudentAnswerDto[] = [];
    switch (question.type) {
      case EQuestion.MCQ:
        question.answers.forEach(ans => {
          if (ans.isSelected) {
            let stuAns = new StudentAnswerDto();
            stuAns.questionId = question.id;
            stuAns.testAttempId = this.quizOption.testingAttempt.id;
            stuAns.answerId = ans.id;
            stuAnswers.push(stuAns);
          }
        })
        if (stuAnswers.length === 0) {
          this.checkValidSubmit = false;
          // abp.message.error(this.l("You don't select any answer"));
        }
        break;
      case EQuestion.FixedWord:
      case EQuestion.TrueFalse:
      case EQuestion.SCQ:
        if (question.selectedAnswerId) {
          let stuAns = new StudentAnswerDto();
          stuAns.questionId = question.id;
          stuAns.testAttempId = this.quizOption.testingAttempt.id;
          stuAns.answerId = question.selectedAnswerId;
          stuAnswers.push(stuAns);
        } else {
          this.checkValidSubmit = false;
          // abp.message.error(this.l("You don't select any answer"));
        }
        break;
      case EQuestion.Matching:
        let lstAnswers = question.answers.filter(s => s.matchTo);
        if (lstAnswers.length !== question.answers.length) {
          this.checkValidSubmit = false;
          // abp.message.error(this.l("You haven't completed yet"));
          break;
        }
        lstAnswers.forEach(ans => {
          let stuAns = new StudentAnswerDto();
          stuAns.questionId = question.id;
          stuAns.testAttempId = this.quizOption.testingAttempt.id;
          stuAns.answerId = ans.id;
          stuAns.answerText = ans.matchTo;
          stuAnswers.push(stuAns);
        });
        return stuAnswers;

      case EQuestion.OpenEnd:
        // if (question.selectedAnswerId) {
        let stuAns = new StudentAnswerDto();
        stuAns.questionId = question.id;
        stuAns.testAttempId = this.quizOption.testingAttempt.id;
        stuAns.answerText = question.answerText;
        stuAnswers.push(stuAns);
        break;
      // } else {
      //   this.checkValidSubmit = false;
      //   // abp.message.error(this.l("You don't select any answer"));
      // }

      case EQuestion.Ranked:
        const rankedAnswers = question.answers.filter(s => s.selectedSequenceOrder != null);
        if (rankedAnswers.length !== question.answers.length) {
          this.checkValidSubmit = false;
          // abp.message.error(this.l("You haven't completed yet"));
          break;
        }
        rankedAnswers.forEach(ans => {
          let stuAns = new StudentAnswerDto();
          stuAns.questionId = question.id;
          stuAns.testAttempId = this.quizOption.testingAttempt.id;
          stuAns.answerId = ans.id;
          stuAns.answerText = ans.selectedSequenceOrder + '';
          stuAnswers.push(stuAns);
        });

        break;
      case EQuestion.MatrixTableQuestion:

    }

    return stuAnswers;

  }
  protected delete(entity: QuestionDto) {

  }

  ngOnDestroy() {
    this.quizId = null;
    this.destroy.next(true);
    this.destroy.complete();
    window['enableReloadFunction'] = false;
  }

  public getLocal(name: string) { // get item from localStorage
    return JSON.parse(localStorage.getItem(name));
  }
  public setLocal(name: string, value: any): void { // set item to localStorage
    localStorage.setItem(name, JSON.stringify(value));
  }

  public deleteLocal(name: string) { // delete item from localStorage
    localStorage.removeItem(name);
  }

  
}

