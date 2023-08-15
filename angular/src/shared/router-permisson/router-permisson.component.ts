import { Component, OnInit, Injector } from '@angular/core';
import { PermissionCheckerService } from 'abp-ng2-module/dist/src/auth/permission-checker.service';
import { Router } from '@angular/router';
import { AppSessionService } from '@shared/session/app-session.service';
import { PermissonConstants } from '@app/models/constant';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-router-permisson',
  templateUrl: './router-permisson.component.html',
  styleUrls: ['./router-permisson.component.scss']
})
export class RouterPermissonComponent extends AppComponentBase implements OnInit {

  constructor(
    private injector: Injector,
    private _permissionChecker: PermissionCheckerService,
    private _router: Router,
    private _sessionService: AppSessionService
  ) { super(injector) }

  ngOnInit() {
    this._router.navigate([this.selectBestRoute()]);
  }
  selectBestRoute(): string {
    if (!this._sessionService.user) {
      return '/account/login';
    }
    else if (this._permissionChecker.isGranted(PermissonConstants.Pages_Tenants)) {
      return '/app/root-admin/tenants';
    }
    else if (this._permissionChecker.isGranted('Pages.Settings')) {
      return '/app/courses';
    } else if (this._permissionChecker.isGranted('Pages.Course')) {
      return '/app/courses';
    } else {
      const studentDefaultView = parseInt(abp.setting.get('Tenant.StudentDefaultView'));
      if (studentDefaultView === 0) {
        return '/app/student/courses';
      } else if (studentDefaultView === 1) {
        return '/app/student/dashboard';
      } else if (studentDefaultView === 2) {
        return '/app/student/calendar';
      }
    }
  }
}
