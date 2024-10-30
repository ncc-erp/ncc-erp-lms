import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourseDetailRoutingModule } from './course-detail-routing.module';
import { CreateStudentComponent } from './create-student/create-student.component';
import { CourseDetailComponent } from './course-detail.component';
import { SharedModule } from '@shared/shared.module';
import { AbpModule } from 'abp-ng2-module/dist/src/abp.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap';
import { NgxPaginationModule } from 'ngx-pagination';
import { DndModule } from 'ngx-drag-drop';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { EditorModule } from '@tinymce/tinymce-angular';
import { TabGeneralComponent } from './tab-general/tab-general.component';
import { TabModulesComponent } from './tab-modules/tab-modules.component';
import { DialogCreateModuleComponent } from './tab-modules/dialog-create-module/dialog-create-module.component';
import { DialogEditModuleComponent } from './tab-modules/dialog-edit-module/dialog-edit-module.component';
import { DialogCreatePageComponent } from './tab-modules/dialog-create-page/dialog-create-page.component';
import { TabQuizzesComponent } from './tab-quizzes/tab-quizzes.component';
import { TabAssignmentComponent } from './tab-assignment/tab-assignment.component';
import { TabGroupComponent } from './tab-group/tab-group.component';
import { DialogCreateCourseGroupComponent } from './tab-group/dialog-create-course-group/dialog-create-course-group.component';
import { DialogEditCourseGroupComponent } from './tab-group/dialog-edit-course-group/dialog-edit-course-group.component';
import { TabGradesComponent } from './tab-grades/tab-grades.component';
import { TabAnnoucementsComponent } from './tab-annoucements/tab-annoucements.component';
import { TabSettingsComponent } from './tab-settings/tab-settings.component';
import { RouterModule } from '@angular/router';
import { TabPendingApproveComponent } from './tab-pending-approve/tab-pending-approve.component';
import { FileAttachmentsComponent } from './file-attachments/file-attachments.component';
import { GeneralCertificationTemplateComponent } from './tab-general/general-certification-template/general-certification-template.component';
import { TabStatisticsComponent } from './tab-statistics/tab-statistics.component';
import { StudentQuizComponent } from './tab-statistics/student-quiz/student-quiz.component';
import { TabHistoryComponent } from './tab-history/tab-history.component';
import { TabScormstatisticsComponent } from './tab-scormstatistics/tab-scormstatistics.component';


@NgModule({
  imports: [
    CommonModule,
    CourseDetailRoutingModule,
    SharedModule,
    AbpModule,
    FormsModule,
    RouterModule,
    ReactiveFormsModule,
    ModalModule.forRoot(),
    NgxPaginationModule,
    DndModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
    NgMultiSelectDropDownModule.forRoot(),
    EditorModule
  ],

  entryComponents: [
    DialogCreateModuleComponent,
    DialogEditModuleComponent,
    DialogCreatePageComponent,
    DialogCreateCourseGroupComponent,
    DialogEditCourseGroupComponent
  ],

  declarations: [
    CourseDetailComponent,
    CreateStudentComponent,
    TabGeneralComponent,
    TabModulesComponent,
    DialogCreateModuleComponent,
    DialogEditModuleComponent,
    DialogCreatePageComponent,
    TabQuizzesComponent,
    TabAssignmentComponent,
    TabGroupComponent,
    DialogCreateCourseGroupComponent,
    DialogEditCourseGroupComponent,
    TabGradesComponent,
    TabAnnoucementsComponent,
    TabSettingsComponent,
    TabPendingApproveComponent,
    // FileAttachmentsComponent,
    GeneralCertificationTemplateComponent,
    TabStatisticsComponent,
    StudentQuizComponent,
    TabHistoryComponent,
    TabScormstatisticsComponent
  ]
})
export class CourseDetailModule { }
