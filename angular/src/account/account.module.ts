import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { JsonpModule } from '@angular/http';

import { ModalModule } from 'ngx-bootstrap';

import { AbpModule } from '@abp/abp.module';

import { AccountRoutingModule } from './account-routing.module';

import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';

import { SharedModule } from '@shared/shared.module';

import { AccountComponent } from './account.component';
import { AccountLanguagesComponent } from './layout/account-languages.component';
import { LoginComponent } from './login/login.component';
import { LoginService } from './login/login.service';
import { TenantChangeModalComponent } from './tenant/tenant-change-modal.component';
import { TenantChangeComponent } from './tenant/tenant-change.component';

import { AppAuthService } from '@shared/auth/app-auth.service';
import { SocialAuthServiceConfig, SocialLoginModule } from 'angularx-social-login';
import { NgxCaptchaModule } from 'ngx-captcha'; // npm i ngx-captcha
import { CallbackComponent } from './callback/callback.component';
import { ReCaptcha2Component } from './login/re-captcha2.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        HttpClientModule,
        JsonpModule,
        AbpModule,
        SharedModule,
        ServiceProxyModule,
        AccountRoutingModule,
        ModalModule.forRoot(),
        ReactiveFormsModule,
        NgxCaptchaModule,
        SocialLoginModule
    ],
    declarations: [
        AccountComponent,
        TenantChangeComponent,
        TenantChangeModalComponent,
        LoginComponent,
        CallbackComponent,
        AccountLanguagesComponent,
        ReCaptcha2Component
    ],
    providers: [
        LoginService,
        AppAuthService,
        {
            provide: 'SocialAuthServiceConfig',
            useValue: {
                autoLogin: false,
                providers: [],
            } as SocialAuthServiceConfig,
        }
    ]
})
export class AccountModule {

}
