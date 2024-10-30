import { ResetPasswordUserComponent } from './reset-password-user/reset-password-user.component';
import { EditUserComponent } from './edit-user/edit-user.component';
import { CreateUserComponent } from './create-user/create-user.component';
import { Component, Injector, ViewChild, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { UserServiceProxy, UserDto, PagedResultDtoOfUserDto } from '@shared/service-proxies/service-proxies';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { TenantServiceProxy, TenantDto, PagedResultDtoOfTenantDto } from '@shared/service-proxies/service-proxies';

@Component({
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

    displaySelectTenant: string = 'none';
    ngOnInit() {
        this.checkShowSelectTenant();
    }

    constructor(
        injector: Injector,
        private _userService: UserServiceProxy,
        private _tenantServiceProxy: TenantServiceProxy
    ) {
        super(injector);
    }

    protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
        //this._userService.getAll(request.skipCount, request.maxResultCount)
        this._userService.getUsersByTenantId(this.selectedTenantId, request.skipCount, request.maxResultCount)
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
            'Delete user \'' + user.fullName + '\'?',
            (result: boolean) => {
                if (result) {
                    this._userService.DeleteAdmin(user.id, this.selectedTenantId)
                        .subscribe(() => {
                            abp.notify.info('Deleted User: ' + user.fullName);
                            // this.refresh();
                            for (let i = 0; i < this.users.length; i++) {
                                if (this.users[i].id === user.id) {
                                    this.users.splice(i, 1);
                                    break;
                                }
                            }
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
        this._tenantServiceProxy.getAll(0, 10)
            .subscribe(result => {
                this.tenants = result.items;
            });
    }

    checkShowSelectTenant() {
        if (!this.appSession.tenant) {
            this.displaySelectTenant = 'block';
            this.getTenants();
            this.refresh();
        } else {
            this.displaySelectTenant = 'none';
            this.selectedTenantId = this.appSession.tenant.id;
            this.refresh();
        }
    }

    // Show Modals
    createUser(): void {
        this.createUserModal.show(this.selectedTenantId);
    }

    editUser(user: UserDto): void {
        this.editUserModal.show(user.id, this.selectedTenantId);
    }

    resetPasswordUser(user: UserDto): void {
        this.resetpasswordUserModal.show(user.id, user.userName, this.selectedTenantId);
    }
}
