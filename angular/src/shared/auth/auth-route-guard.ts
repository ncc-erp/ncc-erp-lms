import { Injectable } from '@angular/core';
import { PermissionCheckerService } from '@abp/auth/permission-checker.service';
import { AppSessionService } from '../session/app-session.service';

import {
    CanActivate, Router,
    ActivatedRouteSnapshot,
    RouterStateSnapshot,
    CanActivateChild
} from '@angular/router';
import { PermissonConstants } from '@app/models/constant';

@Injectable()
export class AppRouteGuard implements CanActivate, CanActivateChild {

    constructor(
        private _permissionChecker: PermissionCheckerService,
        private _router: Router,
        private _sessionService: AppSessionService,
    ) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        if (!this._sessionService.user) {
            this._router.navigate(['/account/login'], { queryParams: { "returnUrl": state.url } });
            return false;
        }

        if (!route.data || !route.data["permission"]) {
            return true;
        }

        if (this._permissionChecker.isGranted(route.data["permission"])) {
            return true;
        }
        this._router.navigate([this.selectBestRoute()]);
        return false;
    }

    canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        return this.canActivate(route, state);
    }

    selectBestRoute(): string {
        if (!this._sessionService.user) {
            return '/account/login';
        }
        else if (this._permissionChecker.isGranted(PermissonConstants.Pages_Tenants)) {
            return '/app/root-admin/tenants';
        }
        else if (this._permissionChecker.isGranted(PermissonConstants.Pages_Users)) {
            return '/app/courses'
        }
        else if (this._permissionChecker.isGranted(PermissonConstants.Pages_Courses)) {
            return '/app/courses'
        }
        else {
            const studentDefaultView = parseInt(abp.setting.get('Tenant.StudentDefaultView'));
            if (studentDefaultView === 0) {
                return '/app/student/dashboard';
            } else if (studentDefaultView === 1) {
                return '/app/student/courses';
            } else if (studentDefaultView === 2) {
                return '/app/student/calendar';
            }
        }
    }
}
