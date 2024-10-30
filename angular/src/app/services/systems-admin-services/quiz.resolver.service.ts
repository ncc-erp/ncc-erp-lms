import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of, EMPTY } from 'rxjs';
import { mergeMap } from 'rxjs/operators';
import { QuestionsService } from './questions.service';
import { QuestionDto } from '@app/models/question-dto';
import { QuizOptionDto } from '@app/models/quizzes-dto';
import { QuizzesService } from './quizzes.service';


@Injectable({
  providedIn: 'root',
})
export class QuizResolverService implements Resolve<QuizResolverResult> {
  constructor(
    private questionService: QuestionsService, 
    private quizService: QuizzesService,
    private router: Router
  ) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<QuizResolverResult> | Observable<never> {
    const quizId = route.paramMap.get('quizId');
    const courseInstanceId = route.paramMap.get('courseInstanceId');
    var quizResolverData = {} as QuizResolverResult;
    return this.questionService.GetQuestionsByQuizIdNotPagging(quizId).pipe(mergeMap(result => {
      if (result.result) {
        quizResolverData.questions = result.result;
        return this.quizService.GetQuizOptions(quizId, courseInstanceId).pipe(mergeMap(resultQuiz => {
          if (resultQuiz.result){
            quizResolverData.quizOption = resultQuiz.result;
            return of(quizResolverData);
          } else { // id not found
              this.router.navigate(['/app/systems-admin/home']);
              return EMPTY;
            }
        }));
      } else { // id not found
        this.router.navigate(['']);
        return EMPTY;
      }
    }));    
  }
}

export class QuizResolverResult {
  questions: QuestionDto[] = [];
  quizOption : QuizOptionDto;
}
