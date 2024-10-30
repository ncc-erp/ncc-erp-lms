import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
import { AbpModule } from 'abp-ng2-module/dist/src/abp.module';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ModalModule } from 'ngx-bootstrap';
import { EditorModule } from '@tinymce/tinymce-angular';
import { QuizzesComponent } from './quizzes.component';
import { CreateQuestionComponent } from './create-question/create-question.component';
import { EditQuestionComponent } from './edit-question/edit-question.component';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { NgxPaginationModule } from 'ngx-pagination';
import { DndModule } from 'ngx-drag-drop';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';

@NgModule({
  imports: [
    CommonModule,
    CommonModule,
    SharedModule,
    AbpModule,
    FormsModule,
    RouterModule.forChild([
      {
        path: '',
        component: QuizzesComponent
      }
    ]),
    SharedModule,
    AbpModule,
    FormsModule,
    ModalModule.forRoot(),
    NgxPaginationModule,
    DndModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
    NgMultiSelectDropDownModule.forRoot(),
    EditorModule
  ],
  declarations: [
    QuizzesComponent,
    CreateQuestionComponent,
    EditQuestionComponent
  ]
})
export class QuizzesModule { }
