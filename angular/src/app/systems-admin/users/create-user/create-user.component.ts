import { Component, ElementRef, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { PasswordValidator } from '@app/general/user-profile/component/tab-profile/password-requied';
import { finalize } from '@node_modules/rxjs/operators';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateUserDto, RoleDto, UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap';

@Component({
    selector: 'create-user-modal',
    templateUrl: './create-user.component.html'
})
export class CreateUserComponent extends AppComponentBase implements OnInit {

    @ViewChild('createUserModal') modal: ModalDirective;
    @ViewChild('modalContent') modalContent: ElementRef;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active: boolean = false;
    saving: boolean = false;
    user: CreateUserDto = null;
    roles: RoleDto[] = null;
    passwordInValid: boolean = false;
    messageRequidPassword = 'Minimum 8 characters and contain at least three of UPPER CASE, lower case, numberic and symbols';
    constructor(
        injector: Injector,
        private _userService: UserServiceProxy,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._userService.getRoles()
            .subscribe((result) => {
                this.roles = result.items;
            });
    }

    show(): void {
        this.active = true;
        this.modal.show();
        this.user = new CreateUserDto();
        this.user.init({ isActive: true });
    }

    onShown(): void {
        $.AdminBSB.input.activate($(this.modalContent.nativeElement));
    }

    save(): void {
        //TODO: Refactor this, don't use jQuery style code
        var roles = [];
        $(this.modalContent.nativeElement).find("[name=role]").each((ind: number, elem: Element) => {
            if ($(elem).is(":checked") == true) {
                roles.push(elem.getAttribute("value").valueOf());
            }
        });

        this.user.roleNames = roles;
        this.saving = true;
        console.log("USER", this.user);
        this._userService.create(this.user)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit();
            });
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
    onPasswordChange(pass) {
        this.passwordInValid = PasswordValidator.passwordInvalid(pass);
    }
}
