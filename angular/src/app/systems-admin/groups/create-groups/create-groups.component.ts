import { GroupsDto, GroupsDDDto, Result } from './../../../models/groups-dto';
import { GroupsService } from './../../../services/systems-admin-services/groups.service';
import {
  Component,
  ViewChild,
  Injector,
  Output,
  EventEmitter,
  ElementRef,
  OnInit
} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';
import { UserServiceProxy, RoleDto } from '@shared/service-proxies/service-proxies';
import { Subject } from 'rxjs';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
@Component({
  selector: 'create-groups-modal',
  templateUrl: './create-groups.component.html'
})
export class CreateGroupsComponent extends AppComponentBase implements OnInit {
  @ViewChild('createGroupsModal') modal: ModalDirective;
  @ViewChild('modalContent') modalContent: ElementRef;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  
  active: boolean = false;
  saving: boolean = false;
  roles: RoleDto[];
  lstGroups: GroupsDDDto[] = []; // Data for Dropdownlist
  group = {} as GroupsDto;  
  constructor(
    injector: Injector,
    private _userService: UserServiceProxy,
    private _groupsService: GroupsService,
    private fb : FormBuilder
  ) {
    super(injector);
  }

  ngOnInit (): void {
    // this._userService.getRoles().subscribe(result => {
    //   this.roles = result.items;
    // });
    //console.log("ngOnInit()");
    this.getListGroups();
  }

  show(): void {
    //console.log("show()");
    this.getListGroups();
    this.active = true;
    this.modal.show();
  }

  getListGroups () {
    this._groupsService.getAllNotPaging().subscribe( ( result: Result ) => {
      this.lstGroups = result.result.items;
    } );

  }

  onShown (): void {
    $.AdminBSB.input.activate($(this.modalContent.nativeElement));
  }

  save(): void {
    //TODO: Refactor this, don't use jQuery style code
    var roles = [];
    this.saving = true;
    this._groupsService
      .create(this.group)
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
