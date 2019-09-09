import { Component, OnInit } from '@angular/core';
import { AppsClient, App } from 'services/portal.service';
import { Observable, of } from 'rxjs';
import { SessionService } from 'services/session.service';
import { Router } from '@angular/router';
import { Store } from '@ngxs/store';
import { UserSelectAppAction } from 'stores/apps/app.action';
import * as _ from 'lodash';
import { mergeMap, tap } from 'rxjs/operators';
import { MatProgressButtonOptions } from 'mat-progress-buttons';
import { SecurityService } from 'app/core/security/security.service';
import { AuthUser } from 'app/core/security/auth.model';
import { NGXLogger } from 'ngx-logger';

@Component({
    selector: 'let-app-dashboard',
    templateUrl: './app-dashboard.component.html',
    styleUrls: ['./app-dashboard.component.scss']
})
export class AppDashboardComponent implements OnInit {

    apps$: Observable<Array<App>>

    loadingApps$: Observable<{ app: App, loading: boolean, btnOption: MatProgressButtonOptions }[]>
    constructor(
        private appsClient: AppsClient,
        private security: SecurityService,
        private sessionService: SessionService,
        private router: Router,
        private store: Store,
        private logger: NGXLogger
    ) { }

    ngOnInit(): void {
        this.security.getPortalClaims().pipe(
            tap(
                res => {
                    if(res){
                        this.loadingApps$ = this.appsClient.getMany(
                            this.getAvailableAppIds(this.security.getAuthUser())
                        ).pipe(
                            mergeMap(
                                apps => {
                                    let loadingApps: { app: App, loading: boolean, btnOption: MatProgressButtonOptions }[] = []
                                    _.forEach(apps, app => {
                                        loadingApps.push({
                                            app: app,
                                            loading: false,
                                            btnOption: {
                                                active: false,
                                                text: 'Enter',
                                                buttonColor: 'primary',
                                                barColor: 'primary',
                                                raised: false,
                                                stroked: false,
                                                fab: false,
                                                mode: 'indeterminate',
                                                disabled: false,
                                            }
                                        })
                                    })
                
                                    return of(loadingApps)
                                }
                            )
                        )
                    }  
                }
            )
        ).subscribe()        
    }

    onSelectingApp(app: { app: App, loading: boolean, btnOption: MatProgressButtonOptions }) {
        app.btnOption.active = true
        this.sessionService.setCurrentApp(app.app);
        this.store.dispatch(new UserSelectAppAction(app.app))
        this.router.navigateByUrl(app.app.defaultUrl);
    }

    private getAvailableAppIds(user: AuthUser){
        let ids = ''

        const apps = user.claims.find(a => a.name === 'apps')
        apps.claims.forEach(element => {
            if(apps.claims.indexOf(element) === apps.claims.length - 1){
                ids += element
            }
            else{
                ids += element + ';'
            }            
        });
        return ids
    }
}
