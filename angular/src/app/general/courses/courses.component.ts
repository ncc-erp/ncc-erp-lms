import { CreateCourseComponent } from './create-course/create-course.component';
import { CourseDto as EditDto } from '../../models/courses-dto';
import { CoursesService } from '../../services/systems-admin-services/courses.service';
import { Component, Injector, ViewChild, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { InputFilterDto } from 'shared/filter/filter.component';
import { ICourseDashboard } from '@app/models/student-dto';
import { AppComponentBase } from '@shared/app-component-base';
import { CourseState } from '@shared/AppEnums';

@Component({
  selector: 'app-courses',
  templateUrl: './courses.component.html',
  styleUrls: ['./courses.component.scss'],
  animations: [appModuleAnimation()]
})
export class CoursesComponent extends AppComponentBase implements OnInit {

  @ViewChild('createModal') createModal: CreateCourseComponent;
  // @ViewChild('editModal') editModal: EditCourseComponent;

  courses: ICourseDashboard[] = [];
  publishedCourses: ICourseDashboard[] = [];
  draftCourses: ICourseDashboard[] = [];
  archivedCourses: ICourseDashboard[] = [];
  searchText = '';
  searchArchivedText = '';
  /**
    * list comparision:
    * 0 -> 'Equal',
    * 1 -> 'LessThan',
    * 2 -> 'LessThanOrEqual',
    * 3 -> 'GreaterThan',
    * 4 -> 'GreaterThanOrEqual',
    * 5 -> 'NotEqual',
    * 6 -> 'Contains', //for string only
    * 7 -> 'StartsWith', //for string only
    * 8 -> 'EndsWith', //for string only
    * 9 -> 'In' //for list item
   */
  tabIndex = 0;
  private tabCourseSession = 'tabCourseSession';

  public readonly FILTER_CONFIG: InputFilterDto[] = [
    { propertyName: 'Name', comparisions: [0, 6, 7, 8] },
    { propertyName: 'Description', comparisions: [0, 6, 7, 8] }];

  constructor(
    injector: Injector,
    public _service: CoursesService) {
    super(injector);

  }

  ngOnInit() {
    this.tabIndex = this.getSession(this.tabCourseSession) === null ? 0 : this.getSession(this.tabCourseSession);
    this.getAllCourse();
  }

  getAllCourse() {
    this._service.GetAllCourseNotPagging().subscribe(result => {
      this.courses = result.result;
      this.publishedCourses = this.courses
      .filter(c => c.state === CourseState.Published && !c.isArchived && c.name.includes(this.searchText));
      this.draftCourses = this.courses.filter(c => c.state === CourseState.Draft && c.name.includes(this.searchText));
      this.archivedCourses = this.courses
      .filter(c => c.state === CourseState.Published && c.isArchived && c.name.includes(this.searchArchivedText));

    })
  }

  protected delete(item: EditDto): void {
    abp.message.confirm(
      'Delete course ' + item.name + '?',
      (result: boolean) => {
        if (result) {
          this._service.delete(item.courseId).subscribe(() => {
            abp.notify.info('Deleted course: ' + item.name);
            this.getAllCourse();
          });
        }
      }
    );
  }


  searchPublishAndDraftCourses() {
    this.publishedCourses = this.courses.filter(c => c.state === CourseState.Published && c.name.includes(this.searchText));
    this.draftCourses = this.courses.filter(c => c.state === CourseState.Draft && c.name.includes(this.searchText));
  }

  searchArchivedCourses() {
    this.archivedCourses = this.courses.filter(c => c.state === CourseState.Archived && c.name.includes(this.searchArchivedText));
  }

  createItem(): void {
    this.createModal.show();
  }
  onTabChanged(event) {
    this.setSession(this.tabCourseSession, event.index);
  }
  onRepublishCourse(value) {
    if (value) {
      this.getAllCourse();
    }
  }
   onDeletedCourse(value) {
    if (value) {
      this.getAllCourse();
    }
  }
}
