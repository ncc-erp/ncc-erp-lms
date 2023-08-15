import { GroupsComponent } from './groups/groups.component';
import { RolesComponent } from './roles/roles.component';
import { UsersComponent } from './users/users.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { HomeComponent } from './home/home.component';
import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CategoriesComponent } from './categories/categories.component';
import { ConfigurationComponent } from './configuration/configuration.component';
import { ReportsComponent } from './reports/reports.component';
@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild([
      {
        path: '',
        children: [
          { path: '', redirectTo: '/app/system-admin/courses', pathMatch: 'full' },
          { path: 'home', component: HomeComponent, canActivate: [AppRouteGuard] },
          { path: 'users', component: UsersComponent, data: { permission: 'Pages.UserGroups' }, canActivate: [AppRouteGuard] },
          { path: 'roles', component: RolesComponent, data: { permission: 'Pages.Roles' }, canActivate: [AppRouteGuard] },
          { path: 'groups', component: GroupsComponent, data: { permission: 'Pages.Users' }, canActivate: [AppRouteGuard] },
          { path: 'categories', component: CategoriesComponent, data: { permission: 'Pages.Categories' }, canActivate: [AppRouteGuard] },
          { path: 'configurations', component: ConfigurationComponent, data: { permission: 'Pages.Configurations' }, canActivate: [AppRouteGuard] },
          {
            path: 'setting', loadChildren: './setting/setting.module#SettingModule',
            data: { permission: 'Pages.Settings' }, canActivate: [AppRouteGuard]
          },
          { path: 'reports', component: ReportsComponent, data: { permission: 'Pages.Report' }, canActivate: [AppRouteGuard] },
        ]
      }
    ])
  ],
  exports: [
    RouterModule
  ],
  declarations: []
})
export class SystemsAdminRoutingModule { }
