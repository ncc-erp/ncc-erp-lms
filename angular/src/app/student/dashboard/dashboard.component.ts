import { AppConsts } from './../../../shared/AppConsts';
import { CoursesService } from './../../services/systems-admin-services/courses.service';
import { Component, OnInit, Injector } from '@angular/core';
import { PagedListingComponentBase, PagedResultResultDto, PagedRequestDto } from '@shared/paged-listing-component-base';
import { CourseDto } from '@app/models/courses-dto';
import { finalize, map } from 'rxjs/operators';
import { StudentService } from '@app/services/student-service/student.service';
import { IStatictisDto, ICourseDashboard } from '@app/models/student-dto';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent extends PagedListingComponentBase<CourseDto>{

  constructor(private _courseService: CoursesService, private _service: StudentService, injector: Injector) {
    super(injector)
    // _courseService.NAME = 'Course';
  }
  statictis = {} as IStatictisDto;
  course: ICourseDashboard[] = [];
  ngOnInit() {
    const data = parseInt(abp.setting.get('Tenant.DashboardDefaultView'));
    if (data == 0) {
      this._service.setList(false);
    } else {
      this._service.setList(true);
    }
    this.refresh();
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this._service
      .getAllCourse(request)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: PagedResultResultDto) => {
        this.course = result.result.items;
        //console.log(this.course)
        this.showPaging(result.result, pageNumber);
      });
    this.onGetCourseStatistic();
  }

  public onGetCourseStatistic() {
    this._service.getCourseStatistic().pipe(map(r => r.result)).subscribe(e => {
      const data: any = e;
      this.statictis = data;
    });
  }
  protected delete(entity: CourseDto): void {

  }

  onViewItems() {
    this._service.setList(false);
  }

  onViewList() {
    this._service.setList(true);
  }

}
