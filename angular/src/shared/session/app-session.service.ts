import { Injectable } from '@angular/core';
import {
    SessionServiceProxy,
    UserLoginInfoDto,
    TenantLoginInfoDto,
    ApplicationInfoDto,
    GetCurrentLoginInformationsOutput
} from '@shared/service-proxies/service-proxies'
import { AbpMultiTenancyService } from '@abp/multi-tenancy/abp-multi-tenancy.service'
import { DateTimePipe } from '@shared/pipes/date-local-utc.pipe';
import { Common } from '@shared/Common';
import { AppAuthService } from '@shared/auth/app-auth.service';

@Injectable()
export class AppSessionService {

    private _user: UserLoginInfoDto;
    private _tenant: TenantLoginInfoDto;
    private _application: ApplicationInfoDto;

    constructor(
        private _sessionService: SessionServiceProxy,
        private _abpMultiTenancyService: AbpMultiTenancyService,
        private _authService: AppAuthService
    ) {
    }

    get application(): ApplicationInfoDto {
        return this._application;
    }

    get user(): UserLoginInfoDto {
        return this._user;
    }

    get userId(): number {
        return this.user ? this.user.id : null;
    }

    get tenant(): TenantLoginInfoDto {
        return this._tenant;
    }

    get tenantId(): number {
        return this.tenant ? this.tenant.id : null;
    }

    getShownLoginName(): string {
        if(!this._user) {
            abp.notify.error("Người dùng chưa đăng nhập vào hệ thống!");
            setTimeout(() => {
                this._authService.logout(); 
            }, 2000);
        }
        let userName = this._user.userName;
        if (!this._abpMultiTenancyService.isEnabled) {
            return userName;
        }

        return (this._tenant ? this._tenant.tenancyName : ".") + "\\" + userName;
    }

    init(): Promise<boolean> {
        return new Promise<boolean>((resolve, reject) => {
            this._sessionService.getCurrentLoginInformations().toPromise().then((result: GetCurrentLoginInformationsOutput) => {
                this._application = result.application;
                this._user = result.user;
                this._tenant = result.tenant;
               if(this.user && this.user.roleName== 'Student' && typeof window['postSentryLog'] == 'function'){
                    window['postSentryLog'](`Student ${result.user.userName} Login or Refresh page`, this.user)
               }
                if (result.user && result.user.baseUtcOffset) {
                    const timezone = result.user.baseUtcOffset;
                    DateTimePipe.timezone = timezone
                    const hour = +timezone.substring(1, 3);
                    const minute = +timezone.substring(3, 5);
                    const ext = +(timezone.substring(0, 1) + '1');//1 or -1
                    DateTimePipe.timezoneSecon = (hour * 60 + minute) * ext * 60;                    
                }
                if (this._user){
                    Common.userRoleName = this._user.roleName;
                }                
                resolve(true);
            }, (err) => {
                reject(err);
            });
        });
    }

    changeTenantIfNeeded(tenantId?: number): boolean {
        if (this.isCurrentTenant(tenantId)) {
            return false;
        }

        abp.multiTenancy.setTenantIdCookie(tenantId);
        location.reload();
        return true;
    }

    private isCurrentTenant(tenantId?: number) {
        if (!tenantId && this.tenant) {
            return false;
        } else if (tenantId && (!this.tenant || this.tenant.id !== tenantId)) {
            return false;
        }

        return true;
    }
}
