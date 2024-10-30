import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { RouterPermissonComponent } from '@shared/router-permisson/router-permisson.component';

const routes: Routes = [
  { path: '', component: RouterPermissonComponent, pathMatch: 'full' },
  {
    path: 'account',
    loadChildren: 'account/account.module#AccountModule', //Lazy load account module
    data: { preload: true }
  },
  {
    path: 'app',
    loadChildren: 'app/app.module#AppModule', //Lazy load account module
    data: { preload: true }
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: []
})
export class RootRoutingModule { }
