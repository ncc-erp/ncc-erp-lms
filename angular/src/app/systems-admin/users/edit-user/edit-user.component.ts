import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { UserServiceProxy, UserDto, RoleDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';
import { UserStatusDto, Result as UserStatusResult } from '@app/models/user-status-dto';
import { UserStatusService } from './../../../services/systems-admin-services/user.status.service';
import { GroupsDto, Result as GroupResult } from '@app/models/groups-dto';
import { GroupsService } from './../../../services/systems-admin-services/groups.service';
import { IGroupsToUserDto, GroupsToUserDto, Result as UserGroupResult, GroupsByUserIdResult } from '@app/models/user-group-dto';
import { UserGroupService } from './../../../services/systems-admin-services/user.group.service';
import { group } from '@angular/animations';


@Component({
    selector: 'edit-user-modal',
    templateUrl: './edit-user.component.html'
})
export class EditUserComponent extends AppComponentBase implements OnInit {

    @ViewChild('editUserModal') modal: ModalDirective;
    @ViewChild('modalContent') modalContent: ElementRef;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active: boolean = false;
    saving: boolean = false;

    user: UserDto = null;
    roles: RoleDto[] = null;
    statuses: UserStatusDto[] = null;
    //groups: GroupsDto[] = null;

    dropdownListGroup = [];
    selectedGroupItems = [];
    dropdownSettings = {};

    constructor(
        injector: Injector,
        private _userService: UserServiceProxy,
        private _userStatusService: UserStatusService,
        private _groupsService: GroupsService,
        private _userGroupService: UserGroupService
    ) {
        super(injector);
    }


    ngOnInit() {
        this.dropdownSettings = {
            singleSelection: false,
            idField: 'item_id',
            textField: 'item_text',
            selectAllText: 'Select All',
            unSelectAllText: 'UnSelect All',
            itemsShowLimit: 30,
            allowSearchFilter: true
        };
    }

    onItemSelect(item: any) {
    }
    onSelectAll(items: any) {
    }
    onItemDeSelect(item: any) {
    }

    userInRole(role: RoleDto, user: UserDto): string {
        if (user.roleNames.indexOf(role.normalizedName) !== -1) {
            return "checked";
        }
        else {
            return "";
        }
    }

    show(id: number): void {
        this._userService.getRoles()
            .subscribe((result) => {
                this.roles = result.items;
            });

        this._userService.get(id)
            .subscribe(
                (result) => {
                    this.user = result;
                    this.active = true;
                    this.modal.show();
                }
            );
        this.getStatuses();
        this.getAllGroups();
        this.getGroupsByUserId(id);
    }

    onShown(): void {
        $.AdminBSB.input.activate($(this.modalContent.nativeElement));
    }

    save(): void {
        var roles = [];
        $(this.modalContent.nativeElement).find("[name=role]").each(function (ind: number, elem: Element) {
            if ($(elem).is(":checked")) {
                roles.push(elem.getAttribute("value").valueOf());
            }
        });

        this.user.roleNames = roles;

        this.saving = true;
        this._userService.update(this.user)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('Save Successfully'));
                this.close();
                this.modalSave.emit(null);
                var groupsToUser = new GroupsToUserDto();
                groupsToUser.userId = this.user.id;
                groupsToUser.groupIds = [];
                this.selectedGroupItems.forEach(group => {
                    groupsToUser.groupIds.push(group.item_id);
                });

                this._userGroupService.addGroupsToUser(groupsToUser)
                    .pipe(finalize(() => { this.saving = false; }))
                    .subscribe(() => {
                        this.notify.info(this.l('Add this user to groups successfully'));
                        // this.close();
                        // this.modalSave.emit(null);
                    });
            });
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }

    getStatuses(): void {
        this._userStatusService.getAllNotPagging().subscribe((result: UserStatusResult) => {
            this.statuses = result.result.items;
        });
    }

    getAllGroups() {
        this.dropdownListGroup = [];
        this._groupsService.getAllNotPaging().subscribe((result: GroupResult) => {
            result.result.items.forEach(element => {
                this.dropdownListGroup.push({ item_id: element.id, item_text: element.name });
            });
        });
    }

    getGroupsByUserId(userId: number) {
        this.selectedGroupItems = [];
        this._userGroupService.getGroupsByUserIdNotPaging(userId).subscribe((result: GroupsByUserIdResult) => {
            result.result.items && result.result.items.length > 0 && result.result.items.forEach(group => {
                if (this.selectedGroupItems.indexOf(group) < 0) {
                    this.selectedGroupItems.push({ item_id: group.id, item_text: group.name });
                }
            });
        });
    }
}
