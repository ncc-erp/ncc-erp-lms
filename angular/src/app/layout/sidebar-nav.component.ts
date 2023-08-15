import { Component, Injector, ViewEncapsulation, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MenuItem } from '@shared/layout/menu-item';
import { AppPreBootstrap } from 'AppPreBootstrap';
import { PERMISSION } from '@app/models/constant';

@Component({
    templateUrl: './sidebar-nav.component.html',
    selector: 'sidebar-nav',
    encapsulation: ViewEncapsulation.None
})
export class SideBarNavComponent extends AppComponentBase implements OnInit {

    menuItems: MenuItem[] = [];

    constructor(
        injector: Injector
    ) {
        super(injector);
        this.onInitMenuItems();
    }

    ngOnInit(): void {
        // this.onInitMenuItems();
    }

    onInitMenuItems() {
        AppPreBootstrap.getUserConfiguration(() => {
        }).pipe(e => {
            //var permisson = e.auth.grantedPermissions;
            this.menuItems = [];
          
            if (this.permission.isGranted('Pages.Tenants')) {
                this.menuItems = [                    
                    new MenuItem(this.l('Account'), 'Pages.Account', 'person', '/app/account'),
                    new MenuItem(this.l('Tenants'), 'Pages.Tenants', 'business', '/app/root-admin/tenants'),
                    new MenuItem(this.l('Users'), 'Pages.Tenants', 'person', '/app/root-admin/users')
                ];
                return;
            }
            if (this.permission.isGranted('Pages.Account')) {
                this.menuItems.push({ name: this.l('Account'), permissionName: 'Pages.Account', icon: 'person', route: '/app/account', items: [] });
            }
            if (this.permission.isGranted('Pages.Course')) {
                this.menuItems.push({ name: this.l('Manage Courses'), permissionName: 'Pages.Course', icon: 'assignment', route: '/app/courses', items: [] });
            }
            if (this.permission.isGranted('Pages.Categories')) {
                this.menuItems.push({ name: this.l('Manage Categories'), permissionName: 'Pages.Categories', icon: 'assignment', route: '/app/systems-admin/categories', items: [] });
            }
            if (this.permission.isGranted('Pages.Configurations')) {
                this.menuItems.push({ name: this.l('Configurations'), permissionName: 'Pages.Configurations', icon: 'settings_applications', route: '/app/systems-admin/configurations', items: [] });
            }
            if (this.permission.isGranted('Pages.Settings')) {
                this.menuItems.push({ name: this.l('Setting'), permissionName: 'Pages.Settings', icon: 'settings', route: '/app/systems-admin/setting', items: [] });
            }
            if (this.permission.isGranted('Pages.UserGroups')) {
                this.menuItems.push({ name: this.l('Manage Users & Group'), permissionName: 'Pages.UserGroups', icon: 'person', route: '/app/systems-admin/users', items: [] });
            }
            if (this.permission.isGranted('Pages.Report')) {
                this.menuItems.push({ name: this.l('Reports'), permissionName: 'Pages.Report', icon: 'assignment', route: '/app/systems-admin/reports', items: [] });
            }
            if (this.permission.isGranted('Pages.Dashboard')) {
                this.menuItems.push({ name: this.l('Dashboard'), permissionName: 'Pages.Dashboard', icon: 'poll', route: '/app/student/dashboard', items: [] });
            }
            if (this.permission.isGranted('Pages.CourseView')) {
                this.menuItems.push({ name: this.l('Courses'), permissionName: 'Pages.CourseView', icon: 'assignment', route: '/app/student/courses', items: [] });
            }
            if (this.permission.isGranted('Pages.Calendar')) {
                this.menuItems.push({ name: this.l('Calendar'), permissionName: 'Pages.Calendar', icon: 'calendar_today', route: '/app/student/calendar', items: [] });
            }
        });
    }

    showMenuItem(menuItem): boolean {
        // if (menuItem.permissionName) {
        //     return this.permission.isGranted(menuItem.permissionName);
        // }
        return true;
    }
}
