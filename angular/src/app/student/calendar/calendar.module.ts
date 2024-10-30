import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CalendarComponent } from './calendar.component';
import { RouterModule } from '@angular/router';
import { SharedModule } from '@shared/shared.module';
import { FullCalendarModule } from 'ng-fullcalendar';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    NgbModule,
    SharedModule,
    FullCalendarModule,
    RouterModule.forChild([
      {
        path: '',
        component: CalendarComponent
      }
    ])
  ],
  declarations: [CalendarComponent]
})
export class CalendarModule { }
