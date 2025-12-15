import { Injectable } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ReportService } from '@app/services/systems-admin-services/report.service';
import { AppConsts } from '@shared/AppConsts';
import { Subject, Subscription } from 'rxjs';
@Injectable()
export class AppAuthService {
    private userHashData = new Subject<string>();
    private isInMezon = new Subject<boolean>();

    userHashData$ = this.userHashData.asObservable();
    isInMezon$ = this.isInMezon.asObservable();

    constructor(
        private _reportService: ReportService,
    ) { }
    logout(reload?: boolean): void {
        // Add Log out to table AbpUserLoginAttempts
        this._reportService.CreateUserLogoutInfo().subscribe();

        abp.auth.clearToken();
        if (reload !== false) {
            location.href = AppConsts.appBaseUrl;
        }
    }
}
