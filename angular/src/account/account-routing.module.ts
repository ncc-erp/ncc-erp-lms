import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AccountComponent } from './account.component';
import { CallbackComponent } from './callback/callback.component';
import { LoginComponent } from './login/login.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AccountComponent,
                children: [
                    { path: 'login', component: LoginComponent },
                ]
            },
            {path: 'login/callback', component: CallbackComponent}
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class AccountRoutingModule { }