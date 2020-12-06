import { HttpClient } from '@angular/common/http';
import { AfterViewChecked, AfterViewInit, ChangeDetectionStrategy, Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NavigationExtras, Router } from '@angular/router';
import { Store } from '@ngxs/store';
import { ExtendedPageSection } from 'app/core/models/extended.models';
import { Translator } from 'app/core/shell/translates/translate.pipe';
import { ObjectUtils } from 'app/core/utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { ListOptions } from 'portal/modules/models/dynamiclist.extended.model';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { debounceTime, filter, tap } from 'rxjs/operators';
import { ExportService } from 'services/export.service';
import { LocalizationService } from 'services/localization.service';
import { PageService } from 'services/page.service';
import { ActionType, CommandButtonInList, CommandPositionType, DynamicList } from 'services/portal.service';
import { AddSectionBoundData, BeginBuildingBoundData, GatherSectionValidations, SectionValidationStateAction } from 'stores/pages/page.actions';
import { PageStateModel } from 'stores/pages/page.state';
import { DynamicListGridComponent } from './components/dynamic-list.grid.component';
import { CommandClicked } from './models/commandClicked';
 

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
        private localizationService: LocalizationService,
        private httpClient: HttpClient
    ) {
    }

    ngOnInit(): void {
        this.localization()
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
                                storeName: this.section.name,
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
            const commandOutList = this.dynamicList.commandsList.commandButtonsInList.filter((element: CommandButtonInList) => {
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
            Object.keys(firstElem)?.forEach(prop => {
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

            exportedData?.forEach(elem => {
                jsonParsedProps?.forEach(prop => {
                    elem[prop] = JSON.stringify(elem[prop])
                })
                deletedProps?.forEach(prop => {
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
            this.dynamicList.commandsList.commandButtonsInList?.forEach(command => {
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

    private localization(){
        if(this.localizationService.allowTranslate){
            const displayName = this.localizationService.getText(`dynamicLists.${this.dynamicList.name}.options.displayName`)
            if(ObjectUtils.isNotNull(displayName)){
                this.dynamicList.displayName = displayName
            }

            if(ObjectUtils.isNotNull(this.dynamicList.columnsList) && 
                ObjectUtils.isNotNull(this.dynamicList.columnsList.columnDefs)){
                this.dynamicList.columnsList.columnDefs?.forEach(col => {
                    if(!col.isHidden){
                        const colName = this.localizationService.getText(`dynamicLists.${this.dynamicList.name}.cols.${col.name}.displayName`)
                        if(ObjectUtils.isNotNull(colName)){
                            col.displayName = colName
                        }
                    }
                })
            }

            if(ObjectUtils.isNotNull(this.dynamicList.commandsList) 
                && ObjectUtils.isNotNull(this.dynamicList.commandsList.commandButtonsInList)){
                    this.dynamicList.commandsList.commandButtonsInList?.forEach(command => {
                        const commandName = this.localizationService.getText(`dynamicLists.${this.dynamicList.name}.commands.${command.name}.displayName`)
                        if(ObjectUtils.isNotNull(commandName)){
                            command.displayName = commandName
                        }

                        switch(command.actionCommandOptions.actionType){
                            case ActionType.Redirect:
                                break
                            case ActionType.ExecuteDatabase:
                            case ActionType.CallHttpService:
                            default:
                                if(ObjectUtils.isNotNull(command.actionCommandOptions.confirmationOptions)){
                                    const confirmationText = this.localizationService.getText(`dynamicLists.${this.dynamicList.name}.commands.${command.name}.confirmation.text`)
                                    if(ObjectUtils.isNotNull(confirmationText)){
                                        command.actionCommandOptions.confirmationOptions.confirmationText = confirmationText
                                    }
                                }  
                                
                                if(ObjectUtils.isNotNull(command.actionCommandOptions.notificationOptions)){
                                    const successText = this.localizationService.getText(`dynamicLists.${this.dynamicList.name}.commands.${command.name}.notification.success`)
                                    const failedText = this.localizationService.getText(`dynamicLists.${this.dynamicList.name}.commands.${command.name}.notification.failed`)

                                    if(ObjectUtils.isNotNull(successText)){
                                        command.actionCommandOptions.notificationOptions.completeMessage = successText
                                    }
                                    if(ObjectUtils.isNotNull(failedText)){
                                        command.actionCommandOptions.notificationOptions.failedMessage = failedText
                                    }
                                }
                                break
                        }
                    })
                }
        }
    }
}
