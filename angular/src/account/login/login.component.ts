import { Component, Injector, ElementRef, ViewChild, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { LoginService } from './login.service';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AbpSessionService } from '@abp/session/abp-session.service';
import { BaseService } from '@app/services/base-service/base.service';
import { IsTenantAvailableInput, IsTenantAvailableOutput } from '@shared/service-proxies/service-proxies';
import { AppTenantAvailabilityState } from '@shared/AppEnums';
import { ReCaptcha2Component } from './re-captcha2.component';
import { GoogleLoginProvider, SocialAuthService, SocialUser } from 'angularx-social-login';

@Component({
    templateUrl: './login.component.html',
    styleUrls: [
        './login.component.less'
    ],
    animations: [accountModuleAnimation()]
})
export class LoginComponent extends AppComponentBase {

    @ViewChild('cardBody') cardBody: ElementRef;
    @ViewChild(ReCaptcha2Component) childReCaptcha: ReCaptcha2Component;

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
        this.showCaptcha = this.loginService.checkReCaptcha();
        if (this.appSession.tenant) {
            this.tenancyName = this.appSession.tenant.tenancyName;
            this.name = this.appSession.tenant.name;
        }
        this.authService.authState.subscribe((user) => {
            this.authService.authState.subscribe((user) => {
                if (user) {
                    this.loginService.authenticateGoogle(user.idToken, this.tenancyName, this.returnUrl);
                }
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
    signInWithGoogle() {
        this.authService.signIn(GoogleLoginProvider.PROVIDER_ID);
    }

}

