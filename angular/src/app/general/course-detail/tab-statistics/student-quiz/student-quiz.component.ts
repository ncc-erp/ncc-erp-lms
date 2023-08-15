import { Component, OnInit, Injector, OnChanges, SimpleChanges, Input, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { TestAttemptDto, TestAttemptService } from '@app/services/student-service/test.attempt.service';
import { QuizDto } from '../tab-statistics.component';
import { QuestionDto } from '@app/models/question-dto';
import { QuizOptionDto } from '@app/models/quizzes-dto';
import { ResponseContentType } from '@angular/http';
import { QuestionsService } from '@app/services/systems-admin-services/questions.service';
import { EQuestion, Common, QuizScoreKeepType } from '@shared/AppEnums';
import { AnswerDto } from '@app/models/answer-dto';
import { StudentAnswerDto } from '@app/models/student-answer-dto';

@Component({
  selector: 'app-student-quiz',
  templateUrl: './student-quiz.component.html',
  styleUrls: ['./student-quiz.component.scss']
})
export class StudentQuizComponent extends AppComponentBase implements OnInit, OnChanges {

  @Input() quiz = {} as QuizDto;
  @Input() studentId: number;
  @Output() quizChange: EventEmitter<QuizDto> = new EventEmitter();

  constructor(
    injector: Injector,
    private _testAttemptService: TestAttemptService,
    private _questionService: QuestionsService,
  ) {
    super(injector)
  }


  testAttempts: TestAttemptDto[] = [];
  currentAttempt = {} as TestAttemptDto;

  questions: QuestionDto[] = [];
  originQuestions: QuestionDto[] = [];
  option = {
    lookQuestionAfterAnswer: true,
    responseType: "0",
  } as QuizOptionDto;
  isViewing = false;
  quizType: string;
  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges) {
    this.quizType = this.getQuizType(this.quiz.quizType);
    this.getStudentTestAttempts(this.studentId, this.quiz.id);
    // this.getQuestions(this.quiz.id)
  }

  getStudentTestAttempts(studentId: number, quizSettingId: string) {
    this._testAttemptService.GetStudentTestAttempts(studentId, quizSettingId).subscribe(result => {
      this.testAttempts = result.result;
      if (this.testAttempts.length > 0) {
        if (this.testAttempts.length === 1) {
          this.viewTestAttempt(this.testAttempts[0]);
        } else {
          if (this.quiz.scoreToKeepType === QuizScoreKeepType.Hightest) {
            this.viewTestAttempt(this.testAttempts.sort((a: TestAttemptDto, b: TestAttemptDto) => b.score - a.score)[0]);
          } else {
            this.viewTestAttempt(this.testAttempts[0]);
          }
        }
      }
    })
  }

  getQuestions(testAttemptId: string) {
    this._questionService.GetQuestionsByTestAttemptIdNotPagging(testAttemptId).subscribe(result => {
      this.questions = result.result;  
      this.tipCorrectAnswers(this.questions);
      this.originQuestions = this.questions;
      this.getStudentAnswers(this.currentAttempt.id);  
      // if (this.quizOption.testingAttempt && this.quizOption.testingAttempt.studentAnswers) {
      //   this.bindSudentAnswerToQuestion(this.questions, this.quizOption.testingAttempt.studentAnswers);
      // }

      // this.lockQuestionsAfterAnswer(this.questions);
      // this.isAllSubmitted = this.isAllQuestionsSubmitted(this.questions);
      // this.refresh();
    })
  }

  isViewingQuestions = false;
  viewTestAttempt(item: TestAttemptDto) {
    this.currentAttempt = item;
    this.getQuestions(item.id);
    this.isViewingQuestions = false;
    // this.currentAttempt = item;
    this.testAttempts.forEach(s => {
      s.isViewing = false;
    })
    // this.currentAttempt.isViewing = true;
    // this.getStudentAnswers(this.currentAttempt.id);
  }

  getStudentAnswers(testAttemptId: string) {    
    this._questionService.GetStudentAnswersNotPagging(testAttemptId).subscribe(result => {
      this.questions = this.originQuestions.filter(s => s != null);
      this.bindSudentAnswerToQuestion(this.questions, result.result);
      // this.questions = this.questions.filter((question,index,list)=>list.findIndex(qs=>(qs.questionQuizId===question.questionQuizId))===index)
      // if(result.result && result.result.length > 0){
      //   this.questions[this.questions.length-1].answerText = result.result[result.result.length-1].answerText
      // }
      this.isViewingQuestions = true;
      if (this.currentAttempt)
        this.currentAttempt.isViewing = true;
    })
  }

  tipCorrectAnswers(questions: QuestionDto[]) {

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
            let lAnswers = question.answers.map(item => item.lAnswer).filter((value, index, self) => self.indexOf(value) === index);
            question.lAnswers = [...lAnswers]
          }
          question.tipAnswers = question.answers.map(x => x.rAnswer + " -> " + question.lAnswers.find(y => y === x.lAnswer));
          break;

        case EQuestion.OpenEnd:
          break;
        case EQuestion.Ranked:
          question.tipAnswers = question.answers.sort((a: AnswerDto, b: AnswerDto) => a.sequenceOrder - b.sequenceOrder).map(x => x.rAnswer);
          break;
        case EQuestion.MatrixTableQuestion:

      }
    })
  }

  bindSudentAnswerToQuestion(questions: QuestionDto[], studentDidAnswers: StudentAnswerDto[]) {
    if (!questions || questions.length == 0 || !studentDidAnswers || studentDidAnswers.length == 0) {
      return;
    }

    questions.forEach(question => {
      question.submitted = true;
      question.isDisable = true;
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
          var item = studentDidAnswers.find(x => x.questionId === question.id);
          if (item) {
            question.selectedAnswerId = item.answerId;
            question.submitted = true;
          }
          break;
        case EQuestion.Matching:
          question.answers.forEach(ans => {
            var item = studentDidAnswers.find(x => x.answerId === ans.id && x.questionId === question.id);
            if (item) {
              ans.matchTo = item.answerText;
              question.submitted = true;
            }
          })

        case EQuestion.OpenEnd:
          var item = studentDidAnswers.find(x => x.questionId === question.id);
          if (item) {
            question.answerText = item.answerText;
            question.submitted = true;
            question.studentPoint = item.mark;
          }
          break;
        case EQuestion.Ranked:
          question.answers.forEach(ans => {
            let item = studentDidAnswers.find(x => x.questionId === question.id && x.answerId === ans.id);
            if (item) {
              ans.selectedSequenceOrder = Number.parseInt(item.answerText);
              question.submitted = true;
            }
          })

          break;
        case EQuestion.MatrixTableQuestion:

      }
    })

  }

  isShowOpenEndQuestionsOnly: boolean = false;
  onShowOpenEndQuestionsChange() {
    if (this.isShowOpenEndQuestionsOnly) {
      this.questions = this.originQuestions.filter(s => s.type === EQuestion.OpenEnd)
    } else {
      this.questions = this.originQuestions.filter(s => s != null);
    }
  }

  onTestAttemptChangeScore(item: TestAttemptDto) {
    let find = this.testAttempts.find(s => s.id === item.id);
    if (find) {
      find.lastModificationTime = item.lastModificationTime;
      find.score = item.score;

      if (this.testAttempts.length == 1) {
        this.quiz.studentScore = this.currentAttempt.maxScore ? this.currentAttempt.score * this.quiz.quizScore / this.currentAttempt.maxScore : 0;
      } else if (this.testAttempts.length > 1) {
        if (this.quiz.scoreToKeepType === QuizScoreKeepType.Hightest) {
          let highestScore = this.testAttempts.sort((a: TestAttemptDto, b: TestAttemptDto) => b.score/b.maxScore - a.score/a.maxScore)[0];
          this.quiz.studentScore = highestScore.score / highestScore.maxScore * this.quiz.quizScore;
        } else {
          let avarageScore = this.testAttempts.map(s => s.score/s.maxScore).reduce((a, b) => a + b, 0) / this.testAttempts.length;
          this.quiz.studentScore = avarageScore * this.quiz.quizScore;
        }
      }
      this.quizChange.emit(this.quiz);
    }
  }
  getQuizType(type: number) {
    switch (type) {
      case 0:
        return 'quiz';
      case 1:
        return 'survey';
      default:
        break;
    }
  }
}


