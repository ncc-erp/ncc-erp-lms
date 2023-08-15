import { ResetPasswordUserComponent } from './users/reset-password-user/reset-password-user.component';
import { AbpModule } from '@abp/abp.module';
import { SharedModule } from '@shared/shared.module';
import { FormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap';
import { NgxPaginationModule } from 'ngx-pagination';
import { EditRoleComponent } from './roles/edit-role/edit-role.component';
import { CreateRoleComponent } from './roles/create-role/create-role.component';
import { RolesComponent } from './roles/roles.component';
import { EditUserComponent } from './users/edit-user/edit-user.component';
import { CreateUserComponent } from './users/create-user/create-user.component';
import { UsersComponent } from './users/users.component';
import { EditTenantComponent } from './tenants/edit-tenant/edit-tenant.component';
import { CreateTenantComponent } from './tenants/create-tenant/create-tenant.component';
import { TenantsComponent } from './tenants/tenants.component';
import { HomeComponent } from './home/home.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RootAdminRoutingModule } from './/root-admin-routing.module';
import { AllMaterialModuleModule } from '@shared/all-material-module/all-material-module.module';
import { UserProfileModule } from '@app/general/user-profile/user-profile.module';
@NgModule({
  imports: [
    CommonModule,
    RootAdminRoutingModule,
    FormsModule,
    UserProfileModule,
    ModalModule.forRoot(),
    SharedModule,
    NgxPaginationModule,
    AllMaterialModuleModule,
    AbpModule
  ],
  exports: [
    AllMaterialModuleModule
  ],
  declarations: [
    HomeComponent,
    TenantsComponent,
    CreateTenantComponent,
    EditTenantComponent,
    UsersComponent,
    CreateUserComponent,
    EditUserComponent,
    ResetPasswordUserComponent,
    RolesComponent,
    CreateRoleComponent,
    EditRoleComponent
  ]
})
export class RootAdminModule { }
