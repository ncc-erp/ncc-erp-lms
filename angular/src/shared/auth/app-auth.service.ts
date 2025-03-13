import { Injectable } from '@angular/core';
import { ReportService } from '@app/services/systems-admin-services/report.service';
import { AppConsts } from '@shared/AppConsts';
import { Subject } from 'rxjs';
import { MezonAppEvent, MezonWebViewEvent } from 'types/webview';

@Injectable()
export class AppAuthService {
    private userHashInfo = new Subject<any>();
    private currentUserInfo = new Subject<any>();
    private isInMezon = new Subject<boolean>();

    userHashInfo$ = this.userHashInfo.asObservable();
    currentUserInfo$ = this.currentUserInfo.asObservable();
    isInMezon$ = this.isInMezon.asObservable();

    constructor(private _reportService: ReportService) { }

    ping() {
        window.Mezon.WebView.postEvent("PING" as MezonWebViewEvent, { message: "PING" }, () => { })
    }

    listenToPong() {
        window.Mezon.WebView.onEvent("PONG" as MezonAppEvent, () => {
            this.isInMezon.next(true);
        });
    }

    sendBotId() {
        window.Mezon.WebView.postEvent("SEND_BOT_ID" as MezonWebViewEvent, { appId: AppConsts.mezonAppId }, () => { })
    }

    listenToUserHashInfo() {
        window.Mezon.WebView.onEvent("USER_HASH_INFO" as MezonAppEvent, async (_, userHashData: any) => {
            this.userHashInfo.next(userHashData.message);
        });
    }

    listenToCurrentUserInfo() {
        window.Mezon.WebView.onEvent("CURRENT_USER_INFO" as MezonAppEvent, async (_, userData: any) => {
            if (!userData || !userData.user) {
                return;
            }
            const mezonUser = {
                email: userData.email,
                mezon_id: userData.mezon_id,
                user: {
                    avatar_url: userData.user.avatar_url,
                    display_name: userData.user.display_name,
                    id: userData.user.id,
                    username: userData.user.username,
                },
                wallet: userData.wallet,
            };
            this.currentUserInfo.next(mezonUser);
        });
    }

    removeEventListeners() {
        window.Mezon.WebView.offEvent("CURRENT_USER_INFO" as MezonAppEvent, () => { })
        window.Mezon.WebView.offEvent("USER_HASH_INFO" as MezonAppEvent, () => { })
    }

    logout(reload?: boolean): void {
        // Add Log out to table AbpUserLoginAttempts
        this._reportService.CreateUserLogoutInfo().subscribe();

        abp.auth.clearToken();
        if (reload !== false) {
            location.href = AppConsts.appBaseUrl;
        }
    }
}
