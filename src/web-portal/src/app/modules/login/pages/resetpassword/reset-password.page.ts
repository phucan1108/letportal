import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountsClient } from 'services/identity.service';
import { NGXLogger } from 'ngx-logger';

@Component({
    selector: 'let-reset-password',
    templateUrl: './reset-password.page.html',
    styleUrls: ['./reset-password.page.scss']
})
export class ResetPasswordPage implements OnInit {

    resetPasswordForm: FormGroup
    errorMessage = ''
    isRecoveryMode = false
    isComplete = false

    private user = ''
    private code = ''

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private activatedRouter: ActivatedRoute,
        private accountClient: AccountsClient,
        private logger: NGXLogger
    ) {

    }

    ngOnInit(): void {
        this.resetPasswordForm = this.fb.group({
            newPassword: ['', Validators.required]
        })

        this.activatedRouter.queryParamMap.subscribe(queryParams => {
            this.user = queryParams.get('user')
            this.code = queryParams.get('code')
            if(this.user && this.code)
            {
                this.isRecoveryMode = true
            }
        })
    }

    resetPassword(){
        if(!this.resetPasswordForm.invalid){
            this.accountClient.recoveryPassword({
                newPassword: this.resetPasswordForm.value.newPassword,
                userId: this.user,
                validateCode: this.code
            }).subscribe(
                result => {
                    this.isComplete = true
                },
                err => {
                    this.errorMessage = err.messageContent
                }
            )
        }
    }

    moveToLogin(){
        this.router.navigateByUrl('/')
    }
}
