import { AvatarModule } from 'ngx-avatar';
import { EditGroupsComponent } from './groups/edit-groups/edit-groups.component';
import { CreateGroupsComponent } from './groups/create-groups/create-groups.component';
import { AbpModule } from '@abp/abp.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { SharedModule } from '@shared/shared.module';
import { ModalModule } from 'ngx-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EditRoleComponent } from './roles/edit-role/edit-role.component';
import { CreateRoleComponent } from './roles/create-role/create-role.component';
import { RolesComponent } from './roles/roles.component';
import { EditUserComponent } from './users/edit-user/edit-user.component';
import { CreateUserComponent } from './users/create-user/create-user.component';
import { UsersComponent } from './users/users.component';
import { HomeComponent } from './home/home.component';
import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { SystemsAdminRoutingModule } from './/systems-admin-routing.module';
import { GroupsComponent } from './groups/groups.component';
import { ResetPasswordUserComponent } from './users/reset-password-user/reset-password-user.component';
import { EditorModule } from '@tinymce/tinymce-angular';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { CreateCategoryComponent } from './categories/create-category/create-category.component';
import { EditCategoryComponent } from './categories/edit-category/edit-category.component';
import { CategoriesComponent } from './categories/categories.component';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
// import { BrowserModule } from '@angular/platform-browser';
// import { FroalaEditorModule, FroalaViewModule } from 'angular-froala-wysiwyg';
import { DndModule } from 'ngx-drag-drop';
// import { ModuleDetailComponent } from './module-detail/module-detail.component';
import { CalendarComponent } from './calendar/calendar.component';
import { ReportsComponent } from './reports/reports.component';
import { UserProfileModule } from '@app/general/user-profile/user-profile.module';
import { ConfigurationComponent } from './configuration/configuration.component';
// import { SettingComponent } from './setting/setting.component';
import { TabGroupComponent, DialogTabGroupComponent } from './users/tab-group/tab-group.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ExportService } from '@app/services/systems-admin-services/export.service';

@NgModule({
  imports: [
    CommonModule,
    SystemsAdminRoutingModule,
    SharedModule,
    UserProfileModule,
    AbpModule,
    FormsModule,
    ReactiveFormsModule,
    ModalModule.forRoot(),
    NgxPaginationModule,
    DndModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
    NgMultiSelectDropDownModule.forRoot(),
    EditorModule,
    AvatarModule,
    SharedModule,
    NgbModule,
  ],
  declarations: [
    HomeComponent,
    UsersComponent,
    CreateUserComponent,
    ConfigurationComponent,
    EditUserComponent,
    RolesComponent,
    ResetPasswordUserComponent,
    CreateGroupsComponent,
    CreateRoleComponent,
    EditGroupsComponent,
    EditRoleComponent,
    GroupsComponent,
    CategoriesComponent, CreateCategoryComponent, EditCategoryComponent,
    CalendarComponent,
    ReportsComponent,
    // SettingComponent,
    // UserGroupComponent,
    TabGroupComponent,
    DialogTabGroupComponent,
  ],
  entryComponents: [
    DialogTabGroupComponent
  ],
  providers: [
    ExportService,
    DatePipe
  ]
})
export class SystemsAdminModule { }
