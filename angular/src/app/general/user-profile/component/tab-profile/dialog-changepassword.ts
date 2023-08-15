import { AppComponentBase } from '@shared/app-component-base';
import { FormGroup, FormBuilder, Validators, FormControl, MinLengthValidator } from '@angular/forms';
import { Component, OnInit, Injector } from '@angular/core';

import { MatDialogRef } from '@angular/material';
import { UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { ChangePasswordDto } from '@app/models/user-dto';
import { AppSessionService } from '@shared/session/app-session.service';
import { PasswordValidator } from './password-requied';

@Component({
  selector: 'app-dialog-changepassword',
  templateUrl: 'dialog-changepassword.html',
})

export class TabProfileChangePasswordDialogComponent extends AppComponentBase implements OnInit {

  registrationFormGroup: FormGroup;
  passwordFormGroup: FormGroup;

  user: ChangePasswordDto = new ChangePasswordDto();

  passwordInValid: boolean = false;
  messageRequidPassword = 'Minimum 8 characters and contain at least three of UPPER CASE, lower case, numberic and symbols';

  constructor(private _userService: UserServiceProxy,
    private _userSessionService: AppSessionService,
    private formBuilder: FormBuilder,
    private injector: Injector,
    public dialogRef: MatDialogRef<TabProfileChangePasswordDialogComponent>) {
    super(injector);
    this.formChangePassBuilder();
  }

  formChangePassBuilder() {
    this.passwordFormGroup = this.formBuilder.group({
      password: ['', [Validators.required]],
      repeatPassword: ['', Validators.required]
    },
      {
        validator: PasswordValidator.validate.bind(this)
      });


    this.registrationFormGroup = this.formBuilder.group({
      currentPass: ['', Validators.required],
      passwordFormGroup: this.passwordFormGroup
    });
  }

  ngOnInit() {

  }
  // convenience getter for easy access to form fields
  get fp() { return this.registrationFormGroup.controls; }
  get fc() { return this.passwordFormGroup.controls; }
  get fcp() { return this.passwordFormGroup.controls.password.errors; }

  onSubmit() {
    this.user.oldPassword = this.registrationFormGroup.controls.currentPass.value;
    this.user.newPassword = this.passwordFormGroup.controls.password.value;
    if (this.user.oldPassword === this.user.newPassword) {
      abp.notify.info(this.l('The old password is the same as the new password.'), this.l('Password does not change!'));
      return;
    }

    this.user.id = this._userSessionService.userId;
    this.user.oldPassword = this.registrationFormGroup.controls.currentPass.value;
    this.user.newPassword = this.passwordFormGroup.controls.password.value;
    this._userService.changePassword(this.user)
      .subscribe(data => {
        if (data.success) {
          if (data.result) {
            abp.notify.success(this.l('UpdateSuccessfully'));
            this.dialogRef.close();
          } else {
            abp.notify.info(this.l('Current password incorrect'));
          }
        }
      })
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  onPasswordChange(pass) {
    this.passwordInValid = PasswordValidator.passwordInvalid(pass);
  }
}

