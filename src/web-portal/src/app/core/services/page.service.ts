import { Injectable } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Store } from '@ngxs/store';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { EventDialogType, MessageType, ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { Guid } from 'guid-typescript';
import { NGXLogger } from 'ngx-logger';
import { StateReset } from 'ngxs-reset-plugin';
import { forkJoin, Observable, of, Subscription } from 'rxjs';
import { filter, map, mergeMap, tap } from 'rxjs/operators';
import { BeginRenderingPageSectionsAction, ChangeControlValueEvent, ClickControlEvent, CompleteGatherSectionValidations, EndBuildingBoundDataComplete, GatherSectionValidations, InitPageInfo, InsertOneItemForStandardArray, LoadDatasource, LoadDatasourceComplete, PageReadyAction, RemoveOneItemForStandardArray, RenderedPageSectionAction, SectionValidationStateAction, UpdateOneItemForStandardArray, UserClicksOnButtonAction } from 'stores/pages/page.actions';
import { PageState, PageStateModel } from 'stores/pages/page.state';
import { TriggerControlChangeValueEvent } from 'stores/pages/pagecontrol.actions';
import { ConfigurationProvider } from '../configs/configProvider';
import { BoundControl } from '../context/bound-control';
import { ArrayBoundSection, BoundSection, StandardBoundSection, TreeBoundSection } from '../context/bound-section';
import { PageContext } from '../context/page-context';
import { Interceptor } from '../interceptors/interceptor';
import { InterceptorsProvider } from '../interceptors/interceptor.provider';
import { ExtendedPageButton } from '../models/extended.models';
import { PageControlActionEvent, PageLoadedDatasource, PageResponse, PageShellData, TriggeredControlEvent } from '../models/page.model';
import { AuthUser } from '../security/auth.model';
import { PortalStandardClaims } from '../security/portalClaims';
import { SecurityService } from '../security/security.service';
import { ShellConfig, ShellConfigType } from '../shell/shell.model';
import { ShellConfigProvider } from '../shell/shellconfig.provider';
import { Translator } from '../shell/translates/translate.pipe';
import { ObjectUtils } from '../utils/object-util';
import { CustomHttpService } from './customhttp.service';
import { DatasourceOptionsService } from './datasourceopts.service';
import { ActionCommandOptions, ActionType, ConfirmationOptions, DatabasesClient, DatasourceControlType, DatasourceOptions, EventActionType, ExecuteDynamicResultModel, HttpServiceOptions, LoopDataModel, Page, PageAsyncValidatorModel, PageButton, PageControlEvent, PageDatasource, PageParameterModel, PagesClient, SectionContructionType } from './portal.service';
import { SessionService } from './session.service';

/**
 * This class contains all base methods for interacting the Page
 */
@Injectable()
export class PageService {
    private options = new Object()
    private queryparams = new Object()
    private claims: any
    private data: any
    private sectionCounter: number = 0
    private page: Page
    private dataSub: Subscription
    private commandSub: Subscription
    private eventSub: Subscription
    private renderSub: Subscription
    private configs: any
    private sectionValidationCounter = 0
    private interceptor: Interceptor
    public context: PageContext
    constructor(
        private translate: TranslateService,
        private customHttpService: CustomHttpService,
        private configurationProvider: ConfigurationProvider,
        private pageClients: PagesClient,
        private security: SecurityService,
        private session: SessionService,
        private router: Router,
        private translator: Translator,
        private shellConfigProvider: ShellConfigProvider,
        private shortcutUtil: ShortcutUtil,
        private databasesClient: DatabasesClient,
        private datasourceOptsService: DatasourceOptionsService,
        private logger: NGXLogger,
        private store: Store,
        private interceptorProvider: InterceptorsProvider
    ) {
        this.configs = this.configurationProvider.getCurrentConfigs()
    }


    /**
     * *This method is using to retrieve a page info from Service and then it checks a current user who is allowed to access
     * !This method is only used when it is a custom page, after retrieving a page, pass it into initRender
     * @param pageName Name of page
     * @returns Observable PageResponse
     */
    init(pageName: string): Observable<PageResponse> {
        return this.pageClients.getOneForRender(pageName).pipe(
            map<Page, PageResponse>((page: Page) => {
                this.page = page
                return { page, allowAccess: this.isAllowAccess(this.security.getAuthUser(), page) }
            }),
            tap(
                result => {
                    if (!result.allowAccess) {
                        this.shortcutUtil.toastMessage(this.translate.instant('pageService.messages.notAllowedAccessPage'), ToastType.Warning)
                        this.router.navigateByUrl(this.session.getDefaultAppPage())
                    }
                },
                err => {
                    this.shortcutUtil.toastMessage(this.translate.instant('common.somethingWentWrong'), ToastType.Error)
                    this.router.navigateByUrl(this.session.getDefaultAppPage())
                }
            )
        )
    }

    /**
     * *This method is using to construct some parts of Page (use render mode).
     * !This method should be passed Page object instead of performing Service to retrieve, call init() first to get Page object
     * @param page Page object, get from API
     * @returns Observable PageStateModel
     */
    initRender(page: Page, activatedRoute: ActivatedRoute): Observable<PageStateModel> {
        this.interceptor = this.interceptorProvider.getPageInterceptor(page.name)
        const claims$ = this.initPageClaims(page.name)
        claims$.subscribe(
            claims => {
                this.claims = claims
                if (!this.isAllowAccess(this.security.getAuthUser(), page)) {
                    this.shortcutUtil.toastMessage(this.translate.instant('pageService.messages.notAllowedAccessPage'), ToastType.Warning)
                    this.router.navigateByUrl(this.session.getDefaultAppPage())
                }
            }
        )
        const pageState = this.store.select<PageStateModel>(state => state.page)
        this.store.dispatch(new InitPageInfo(page))
        this.page = page
        const prepareQueryParam$ = this.initQueryParams(activatedRoute, this.shellConfigProvider)

        const preapreOptions$ = this.initPageOptions(page, this.shellConfigProvider)

        const sub$ = prepareQueryParam$.subscribe(
            res => {
                const subOptions$ = preapreOptions$.subscribe(
                    res => {
                        const prepareDatasource$ = this.loadDatasource(page, this.store, this.translator, this.pageClients)
                        const subDatasource$ = prepareDatasource$.subscribe(
                            res => {
                                this.store.dispatch(new PageReadyAction(this.options, this.queryparams))
                            }
                        )
                    }
                )
            })

        this.commandSub = pageState
            .pipe(
                filter(state => state.filterState
                    && (state.filterState === UserClicksOnButtonAction
                        || state.filterState === SectionValidationStateAction
                        || state.filterState === CompleteGatherSectionValidations)),
                tap(
                    (state: PageStateModel) => {
                        switch (state.filterState) {
                            case UserClicksOnButtonAction:
                                this.logger.debug('User has clicked on a button : ' + state.clickingButton.name)
                                this.sectionValidationCounter = ObjectUtils.isNotNull(state.clickingButton.placeSectionId) ?
                                    1 : this.sectionCounter
                                if (state.clickingButton.isRequiredValidation) {
                                    this.store.dispatch(
                                        new GatherSectionValidations(
                                            ObjectUtils.isNotNull(state.clickingButton.placeSectionId) ?
                                                this.page.builder.sections.find(a => a.id === state.clickingButton.placeSectionId).name : null))
                                }
                                else {
                                    this.executeByActionOptions(
                                        state.clickingButton.name,
                                        state.clickingButton.buttonOptions.actionCommandOptions,
                                        () => {
                                            this.routingCommand(state.clickingButton)
                                        })
                                }
                                break
                            case SectionValidationStateAction:
                                this.logger.debug('Hit checking section validation', state.sectionValidations)
                                this.sectionValidationCounter--
                                this.logger.debug('Section validation counter', this.sectionValidationCounter)
                                if (this.sectionValidationCounter === 0) {
                                    this.store.dispatch(new CompleteGatherSectionValidations())
                                }
                                break
                            case CompleteGatherSectionValidations:
                                if (state.wholePageValid) {
                                    this.executeByActionOptions(
                                        state.clickingButton.name,
                                        state.clickingButton.buttonOptions.actionCommandOptions,
                                        () => {
                                            this.routingCommand(state.clickingButton)
                                        })
                                }
                                else {
                                    this.shortcutUtil.toastMessage(this.translate.instant('pageService.messages.completeAllRequiredFields'), ToastType.Warning)
                                }
                                break
                        }
                    }
                )
            ).subscribe()

        this.dataSub = this.listenDataChange$().subscribe(
            data => {
                this.data = data
            }
        )

        this.sectionCounter = 0
        this.renderSub = pageState.pipe(
            filter(state => state.filterState
                && (state.filterState === RenderedPageSectionAction)),
            tap(
                res => {
                    this.sectionCounter++
                    this.logger.debug('Hit rendered section', this.sectionCounter)
                }
            )
        ).subscribe()

        // Events for linking sections on one page
        this.eventSub =
            this.listenControlEvent$().pipe(
                filter(state => !!state),
                tap(
                    event => {
                        this.logger.debug('Current event', event)
                        this.page.events?.forEach(evt => {
                            if (evt.eventName === event.name) {
                                switch (evt.eventActionType) {
                                    case EventActionType.TriggerEvent:
                                        evt.triggerEventOptions.eventsList?.forEach(triggerEvt => {
                                            this.notifyTriggeringEvent(triggerEvt, null)
                                        })
                                        break
                                    case EventActionType.WebService:
                                        break
                                }
                            }
                        })
                    }
                )
            ).subscribe()

        let boundSections: BoundSection[] = []
        this.page.builder.sections?.forEach(section => {
            switch (section.constructionType) {
                case SectionContructionType.Array:
                    boundSections.push(new ArrayBoundSection(section.name))
                    break
                case SectionContructionType.Tree:
                    boundSections.push(new TreeBoundSection(section.name))
                    break
                default:
                    boundSections.push(new StandardBoundSection(section.name, null, null))
                    break
            }
        })

        this.context = new PageContext(this.page, boundSections)

        return pageState
    }

    /**
     * *Destroys page service
     * !Please always call it in OnDestroy
     */
    destroy() {
        this.options = null
        this.queryparams = null
        this.options = new Object()
        this.queryparams = new Object()
        this.commandSub.unsubscribe()
        this.dataSub.unsubscribe()
        this.eventSub.unsubscribe()
        this.renderSub.unsubscribe()
        this.store.dispatch(new StateReset(PageState))
    }

    /**
     * Listens data change$
     * @returns data change$
     */
    listenDataChange$(): Observable<any> {
        return this.store.select(state => state.page).pipe(
            filter(state => state.filterState && (
                state.filterState === EndBuildingBoundDataComplete
                || state.filterState === ChangeControlValueEvent
                || state.filterState === UpdateOneItemForStandardArray
                || state.filterState === InsertOneItemForStandardArray
                || state.filterState === RemoveOneItemForStandardArray)),
            map(state => {
                return state.data
            })
        )
    }

    listenOptionsAndParamsChange$(): Observable<any> {
        return this.store.select(state => state.page).pipe(
            filter(state => state.filterState && (state.filterState === BeginRenderingPageSectionsAction)),
            map(state => {
                return { options: state.options, queryparams: state.queryparams }
            })
        )
    }

    getDataByBindName(bindName: string) {
        const evaluated = Function('data', 'return data.' + bindName)
        return evaluated(this.data)
    }

    /**
     * Listens control event$
     * @returns control event$
     */
    listenControlEvent$(): Observable<PageControlActionEvent> {
        return this.store.select(state => state.page.lastEvent)
    }

    /**
     * Listens triggering control event$
     * @returns triggering control event$
     */
    listenTriggeringControlEvent$(): Observable<TriggeredControlEvent> {
        return this.store.select(state => state.controlevents.effectedControlEvent)
    }

    /**
     * Changes control value
     * @param controlFullName Pattern: {sectionName}_{controlName}, Sensistive name Ex: databaseinfo_connectionString
     * @param data new value will be set in state
     * @param sectionRef BoundSection of control, also it can be ArrayBoundSection or TreeBoundSection
     * @param controlRef BoundControl
     * @returns if returns false, means skip chaning events, otherwise continuing chaning events
     */
    changeControlValue(
        controlFullName: string,
        data: any,
        sectionRef: BoundSection,
        controlRef: BoundControl
    ): boolean {
        const splitted = controlFullName.split('_')
        const section = splitted[0]
        const control = splitted[1]
        // Note: If there are no defined interceptor, the default chaining events is true
        const checked = this.interceptor ? this.checkInterceptorEvent(section, control, 'change', sectionRef, controlRef) : true
        this.store.dispatch(new ChangeControlValueEvent({
            name: controlFullName + '_change',
            controlFullName: controlFullName,
            controlName: control,
            sectionName: section,
            data,
            triggeredByEvent: '',
            allowChainingEvents: checked
        }))

        return checked
    }
    
    executeCommandByName(commandName: string) {
        const found = this.page.commands.find(command => command.name === commandName)
        if (found) {
            this.executeCommandClickEvent(found as ExtendedPageButton)
        }
    }

    executeCommand(commandAction: ExtendedPageButton) {
        this.executeCommandClickEvent(commandAction)
    }

    notifyTriggeringEvent(controlEvent: string, data: any = null) {
        const splitted = controlEvent.split('_')
        const event: TriggeredControlEvent = {
            controlFullName: splitted[0] + '_' + splitted[1],
            data,
            eventType: splitted[2],
            fullEventType: controlEvent
        }
        this.store.dispatch(new TriggerControlChangeValueEvent(event))
    }

    fetchDatasourceOptions(datasourceOpts: DatasourceOptions): Observable<any> {
        return this.datasourceOptsService.executeDatasourceOptions(datasourceOpts, this.getPageShellData())
    }

    fetchControlSelectionDatasource(
        sectionName: string, 
        controlName: string, 
        compositeControlId: string,
        isChildCompositeControl: boolean,
        parameters: PageParameterModel[]): Observable<ExecuteDynamicResultModel> {
        return this.pageClients.fetchControlDatasource(this.page.id, {
            sectionName,
            controlName,
            parameters,
            compositeControlId,
            isChildCompositeControl
        }).pipe(
            map(res => ObjectUtils.isArray(res.result) ? res.result : [res.result])
        )
    }

    evaluatedExpression(evaluteStr: string, data: any = null): boolean {
        const func = new Function('user', 'claims', 'configs', 'options', 'queryparams', 'data', `return ${evaluteStr} ? true : false;`);
        return func(this.security.getAuthUser(), this.claims, this.configs, this.options, this.queryparams, ObjectUtils.isNotNull(data) ? data : this.data) as boolean
    }

    translateData(translateStr: string, data: any = null, isMergingData: boolean = false): string {
        if (ObjectUtils.isNotNull(data)) {
            const translated = this.translator.translateDataWithShell(translateStr, data)

            return translated
        }
        else {
            const translated = this.translator.translateDataWithShell(translateStr, this.getPageShellData())

            return translated
        }

    }
    retrieveParameters(translateStr: string, data: any = null, isMergingData: boolean = false): PageParameterModel[] {
        const preparedData = this.getPageShellData()
        if (data != null && isMergingData) {
            preparedData.data = {
                ...preparedData.data,
                ...data
            }
        }
        return this.translator.retrieveParameters(translateStr, preparedData)
    }

    getPageShellData(data?: any, parent?: any): PageShellData {
        return {
            data: ObjectUtils.isNotNull(data) ? data : this.data,
            appsettings: {},
            claims: this.claims,
            configs: this.configs,
            options: this.options,
            queryparams: this.queryparams,
            user: this.security.getAuthUser(),
            parent: parent
        }
    }

    executeAsyncValidator(asyncValidatorModel: PageAsyncValidatorModel, data?: any) {
        return this.pageClients.executeAsyncValidator(this.page.id, asyncValidatorModel)
    }

    executeActionEventOnDatabase(triggeringEvent: PageControlEvent, sectionName: string, controlName: string) {
        const paramters = this.retrieveParameters(triggeringEvent.eventDatabaseOptions.query)
        return this.pageClients.executeTriggeredEvent(this.page.id, {
            sectionName,
            controlName,
            eventName: triggeringEvent.eventName,
            parameters: paramters
        }).pipe(
            filter(a => a.isSuccess),
            map(res => ObjectUtils.isNotNull(triggeringEvent.eventDatabaseOptions.outputProjection) ?
                ObjectUtils.projection(triggeringEvent.eventDatabaseOptions.outputProjection, res.result) : res.result)
        )
    }

    executeActionEventOnWebService(triggeringEvent: PageControlEvent) {
        const translatedUrl = this.translateData(triggeringEvent.eventHttpServiceOptions.httpServiceUrl)
        const translatedBody = this.translateData(triggeringEvent.eventHttpServiceOptions.jsonBody)
        return this.customHttpService.performHttp(
            translatedUrl,
            triggeringEvent.eventHttpServiceOptions.httpMethod,
            translatedBody,
            triggeringEvent.eventHttpServiceOptions.httpSuccessCode,
            triggeringEvent.eventHttpServiceOptions.outputProjection)
    }

    executeHttpWithBoundData(httpServiceOptions: HttpServiceOptions) {
        const translatedUrl = this.translateData(httpServiceOptions.httpServiceUrl)
        const translatedBody = this.translateData(httpServiceOptions.jsonBody)
        return this.customHttpService.performHttp(
            translatedUrl,
            httpServiceOptions.httpMethod,
            translatedBody,
            httpServiceOptions.httpSuccessCode,
            httpServiceOptions.outputProjection)
    }

    executeByActionOptions(
        buttonName: string,
        actionCommandOptions: ActionCommandOptions,
        onComplete: () => void,
        data?: any) {
        if (actionCommandOptions.isEnable) {

            switch (actionCommandOptions.actionType) {
                case ActionType.ExecuteDatabase:
                    let combinedCommand = ''
                    let loopDatas: LoopDataModel[] = []
                    actionCommandOptions.dbExecutionChains.steps?.forEach((step, index) => {
                        if (ObjectUtils.isNotNull(step.dataLoopKey)) {
                            // Do data loop proccess
                            const evaluated = Function('data', 'return ' + step.dataLoopKey)
                            const passingData = evaluated(this.data)
                            let loopData: LoopDataModel = {
                                name: step.dataLoopKey,
                                parameters: []
                            }

                            this.logger.debug('All loop data', passingData)
                            if (ObjectUtils.isArray(passingData)) {
                                passingData?.forEach(p => {
                                    loopData.parameters
                                        .push(
                                            this.translator.retrieveParameters(
                                                step.executeCommand,
                                                this.getPageShellData(p, this.data))
                                        );
                                })
                            }

                            loopDatas.push(loopData)
                        }
                        else {
                            if (index > 0)
                                combinedCommand += ';' + step.executeCommand
                            else
                                combinedCommand += step.executeCommand
                        }

                    })
                    const params = this.translator.retrieveParameters(
                        combinedCommand, this.getPageShellData(data));
                    this.pageClients
                        .submitCommand(this.page.id, {
                            buttonName,
                            parameters: params,
                            loopDatas: loopDatas
                        }).subscribe(
                            res => {
                                this.shortcutUtil
                                    .eventDialog('Success',
                                        actionCommandOptions.notificationOptions.completeMessage,
                                        EventDialogType.Success)
                                if (onComplete) {
                                    onComplete()
                                }
                            },
                            err => {
                                this.shortcutUtil
                                    .eventDialog('Error',
                                        actionCommandOptions.notificationOptions.failedMessage,
                                        EventDialogType.Error)
                            })
                    break
                case ActionType.CallHttpService:
                    const url = this.translator.translateDataWithShell(
                        actionCommandOptions.httpServiceOptions.httpServiceUrl,
                        this.getPageShellData(data)
                    )
                    const body = this.translator.translateDataWithShell(
                        actionCommandOptions.httpServiceOptions.jsonBody,
                        this.getPageShellData(data)
                    )

                    this.customHttpService.performHttp(
                        url,
                        actionCommandOptions.httpServiceOptions.httpMethod,
                        body,
                        actionCommandOptions.httpServiceOptions.httpSuccessCode,
                        actionCommandOptions.httpServiceOptions.outputProjection).subscribe(
                            res => {
                                this.shortcutUtil
                                    .eventDialog('Success',
                                        actionCommandOptions.notificationOptions.completeMessage,
                                        EventDialogType.Success)
                                if (onComplete) {
                                    onComplete()
                                }
                            },
                            err => {
                                this.shortcutUtil
                                    .eventDialog('Error',
                                        actionCommandOptions.notificationOptions.failedMessage,
                                        EventDialogType.Error)
                            }
                        )

                    break
                case ActionType.CallWorkflow:
                    break
                case ActionType.Redirect:
                    if (onComplete) {
                        onComplete()
                    }
                    break
            }
        }
        else {
            if (onComplete) {
                onComplete()
            }
        }
    }

    openConfirmationDialog(confirmationOptions: ConfirmationOptions, onProceed: () => void) {
        if (confirmationOptions.isEnable) {
            const _title = 'Confirmation'
            const _description = confirmationOptions.confirmationText
            const _waitDesciption = 'Waiting...'
            const dialogRef = this.shortcutUtil
                .confirmationDialog(
                    _title,
                    _description,
                    _waitDesciption,
                    MessageType.Custom,
                    'Proceed');
            dialogRef.afterClosed().subscribe(res => {
                if (!res) {
                    return
                }

                if (onProceed) {
                    onProceed()
                }
            })
        }
        else {
            if (onProceed) {
                onProceed()
            }
        }
    }

    private initPageClaims(pageName: string): Observable<any> {
        return this.security.getPortalClaims().pipe(
            mergeMap(res => {
                if (res) {
                    return of(this.security.getAuthUser().getClaimsPerPage(pageName))
                }
                else {
                    of([])
                }
            })
        )
    }

    private initQueryParams(activatedRoute: ActivatedRoute, shellConfigProvider: ShellConfigProvider): Observable<ShellConfig[]> {
        return activatedRoute.queryParamMap.pipe(
            map<ParamMap, ShellConfig[]>(param => {
                this.logger.debug('Hit param', this.queryparams)
                const shellConfigs: ShellConfig[] = []
                param.keys?.forEach(key => {
                    this.queryparams[key] = param.get(key)
                    shellConfigs.push({ key: `queryparams.${key}`, value: param.get(key), type: ShellConfigType.Constant })
                })

                return shellConfigs
            }),
            tap(
                shellConfigs => {
                    shellConfigProvider.appendShellConfigs(shellConfigs)
                }
            )
        )
    }

    private initPageOptions(page: Page, shellConfigProvider: ShellConfigProvider): Observable<ShellConfig[]> {
        this.logger.debug('Hit options')
        const shellConfigs: ShellConfig[] = []
        page.shellOptions?.forEach(option => {
            shellConfigs.push({ key: 'options.' + option.key, value: option.value, type: ShellConfigType.Constant })
            this.options[option.key] = option.value
        })
        // Add some available options
        shellConfigs.push({ key: 'options.name', value: page.name, type: ShellConfigType.Constant })
        shellConfigProvider.appendShellConfigs(shellConfigs)
        return of(shellConfigs)
    }

    private loadDatasource(
        page: Page,
        store: Store,
        translator: Translator,
        pagesClient: PagesClient): Observable<PageLoadedDatasource[]> {
        if (!!page.pageDatasources && page.pageDatasources.length > 0) {
            store.dispatch(new LoadDatasource())
            const datasources$: Observable<PageLoadedDatasource>[] = []
            page.pageDatasources?.forEach((ds: PageDatasource) => {
                if (ds.isActive) {
                    this.logger.debug('Current options', this.options)
                    this.logger.debug('Current queryparams', this.queryparams)
                    const allowTrigger = this.evaluatedExpression(ds.triggerCondition)
                    if (allowTrigger) {
                        switch (ds.options.type) {
                            case DatasourceControlType.StaticResource:
                                const translatedDs = this.translateData(ds.options.datasourceStaticOptions.jsonResource, this.getPageShellData(), false)
                                datasources$.push(of<PageLoadedDatasource>({
                                    data: JSON.parse(translatedDs),
                                    name: ds.name,
                                    key: Guid.create().toString()
                                }))
                                break
                            case DatasourceControlType.WebService:
                                const jsonBody = translator.translateDataWithShell(ds.options.databaseOptions.query, this.getPageShellData())
                                const url = translator.translateDataWithShell(ds.options.httpServiceOptions.httpServiceUrl, this.getPageShellData())

                                switch (ds.options.httpServiceOptions.httpMethod.toUpperCase()) {
                                    case 'GET':
                                        break
                                    case 'POST':
                                        break
                                    case 'PUT':
                                        break
                                    case 'DELETE':
                                        break
                                }
                                break
                            case DatasourceControlType.Database:
                                const params = translator.retrieveParameters(ds.options.databaseOptions.query, this.getPageShellData())
                                const dsName = ds.name
                                datasources$.push(
                                    pagesClient
                                        .getDatasourceForPage(page.id, {
                                            datasourceId: ds.id,
                                            parameters: params
                                        })
                                        .pipe(
                                            filter(res => res.isSuccess),
                                            map<ExecuteDynamicResultModel, PageLoadedDatasource>((res: ExecuteDynamicResultModel) => {
                                                // Ensure if result is array, take 1st elem for data
                                                if (ObjectUtils.isArray(res.result)) {
                                                    return { name: dsName, data: res.result[0], key: Guid.create().toString() }
                                                }
                                                return { name: dsName, data: res.result, key: Guid.create().toString() }
                                            })
                                        ))
                                break
                        }
                    }
                }
            })

            if (datasources$.length > 0) {
                return forkJoin(datasources$).pipe(
                    tap(
                        results => {
                            store.dispatch(new LoadDatasourceComplete(results))
                        }
                    )
                )
            }
        }

        store.dispatch(new LoadDatasourceComplete([]))
        return of([])
    }

    private isAllowAccess(user: AuthUser, page: Page) {
        return user.hasClaim(page.name, PortalStandardClaims.AllowAccess.name) || user.isInRole('SuperAdmin')
    }

    private executeCommandClickEvent(command: ExtendedPageButton) {
        if (command.buttonOptions.actionCommandOptions.isEnable) {
            this.openConfirmationDialog(
                command.buttonOptions.actionCommandOptions.confirmationOptions,
                () => {
                    this.store.dispatch(new UserClicksOnButtonAction(command))
                })
        }
        else {
            this.store.dispatch(new UserClicksOnButtonAction(command))
        }
    }

    private routingCommand(command: PageButton) {
        let foundRoute = false
        if (!ObjectUtils.isNotNull(command.buttonOptions.routeOptions)) {
            return
        }
        command.buttonOptions.routeOptions.routes?.forEach(route => {
            const allowed = this.evaluatedExpression(route.condition)
            if (allowed && !foundRoute) {
                foundRoute = true
                const url = this.translator.translateDataWithShell(route.redirectUrl, this.getPageShellData())
                this.logger.debug('Redirecting to...', url)
                if (route.isSameDomain)
                    this.router.navigateByUrl(url)
                else
                    window.open(url, '_blank')
                return false
            }
        })

        if (!foundRoute) {
            // Notify click event for chaining events
            this.store.dispatch(new ClickControlEvent({
                controlName: command.name.toLowerCase(),
                name: (command.name + '_click').toLowerCase(),
                controlFullName: command.name.toLowerCase(),
                sectionName: '',
                triggeredByEvent: '',
                data: null,
                allowChainingEvents: true
            }))
        }
    }

    private checkInterceptorEvent(
        section: string,
        control: string,
        event: string,
        sectionRef: BoundSection,
        controlRef: BoundControl): boolean {
        return this.interceptor.executeControlEvent(
            section,
            control,
            event,
            {
                pageService: this,
                pageContext: this.context,
                sectionRef: sectionRef,
                controlRef: controlRef
            })
    }
}