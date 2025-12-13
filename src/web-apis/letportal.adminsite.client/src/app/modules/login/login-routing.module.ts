import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginPage } from './pages/login/login.page';
import { ForgotPasswordPage } from './pages/forgotpassword/forgot-password.page';
import { ResetPasswordPage } from './pages/resetpassword/reset-password.page';

const routes: Routes = [
    {
        path: '',
        component: LoginPage,
    },
    {
        path: 'forgot-password',
        component: ForgotPasswordPage
    },
    {
        path: 'reset-password',
        component: ResetPasswordPage
    },
];
@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: [],
})
export class LoginRoutingModule { }