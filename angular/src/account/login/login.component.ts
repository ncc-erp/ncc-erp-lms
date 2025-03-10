import { AbpSessionService } from '@abp/session/abp-session.service';
import { Component, ElementRef, Injector, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BaseService } from '@app/services/base-service/base.service';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { AppConsts } from '@shared/AppConsts';
import { AppTenantAvailabilityState } from '@shared/AppEnums';
import { IsTenantAvailableInput, IsTenantAvailableOutput } from '@shared/service-proxies/service-proxies';
import { SocialAuthService, SocialUser } from 'angularx-social-login';
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
    loggedIn: boolean;
    returnUrl: string;

    captchaSuccess: boolean = false;
    showCaptcha: boolean = false;

    constructor(
        injector: Injector,
        public loginService: LoginService,
        private _router: Router,
        private _sessionService: AbpSessionService,
        private baseService: BaseService,
        private authService: SocialAuthService,
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
                // if (user) {
                //     this.loginService.authenticateGoogle(user.idToken, this.tenancyName, this.returnUrl);
                // }
            }, err => this.authService.signOut());
        })
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



    login(): void {
        this.submitting = true;
        const input = new IsTenantAvailableInput();
        input.tenancyName = this.tenancyName;

        // this.showCaptcha = this.loginService.checkReCaptcha(true);
        // if (this.showCaptcha) {
        //     if (!this.captchaSuccess) { return } else { this.childReCaptcha.reset(); }
        // }
        if (this.tenancyName != null && this.tenancyName !== '') {
            this.baseService.accountService.isTenantAvailable(input)
                .subscribe((result: IsTenantAvailableOutput) => {
                    switch (result.state) {
                        case AppTenantAvailabilityState.Available:
                            abp.multiTenancy.setTenantIdCookie(result.tenantId);
                            this.loginService.authenticate(this.tenancyName, this.returnUrl,
                                () => this.submitting = false
                            );
                            return;
                        case AppTenantAvailabilityState.InActive:
                            this.message.warn(this.l('TenantIsNotActive', this.tenancyName));
                            break;
                        case AppTenantAvailabilityState.NotFound: // NotFound
                            this.message.warn(this.l('ThereIsNoTenantDefinedWithName{0}', this.tenancyName));
                            break;
                    }
                });
        } else {
            abp.multiTenancy.setTenantIdCookie(undefined);
            this.loginService.authenticate(this.tenancyName, this.returnUrl,
                () => this.submitting = false
            );
        }
    }
    // @ts-ignore
    signInWithMezon() {
        const authServerUrl = AppConsts.mezonAuthServerUrl;
        const state = Date.now().toString()
        const scope = 'openid offline';
        const responseType = 'code';
        const searchParams = new URLSearchParams()

        searchParams.set('client_id', AppConsts.mezonClientId)
        searchParams.set('redirect_uri', AppConsts.redirectUri)
        searchParams.set('response_type', responseType)
        searchParams.set('scope', scope)
        searchParams.set('state', state)

        const url = `${authServerUrl}/oauth2/auth?${searchParams.toString()}`
        window.location.href = url;
    }

}

