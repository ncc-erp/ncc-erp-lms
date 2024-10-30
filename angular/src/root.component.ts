import { Component, OnInit } from '@angular/core';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { PermissionCheckerService } from 'abp-ng2-module/dist/src/auth/permission-checker.service';
import { PermissonConstants } from '@app/models/constant';
import { Router } from '@angular/router';
import { AppSessionService } from '@shared/session/app-session.service';
@Component({
  selector: 'app-root',
  template: `
    <ngx-loading-bar></ngx-loading-bar>
    <router-outlet></router-outlet>
  `
})
export class RootComponent {
  private isLoading: boolean = false;
  private count: number = 0;
  constructor(private loadingBar: LoadingBarService) {
    loadingBar.progress$.subscribe(progress => {
      if (progress == 100 || progress == 0) {
        this.isLoading = false;
      } else {
        this.isLoading = true;
      }
    });
  }
}
