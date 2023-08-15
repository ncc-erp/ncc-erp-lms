import { Component, OnInit, Injector, Input } from '@angular/core';
import { UserDto } from '@app/models/user-dto';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { BaseService } from '@app/services/base-service/base.service';
import { LanguageDto } from '@app/models/language-dto';
import { TimezoneDto } from '@app/models/timezone-dto';
import * as $ from 'jquery';
import { PERMISSION } from '@app/models/constant';
import { DateTimePipe } from '@shared/pipes/date-local-utc.pipe';
@Component({
  selector: 'app-tab-setting',
  templateUrl: './tab-setting.component.html',
  styleUrls: ['./tab-setting.component.scss']
})
export class TabSettingComponent extends PagedListingComponentBase<UserDto> {
  users = {} as UserDto;
  languages: LanguageDto[] = [];
  timeZone: TimezoneDto[] = [];
  isEdit: boolean = false;
  infoPulic: boolean = false;
  linksPublic: boolean = false;
  permissonName: string;
  dateNow: Date = new Date();
  readonly permissonStudent = PERMISSION.STUDENTS;
  readonly permissionTenants = PERMISSION.TENANTS;
  constructor(private injector: Injector, private _baseService: BaseService) {
    super(injector);
  }
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this._baseService._studentService.users$.subscribe(e => {
      this.users = e;
    });
    this._baseService._studentService.languages$.subscribe(e => {
      this.languages = e;
    });
    this._baseService._studentService.timeZone$.subscribe(e => {
      this.timeZone = e;
    });
    this._baseService._studentService.permissonName$.subscribe(e => {
      this.permissonName = e;
    });
  }

  onSave() {
    this._baseService._studentService.updateUser(this.users).subscribe(e => {
      const baseOffset = e.result.baseUtcOffset;
      if (baseOffset) {
        DateTimePipe.timezone = baseOffset
        const hour = +baseOffset.substring(1, 3);
        const minute = +baseOffset.substring(3, 5);
        const ext = +(baseOffset.substring(0, 1) + '1'); // 1 or -1
        DateTimePipe.timezoneSecon = (hour * 60 + minute) * ext * 60;
      }
      this._baseService._studentService.isChange(true);
      this.onCancel();
    })
  }

  onEdit() {
    this.isEdit = true;
    if ($('select').hasClass('readonly-select')) {
      $('select').removeClass('readonly-select');
      $('select').removeAttr('disabled');
      $('input').removeAttr('disabled');
    }
  }

  onCancel() {
    this.isEdit = false;
    if (!$('select').hasClass('readonly-select')) {
      $('select').addClass('readonly-select');
      $('select').attr('disabled', 'disabled');
      $('input').attr('disabled', 'disabled');
    }
  }

  protected delete(entity: UserDto): void {
  }

}
