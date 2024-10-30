import { Component, Input, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { PagedListingComponentBase, PagedRequestDto, FilterDto } from '@shared/paged-listing-component-base';
import { AnnoucementDto as EditDto, AnnoucementDto } from '@app/models/annoucement-dto';
import { AnnoucementService } from '@app/services/systems-admin-services/annoucement.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-tab-annoucements',
  templateUrl: './tab-annoucements.component.html',
  styleUrls: ['./tab-annoucements.component.scss']
})
export class TabAnnoucementsComponent extends PagedListingComponentBase<EditDto> {
  initTinymce = {
    height: 280,
  }
  @Input() courseId: string;
  @Input() courseInstanceId: string;

  AnnouListPanel = true;
  annoucement = {} as EditDto;
  annoucementList: AnnoucementDto[] = [];
  searchText: string;
  constructor(
    injector: Injector,
    private router: Router,
    private _service: AnnoucementService,

  ) {
    super(injector);
  }
  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.sort = 'CreationTime';
    request.sortDirection = 1;
    this._service
      .getAnnoucementByCourseInstanceIdPagging(request, this.courseInstanceId)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: any) => {
        this.annoucementList = result.result.items;
        this.showPaging(result.result, pageNumber);
      });
  }
  protected delete(item: EditDto): void {
    abp.message.confirm(
      'Delete annoucement \'' + item.title + '\'?',
      (result: boolean) => {
        if (result) {
          this._service.delete(item.id).subscribe(() => {
            abp.notify.info('Deleted category: ' + item.title);
            this.refresh();
          });
        }
      }
    );
  }
  newAnnoucement() {
    this.annoucement = {} as EditDto;
    this.AnnouListPanel = false;
  }
  changePanel() {
    this.AnnouListPanel = !this.AnnouListPanel;
  }
  saveAnnoucement() {
    if (this.annoucement.id == null) {
      this.annoucement.courseInstanceId = this.courseInstanceId;
      this._service.create(this.annoucement).subscribe((result: any) => {
        this.refresh();
        this.changePanel();
      })
    } else {
      this._service.update(this.annoucement).subscribe((result: any) => {
        this.refresh();
        this.changePanel();
      })
    }
  }
  editAnnoucement(item: EditDto) {
    this.annoucement = item;
    this.changePanel();
  }
}
