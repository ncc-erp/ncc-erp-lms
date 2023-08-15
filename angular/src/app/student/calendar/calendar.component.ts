import { AppComponentBase } from '@shared/app-component-base';
import { Component, OnInit, Injector, AfterViewInit, ViewChild } from '@angular/core';
import { BaseService } from '@app/services/base-service/base.service';
import { map } from 'rxjs/operators';
import { CourseFullDto, CourseAcceptCalendar } from '@app/models/course-full-dto';
import { CalendarComponent as Calendar } from 'ng-fullcalendar';
import { Options } from 'fullcalendar';
import { ICourseColor } from '@app/models/courses-dto';

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})
export class CalendarComponent extends AppComponentBase implements OnInit, AfterViewInit {
  courseAccept: CourseFullDto[] = [];
  courseAcceptCalendar: CourseAcceptCalendar[] = [];
  calendarOptions: Options;
  colorSelected: string;
  //#region List Color Suggest
  listColorSuggest = [
    {
      color: '#bd3c14'
    },
    {
      color: '#ff2717'
    },
    {
      color: '#e71f63'
    },
    {
      color: '#8f3e97'
    },
    {
      color: '#65499d'
    },
    {
      color: '#4554a4'
    },
    {
      color: '#1770ab'
    },
    {
      color: '#0b9be3'
    },
    {
      color: '#06a3b7'
    },
    {
      color: '#009688'
    },
    {
      color: '#009606'
    },
    {
      color: '#8d9900'
    },
    {
      color: '#d97900'
    },
    {
      color: '#fd5d10'
    },
    {
      color: '#f06291'
    },
  ]
  //#endregion
  @ViewChild(Calendar) ucCalendar: Calendar;
  constructor(injector: Injector, private _base: BaseService) {
    super(injector);
  }

  ngOnInit() {
  }



  onGetData() {
    this._base._courseService.GetAcceptedCoursesForCalendar().pipe(map(m => m.result)).subscribe(e => {
      this.courseAccept = e;
      for (let index = 0; index < this.courseAccept.length; index++) {
        const element = this.courseAccept[index];
        this.courseAcceptCalendar.push({
          courseId: element.courseId,
          title: element.name,
          start: this.convertFromStringToDate(element.startTime),
          end: this.convertFromStringToDate(element.endTime),
          backgroundColor: element.colorCode != null ? element.colorCode : ''
        });

      }
      this.calendarOptions = {
        editable: false,
        eventLimit: false,
        header: {
          left: 'today,prev,next',
          center: 'title',
          right: 'month,agendaWeek,agendaDay,listMonth'
        },
        buttonText: {
          month: 'Month',
          week: 'Week',
          day: 'Day',
          list: 'Agenda',
          prev: 'Back',
          next: 'Next',
          today: 'Today'
        },
        events: this.courseAcceptCalendar,
        contentHeight: 'auto',

      };
      // this.ucCalendar.fullCalendar('removeEvents', this.calendarOptions.events);
      // this.ucCalendar.fullCalendar('renderEvents', this.courseAcceptCalendar);
    });
  }

  convertFromStringToDate(value) {
    let time = new Date(value);
    return time;
  }

  onSave(index, courseId) {
    this.courseAcceptCalendar = [];
    const data = {
      courseId: courseId,
      colorCode: this.colorSelected
    }
    this._base._courseService.PostAcceptedCourses(data).pipe(map(m => m.result)).subscribe((e) => {
      this.onGetData();
      this.onShowColor(index, '');
    })
  }

  onShowColor(id, color) {
    if ($('#popover-content-' + id).hasClass('none')) {
      this.colorSelected = color;
      $('#popover-content-' + id).removeClass('none')
      $('#popover-content-' + id).addClass('block');
    }
    else {
      $('#popover-content-' + id).addClass('none');
      $('#popover-content-' + id).removeClass('block');
    }

  }

  setColorValue(value) {
    this.colorSelected = value;
  }

  onCancel(id) {
    this.onShowColor(id, '');
    this.colorSelected = '';
  }

  ngAfterViewInit() {
    this.onGetData();
  }

}
