import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { StudentRoutingModule } from './/student-routing.module';
import { DashboardComponent } from './dashboard/dashboard.component';
import { SharedModule } from '@shared/shared.module';
import { AllMaterialModuleModule } from '@shared/all-material-module/all-material-module.module';
import { CourseDetailComponent } from './course-detail/course-detail.component';
import { UserProfileModule } from '@app/general/user-profile/user-profile.module';
import { EditorModule } from '@tinymce/tinymce-angular';
import { CoursesComponent } from './courses/courses.component';
import { CourseComponent } from './course/course.component';
import { TabOverviewComponent } from './course/tab-overview/tab-overview.component';
import { TabAnnoucementComponent } from './course/tab-annoucement/tab-annoucement.component';
import { TabCourseContentComponent } from './course/tab-course-content/tab-course-content.component';
import { TabBookmarkComponent } from './course/tab-bookmark/tab-bookmark.component';
import { TabQAComponent } from './course/tab-qa/tab-qa.component';
import { TabGradeComponent } from './course/tab-grade/tab-grade.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { StudentCourseViewItemComponent } from './courses/student-course-view-item/student-course-view-item.component';
import { RouterModule } from '@angular/router';
import { AvatarModule } from 'ngx-avatar'; // npm install or npm i ngx-avatar
import { TabQADetailComponent } from './course/tab-qa/tab-qa-detail.component';
import { TabScormgradeComponent } from './course/tab-scormgrade/tab-scormgrade.component';
// import { BrowsingComponent } from './browsing/browsing.component';
@NgModule({
  imports: [
    FormsModule,
    CommonModule,
    StudentRoutingModule,
    SharedModule,
    RouterModule,
    UserProfileModule,
    EditorModule,
    AllMaterialModuleModule,
    NgxPaginationModule,
    AvatarModule
  ],
  exports: [
    AllMaterialModuleModule
  ],
  declarations: [
    DashboardComponent
    , CourseDetailComponent
    , CoursesComponent
    , CourseComponent
    , TabOverviewComponent
    , TabAnnoucementComponent
    , TabCourseContentComponent
    , TabBookmarkComponent
    , TabQAComponent
    , TabQADetailComponent
    , TabGradeComponent
    , StudentCourseViewItemComponent, TabScormgradeComponent
  ]
})
export class StudentModule { }
