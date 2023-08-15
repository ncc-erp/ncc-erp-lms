import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef, OnInit, OnChanges } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { UserServiceProxy, CreateUserDto, RoleDto, TenantDto, TenantServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'create-user-modal',
    templateUrl: './create-user.component.html'
})
export class CreateUserComponent extends AppComponentBase implements OnInit {

    @ViewChild('createUserModal') modal: ModalDirective;
    @ViewChild('modalContent') modalContent: ElementRef;

    @Output() modalSave: EventEmitter<number> = new EventEmitter<number>();

    active: boolean = false;
    saving: boolean = false;
    user: CreateUserDto = null;
    roles: RoleDto[] = null;
    selectedTenantId: number = 0;
    tenants: TenantDto[] = [];
    tenantId: number = 0;
    displaySelectTenant: string = 'none';
    constructor(
        injector: Injector,
        private _userService: UserServiceProxy,
        private _tenantServiceProxy: TenantServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {

        if (!this.appSession.tenant) {
            this.displaySelectTenant = 'block';
        }

    }

    getRoles() {
        this._userService.GetRolesByTenantId(this.tenantId)
            .subscribe((data) => {
                //console.log('data', data);

                this.roles = data.result.items;
            });
    }

    show(tenanId: number): void {
        this.tenantId = tenanId;

        this.active = true;
        this.modal.show();
        this.user = new CreateUserDto();
        this.user.isActive = true;
        this.getTenants();
        this.getRoles();
    }

    onShown(): void {
        $.AdminBSB.input.activate($(this.modalContent.nativeElement));
    }

    save(): void {
        //TODO: Refactor this, don't use jQuery style code
        let roles = [];
        $(this.modalContent.nativeElement).find('[name=role]').each((ind: number, elem: Element) => {
            if ($(elem).is(':checked') === true) {
                roles.push(elem.getAttribute('value').valueOf());
            }
        });

        this.user.roleNames = roles;
        this.user.tenantId = this.tenantId === 0 ? null : this.tenantId;
        this.saving = true;
        this._userService.create(this.user)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(this.tenantId);
            });
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }

    getTenants(): void {
        this._tenantServiceProxy.getAll(0, 100)
            .subscribe(result => {
                this.tenants = result.items;
                const newTenant: TenantDto = new TenantDto();
                newTenant.id = 0;
                newTenant.name = 'No Tenant';
                this.tenants.unshift(newTenant);
            });
    }

    onSelectTenant_change(): void {

        //console.log(this.tenantId);
        this.getRoles();
        // this.refresh();
    }
}
