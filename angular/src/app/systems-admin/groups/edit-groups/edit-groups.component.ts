import { GroupsService } from './../../../services/systems-admin-services/groups.service';
import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { UserServiceProxy, UserDto, RoleDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';
import { GroupsDto, GroupsDDDto, Result } from '@app/models/groups-dto';

@Component({
  selector: 'edit-groups-modal',
  templateUrl: './edit-groups.component.html'
})
export class EditGroupsComponent extends AppComponentBase implements OnInit {
  @ViewChild('editGroupsModal') modal: ModalDirective;
  @ViewChild('modalContent') modalContent: ElementRef;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active: boolean = false;
  saving: boolean = false;

  group = {} as GroupsDto;
  roles: RoleDto[] = null;
  lstGroups: GroupsDDDto[] = []; // Data for Dropdownlist

  constructor(injector: Injector, private _userService: UserServiceProxy, private _groupsService: GroupsService) {
    super(injector);
  }

  ngOnInit(): void {
    // this._userService.getRoles().subscribe(result => {
    //   this.roles = result.items;
    // });
    // this.getListGroups();
  }

  userInRole(role: RoleDto, user: UserDto): string {
    if (user.roleNames.indexOf(role.normalizedName) !== -1) {
      return 'checked';
    } else {
      return '';
    }
  }

  show(id: string): void {
    this._userService.getRoles().subscribe(result => {
      this.roles = result.items;
    });
    this.getListGroups(id);
    this._groupsService.getById(id).subscribe(result => {
      this.group = result.result;
      this.active = true;
      this.modal.show();
    });
  }

  getListGroups(id: string) {
    this.lstGroups = [];
    this._groupsService.getAllNotPaging().subscribe((result: Result) => {
      result.result.items.forEach(element => {
        if (element.id !== id) {
          this.lstGroups.push(element);
        }
      });
      //this.lstGroups = result.result.items;      
    });

  }

  onShown(): void {
    $.AdminBSB.input.activate($(this.modalContent.nativeElement));
  }

  save(): void {
    var roles = [];
    $(this.modalContent.nativeElement)
      .find('[name=role]')
      .each(function (ind: number, elem: Element) {
        if ($(elem).is(':checked')) {
          roles.push(elem.getAttribute('value').valueOf());
        }
      });

    // this.group.roleNames = roles;

    this.saving = true;
    this._groupsService
      .update(this.group)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
