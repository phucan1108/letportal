import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { JsonEditorComponent, JsonEditorOptions } from 'ang-jsoneditor';
import { ArrayUtils } from 'app/core/utils/array-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ClipboardService } from 'ngx-clipboard';
import { NGXLogger } from 'ngx-logger';
import { StaticResources } from 'portal/resources/static-resources';
import { BehaviorSubject } from 'rxjs';
import { ActionCommandOptions, ActionType, ConfirmationOptions, DatabaseConnection, DatabaseExecutionStep, DatabasesClient, MapWorkflowInput, NotificationOptions, PagesClient, ShortPageModel } from 'services/portal.service';
import { ActionCommandRenderOptions } from './actioncommandrenderoptions';
 

@Component({
    selector: 'let-commandoptions',
    templateUrl: './commandoptions.component.html'
})
export class CommandOptionsComponent implements OnInit {

    @Input()
    options: ActionCommandRenderOptions

    @Input()
    actionCommandOptions: ActionCommandOptions

    actionTypes = StaticResources.actionTypes()
    currentActionType: ActionType = ActionType.Redirect

    actionType = ActionType

    isEnable = false

    databaseOptionsForm: FormGroup

    httpOptionsForm: FormGroup

    redirectionOptionsForm: FormGroup

    notificationOptionsForm: FormGroup

    confirmationOptions: ConfirmationOptions
    confirmationFormGroup: FormGroup

    steps: Array<DatabaseExecutionStep> = []

    @ViewChild('jsonPayloadEditor', { static: false }) jsonPayloadEditor: JsonEditorComponent
    public editorOptions1: JsonEditorOptions = new JsonEditorOptions()
    jsonHttpBodyData: any = {};
    isJsonHttpBodyEditorValid = true;
    _httpMethods = StaticResources.httpCallMethods()

    @ViewChild('jsonDataEditor', { static: false }) jsonDataEditor: JsonEditorComponent
    public editorOptions3: JsonEditorOptions = new JsonEditorOptions()
    jsonData: any = {}
    databaseConnections: Array<DatabaseConnection>;
    hintText = ''
    isHintClicked = false
    mapWorkflowInputs: MapWorkflowInput[] = []
    isReadyToRender = false

    page$: BehaviorSubject<Array<ShortPageModel>> = new BehaviorSubject([])
    pages: Array<ShortPageModel> = []

    hideDatabaseOption = false
    hideHttpOption = false
    hideWorkflowOption = false
    hideRedirectionOption = false

    constructor(
        private fb: FormBuilder,
        private databaseClient: DatabasesClient,
        private clipboardService: ClipboardService,
        private shortcutUtil: ShortcutUtil,
        private pagesClient: PagesClient,
        private logger: NGXLogger
    ) { }

    ngOnInit(): void {
        this.isEnable = this.actionCommandOptions.isEnable
        this.hideDatabaseOption = this.options.modes.indexOf('database') < 0
        this.hideHttpOption = this.options.modes.indexOf('http') < 0
        this.hideWorkflowOption = this.options.modes.indexOf('workflow') < 0
        this.hideRedirectionOption = this.options.modes.indexOf('redirect') < 0

        if(this.hideDatabaseOption){
            this.actionTypes = ArrayUtils.removeOneItem(this.actionTypes, a => a.value === ActionType.ExecuteDatabase)
        }

        if(this.hideHttpOption){
            this.actionTypes = ArrayUtils.removeOneItem(this.actionTypes, a => a.value === ActionType.CallHttpService)
        }

        if(this.hideWorkflowOption){
            this.actionTypes = ArrayUtils.removeOneItem(this.actionTypes, a => a.value === ActionType.CallWorkflow)
        }

        if(this.hideRedirectionOption){
            this.actionTypes = ArrayUtils.removeOneItem(this.actionTypes, a => a.value === ActionType.Redirect)
        }

        if (this.actionCommandOptions.confirmationOptions) {
            this.confirmationOptions = this.actionCommandOptions.confirmationOptions
        }
        else {
            
            this.actionCommandOptions.confirmationOptions = {
                isEnable: false,
                confirmationText: 'Are you sure to proceed it?'
            }
            this.confirmationOptions = this.actionCommandOptions.confirmationOptions
        }

        this.actionCommandOptions = ObjectUtils.clone(this.actionCommandOptions)
        this.currentActionType = this.actionCommandOptions.actionType
        this.initDatabaseChains()
        this.initHttpOptions()
        this.initWorkflowOptions()
        this.initRedirectionOptions()
        this.initNotificationOptions()
        this.initConfirmationFormGroup()

        this.steps = ObjectUtils.clone(this.actionCommandOptions.dbExecutionChains.steps)

        this.databaseClient.getAll().subscribe(
            res => {
                this.databaseConnections = res
                this.isReadyToRender = true
            }
        )

        this.page$.subscribe(res => {
            this.pages = res
        })
    }

    initNotificationOptions() {
        if(!this.actionCommandOptions.notificationOptions){
            this.actionCommandOptions.notificationOptions = {
                completeMessage: 'Action successfully!',
                failedMessage: 'Action failed!'
            }
        }

        this.notificationOptionsForm = this.fb.group({
            completeMessage: [this.actionCommandOptions.notificationOptions.completeMessage, Validators.required],
            failedMessage: [this.actionCommandOptions.notificationOptions.failedMessage, Validators.required]
        })
    }

    initRedirectionOptions(){
        if(!this.actionCommandOptions.redirectOptions){
            this.actionCommandOptions.redirectOptions = {
                redirectUrl: '',
                isSameDomain: true
            }
        }

        this.pagesClient.getAllShortPages().subscribe(res => {
            this.page$.next(res)
        })

        this.redirectionOptionsForm = this.fb.group({
            isSameDomain: [this.actionCommandOptions.redirectOptions.isSameDomain],
            redirectUrl: [this.actionCommandOptions.redirectOptions.redirectUrl],
            targetPageId: ['']
        })

        this.redirectionOptionsForm.get('targetPageId').valueChanges.subscribe(newValue => {
            if(newValue){
                const found = this.pages.find(a => a.id == newValue)
                this.redirectionOptionsForm.get('redirectUrl').setValue(found.urlPath)
            }
        })
    }

    initWorkflowOptions(){
        if(!this.actionCommandOptions.workflowOptions){
            this.actionCommandOptions.workflowOptions = {
                workflowId: '',
                mapWorkflowInputs: []
            }
        }
        this.mapWorkflowInputs = this.actionCommandOptions.workflowOptions.mapWorkflowInputs
    }

    initDatabaseChains(){
        if(!this.actionCommandOptions.dbExecutionChains){
            this.actionCommandOptions.dbExecutionChains = {
                steps: []
            }
        }
    }

    initHttpOptions() {
        if (!this.actionCommandOptions.httpServiceOptions) {
            this.actionCommandOptions.httpServiceOptions = {
                httpMethod: 'Get',
                httpServiceUrl: '',
                httpSuccessCode: '200',
                jsonBody: '',
                outputProjection: ''
            }
        }

        this.editorOptions1.mode = 'code'
        this.jsonHttpBodyData = this.actionCommandOptions.httpServiceOptions.jsonBody ? JSON.parse(this.actionCommandOptions.httpServiceOptions.jsonBody) : {}
        this.editorOptions1.onChange = () => {
            try {
                this.httpOptionsForm.get('httpJsonPayload').setValue(JSON.stringify(this.jsonPayloadEditor.get()))
            }
            catch{

            }
        }

        this.httpOptionsForm = this.fb.group({
            httpCallUrl: [this.actionCommandOptions.httpServiceOptions.httpServiceUrl, Validators.required],
            httpCallMethod: [this.actionCommandOptions.httpServiceOptions.httpMethod, Validators.required],
            httpJsonPayload: [this.actionCommandOptions.httpServiceOptions.jsonBody],
            httpOutputProjection: [this.actionCommandOptions.httpServiceOptions.outputProjection],
            httpSuccessCode: [this.actionCommandOptions.httpServiceOptions.httpSuccessCode, Validators.required]
        })
    }

    initConfirmationFormGroup() {
        this.confirmationFormGroup = this.fb.group({
            isEnable: [this.confirmationOptions.isEnable],
            confirmationText: [this.confirmationOptions.confirmationText, [Validators.required, Validators.maxLength(250)]]
        })
    }

    isValid(){
        switch(this.currentActionType){
            case ActionType.ExecuteDatabase:
                return this.confirmationFormGroup.valid && this.isDbChainsValid() && this.notificationOptionsForm.valid
            case ActionType.CallHttpService:
                return this.confirmationFormGroup.valid && this.httpOptionsForm.valid && this.notificationOptionsForm.valid
            case ActionType.Redirect:
                return this.redirectionOptionsForm.valid
        }
    }

    onExecutionDrop($event){

    }

    addStep(){
        const newStep = {
            databaseConnectionId: '',
            executeCommand: ''
        }
        this.actionCommandOptions.dbExecutionChains.steps.push(newStep)

        this.steps.push(newStep)
    }

    stepChanged($event: DatabaseExecutionStep,index){
        this.actionCommandOptions.dbExecutionChains.steps[index] = $event
        this.logger.debug('After changed', this.actionCommandOptions.dbExecutionChains)
    }

    get(): ActionCommandOptions{
        switch(this.currentActionType){
            case ActionType.ExecuteDatabase:
                return {
                    isEnable: this.isEnable,
                    actionType: this.currentActionType,
                    dbExecutionChains: this.actionCommandOptions.dbExecutionChains,
                    notificationOptions: this.getNotification(),
                    confirmationOptions: this.getConfirmationOptions()
                }
            case ActionType.CallHttpService:
                const httpFormValue = this.httpOptionsForm.value
                return {
                    isEnable: this.isEnable,
                    actionType: this.currentActionType,
                    httpServiceOptions: {
                        httpMethod: httpFormValue.httpCallMethod,
                        httpServiceUrl: httpFormValue.httpCallUrl,
                        httpSuccessCode: httpFormValue.httpSuccessCode,
                        jsonBody: httpFormValue.httpJsonPayload,
                        outputProjection: httpFormValue.httpOutputProjection
                    },
                    notificationOptions: this.getNotification(),
                    confirmationOptions: this.getConfirmationOptions()
                }
            case ActionType.Redirect:
                const redirectFormValue = this.redirectionOptionsForm.value
                return {
                    isEnable: this.isEnable,
                    actionType: this.currentActionType,
                    redirectOptions: {
                        redirectUrl: redirectFormValue.redirectUrl,
                        isSameDomain: redirectFormValue.isSameDomain
                    }
                }
        }
    }

    private getConfirmationOptions(){
        const confirmationValues = this.confirmationFormGroup.value
        return {
            isEnable: confirmationValues.isEnable,
            confirmationText: confirmationValues.confirmationText
        }
    }

    private getNotification(): NotificationOptions{
        const notificationFormValues =  this.notificationOptionsForm.value
        return {
            completeMessage: notificationFormValues.completeMessage,
            failedMessage: notificationFormValues.failedMessage
        }
    }

    private isDbChainsValid(): boolean{
        let isValid = true
        this.actionCommandOptions.dbExecutionChains.steps?.forEach(step => {
            if(!ObjectUtils.isNotNull(step.databaseConnectionId) || !ObjectUtils.isNotNull(step.executeCommand)){
                isValid = false
                return false
            }
        });

        return isValid
    }
}
