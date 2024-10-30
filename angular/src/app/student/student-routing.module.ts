import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CourseDetailComponent } from './course-detail/course-detail.component';
import { CoursesComponent } from './courses/courses.component';
import { CourseComponent } from './course/course.component';
@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild([
      {
        path: '',
        children: [
          { path: '', redirectTo: '/app/student/courses', pathMatch: 'full' },
          {
            path: 'courses',
            component: CoursesComponent
          },
          {
            path: 'dashboard',
            data: { permission: 'Pages.Dashboard' },
            component: DashboardComponent
          },
          {
            path: 'course-detail/:id',
            component: CourseDetailComponent
          },
          {
            path: 'course/:id',
            component: CourseComponent
          },
          {
            path: 'calendar',
            data: { permission: 'Pages.Calendar' },
            loadChildren: './calendar/calendar.module#CalendarModule'
          }
        ]
      }
    ])
  ],
  exports: [
    RouterModule
  ]
})
export class StudentRoutingModule { }
