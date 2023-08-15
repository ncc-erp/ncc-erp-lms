import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowsingComponent } from './browsing.component';
import { RouterModule } from '@angular/router';
import { QuizComponent } from './quiz/quiz.component';
import { SharedModule } from '@shared/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { CountdownModule } from 'ngx-countdown';
import { FormsModule } from '@angular/forms';
import { StudentAssignmentComponent } from './student-assignment/student-assignment.component';
import { PopupSubmitAnswerComponent } from './popup-submit-answer/popup-submit-answer.component';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    NgxPaginationModule,
    FormsModule,
    CountdownModule,
    RouterModule.forChild([
      {
        path: '',
        component: BrowsingComponent
      }
    ]),
  ],
  declarations: [BrowsingComponent, QuizComponent, StudentAssignmentComponent, PopupSubmitAnswerComponent],
  entryComponents: [PopupSubmitAnswerComponent]
})
export class BrowsingModule { }
