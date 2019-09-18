import { Component, OnInit, HostListener } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountsClient } from 'services/identity.service';
import { CustomValidators } from 'ngx-custom-validators';

@Component({
    selector: 'let-forgot-password',
    templateUrl: './forgot-password.page.html',
    styleUrls: ['./forgot-password.page.scss']
})
export class ForgotPasswordPage implements OnInit {

    forgotPasswordForm: FormGroup
    errorMessage = ''
    isComplete = false

    constructor(private fb: FormBuilder,
        private router: Router,
        private accountClient: AccountsClient) { }

    ngOnInit(): void {
        this.forgotPasswordForm = this.fb.group({
            email: ['', [Validators.required, CustomValidators.email]]
        })
    }

    forgotPassword(){
        if(!this.forgotPasswordForm.invalid){
            this.accountClient.forgotPassword({ email: this.forgotPasswordForm.value.email })
                .subscribe(
                    result => {
                        this.isComplete = true
                        this.errorMessage = ''
                    },
                    err => {
                        this.errorMessage = err.messageContent
                    }
                )
        }
    }

    @HostListener('window:keydown.enter',  ['$event'])
    handleEnterPress(event: KeyboardEvent){
        this.forgotPassword()
    }

    moveToLogin(){
        this.router.navigateByUrl('/')
    }
}
