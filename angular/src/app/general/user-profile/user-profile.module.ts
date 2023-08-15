import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AllMaterialModuleModule } from '@shared/all-material-module/all-material-module.module';
import { UserProfileComponent } from './user-profile.component';
import { TabProfileComponent } from './component/tab-profile/tab-profile.component';
import { TabCertificationComponent } from './component/tab-certification/tab-certification.component';
import { TabSettingComponent } from './component/tab-setting/tab-setting.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '@shared/shared.module';
import { TabProfileChangePasswordDialogComponent } from './component/tab-profile/dialog-changepassword';
import { RouterModule } from '@angular/router';

@NgModule({
  imports: [
    CommonModule,
    AllMaterialModuleModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild([
      {
        path : '',
        component : UserProfileComponent
      }
    ]),
    SharedModule
  ],
  exports: [
    AllMaterialModuleModule,
    UserProfileComponent,
    TabProfileComponent,
    TabCertificationComponent,
    TabSettingComponent
  ],
  declarations: [
    UserProfileComponent,
    TabProfileComponent,
    TabCertificationComponent,
    TabSettingComponent,
    TabProfileChangePasswordDialogComponent,
  ],
  entryComponents: [
    TabProfileChangePasswordDialogComponent,
  ],
})
export class UserProfileModule { }
