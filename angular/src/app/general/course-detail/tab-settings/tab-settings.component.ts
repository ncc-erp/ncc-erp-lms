import { Component, OnInit, Injector, Input } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CourseSettingFeatualModel } from '@app/models/course-setting-dto';
import { BaseService } from '@app/services/base-service/base.service';
import { CoursesService } from '@app/services/systems-admin-services/courses.service';

@Component({
  selector: 'app-tab-settings',
  templateUrl: './tab-settings.component.html',
  styleUrls: ['./tab-settings.component.scss']
})
export class TabSettingsComponent extends AppComponentBase implements OnInit {
  @Input() courseId: string;
  @Input() courseInstanceId: string;

  courseSettingFeatual: CourseSettingFeatualModel[] = [];
  constructor(
    injector: Injector,
    private _courseService: CoursesService,
    private _baseService: BaseService
  ) {
    super(injector);
  }

  ngOnInit() {

    this.courseSettingFeatual = [
      {
        key: 'Show_recent_annoucements_as_first_page',
        name: this.l('Show_recent_annoucements_as_first_page'),
        description: this.l('Show_recent_annoucements_as_first_page_des'),
        entityId: this.courseId,
        value: false,
        checkbox: true
      },
      {
        key: 'Number_of_annoucements_shown_on_homepage',
        name: this.l('Number_of_annoucements_shown_on_homepage'),
        description: this.l('Number_of_annoucements_shown_on_homepage_des'),
        entityId: this.courseId,
        value: 0,
        checkbox: false
      },
      {
        key: 'Allow_students_create_disscussion_on_QA',
        name: this.l('Allow_students_create_disscussion_on_QA'),
        description: this.l('Allow_students_create_disscussion_on_QA_des'),
        entityId: this.courseId,
        value: false,
        checkbox: true
      },
      {
        key: 'Grades_Summary_List_students_by_name',
        name: this.l('Grades_Summary_List_students_by_name'),
        description: this.l('Grades_Summary_List_students_by_name_des'),
        entityId: this.courseId,
        value: false,
        checkbox: true
      },
      {
        key: 'Allow_completed_students_to_re_enroll',
        name: this.l('Allow_completed_students_to_re_enroll'),
        description: this.l('Allow_completed_students_to_re_enroll_des'),
        entityId: this.courseId,
        value: false,
        checkbox: true
      },
      // {
      //   key: 'Hide_totals_in_student_grades_summary',
      //   name: this.l('Hide_totals_in_student_grades_summary'),
      //   description: this.l('Hide_totals_in_student_grades_summary_des'),
      //   entityId: this.courseId,
      //   value: false,
      //   checkbox: true
      // },


    ];

    // Get value get from database
    this._courseService.GetCourseLMSSettingValue(this.courseId)
      .subscribe(items => {
        const temp: any = items.result.items;
        temp.forEach(itemNew => {
          this.courseSettingFeatual.forEach(itemOld => {
            if (itemNew.key === itemOld.key) {
              if (itemNew.value === 'true') {
                itemOld.value = true;
              } else if (itemNew.value === 'false') {
                itemOld.value = false;
              } else {
                itemOld.value = itemNew.value;
              }
            }
          });
        });

      });
  }
  onSaveSettingFeatualClick() {
    this._courseService.updateCourseSettingFeatual(this.courseSettingFeatual).subscribe(item => {
      this.notify.info(this.l('SaveSuccessfully'));
    });
  }

}
