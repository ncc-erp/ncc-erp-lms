import { Component, Injector, OnInit, ViewContainerRef, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { AppAuthService } from '@shared/auth/app-auth.service';

@Component({
    templateUrl: './account.component.html',
    styleUrls: [
        './account.component.less'
    ],
    encapsulation: ViewEncapsulation.None
})
export class AccountComponent extends AppComponentBase implements OnInit {

    private viewContainerRef: ViewContainerRef;

    // versionText: string;
    // currentYear: number;

    public constructor(
        injector: Injector,
        private _appAuthService: AppAuthService
    ) {
        super(injector);

        this._appAuthService.ping();
        this._appAuthService.sendBotId();
    
        this._appAuthService.listenToPong();
        this._appAuthService.listenToUserHashInfo();
        this._appAuthService.listenToCurrentUserInfo();
        // this.currentYear = new Date().getFullYear();
        // this.versionText = this.appSession.application.version + ' [' + this.appSession.application.releaseDate.format('YYYYDDMM') + ']';
    }

    showTenantChange(): boolean {
        return abp.multiTenancy.isEnabled;
    }

    ngOnInit(): void {
        $('body').attr('class', 'login-page');
    }
}