import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
import { AbpModule } from 'abp-ng2-module/dist/src/abp.module';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ModalModule } from 'ngx-bootstrap';
import { EditorModule } from '@tinymce/tinymce-angular';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { NgxPaginationModule } from 'ngx-pagination';
import { DndModule } from 'ngx-drag-drop';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { ScormComponent } from './scorm.component';



@NgModule({
  imports: [
    CommonModule,
    AbpModule,
    FormsModule,
    RouterModule.forChild([
      {
        path: '',
        component: ScormComponent
      }
    ]),
    SharedModule,
    ModalModule.forRoot(),
    NgxPaginationModule,
    DndModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
    NgMultiSelectDropDownModule.forRoot(),
    EditorModule,
  ],
  declarations: [
    ScormComponent   

  ]
})
export class ScormModule { }
