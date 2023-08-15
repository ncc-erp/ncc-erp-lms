import { Injector, ElementRef } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { LocalizationService } from '@abp/localization/localization.service';
import { PermissionCheckerService } from '@abp/auth/permission-checker.service';
import { FeatureCheckerService } from '@abp/features/feature-checker.service';
import { NotifyService } from '@abp/notify/notify.service';
import { SettingService } from '@abp/settings/setting.service';
import { MessageService } from '@abp/message/message.service';
import { AbpMultiTenancyService } from '@abp/multi-tenancy/abp-multi-tenancy.service';
import { AppSessionService } from '@shared/session/app-session.service';
import { DateTimePipe } from './pipes/date-local-utc.pipe';
import * as moment from 'moment';

export abstract class AppComponentBase {

    localizationSourceName = AppConsts.localization.defaultLocalizationSourceName;

    localization: LocalizationService;
    permission: PermissionCheckerService;
    feature: FeatureCheckerService;
    notify: NotifyService;
    setting: SettingService;
    message: MessageService;
    multiTenancy: AbpMultiTenancyService;
    appSession: AppSessionService;
    elementRef: ElementRef;


    constructor(injector: Injector) {
        this.localization = injector.get(LocalizationService);
        this.permission = injector.get(PermissionCheckerService);
        this.feature = injector.get(FeatureCheckerService);
        this.notify = injector.get(NotifyService);
        this.setting = injector.get(SettingService);
        this.message = injector.get(MessageService);
        this.multiTenancy = injector.get(AbpMultiTenancyService);
        this.appSession = injector.get(AppSessionService);
        this.elementRef = injector.get(ElementRef);
    }

    l(key: string, ...args: any[]): string {
        let localizedText = this.localization.localize(key, this.localizationSourceName);

        if (!localizedText) {
            localizedText = key;
        }

        if (!args || !args.length) {
            return localizedText;
        }

        args.unshift(localizedText);
        return abp.utils.formatString.apply(this, args);
    }

    isGranted(permissionName: string): boolean {
        return this.permission.isGranted(permissionName);
    }

    public getImageServerPath(imageCover: string) {
        return AppConsts.remoteServiceBaseUrl + '/' + imageCover;
    }
    protected log(msg: any) {
        //console.log(msg);
    }

    public readonly tinymceApiKey = 'f9hmujxljs7c65gsqy9gb5mbd0xku792rf9v5fpjoz6gtko5';

    public getSession(name: string) {
        return JSON.parse(sessionStorage.getItem(name));
    }
    public setSession(name: string, value: any): void {
        sessionStorage.setItem(name, JSON.stringify(value))
    }

    // Return date of local to show data from dateUTC
    public getDateLocal(dateUTC: any): Date {
        if (!dateUTC) { return null; }
        const timezone = DateTimePipe.timezone;
        if (timezone) {
            const newDate = new Date(dateUTC);
            const timeLocalOffset = DateTimePipe.timezoneSecon * 1000;
            const timezoneOffset = (-1) * newDate.getTimezoneOffset() * 60 * 1000;
            return new Date(newDate.getTime() + timezoneOffset + timeLocalOffset);
        } else {
            return dateUTC;
        }
    }

    // Return dateUTC from local date
    public getDateUTC(dateLocal: any): Date {
        if (!dateLocal) { return null; }
        const timezone = DateTimePipe.timezone;
        if (timezone) {
            const newDate = new Date(dateLocal);
            const timeLocalOffset = DateTimePipe.timezoneSecon * 1000;
            const timezoneOffset = (-1) * newDate.getTimezoneOffset() * 60 * 1000;
            const result = new Date(newDate.getTime() + timezoneOffset - timeLocalOffset);
            return result;
        } else {
            return dateLocal;
        }
    }
    public logStudentProcessToSentry(message:string, args:any){
        if(typeof window['postSentryLog'] == 'function'){
            let user = this.appSession.user
            window['postSentryLog'](`(${moment(new Date()).utcOffset(+7).format("YYYY/MM/DD hh:mm A")})  ${user.userName}: ${message}`, args)
       }
    }
    public formatTime(date: Date){
        return moment(date).format("DD/MM/YYYY hh:mm").toString()
    }
}
