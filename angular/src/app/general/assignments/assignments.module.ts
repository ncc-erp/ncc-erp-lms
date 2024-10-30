import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AssignmentsComponent } from './assignments.component';
import { SharedModule } from '@shared/shared.module';
import { AbpModule } from 'abp-ng2-module/dist/src/abp.module';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ModalModule } from 'ngx-bootstrap';
import { EditorModule } from '@tinymce/tinymce-angular';
import { NgxPaginationModule } from 'ngx-pagination';
import { DndModule } from 'ngx-drag-drop';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    AbpModule,
    FormsModule,
    RouterModule.forChild([
      {
        path: '',
        component: AssignmentsComponent
      }
    ]),
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
    AssignmentsComponent
  ]
})
export class AssignmentsModule { }
