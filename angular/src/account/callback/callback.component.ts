import { AbpSessionService } from '@abp/session/abp-session.service';
import { Component, ElementRef, Injector, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { AppConsts } from '@shared/AppConsts';
import { LoginService } from 'account/login/login.service';
import { SocialUser } from 'angularx-social-login';

@Component({
    templateUrl: './callback.component.html',
    styleUrls: [
        './callback.component.less'
    ],
})
export class CallbackComponent extends AppComponentBase {

    @ViewChild('cardBody') cardBody: ElementRef;
    name: string;
    user: SocialUser;
    loggedIn: boolean;
    returnUrl: string;
    tenancyName: string;
    isAuthenFailed: boolean = false;
    COUNTDOWN_TIME = 10 // 10 seconds
    authCode: string;


    constructor(
        injector: Injector,
        private _router: Router,
        private _sessionService: AbpSessionService,
        public loginService: LoginService,
        private route: ActivatedRoute,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '';
        this.authCode = this.route.snapshot.queryParams['code'] || '';
        this.tenancyName = localStorage.getItem('tenancyName') ? localStorage.getItem('tenancyName') : 'NCC';
        this.mezonAuthenticate(this.authCode);
    }

    mezonAuthenticate(authCode: string): void {
        const redirectUri = AppConsts.redirectUri
        if (!authCode || !redirectUri) {
            this._router.navigate(['/account/login']);
            return;
        }
        this.loginService.authenticateMezon(authCode, redirectUri, this.tenancyName, (error) => {
            console.log("Error: ", error);
            this.isAuthenFailed = true;
            const intervalId = setInterval(() => {
                this.COUNTDOWN_TIME--;
                if (this.COUNTDOWN_TIME === 0) {
                    clearInterval(intervalId);
                    this._router.navigate(['/account/login']);
                }
            }
            , 1000);
        }
        );
    }

    get multiTenancySideIsTeanant(): boolean {
        return this._sessionService.tenantId > 0;
    }

    get isSelfRegistrationAllowed(): boolean {
        if (!this._sessionService.tenantId) {
            return false;
        }

        return true;
    }

}

