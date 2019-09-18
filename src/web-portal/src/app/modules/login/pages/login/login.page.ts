import { Component, OnInit, HostListener } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AccountsClient, Role, RolesClient } from 'services/identity.service';
import { environment } from 'environments/environment';
import { NGXLogger } from 'ngx-logger';
import { SessionService } from 'services/session.service';
import { SecurityService } from 'app/core/security/security.service';
import { AuthToken } from 'app/core/security/auth.model';
import { Router } from '@angular/router';

@Component({
    selector: 'let-login',
    templateUrl: './login.page.html',
    styleUrls: ['./login.page.scss']
})
export class LoginPage implements OnInit {

    loginForm: FormGroup
    errorMessage = ''

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private accountClient: AccountsClient,
        private logger: NGXLogger,
        private session: SessionService,
        private security: SecurityService,
        private roleClient: RolesClient
    ) { }

    ngOnInit(): void { 

        // Ensure user will be signed out when be back to login page
        this.session.clear()
        this.security.userLogout()

        this.loginForm = this.fb.group({
            username: ['', Validators.required],
            password: ['', Validators.required],
            rememberMe: [false]
        })
    }

    @HostListener('window:keydown.enter',  ['$event'])
    handleEnterPress(event: KeyboardEvent){
        this.signIn()
    }

    signIn(){
        if(!this.loginForm.invalid){
            let formValues = this.loginForm.value;

            this.accountClient.login({
                username: formValues.username,
                password: formValues.password,
                softwareAgent: navigator.userAgent,
                versionInstalled: environment.version
            }).subscribe(
                result => {
                    this.security
                        .setAuthUser(
                            new AuthToken(result.token, result.exp, result.refreshToken, result.expRefresh))
                    
                    this.session.setUserSession(result.userSessionId)
                    this.roleClient.getPortalClaims().subscribe(result =>{
                        this.security.setPortalClaims(result)                        
                        this.router.navigateByUrl('/portal/dashboard')
                    })
                },
                err => {
                    this.errorMessage = err.messageContent
                }
            )
        }
    }

    moveToForgot(){
        this.router.navigateByUrl('/forgot-password')
    }
}
