import { Component, ViewChild, Injector, Output, OnInit, EventEmitter, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { TenantServiceProxy, CreateTenantDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'create-tenant-modal',
    templateUrl: './create-tenant.component.html'
})
export class CreateTenantComponent extends AppComponentBase implements OnInit {

    @ViewChild('createTenantModal') modal: ModalDirective;
    @ViewChild('modalContent') modalContent: ElementRef;
    reEXP : RegExp = new RegExp(`^[a-zA-Z][a-zA-Z0-9_-]{1,}$`);
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active: boolean = false;
    saving: boolean = false;
    tenant: CreateTenantDto = null;

    defaultPassword: string;
    isEditPassword: boolean = false;

    constructor(
        injector: Injector,
        private _tenantService: TenantServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        this.onGenNewPassword_click();
    }

    show(): void {
        this.active = true;
        this.modal.show();
        this.tenant = new CreateTenantDto();
        this.tenant.init({ isActive: true });
    }

    onShown(): void {
        $.AdminBSB.input.activate($(this.modalContent.nativeElement));
    }

    save(): void {
     
        if(this.tenant.tenancyName.match(this.reEXP)){
            this.saving = true;
            this.tenant.defaultPassword = this.defaultPassword;
            this._tenantService.create(this.tenant)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
            });
        }
        else {
            abp.message.error(`Spaces and special characters are not allowed for tenant name`);
        }
       
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }

    onGenNewPassword_click(): void {
        this.defaultPassword = this.randomPass(6, true, false, true);
    }
    private randomPass(length, addUpper, addSymbols, addNums) {
        const lower = 'abcdefghijklmnopqrstuvwxyz';
        const upper = addUpper ? lower.toUpperCase() : '';
        const nums = addNums ? '0123456789' : '';
        const symbols = addSymbols ? '!#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~' : '';

        const all = lower + upper + nums + symbols;
        while (true) {
            let pass = '';
            for (let i = 0; i < length; i++) {
                pass += all[Math.random() * all.length | 0];
            }
            // criteria:
            if (!/[a-z]/.test(pass)) continue; // lowercase is a must
            if (addUpper && !/[A-Z]/.test(pass)) continue; // check uppercase
            if (addSymbols && !/\W/.test(pass)) continue; // check symbols
            if (addNums && !/\d/.test(pass)) continue; // check nums

            return pass; // all good
        }
    }
}
