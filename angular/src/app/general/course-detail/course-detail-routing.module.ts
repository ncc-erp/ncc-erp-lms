import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
import { RouterModule } from '@angular/router';
import { CourseDetailComponent } from './course-detail.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild([
      {
        path: '', component: CourseDetailComponent, canActivate: [AppRouteGuard]
      }
    ])
  ],
  declarations: []
})
export class CourseDetailRoutingModule { }
