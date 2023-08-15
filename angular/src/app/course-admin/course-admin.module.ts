import { HomeComponent } from './home/home.component';
import { SharedModule } from '@shared/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { ModalModule } from 'ngx-bootstrap';
import { FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourseAdminRoutingModule } from './/course-admin-routing.module';
import { DashboardComponent } from './dashboard/dashboard.component';
import { UserProfileModule } from '@app/general/user-profile/user-profile.module';
@NgModule({
  imports: [
    CommonModule,
    CourseAdminRoutingModule,
    FormsModule,
    UserProfileModule,
    ModalModule.forRoot(),
    SharedModule,
    NgxPaginationModule
  ],
  declarations: [
    HomeComponent,
    DashboardComponent
  ],
  exports: [
  ]
})
export class CourseAdminModule { }
