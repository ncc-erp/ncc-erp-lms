import { ResetPasswordUserComponent } from './reset-password-user/reset-password-user.component';
import { EditUserComponent } from './edit-user/edit-user.component';
import { CreateUserComponent } from './create-user/create-user.component';
import { Component, Injector, ViewChild, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { UserServiceProxy, UserDto, PagedResultDtoOfUserDto } from '@shared/service-proxies/service-proxies';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { TenantServiceProxy, TenantDto, PagedResultDtoOfTenantDto } from '@shared/service-proxies/service-proxies';
import { UsersService } from '@app/services/systems-admin-services/user.service';

@Component({
    selector: 'app-user-group-users',
    templateUrl: './users.component.html',
    animations: [appModuleAnimation()]
})
export class UsersComponent extends PagedListingComponentBase<UserDto> implements OnInit {

    @ViewChild('createUserModal') createUserModal: CreateUserComponent;
    @ViewChild('editUserModal') editUserModal: EditUserComponent;
    @ViewChild('resetPasswordUserModal') resetpasswordUserModal: ResetPasswordUserComponent;

    active: boolean = false;
    users: UserDto[] = [];
    selectedTenantId: number = 0;
    tenants: TenantDto[] = [];
    isLoad: boolean = false;
    displaySelectTenant: string = "none";
    curentTab: number = 0;
    private tabUserGroupSession = 'tabUserGroupSession';

    constructor(
        injector: Injector,
        private _userService: UsersService,
        private _userServiceProxy: UserServiceProxy,
        private _tenantServiceProxy: TenantServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        this.checkShowSelectTenant();
        this.curentTab = this.getSession(this.tabUserGroupSession) === null ? 0 : this.getSession(this.tabUserGroupSession);
    }

    protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
        //this._userService.getAll(request.skipCount, request.maxResultCount)
        this._userService.getUsersByTenantId(this.selectedTenantId, request.skipCount, request.maxResultCount, this.searchText)
            .pipe(finalize(() => {
                finishedCallback()
            }))
            .subscribe((data) => {
                this.users = data.result.items;
                this.showPaging(data.result, pageNumber);
            });
    }

    protected delete(user: UserDto): void {
        abp.message.confirm(
            "Delete user '" + user.fullName + "'?",
            (result: boolean) => {
                if (result) {
                    this._userServiceProxy.delete(user.id)
                        .subscribe(() => {
                            abp.notify.info("Deleted User: " + user.fullName);
                            this.refresh();
                        });
                }
            }
        );
    }

    onSelect(event): void {
        this.selectedTenantId = event.target.value;
        this.refresh();
    }

    getTenants(): void {
        this._tenantServiceProxy.getAll(0, 1000000)
            .subscribe(result => {
                this.tenants = result.items;

            });
    }

    checkShowSelectTenant() {
        if (!this.appSession.tenant) {
            this.displaySelectTenant = "block";
            this.getTenants();
            this.refresh();
        } else {
            this.displaySelectTenant = "none";
            this.selectedTenantId = this.appSession.tenant.id;
            this.refresh();
        }
    }

    // Show Modals
    createUser(): void {
        this.isLoad = true;
        this.createUserModal.show();
    }

    editUser(user: UserDto): void {
        this.isLoad = true;
        this.editUserModal.show(user.id);
    }

    resetPasswordUser(user: UserDto): void {
        this.isLoad = true;
        this.resetpasswordUserModal.show(user.id, user.userName, this.selectedTenantId);
    }

    onTabChanged(event) {
        this.setSession(this.tabUserGroupSession, event.index);
    }
}
