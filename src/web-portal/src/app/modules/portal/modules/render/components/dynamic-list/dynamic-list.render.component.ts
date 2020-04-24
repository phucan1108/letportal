import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild, AfterViewInit, ChangeDetectionStrategy, Input, Output, AfterContentChecked, EventEmitter, AfterViewChecked } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
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
import { ObjectUtils } from 'app/core/utils/object-util';
import { ListOptions } from 'portal/modules/models/dynamiclist.extended.model';
import { ExportService } from 'services/export.service';

@Component({
    selector: 'let-dynamic-list-render',
    templateUrl: './dynamic-list.render.component.html',
    styleUrls: ['./dynamic-list.render.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class DynamicListRenderComponent implements OnInit, AfterViewInit, AfterViewChecked, OnDestroy {
    @ViewChild(DynamicListGridComponent, { static: true })
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

    listOptions: ListOptions = ListOptions.DefaultListOptions
    constructor(
        private store: Store,
        public dialog: MatDialog,
        private exportService: ExportService,
        private redirectRoute: Router,
        private logger: NGXLogger,
        private translatePipe: Translator,
        private pageService: PageService,
        private httpClient: HttpClient
    ) {
    }

    ngOnInit(): void {
        this.listOptions = ListOptions.getListOptions(this.dynamicList.options)
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
                            if (state.specificValidatingSection === this.section.name
                                || !ObjectUtils.isNotNull(state.specificValidatingSection)) {
                                this.store.dispatch(new SectionValidationStateAction(this.section.name, true))
                            }                            
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
                            switch (state.eventType) {
                                case 'refetch':
                                    if (state.controlFullName === this.dynamicListFullName) {
                                        this.dynamicListGrid.submitGrid()
                                    }
                                    break
                            }
                        }
                    )
                )
                .subscribe()

        if (this.dynamicList.commandsList && this.dynamicList.commandsList.commandButtonsInList.length > 0) {
            const commandOutList = _.filter(this.dynamicList.commandsList.commandButtonsInList, (element: CommandButtonInList) => {
                return element.commandPositionType === CommandPositionType.OutList
            })
            if (commandOutList) {
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
        if (this.subscription) {
            this.subscription.unsubscribe()
        }
    }

    onCommandClicked(commandClicked: CommandClicked) {
        this.logger.debug('Command clicked', commandClicked)
        switch (commandClicked.command.actionCommandOptions.actionType) {
            case ActionType.Redirect:
                if (commandClicked.command.actionCommandOptions.redirectOptions.isSameDomain) {
                    const navigationExtras: NavigationExtras = {
                        preserveFragment: true,
                        preserveQueryParams: true
                    };
                    const url = commandClicked.command.commandPositionType === CommandPositionType.InList ?
                        this.translatePipe.translateDataWithShell(commandClicked.command.actionCommandOptions.redirectOptions.redirectUrl, {
                            ...this.pageService.getPageShellData(),
                            data: commandClicked.data
                        }) :
                        this.translatePipe.translateDataWithShell(commandClicked.command.actionCommandOptions.redirectOptions.redirectUrl, this.pageService.getPageShellData())
                    this.logger.debug('redirecit url', url)
                    this.redirectRoute.navigateByUrl(url, navigationExtras)
                }
                else {
                    const url = commandClicked.command.commandPositionType === CommandPositionType.InList ?
                        this.translatePipe.translateDataWithShell(commandClicked.command.actionCommandOptions.redirectOptions.redirectUrl, {
                            ...this.pageService.getPageShellData(),
                            data: commandClicked.data
                        }) :
                        this.translatePipe.translateDataWithShell(commandClicked.command.actionCommandOptions.redirectOptions.redirectUrl, this.pageService.getPageShellData())
                    this.logger.debug('redirecit url', url)
                    window.open(url, '_blank')
                }
                break
            default:
                this.pageService.openConfirmationDialog(commandClicked.command.actionCommandOptions.confirmationOptions,
                    () => {
                        this.pageService.executeByActionOptions(
                            this.dynamicList.name + '_' + commandClicked.command.name + '_click',
                            commandClicked.command.actionCommandOptions,
                            () => {
                                if(commandClicked.command.allowRefreshList){
                                    this.refreshList()
                                }
                            },
                            commandClicked.command.commandPositionType == CommandPositionType.OutList ? null : commandClicked.data)
                    })

                break
        }
    }

    callExportExcel(){
        // Check calling client or server-side
        if(ObjectUtils.isNotNull(this.dynamicListGrid.dataSource) && this.dynamicListGrid.dataSource.length > 0 && this.listOptions.maximumClientExport >= this.dynamicListGrid.dataSource.length){
            // Client export
            const excelFileName = this.dynamicList.displayName
            const sheetFileName = this.dynamicList.displayName
            let headers: string[] = []
            let orderedNames: string[] = []
            if(this.listOptions.allowExportHiddenFields){
                headers = this.dynamicListGrid.headers.map(a => a.displayName)
                orderedNames = this.dynamicListGrid.headers.map(a => a.name)
            }
            else{
                headers = this.dynamicListGrid.headers.filter(b => !b.isHidden).map(a => a.displayName)
                orderedNames = this.dynamicListGrid.headers.filter(b => !b.isHidden).map(a => a.name)
            }

            // An overkill performance when exporting over 1k rows,
            // developer needs to use server-side to export instead of client side
            const exportedData: any[] = this.dynamicListGrid.dataSource
            const deletedProps: string[] = []
            const jsonParsedProps: string[] = []
            const firstElem = exportedData[0]
            Object.keys(firstElem).forEach(prop => {
                let inDeletedProps = false
                if(orderedNames.indexOf(prop) < 0){
                    deletedProps.push(prop)
                    inDeletedProps = true
                }

                if((ObjectUtils.isArray(firstElem[prop]) || ObjectUtils.isObject(firstElem[prop]))
                    && !inDeletedProps){
                        jsonParsedProps.push(prop)
                }
            })

            exportedData.forEach(elem => {
                jsonParsedProps.forEach(prop => {
                    elem[prop] = JSON.stringify(elem[prop])
                })
                deletedProps.forEach(prop => {
                    elem[prop] = null
                })
            })

            this.exportService.exportExcelFile(
                excelFileName,
                sheetFileName,
                orderedNames,
                headers,
                exportedData
                )
        }
    }

    private gatherAllButtonFullNames() {
        if (this.dynamicList.commandsList && this.dynamicList.commandsList.commandButtonsInList.length > 0) {
            _.forEach(this.dynamicList.commandsList.commandButtonsInList, command => {
                this.buttonFullNames.push(this.section.name + '_' + command.name)
            })
        }
    }

    private gatherListName() {
        this.dynamicListFullName = this.section.name + '_' + this.dynamicList.name
    }

    private refreshList() {
        this.dynamicListGrid.submitGrid();
    }
}
