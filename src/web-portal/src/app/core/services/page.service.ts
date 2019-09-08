import { Injectable } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { Translator } from '../shell/translates/translate.pipe';
import { ShellConfigProvider } from '../shell/shellconfig.provider';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { DatabasesClient, PagesClient, Page, PageDatasource, DatasourceControlType, ExecuteDynamicResultModel, PageEvent, RouteType, ActionType, PageButton, EventActionType } from './portal.service';
import { NGXLogger } from 'ngx-logger';
import { Store } from '@ngxs/store';
import { SecurityService } from '../security/security.service';
import { Observable, of, forkJoin, Subscription, throwError } from 'rxjs';
import { tap, map, filter, mergeMap } from 'rxjs/operators';
import { AuthUser } from '../security/auth.model';
import { PortalStandardClaims } from '../security/portalClaims';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { PageResponse, PageLoadedDatasource, PageControlEvent, TriggeredControlEvent } from '../models/page.model';
import { PageStateModel, PageState } from 'stores/pages/page.state';
import { ShellConfig, ShellConfigType } from '../shell/shell.model';
import * as _ from 'lodash';
import { InitPageInfo, LoadDatasource, LoadDatasourceComplete, PageReadyAction, ChangeControlValueEvent, BeginRenderingPageSectionsAction, EndBuildingBoundDataComplete, GatherSectionValidations, SectionValidationStateAction, CompleteGatherSectionValidations, UserClicksOnButtonAction, ClickControlEvent } from 'stores/pages/page.actions';
import { StateReset } from 'ngxs-reset-plugin';
import { TriggerControlChangeValueEvent } from 'stores/pages/pagecontrol.actions';
import { ExtendedPageButton } from '../models/extended.models';
import { ConfigurationProvider } from '../configs/configProvider';
import { CustomHttpService } from './customhttp.service';
import { ObjectUtils } from '../utils/object-util';

/**
 * This class contains all base methods for interacting with Page
 */
@Injectable()
export class PageService {
    private options = new Object()
    private queryparams = new Object()
    private claims: any
    private data: any
    private page: Page
    private dataSub: Subscription
    private commandSub: Subscription
    private eventSub: Subscription
    private configs: any
    private sectionValidationCounter = 0

    constructor(
        private customHttpService: CustomHttpService,
        private configurationProvider: ConfigurationProvider,
        private pageClients: PagesClient,
        private security: SecurityService,
        private router: Router,
        private translator: Translator,
        private shellConfigProvider: ShellConfigProvider,
        private shortcutUtil: ShortcutUtil,
        public databasesClient: DatabasesClient,
        private logger: NGXLogger,
        private store: Store
    ) {
        this.configs = this.configurationProvider.getCurrentConfigs()
    }


    /**
     * *This method is using to retrieve a page info from Service and then it checks a current user who is allowed to access
     * !This method is only used when it is a custom page, after retrieving a page, pass it into initRender
     * @param pageName 
     * @returns Observable PageResponse
     */
    init(pageName: string): Observable<PageResponse> {
        return this.pageClients.getOne(pageName).pipe(
            map<Page, PageResponse>((page: Page) => {
                this.page = page
                return { page: page, allowAccess: this.isAllowAccess(this.security.getAuthUser(), page) }
            }),
            tap(
                result => {
                    if (!result.allowAccess) {
                        this.shortcutUtil.notifyMessage(`Sorry, you can access ${result.page.displayName} page !`, ToastType.Warning)
                        this.router.navigateByUrl('/')
                    }
                },
                err => {
                    this.shortcutUtil.notifyMessage("Oops, Something went wrong, please try again!", ToastType.Error)
                    this.router.navigateByUrl('/')
                }
            )
        )
    }

    /**
     * *This method is using to construct some parts of Page (use render mode).
     * !This method should be passed Page object instead of performing Service to retrieve, call init() first to get Page object
     * @param page 
     * @returns Observable PageStateModel
     */
    initRender(page: Page, activatedRoute: ActivatedRoute): Observable<PageStateModel> {
        const claims$ = this.initPageClaims(page.name)
        claims$.subscribe(
            claims => {
                this.claims = claims
            }
        )
        const pageState = this.store.select(state => state.page)
        this.store.dispatch(new InitPageInfo(page))
        this.page = page
        const prepareQueryParam$ = this.initQueryParams(activatedRoute, this.shellConfigProvider)

        const preapreOptions$ = this.initPageOptions(page, this.shellConfigProvider)

        const sub$ = prepareQueryParam$.subscribe(
            res => {
                const subOptions$ = preapreOptions$.subscribe(
                    res => {
                        const prepareDatasource$ = this.loadDatasource(page, this.store, this.shellConfigProvider, this.translator, this.databasesClient, this.options, this.queryparams)
                        const subDatasource$ = prepareDatasource$.subscribe(
                            res => {
                                this.store.dispatch(new PageReadyAction(this.options, this.queryparams))
                            }
                        )
                    }
                )
            })

        this.commandSub = this.store.select(state => state.page)
            .pipe(
                filter(state => state.filterState
                    && (state.filterState === UserClicksOnButtonAction
                        || state.filterState === SectionValidationStateAction
                        || state.filterState === CompleteGatherSectionValidations)),
                tap(
                    (state: PageStateModel) => {
                        switch (state.filterState) {
                            case UserClicksOnButtonAction:
                                this.sectionValidationCounter = this.page.builder.sections.length
                                if (state.clickingButton.isRequiredValidation) {
                                    this.store.dispatch(new GatherSectionValidations())
                                }
                                else {
                                    this.executingCommandHit(state.clickingButton)
                                }
                                break
                            case SectionValidationStateAction:
                                this.logger.debug('Hit checking section validation', state.sectionValidations)
                                this.sectionValidationCounter--
                                if (this.sectionValidationCounter === 0) {
                                    this.store.dispatch(new CompleteGatherSectionValidations())
                                }
                                break
                            case CompleteGatherSectionValidations:
                                if (state.wholePageValid) {
                                    this.logger.debug('Hit all valid')
                                    this.executingCommandHit(state.clickingButton)
                                }
                                else {
                                    this.shortcutUtil.notifyMessage('Please complete all required fields', ToastType.Warning)
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

        this.eventSub =
            this.listenControlEvent$().pipe(
                filter(state => !!state),
                tap(
                    event => {
                        this.logger.debug('Current event', event)
                        _.forEach(this.page.events, evt => {
                            if (evt.eventName === event.name) {
                                switch (evt.eventActionType) {
                                    case EventActionType.TriggerEvent:
                                        _.forEach(evt.triggerEventOptions.eventsList, triggerEvt => {
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
                || state.filterState === ChangeControlValueEvent)),
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
        let evaluated = Function('data', 'return data.' + bindName)
        return evaluated(this.data)
    }

    /**
     * Listens control event$
     * @returns control event$ 
     */
    listenControlEvent$(): Observable<PageControlEvent> {
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
     * @param data
     */
    changeControlValue(controlFullName: string, data: any) {
        let splitted = controlFullName.split('_')
        this.store.dispatch(new ChangeControlValueEvent({
            name: controlFullName + '_change',
            controlName: splitted[1],
            sectionName: splitted[0],
            data: data,
            triggeredByEvent: ''
        }))
    }

    executeCommandByName(commandName: string) {
        const found = _.find(this.page.commands, command => command.name === commandName)
        if (found) {
            this.executeCommandClickEvent(found as ExtendedPageButton)
        }
    }

    executeCommand(commandAction: ExtendedPageButton) {
        this.executeCommandClickEvent(commandAction)
    }

    notifyTriggeringEvent(controlEvent: string, data: any = null) {
        let splitted = controlEvent.split('_')
        let event: TriggeredControlEvent = {
            controlFullName: splitted[0] + '_' + splitted[1],
            data: data,
            eventType: splitted[2],
            fullEventType: controlEvent
        }
        this.store.dispatch(new TriggerControlChangeValueEvent(event))
    }

    fetchDatasource(databaseId: string, query: string): Observable<any>{
        let combineQuery = this.translator.translateData(query, {
            user: this.security.getAuthUser(),
            claims: {},
            configs: this.configs,
            data: this.data,
            options: this.options,
            queryparams: this.queryparams
        })

        return this.databasesClient.executeQueryDatasource({
            databaseId: databaseId,
            query: combineQuery
        }).pipe(
            mergeMap(result => {
                if(result.isSuccess){
                    let array = []
                    if(ObjectUtils.isObject(result.result)){                        
                        array.push(result.result)
                    }
                    else{
                        array = result.result
                    }
                    return of(array)
                }
                else{
                    return throwError(result.error)
                }
            })
        )
    }

    evaluatedExpression(evaluteStr: string, data: any = null): boolean{
        let func = new Function('user', 'claims', 'configs', 'options', 'queryparams', 'data', `return ${evaluteStr} ? true : false;`);
        return func(this.security.getAuthUser(), this.claims, this.configs, this.options, this.queryparams, !!data ? data : this.data) as boolean
    }

    translateData(translateStr: string, data: any = null, isMergingData: boolean = false): string{
        let translated = this.translator.translateData(translateStr, {
            user: this.security.getAuthUser(),
            claims: this.claims,
            configs: this.configs,
            data: !!data ? (isMergingData ? this.mergeData(data) : data) : this.data,
            options: this.options,
            queryparams: this.queryparams
        })

        return translated
    }

    private mergeData(mergingData: any){
        return {
            ...this.data,
            ...mergingData
        }
    }

    private initPageClaims(pageName: string): Observable<Object>{
        return this.security.getPortalClaims().pipe(
            mergeMap(res => {
                if(res){
                    return of(this.security.getAuthUser().getClaimsPerPage(pageName))
                }
                else{
                    of([])
                }
            })
        )        
    }

    private initQueryParams(activatedRoute: ActivatedRoute, shellConfigProvider: ShellConfigProvider): Observable<ShellConfig[]> {
        return activatedRoute.queryParamMap.pipe(
            map<ParamMap, ShellConfig[]>(param => {
                this.logger.debug('Hit param', this.queryparams)
                let shellConfigs: ShellConfig[] = []
                _.forEach(param.keys, key => {
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
        let shellConfigs: ShellConfig[] = []
        _.forEach(page.shellOptions, option => {
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
        shellConfigProvider: ShellConfigProvider,
        translator: Translator,
        databaseClient: DatabasesClient,
        options: any,
        queryparams: any): Observable<PageLoadedDatasource[]> {
        if (!!page.pageDatasources && page.pageDatasources.length > 0) {
            store.dispatch(new LoadDatasource())
            let datasources$: Observable<PageLoadedDatasource>[] = []
            _.forEach(page.pageDatasources, (ds: PageDatasource) => {
                if (ds.isActive) {
                    this.logger.debug('Current options', this.options)
                    this.logger.debug('Current queryparams', this.queryparams)
                    const allowTrigger = this.evaluatedExpression(ds.triggerCondition)
                    if (allowTrigger) {
                        switch (ds.options.type) {
                            case DatasourceControlType.StaticResource:
                                datasources$.push(of<PageLoadedDatasource>({
                                    data: JSON.parse(ds.options.datasourceStaticOptions.jsonResource),
                                    name: ds.name
                                }))
                                break
                            case DatasourceControlType.WebService:
                                const jsonBody = translator.translate(ds.options.databaseOptions.query, {})
                                const url = translator.translate(ds.options.httpServiceOptions.httpServiceUrl, {})

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
                                const query = translator.translate(ds.options.databaseOptions.query, {})
                                const dsName = ds.name
                                datasources$.push(databaseClient.executeQueryDatasource({
                                    databaseId: ds.options.databaseOptions.databaseConnectionId,
                                    query: query
                                }).pipe(
                                    filter(res => res.isSuccess),
                                    map<ExecuteDynamicResultModel, PageLoadedDatasource>((res: ExecuteDynamicResultModel) => {
                                        return { name: dsName, data: res.result }
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
                            this.logger.debug('Datasource', results)
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
        if (command.buttonOptions.confirmationOptions.isEnable) {
            const _title = 'Confirmation'
            const _description = command.buttonOptions.confirmationOptions.confirmationText
            const _waitDesciption = "Waiting..."
            const dialogRef = this.shortcutUtil.actionEntityElement(_title, _description, _waitDesciption);
            dialogRef.afterClosed().subscribe(res => {
                if (!res) {
                    return
                }

                this.store.dispatch(new UserClicksOnButtonAction(command))
            })
        }
        else {
            this.store.dispatch(new UserClicksOnButtonAction(command))
        }
    }
    
    private executingCommandHit(command: PageButton) {
        if (command.buttonOptions.actionCommandOptions.isEnable) {
            switch (command.buttonOptions.actionCommandOptions.actionType) {
                case ActionType.ExecuteDatabase:
                    let dbCommand = this.translator.translateData(
                        command.buttonOptions.actionCommandOptions.databaseOptions.query, {
                            user: this.security.getAuthUser(),
                            configs: this.configs,
                            claims: this.claims,
                            data: this.data,
                            options: this.options,
                            queryparams: this.queryparams
                        });
                    this.databasesClient.executionDynamic(command.buttonOptions.actionCommandOptions.databaseOptions.databaseConnectionId, dbCommand).subscribe(
                        res => {
                            this.shortcutUtil.notifyMessage(command.buttonOptions.actionCommandOptions.notificationOptions.completeMessage, ToastType.Success)
                            this.routingCommand(command)
                        },
                        err => {
                            this.shortcutUtil.notifyMessage(command.buttonOptions.actionCommandOptions.notificationOptions.failedMessage, ToastType.Error)
                        })
                    break
                case ActionType.CallHttpService:
                    let translatorFactors = {
                        user: this.security.getAuthUser(),
                        configs: this.configs,
                        claims: this.claims,
                        data: this.data,
                        options: this.options,
                        queryparams: this.queryparams
                    }
                    let url = this.translator.translateData(
                        command.buttonOptions.actionCommandOptions.httpServiceOptions.httpServiceUrl,
                        translatorFactors
                    )
                    let body = this.translator.translateData(
                        command.buttonOptions.actionCommandOptions.httpServiceOptions.jsonBody,
                        translatorFactors
                    )

                    this.customHttpService.performHttp(
                        url,
                        command.buttonOptions.actionCommandOptions.httpServiceOptions.httpMethod,
                        body,
                        command.buttonOptions.actionCommandOptions.httpServiceOptions.httpSuccessCode,
                        command.buttonOptions.actionCommandOptions.httpServiceOptions.outputProjection).subscribe(
                            res => {
                                this.shortcutUtil.notifyMessage(command.buttonOptions.actionCommandOptions.notificationOptions.completeMessage, ToastType.Success)
                                this.routingCommand(command)
                            },
                            err => {
                                this.shortcutUtil.notifyMessage(command.buttonOptions.actionCommandOptions.notificationOptions.failedMessage, ToastType.Error)
                            }
                        )

                    break
                case ActionType.CallWorkflow:
                    break
                case ActionType.Redirect:
                    this.routingCommand(command)
                    break
            }
        }
        else {
            this.routingCommand(command)
        }
    }

    private routingCommand(command: PageButton) {
        let foundRoute = false
        _.forEach(command.buttonOptions.routeOptions.routes, route => {
            switch (route.routeType) {
                case RouteType.ThroughUrl:
                    let allowed = this.evaluatedExpression(route.condition)
                    if (allowed && !foundRoute) {
                        foundRoute = true
                        let url = this.translator.translate(route.targetUrl, this.data)
                        let path = this.translator.translate(route.passDataPath, this.data)
                        let fullUrl = !!url ? url : ''
                        if (!!path) {
                            fullUrl += '?' + path
                        }
                        this.logger.debug('Redirecting to...', fullUrl)
                        this.router.navigateByUrl(fullUrl)
                    }
                    break
                case RouteType.ThroughPage:
                    let allowedPage = this.evaluatedExpression(route.condition)
                    if (allowedPage && !foundRoute) {
                        this.pageClients.getOneById(route.targetPageId).subscribe(
                            page => {
                                let combineData = this.translator.translate(route.passDataPath, this.data)
                                if (combineData) {
                                    this.router.navigateByUrl(page.urlPath + '?' + combineData)
                                }
                                else {
                                    this.router.navigateByUrl(page.urlPath)
                                }
                            }
                        )
                    }
                    break
            }
        })

        if (!foundRoute) {
            // Notify click event for chaining events
            this.store.dispatch(new ClickControlEvent({
                controlName: command.name.toLowerCase(),
                name: (command.name + '_click').toLowerCase(),
                sectionName: '',
                triggeredByEvent: '',
                data: null
            }))
        }
    }
}