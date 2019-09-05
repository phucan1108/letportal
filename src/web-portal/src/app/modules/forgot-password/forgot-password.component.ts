import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountsClient } from 'services/identity.service';
import { NGXLogger } from 'ngx-logger';
import { SessionService } from 'services/session.service';
import { SecurityService } from 'app/core/security/security.service';
import { CustomValidators } from 'ngx-custom-validators';

@Component({
    selector: 'let-forgot-password',
    templateUrl: './forgot-password.component.html',
    styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent implements OnInit {

    forgotPasswordForm: FormGroup
    errorMessage = ''
    isComplete = false

    constructor(private fb: FormBuilder,
        private router: Router,
        private activatedRouter: ActivatedRoute,
        private accountClient: AccountsClient,
        private logger: NGXLogger) { }

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

    moveToLogin(){
        this.router.navigateByUrl('/login')
    }
}
