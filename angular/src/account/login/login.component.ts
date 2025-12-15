import { AbpSessionService } from '@abp/session/abp-session.service';
import { Component, ElementRef, Injector, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { AppConsts } from '@shared/AppConsts';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { IHashMezonAuthModel } from '@shared/service-proxies/service-proxies';
import { SocialAuthService, SocialUser } from 'angularx-social-login';
import { Base64 } from 'js-base64';
import { LoginService } from './login.service';

@Component({
    templateUrl: './login.component.html',
    styleUrls: [
        './login.component.less'
    ],
    animations: [accountModuleAnimation()]
})
export class LoginComponent extends AppComponentBase {

    @ViewChild('cardBody') cardBody: ElementRef;

    submitting: boolean = false;
    versionText: string;
    currentYear: number;
    tenancyName: string;
    name: string;
    user: SocialUser;
    hashData: string;
    loggedIn: boolean;
    returnUrl: string;
    isMezonApp: boolean = false;
    isAuthenFailed: boolean = false;
    isAuthenticating: boolean = false;

    captchaSuccess: boolean = false;
    showCaptcha: boolean = false;

    constructor(
        injector: Injector,
        public loginService: LoginService,
        private _sessionService: AbpSessionService,
        private authService: SocialAuthService,
        private _appAuthService: AppAuthService,
        private route: ActivatedRoute,
    ) {
        super(injector);
        this.currentYear = new Date().getFullYear();
        this.versionText = this.appSession.application.version + ' [' + this.appSession.application.releaseDate.format('YYYYDDMM') + ']';

        this.tenancyName = localStorage.getItem('tenancyName') ? localStorage.getItem('tenancyName') : 'NCC';
    }
    ngOnInit(): void {
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '';
        if (this.appSession.tenant) {
            this.tenancyName = this.appSession.tenant.tenancyName;
            this.name = this.appSession.tenant.name;
        }
        this.authService.authState.subscribe((user) => {
            this.authService.authState.subscribe((user) => {
            }, err => this.authService.signOut());
        })

        this.route.queryParams.subscribe(params => {
            if (params['data']) {
                this.hashData = params['data'];
                this.isMezonApp = true;
                this.loginWithHash(this.hashData);
            }
        });
    }

    ngAfterViewInit(): void {
        $(this.cardBody.nativeElement).find('input:first').focus();
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

    loginWithHash(hashData: string) {
        if (hashData) {
            this.isAuthenticating = true;
            const hashAuthData: IHashMezonAuthModel = {
                hashData: Base64.encode(hashData),
                tenancyName: this.tenancyName
            }
            this.loginService.authenticateMezonHash(hashAuthData, (error) => {
                console.log("Error: ", error);
                this.isAuthenFailed = true;
            });
        }
    }

    retryHashLogin() {
        this.isAuthenticating = false;
        this.isAuthenFailed = false;
        this.loginWithHash(this.hashData);
    }

    // @ts-ignore
    loginWithMezon() {
        const authServerUrl = AppConsts.mezonAuthServerUrl;
        const state = Date.now().toString()
        const scope = 'openid offline';
        const responseType = 'code';

        const searchParams = new URLSearchParams();
        searchParams.append('client_id', AppConsts.mezonClientId);
        searchParams.append('redirect_uri', AppConsts.redirectUri);
        searchParams.append('response_type', responseType);
        searchParams.append('scope', scope);
        searchParams.append('state', state);
        
        const url = `${authServerUrl}/oauth2/auth?${searchParams.toString()}`
        window.location.href = url;
    }

}

