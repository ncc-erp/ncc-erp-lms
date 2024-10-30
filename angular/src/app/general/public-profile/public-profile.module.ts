import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PublicProfileComponent } from './public-profile.component';
import { AllMaterialModuleModule } from '@shared/all-material-module/all-material-module.module';
import { RouterModule } from '@angular/router';

@NgModule({
  imports: [
    CommonModule,
    AllMaterialModuleModule,
    RouterModule.forChild([
      {
        path: '',
        component: PublicProfileComponent
      }
    ])
  ],
  declarations: [PublicProfileComponent]
})
export class PublicProfileModule { }
