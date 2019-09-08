import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginPage } from './pages/login/login.page';
import { ForgotPasswordPage } from './pages/forgotpassword/forgot-password.page';
import { LoginRoutingModule } from './login-routing.module';
import { ResetPasswordPage } from './pages/resetpassword/reset-password.page';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatFormFieldModule, MatInputModule, MatCardModule, MatButtonModule, MatCheckboxModule, MatIconModule, MatToolbarModule } from '@angular/material';

@NgModule({
    declarations: [
        LoginPage,
        ForgotPasswordPage,
        ResetPasswordPage
    ],
    imports: [
        CommonModule,
        LoginRoutingModule,
        MatFormFieldModule,
        MatInputModule,
        MatCardModule,
        MatButtonModule,
        MatCheckboxModule,
        MatIconModule,
        MatToolbarModule,
        FormsModule,
        ReactiveFormsModule,],
    exports: [],
    providers: [],
})
export class LoginModule { }