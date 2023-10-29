import { ChangeDetectorRef, Component, HostListener, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AuthToken } from 'app/core/security/auth.model';
import { SecurityService } from 'app/core/security/security.service';
import { ObjectUtils } from 'app/core/utils/object-util';
import { environment } from 'environments/environment';
import * as moment from 'moment';
import { NGXLogger } from 'ngx-logger';
import { AccountsClient, RolesClient } from 'services/identity.service';
import { SessionService } from 'services/session.service';
@Component({
    selector: 'let-login',
    templateUrl: './login.page.html',
    styleUrls: ['./login.page.scss']
})
export class LoginPage implements OnInit {

    loginForm: FormGroup
    errorMessage = ''
    versionText = environment.version
    languages = environment.localization.allowedLanguages
    selectedLanguage: string = environment.localization.defaultLanguage
    enableSwitchLanguage = environment.localization.allowSwitchLanguage
    constructor(
        private translate: TranslateService,
        private fb: FormBuilder,
        private router: Router,
        private accountClient: AccountsClient,
        private logger: NGXLogger,
        private session: SessionService,
        private security: SecurityService,        
        private roleClient: RolesClient,
        private cd: ChangeDetectorRef
    ) { }

    ngOnInit(): void {        
        // Ensure user will be signed out when be back to login page
        this.session.clear()
        this.security.userLogout()
        this.loginForm = this.fb.group({
            username: ['', Validators.required],
            password: ['', Validators.required],
            rememberMe: [false],
            language: [this.translate.getDefaultLang(), Validators.required]
        })

        this.loginForm.get('language').valueChanges.subscribe(newValue => {
            this.selectedLanguage = newValue
            this.translate.use(newValue)
        })
    }

    @HostListener('window:keydown.enter',  ['$event'])
    handleEnterPress(event: KeyboardEvent){
        this.signIn()
    }

    signIn(){
        if(!this.loginForm.invalid){
            const formValues = this.loginForm.value;

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
                        localStorage.setItem('lang', this.selectedLanguage)
                        moment.locale(this.selectedLanguage)
                        this.router.navigateByUrl('/portal/dashboard')
                    })
                },
                err => {
                    if(ObjectUtils.isNotNull(err.messageContent)){
                        this.errorMessage = err.messageContent
                    }
                    else{
                        this.translate.get('pages.login.errors.common').subscribe(
                            res => {
                                this.errorMessage = res
                            }
                        )
                    }
                }
            )
        }
    }

    moveToForgot(){
        this.router.navigateByUrl('/forgot-password')
    }
}
