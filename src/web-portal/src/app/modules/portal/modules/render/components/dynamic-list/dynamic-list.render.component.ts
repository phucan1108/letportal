import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild, AfterViewInit, ChangeDetectionStrategy, Input, Output, AfterContentChecked, EventEmitter, AfterViewChecked } from '@angular/core';
import { MatDialog } from '@angular/material';
import { DynamicList, CommandButtonInList, CommandPositionType, EntitySchemasClient, DatabasesClient, ActionType, PageSection } from 'services/portal.service';
import { ActivatedRoute, Router, NavigationExtras } from '@angular/router';
import { Dictionary } from 'lodash';
import * as _ from 'lodash';
import { BehaviorSubject, Subscription, Observable } from 'rxjs';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { CommandClicked } from './models/commandClicked';
import { NGXLogger } from 'ngx-logger';
import { Translator } from 'app/core/shell/translates/translate.pipe';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { tap, filter, debounceTime } from 'rxjs/operators';
import { MessageType, ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { DynamicListGridComponent } from './components/dynamic-list.grid.component';
import { SecurityService } from 'app/core/security/security.service';
import { RouterExtService } from 'app/core/ext-service/routerext.service';
import { SessionService } from 'services/session.service';
import { Store } from '@ngxs/store';
import { PageStateModel } from 'stores/pages/page.state';
import { EndRenderingPageSectionsAction, BeginBuildingBoundData, AddSectionBoundData, GatherSectionValidations, SectionValidationStateAction } from 'stores/pages/page.actions';
import { ExtendedPageSection } from 'app/core/models/extended.models';
import { PageService } from 'services/page.service';
import { TriggeredControlEvent } from 'app/core/models/page.model';

@Component({
    selector: 'let-dynamic-list-render',
    templateUrl: './dynamic-list.render.component.html',
    styleUrls: ['./dynamic-list.render.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class DynamicListRenderComponent implements OnInit, AfterViewInit, AfterViewChecked, OnDestroy {
    @ViewChild(DynamicListGridComponent)
    dynamicListGrid: DynamicListGridComponent

    @Input()
    dynamicList: DynamicList;

    @Input()
    displayName: string

    @Input()
    section: ExtendedPageSection

    @Output()
    onRendered = new EventEmitter()

    commandOutList: Array<CommandButtonInList> = [];

    loading$: BehaviorSubject<boolean> = new BehaviorSubject(false);
    pageState$: Observable<PageStateModel>
    subscription: Subscription
    subscriptionControl: Subscription

    buttonFullNames: string[] = []
    dynamicListFullName: string
    constructor(
        private store: Store,
        public dialog: MatDialog,
        private shortcutUtil: ShortcutUtil,
        private route: ActivatedRoute,
        private redirectRoute: Router,
        private logger: NGXLogger,
        private translatePipe: Translator,
        private pageService: PageService,
        private httpClient: HttpClient
    ) {
    }

    ngOnInit(): void { 
        this.gatherAllButtonFullNames()
        this.gatherListName()       
        this.pageState$ = this.store.select<PageStateModel>(state => state.page)
        this.subscription = this.pageState$.pipe(
            filter(state => state.filterState 
                && (state.filterState === BeginBuildingBoundData
                    || state.filterState === GatherSectionValidations)),
            tap(
                state => {
                    switch (state.filterState) {
                        case BeginBuildingBoundData:
                            this.store.dispatch(new AddSectionBoundData({
                                name: this.section.name,
                                isKeptDataName: true,
                                data: null
                            }, []))
                            break
                        case GatherSectionValidations:
                            this.store.dispatch(new SectionValidationStateAction(this.section.name, true))
                            break
                    }
                }
            )
        ).subscribe()
        this.subscriptionControl = 
            this.pageService.listenTriggeringControlEvent$()
                .pipe(
                    debounceTime(500),
                    filter(state => state 
                        && (this.buttonFullNames.indexOf(state.controlFullName) > -1
                            || state.controlFullName === this.dynamicListFullName)),
                    tap(
                        state => {
                            switch(state.eventType){
                                case 'refetch':
                                    if(state.controlFullName === this.dynamicListFullName){
                                        this.dynamicListGrid.submitGrid()
                                    }
                                    break
                            }
                        }
                    )
                )
                .subscribe()
                                
        if(this.dynamicList.commandsList && this.dynamicList.commandsList.commandButtonsInList.length > 0){
            const commandOutList = _.filter(this.dynamicList.commandsList.commandButtonsInList, (element: CommandButtonInList) => {
                return element.commandPositionType === CommandPositionType.OutList
            })
            if(commandOutList){
                this.commandOutList = commandOutList
            }
        }        
        
        this.redirectRoute.routeReuseStrategy.shouldReuseRoute = () => false;
    }
    ngAfterViewInit(): void {
    }

    ngAfterViewChecked(): void {
        this.onRendered.emit()
    }

    ngOnDestroy(): void {
        if(this.subscription){
            this.subscription.unsubscribe()
        }        
    }

    onCommandClicked(commandClicked: CommandClicked) {
        this.logger.debug('Command clicked', commandClicked)
        switch (commandClicked.command.actionCommandOptions.actionType) {
            case ActionType.Redirect:
                if (commandClicked.command.actionCommandOptions.redirectOptions.isSameDomain) {
                    let navigationExtras: NavigationExtras = {
                        preserveFragment: true,
                        preserveQueryParams: true
                    };
                    let url = commandClicked.command.commandPositionType === CommandPositionType.InList ?
                        this.translatePipe.translateDataWithShell(commandClicked.command.actionCommandOptions.redirectOptions.redirectUrl, {
                            ...this.pageService.getPageShellData(),
                            data: commandClicked.data
                        }) : 
                        this.translatePipe.translateDataWithShell(commandClicked.command.actionCommandOptions.redirectOptions.redirectUrl, this.pageService.getPageShellData())
                    this.logger.debug('redirecit url', url)
                    this.redirectRoute.navigateByUrl(url, navigationExtras)
                }
                else {
                    let url = commandClicked.command.commandPositionType === CommandPositionType.InList ?
                        this.translatePipe.translateDataWithShell(commandClicked.command.actionCommandOptions.redirectOptions.redirectUrl, {
                            ...this.pageService.getPageShellData(),
                            data: commandClicked.data
                        }) :
                        this.translatePipe.translateDataWithShell(commandClicked.command.actionCommandOptions.redirectOptions.redirectUrl, this.pageService.getPageShellData())
                    this.logger.debug('redirecit url', url)
                    window.open(url, '_blank')
                }
                break
            case ActionType.CallHttpService:
                let url = commandClicked.command.commandPositionType === CommandPositionType.InList ?
                    this.translatePipe.translateDataWithShell(commandClicked.command.actionCommandOptions.httpServiceOptions.httpServiceUrl, {
                        ...this.pageService.getPageShellData(),
                        data: commandClicked.data
                    }) :
                    this.translatePipe.translateDataWithShell(commandClicked.command.actionCommandOptions.httpServiceOptions.httpServiceUrl, this.pageService.getPageShellData())
                let body = commandClicked.command.commandPositionType === CommandPositionType.InList ?
                    this.translatePipe.translateDataWithShell(commandClicked.command.actionCommandOptions.httpServiceOptions.jsonBody, {
                        ...this.pageService.getPageShellData(),
                        data: commandClicked.data
                    }) :
                    this.translatePipe.translateDataWithShell(commandClicked.command.actionCommandOptions.httpServiceOptions.jsonBody, this.pageService.getPageShellData())
                switch (commandClicked.command.actionCommandOptions.httpServiceOptions.httpMethod.toUpperCase()) {
                    case 'GET':
                        const get$ = this.httpClient.get(url, {
                            headers: new HttpHeaders({
                                'Content-Type': 'application/json'
                            }), observe: 'response'
                        }).pipe(
                            tap(result => {
                                this.shortcutUtil.toastMessage(commandClicked.command.actionCommandOptions.notificationOptions.completeMessage, ToastType.Success)
                                if (commandClicked.command.allowRefreshList) {
                                    this.refreshList()
                                }
                            },
                                err => {
                                    this.shortcutUtil.toastMessage(commandClicked.command.actionCommandOptions.notificationOptions.failedMessage, ToastType.Error)
                                })
                        ).subscribe()
                        break
                    case 'POST':
                        const post$ = this.httpClient.post(url, body, {
                            headers: new HttpHeaders({
                                'Content-Type': 'application/json'
                            }), observe: 'response'
                        }).pipe(
                            tap(result => {
                                this.shortcutUtil.toastMessage(commandClicked.command.actionCommandOptions.notificationOptions.completeMessage, ToastType.Success)
                                if (commandClicked.command.allowRefreshList) {
                                    this.refreshList()
                                }
                            },
                                err => {
                                    this.shortcutUtil.toastMessage(commandClicked.command.actionCommandOptions.notificationOptions.failedMessage, ToastType.Error)
                                })
                        ).subscribe()
                        break
                    case 'DELETE':
                        const delete$ = this.httpClient.delete(url, {
                            headers: new HttpHeaders({
                                'Content-Type': 'application/json'
                            }), observe: 'response'
                        }).pipe(
                            tap(result => {
                                this.shortcutUtil.toastMessage(commandClicked.command.actionCommandOptions.notificationOptions.completeMessage, ToastType.Success)
                                if (commandClicked.command.allowRefreshList) {
                                    this.refreshList()
                                }
                            },
                                err => {
                                    this.shortcutUtil.toastMessage(commandClicked.command.actionCommandOptions.notificationOptions.failedMessage, ToastType.Error)
                                })
                        ).subscribe()
                        break
                    case 'PUT':
                        const put$ = this.httpClient.put(url, body, {
                            headers: new HttpHeaders({
                                'Content-Type': 'application/json'
                            }), observe: 'response'
                        }).pipe(
                            tap(result => {
                                this.shortcutUtil.toastMessage(commandClicked.command.actionCommandOptions.notificationOptions.completeMessage, ToastType.Success)
                                if (commandClicked.command.allowRefreshList) {
                                    this.refreshList()
                                }
                            },
                                err => {
                                    this.shortcutUtil.toastMessage(commandClicked.command.actionCommandOptions.notificationOptions.failedMessage, ToastType.Error)
                                })
                        ).subscribe()
                        break
                }

                break
        }
    }

    private gatherAllButtonFullNames(){
        if(this.dynamicList.commandsList && this.dynamicList.commandsList.commandButtonsInList.length > 0){
            _.forEach(this.dynamicList.commandsList.commandButtonsInList, command => {
                this.buttonFullNames.push(this.section.name + '_' + command.name)
            })
        }        
    }

    private gatherListName(){
        this.dynamicListFullName = this.section.name + '_' + this.dynamicList.name
    }

    private refreshList() {
        this.dynamicListGrid.submitGrid();
    }
}
