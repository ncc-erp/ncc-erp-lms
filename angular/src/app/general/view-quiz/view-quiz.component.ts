import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { QuestionDto } from '@app/models/question-dto';
import { ActivatedRoute } from '@angular/router';
import { QuizOptionDto } from '@app/models/quizzes-dto';
import { QuizzesService } from '@app/services/systems-admin-services/quizzes.service';
import { QuizResolverResult } from '@app/services/systems-admin-services/quiz.resolver.service';
import { TestAttemptService, TestAttemptDto } from '@app/services/student-service/test.attempt.service';
import { EQuestion, TestAttemptStatus } from '@shared/AppEnums';

@Component({
  selector: 'app-view-quiz',
  templateUrl: './view-quiz.component.html',
  styleUrls: ['./view-quiz.component.scss']
})
export class ViewQuizComponent extends AppComponentBase implements OnInit {
  courseInstanceId: string;
  quizId: string;
  quizSettingId: string;
  questions: QuestionDto[] = [];
  quizOption = {} as QuizOptionDto;
  testAttemptId: string;
  isStarted = false;

  constructor(
    injector: Injector,
    private _quizService: QuizzesService,
    private _testAttemptService: TestAttemptService,
    private route: ActivatedRoute,
  ) { super(injector); }

  ngOnInit() {
    this.courseInstanceId = this.route.snapshot.paramMap.get('courseInstanceId');
    this.quizId = this.route.snapshot.paramMap.get('quizId');
    this.log(this.courseInstanceId);
    this.log(this.quizId);
     this.route.data.subscribe((data: {result: QuizResolverResult}) => {
      this.questions = data.result.questions;
      this.quizOption = data.result.quizOption;
      this.quizSettingId = this.quizOption.settings.id;
    });
  }

  startDoQuiz() {
    const item = new TestAttemptDto();
    item.quizSettingId = this.quizSettingId;
    item.status = TestAttemptStatus.Testing;
    this._testAttemptService.create(item).subscribe(result => {
      this.testAttemptId = result.result.id;
      this.isStarted = true;
    })
  }

}
