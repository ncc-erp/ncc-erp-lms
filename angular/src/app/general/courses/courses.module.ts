import { DndModule } from 'ngx-drag-drop';
import { EditorModule } from '@tinymce/tinymce-angular';
import { AvatarModule } from 'ngx-avatar';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
import { AbpModule } from 'abp-ng2-module/dist/src/abp.module';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ModalModule } from 'ngx-bootstrap';
import { NgxPaginationModule } from 'ngx-pagination';
import { CoursesComponent } from './courses.component';
import { CreateCourseComponent } from './create-course/create-course.component';
import { TabAdminQAComponent } from './tab-qa/tab-qa.component';
import { TabAdminQADetailComponent } from './tab-qa/tab-qa-detail.component';

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
        component: CoursesComponent
      }
    ]),
    SharedModule,
    AbpModule,
    FormsModule,
    ModalModule.forRoot(),
    NgxPaginationModule,
    AvatarModule,
    EditorModule,
    DndModule
  ],
  declarations: [
    CoursesComponent,
    CreateCourseComponent,
    TabAdminQAComponent,
    TabAdminQADetailComponent
  ]
})
export class CoursesModule { }
