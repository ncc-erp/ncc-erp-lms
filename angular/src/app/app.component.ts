import { Component, ViewContainerRef, Injector, OnInit, AfterViewInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';

import { SignalRAspNetCoreHelper } from '@shared/helpers/SignalRAspNetCoreHelper';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { BaseService } from './services/base-service/base.service';

@Component({
    templateUrl: './app.component.html'
})
export class AppComponent extends AppComponentBase implements OnInit, AfterViewInit, OnDestroy {


    private viewContainerRef: ViewContainerRef;
    isShow: boolean = false;
    isBrowsing: boolean = false;
    constructor(
        injector: Injector,
        private _router: Router,
        private _base: BaseService
    ) {
        super(injector);

    }

    ngOnInit(): void {
        this.onCheckCourse();
        this.onCheckBrowsing();
        this._base._studentService.isNavbar$.subscribe(e => {
            if (e) {
                $('section').addClass('content');
                this.onCheckCourse();
                this.onCheckBrowsing();
                $.AdminBSB.activateAll();
                $.AdminBSB.activateDemo();
            }
        })
    }

    onCheckBrowsing() {
        if (this._router.url.indexOf('browsing') > 0 || this._router.url.indexOf('scorm') > 0) {
            this.isBrowsing = $.AdminBSB.isCourse = true;
            $('section.content').removeClass('content');
        }
        this._router.events
            .pipe(filter(e => e instanceof NavigationEnd))
            .subscribe((e: NavigationEnd) => {
                if (e.url.indexOf('browsing') > 0) {
                    this.isBrowsing = $.AdminBSB.isCourse = true;
                    $('section.content').removeClass('content');
                }
            });
    }

    onCheckCourse() {
        // if (this._router.url.indexOf('systems-admin/course') > 0) {
        //     this.isBrowsing = $.AdminBSB.isCourse = true;
        // }
        // this._router.events
        //     .pipe(filter(e => e instanceof NavigationEnd))
        //     .subscribe((e: NavigationEnd) => {
        //         if (e.url.indexOf('systems-admin/course') > 0) {
        //             $.AdminBSB.isCourse = true;
        //         }
        //         else {
        //             this.isBrowsing = false;
        //         }
        //     });

    }

    ngAfterViewInit(): void {
        $.AdminBSB.activateAll();
        $.AdminBSB.activateDemo();
    }

    onResize(event) {
        // exported from $.AdminBSB.activateAll
        $.AdminBSB.leftSideBar.setMenuHeight();
        $.AdminBSB.leftSideBar.checkStatuForResize(false);

        // exported from $.AdminBSB.activateDemo
        $.AdminBSB.demo.setSkinListHeightAndScroll();
        $.AdminBSB.demo.setSettingListHeightAndScroll();
    }

    ngOnDestroy(): void {

    }
}
