import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NGXLogger } from 'ngx-logger';
import { SecurityService } from '../security.service';
import { AccountsClient } from 'services/identity.service';
import { environment } from 'environments/environment';
import { AuthToken } from '../auth.model';
import { SessionService } from 'services/session.service';
import { Router, NavigationEnd } from '@angular/router';
import { Location } from '@angular/common';

@Component({
    selector: 'let-unlock-screen',
    templateUrl: './unlock-screen.component.html'
})
export class UnlockScreenDialogComponent implements OnInit {

    username: string
    errorMessage: string
    confirmPasswordForm: FormGroup

    constructor(
        public dialogRef: MatDialogRef<any>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private security: SecurityService,
        private accountClient: AccountsClient,
        private session: SessionService,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef,
        private logger: NGXLogger,
        private router: Router,
        private location: Location
    ) { }

    ngOnInit(): void {
        // Ensure we don't be forced back to Login page
        if(!!this.security.getAuthUser()){
            this.username = this.security.getAuthUser().username
            this.confirmPasswordForm = this.fb.group({
                password: ['', [Validators.required, Validators.minLength(6)]]
            })
        }
        else{
            this.dialogRef.close()
        }

        this.router.events.subscribe(event => {
            if (event instanceof NavigationEnd) {
                // if user redirects to home page, close this dialog
                if(this.location.path() === ''){
                    this.dialogRef.close()
                }
            }
        })
    }

    onUnlock() {
        if (this.confirmPasswordForm.valid) {
            const formValues = this.confirmPasswordForm.value
            this.accountClient.login({
                username: this.username,
                password: formValues.password,
                softwareAgent: navigator.userAgent,
                versionInstalled: environment.version
            }).subscribe(
                result => {
                    this.security
                        .setAuthUser(
                            new AuthToken(result.token, result.exp, result.refreshToken, result.expRefresh))

                    this.session.setUserSession(result.userSessionId)
                    this.dialogRef.close(true)
                },
                err => {
                    this.errorMessage = err.messageContent
                    this.confirmPasswordForm.get('password').setErrors({
                        wrongpassword: true,
                    })
                }
            )
        }
    }
}
