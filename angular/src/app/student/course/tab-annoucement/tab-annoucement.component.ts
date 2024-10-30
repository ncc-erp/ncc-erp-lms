import { Component, OnInit, Input, Injector } from '@angular/core';
import { StudentAnnoucementDto } from '@app/models/annoucement-dto';
import { PagedRequestDto, PagedListingComponentBase, PagedResultResultDto } from '@shared/paged-listing-component-base';
import { AnnoucementService } from '@app/services/systems-admin-services/annoucement.service';
import { finalize, count } from 'rxjs/operators';
import { AppConsts } from '@shared/AppConsts';
import { CourseSettingFeatualValueDto } from '@app/models/course-setting-dto';

@Component({
  selector: 'app-tab-annoucement',
  templateUrl: './tab-annoucement.component.html',
  styleUrls: ['./tab-annoucement.component.scss']
})
export class TabAnnoucementComponent extends PagedListingComponentBase<any> implements OnInit {
  @Input() courseInstanceId: string;
  @Input() courseId: string;
  @Input() courseSetting: CourseSettingFeatualValueDto;

  annoucements: StudentAnnoucementDto[] = [];

  constructor(injector: Injector,
    private _service: AnnoucementService,
  ) { super(injector); }

  ngOnInit() {
    if (this.courseSetting.Number_of_annoucements_shown_on_homepage > 0) {
      this.pageSize = this.courseSetting.Number_of_annoucements_shown_on_homepage;
    };
    if (this.courseSetting.Number_of_annoucements_shown_on_homepage === 0) {
      this.pageSize = 50; // Show All
    }
    this.refresh();
  }

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this._service
      .GetAnnoucementForStudentByCourseInstanceIdPagging(request, this.courseInstanceId)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: PagedResultResultDto) => {
        this.annoucements = result.result.items;
        this.showPaging(result.result, pageNumber);
      });
  }

  protected delete(item: any): void {

  }

}
