import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { TokenAuthServiceProxy, AuthenticateModel, AuthenticateResultModel, ExternalLoginProviderInfoModel, ExternalAuthenticateModel, ExternalAuthenticateResultModel } from '@shared/service-proxies/service-proxies';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { AppConsts } from '@shared/AppConsts';

import { MessageService } from '@abp/message/message.service';
import { LogService } from '@abp/log/log.service';
import { TokenService } from '@abp/auth/token.service';
import { UtilsService } from '@abp/utils/utils.service';
import { finalize } from 'rxjs/operators';
import { AppSessionService } from '@shared/session/app-session.service';
import { PermissionCheckerService } from 'abp-ng2-module/dist/src/auth/permission-checker.service';
import { AppPreBootstrap } from 'AppPreBootstrap';
import { PermissonConstants } from '@app/models/constant';
import { GoogleLoginService } from '@app/services/google-login-service/google-login.service';

@Injectable()
export class LoginService {

    static readonly twoFactorRememberClientTokenName = 'TwoFactorRememberClientToken';

    authenticateModel: AuthenticateModel;
    authenticateResult: AuthenticateResultModel;

    rememberMe: boolean;

    constructor(
        private _tokenAuthService: TokenAuthServiceProxy,
        private _sessionService: AppSessionService,
        private _router: Router,
        private _permissionChecker: PermissionCheckerService,
        private _utilsService: UtilsService,
        private _messageService: MessageService,
        private _tokenService: TokenService,
        private _logService: LogService,
        private _googleLoginService: GoogleLoginService,
    ) {
        this.clear();
    }

    authenticate(tenancyName: string, redirectUrl: string, finallyCallback?: () => void): void {
        finallyCallback = finallyCallback || (() => { });

        this._tokenAuthService
            .authenticate(this.authenticateModel)
            .pipe(finalize(() => { finallyCallback() }))
            .subscribe((result: AuthenticateResultModel) => {
                this.processAuthenticateResult(result, tenancyName, redirectUrl);
            });
    }
    authenticateGoogle(googleToken: string, tenancyName: string, redirectUrl: string, finallyCallback?: () => void): void {
        finallyCallback = finallyCallback || (() => { });

        this._googleLoginService.googleAuthenticate(googleToken, tenancyName)
            .subscribe((result: any) => {
                this.processAuthenticateResult(result.result, tenancyName, redirectUrl);
            });
    }

    private processAuthenticateResult(authenticateResult: AuthenticateResultModel, tenancyName: string, redirectUrl: string) {
        this.authenticateResult = authenticateResult;

        if (authenticateResult.accessToken) {
            localStorage.setItem('tenancyName', tenancyName);
            //Successfully logged in
            // sessionStorage.setItem('reCaptcha', JSON.stringify(0));
            this.login(authenticateResult.accessToken, authenticateResult.encryptedAccessToken, authenticateResult.expireInSeconds, this.rememberMe, redirectUrl);

        } else {
            //Unexpected result!

            this._logService.warn('Unexpected authenticateResult!');
            this._router.navigate(['account/login']);
        }
    }

    private login(accessToken: string, encryptedAccessToken: string, expireInSeconds: number, rememberMe?: boolean, redirectUrl?: string): void {

        const tokenExpireDate = rememberMe ? (new Date(new Date().getTime() + 1000 * expireInSeconds)) : undefined;

        this._tokenService.setToken(
            accessToken,
            tokenExpireDate
        );

        this._utilsService.setCookieValue(
            AppConsts.authorization.encrptedAuthTokenName,
            encryptedAccessToken,
            tokenExpireDate,
            abp.appPath
        );

        var initialUrl = UrlHelper.initialUrl;
        if (initialUrl.indexOf('/login') > 0) {
            initialUrl = AppConsts.appBaseUrl;
        }
        if (redirectUrl != "") {
            initialUrl = `${AppConsts.appBaseUrl}${redirectUrl}`;
            location.href = initialUrl
        }

        AppPreBootstrap.getUserConfiguration(() => {
            let initialUrl = `${AppConsts.appBaseUrl}${this.selectBestRoute()}`;
            location.href = initialUrl;
        });

        location.href = initialUrl;
    }

    selectBestRoute(): string {

        if (this._permissionChecker.isGranted(PermissonConstants.Pages_Tenants)) {
            return '/app/root-admin/tenants';
        }
        else if (this._permissionChecker.isGranted('Pages.Settings')) {
            return '/app/courses';
        } else if (this._permissionChecker.isGranted('Pages.Course')) {
            return '/app/courses';
        } else {
            const studentDefaultView = parseInt(abp.setting.get('Tenant.StudentDefaultView'));
            if (studentDefaultView === 0) {
                return '/app/student/dashboard';
            } else if (studentDefaultView === 1) {
                return '/app/student/courses';
            } else if (studentDefaultView === 2) {
                return '/app/student/calendar';
            }
        }

    }

    private clear(): void {
        this.authenticateModel = new AuthenticateModel();
        this.authenticateModel.rememberClient = false;
        this.authenticateResult = null;
        this.rememberMe = false;
    }

    checkReCaptcha(add?: boolean): boolean {
        const countSetting: number = AppConsts.reCaptcha.loginCount;
        if (countSetting === 100) {
            return false;
        }
        let count: number = +sessionStorage.getItem('reCaptcha');

        if (count < countSetting - 1) {
            if (add) {
                count++;
                sessionStorage.setItem('reCaptcha', JSON.stringify(count));
            }
            return false;
        } else {
            return true;
        }
    }
}
