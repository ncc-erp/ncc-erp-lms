import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef } from '@angular/core';
import {randomatic} from 'randomatic';
import { ModalDirective } from 'ngx-bootstrap';
import {UserServiceProxy, ResetPasswordUserDto} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';


@Component({
  selector: 'reset-password-user-modal',
  templateUrl: './reset-password-user.component.html'
})
export class ResetPasswordUserComponent extends AppComponentBase {

    @ViewChild('resetPasswordUserModal') modal: ModalDirective;
    @ViewChild('modalContent') modalContent: ElementRef;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active: boolean = false;
    saving: boolean = false;
    showPassword : boolean = false;
    classPassword :string  =  `fa fa-eye fa-fw`;
    user: ResetPasswordUserDto = null;
    userName: string = "";

    constructor(
        injector: Injector,
        private _userService: UserServiceProxy
    ) {
        super(injector);
    }

    show(id: number, userName: string,tenantId : number): void {
        //this._userService.get(id)
        //    .subscribe(
        //        (result) => {
        //            this.user = new ResetPasswordUserDto();
        //            this.user.id = result.id;
        //            this.user.name = result.name;
        //            this.active = true;
        //            this.modal.show();
        //        }
        //    );
        this.user = new ResetPasswordUserDto();
        this.user.tenantId = tenantId;
        this.user.id = id;
        this.userName = userName;
        this.active = true;
        this.modal.show();

    }

    onShown(): void {
        $.AdminBSB.input.activate($(this.modalContent.nativeElement));
    }

    save(): void {
        this.saving = true;
        this._userService.resetPassword(this.user)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('Reset Password Successfully'));
                this.close();
                this.modalSave.emit(null);
            });
    }

    onShowPassword(){
        if(this.showPassword == false){
            this.classPassword = `fa fa-eye-slash fa-fw`;
            this.showPassword = true;
        }
        else{
            this.classPassword =  `fa fa-eye fa-fw`;
            this.showPassword = false;
        }
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }

    generateRandomPassword(): void {
        var randomize = require('randomatic');
        this.user.newPassword = randomize('Aa0', 10);
    }

}
