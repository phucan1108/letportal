import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RouterExtService } from 'app/core/ext-service/routerext.service';
import { SessionService } from 'services/session.service';
import { SecurityService } from 'app/core/security/security.service';

@Component({
    selector: 'let-error',
    templateUrl: './error.component.html',
    styleUrls: ['./error.component.scss']
})
export class ErrorComponent implements OnInit {
    constructor(
        private router: Router,
        private routerEx: RouterExtService,
        private session: SessionService,
        private security: SecurityService
    ) { }

    ngOnInit(): void { }

    moveToHome(){
        const defaultPage = this.session.getDefaultAppPage()
        const userSignedIn = this.security.isUserSignedIn()
        if(defaultPage && userSignedIn){
            this.router.navigateByUrl(defaultPage)
        }
        else{
            this.router.navigateByUrl('/')
        }

    }
}
