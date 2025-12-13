import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginPage } from './pages/login/login.page';
import { ForgotPasswordPage } from './pages/forgotpassword/forgot-password.page';
import { LoginRoutingModule } from './login-routing.module';
import { ResetPasswordPage } from './pages/resetpassword/reset-password.page';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { TranslateModule } from '@ngx-translate/core';
import { MatSelectModule } from '@angular/material/select';

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
        MatSelectModule,
        FormsModule,
        ReactiveFormsModule,
        TranslateModule
    ],
    exports: [],
    providers: [],
})
export class LoginModule { }