import { CommonModule } from '@angular/common';
import { NgModule, ModuleWithProviders } from '@angular/core';
import { AbpModule } from '@abp/abp.module';
import { RouterModule } from '@angular/router';
import { AppSessionService } from './session/app-session.service';
import { AppUrlService } from './nav/app-url.service';
import { AppAuthService } from './auth/app-auth.service';
import { AppRouteGuard } from './auth/auth-route-guard';
import { MaterialInput } from 'shared/directives/material-input.directive';
import { AllMaterialModuleModule } from './all-material-module/all-material-module.module';
import { UploadComponent } from './upload/upload.component';
import { FilterComponent } from './filter/filter.component';
import { FormsModule } from '@angular/forms';
import { ViewCourseItemComponent } from './view-course-item/view-course-item.component';
import { ShortenStringPipe } from './directives/shorten-string.directive';
import { ViewQuestionComponent } from './view-question/view-question.component';
import { DndModule } from 'ngx-drag-drop';
import { InputOnlyNumberDirective } from './directives/input-only-number.directive';
import { NgxPaginationModule } from 'ngx-pagination';
import { ConvertHMTLPipe } from './pipes/convert-html.pipe';
import { ConvertDateTimePipe } from './pipes/convert-datetime.pipe';
import { FormatNumberPipe } from './pipes/format-number.pipe';
import { FilterPipe } from './pipes/filter.pipe';
import { FileAttachmentsComponent } from '@app/general/course-detail/file-attachments/file-attachments.component';;
import { EnterNumberDirective } from './directives/enter-number.directive';
import { RouterPermissonComponent } from './router-permisson/router-permisson.component'
import { DateUtcLocalPipe } from './pipes/date-local-utc.pipe';
import { MaskInputDirective } from './directives/mask-input.directive';
import { EditorModule } from '@tinymce/tinymce-angular';
import { AbpPaginationControlsComponent } from './pagination/abp-pagination-controls.conponent';;
import { BlockCopyPasteDirective } from './directives/block-copy-paste.directive';
@NgModule({
    imports: [
        CommonModule,
        AbpModule,
        FormsModule,
        RouterModule,
        AllMaterialModuleModule,
        DndModule,
        NgxPaginationModule,
        EditorModule
    ],
    declarations: [
        MaterialInput,
        UploadComponent,
        FilterComponent,
        ViewCourseItemComponent,
        ViewQuestionComponent,
        ShortenStringPipe,
        InputOnlyNumberDirective,
        FileAttachmentsComponent,
        ConvertHMTLPipe,
        ConvertDateTimePipe,
        FormatNumberPipe,
        FilterPipe,
        EnterNumberDirective,
        RouterPermissonComponent,
        DateUtcLocalPipe,
        MaskInputDirective,
        AbpPaginationControlsComponent,
        BlockCopyPasteDirective,
    ],
    exports: [
        MaterialInput,
        AllMaterialModuleModule,
        UploadComponent,
        FilterComponent,
        ViewCourseItemComponent,
        InputOnlyNumberDirective,
        ViewQuestionComponent,
        FileAttachmentsComponent,
        ConvertHMTLPipe,
        ConvertDateTimePipe,
        FormatNumberPipe,
        FilterPipe,
        EnterNumberDirective,
        DateUtcLocalPipe,
        MaskInputDirective,
        AbpPaginationControlsComponent,
        BlockCopyPasteDirective
    ]
})
export class SharedModule {
    static forRoot(): ModuleWithProviders {
        return {
            ngModule: SharedModule,
            providers: [
                AppSessionService,
                AppUrlService,
                AppAuthService,
                AppRouteGuard
            ]
        }
    }
}
