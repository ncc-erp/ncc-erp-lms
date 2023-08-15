import { Injectable } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { ReportService } from '@app/services/systems-admin-services/report.service';

@Injectable()
export class AppAuthService {
    constructor(private _reportService: ReportService) { }

    logout(reload?: boolean): void {
        // Add Log out to table AbpUserLoginAttempts
        this._reportService.CreateUserLogoutInfo().subscribe();

        abp.auth.clearToken();
        if (reload !== false) {
            location.href = AppConsts.appBaseUrl;
        }
    }
}
