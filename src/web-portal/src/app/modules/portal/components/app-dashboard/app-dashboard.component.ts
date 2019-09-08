import { Component, OnInit } from '@angular/core';
import { AppsClient, App } from 'services/portal.service';
import { Observable, of } from 'rxjs';
import { SessionService } from 'services/session.service';
import { Router } from '@angular/router';
import { Store } from '@ngxs/store';
import { UserSelectAppAction } from 'stores/apps/app.action';
import * as _ from 'lodash';
import { mergeMap } from 'rxjs/operators';
import { MatProgressButtonOptions } from 'mat-progress-buttons';

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
        private sessionService: SessionService,
        private router: Router,
        private store: Store
    ) { }

    ngOnInit(): void {
        this.loadingApps$ = this.appsClient.getMany("5c162e9005924c1c741bfdc2").pipe(
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
        );
    }

    onSelectingApp(app: { app: App, loading: boolean, btnOption: MatProgressButtonOptions }) {
        app.btnOption.active = true
        this.sessionService.setCurrentApp(app.app);
        this.store.dispatch(new UserSelectAppAction(app.app))
        this.router.navigateByUrl(app.app.defaultUrl);
    }
}
