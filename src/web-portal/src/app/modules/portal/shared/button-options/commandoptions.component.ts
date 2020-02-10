import { Component, OnInit, Input, ViewChild, Output, EventEmitter } from '@angular/core';
import { ActionCommandOptions, ActionType, DatabaseConnection, DatabasesClient, MapWorkflowInput, ShortPageModel, PagesClient, RedirectType, NotificationOptions } from 'services/portal.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActionCommandRenderOptions } from './actioncommandrenderoptions';
import { JsonEditorComponent, JsonEditorOptions } from 'ang-jsoneditor';
import { StaticResources } from 'portal/resources/static-resources';
import { Observable } from 'rxjs';
import { ClipboardService } from 'ngx-clipboard';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import * as _ from 'lodash';
import { ArrayUtils } from 'app/core/utils/array-util';
import { ObjectUtils } from 'app/core/utils/object-util';

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

    @ViewChild('jsonPayloadEditor', { static: false }) jsonPayloadEditor: JsonEditorComponent
    public editorOptions1: JsonEditorOptions = new JsonEditorOptions()
    jsonHttpBodyData: any = {};
    isJsonHttpBodyEditorValid = true;
    _httpMethods = StaticResources.httpCallMethods()

    @ViewChild('jsonDataEditor', { static: false }) jsonDataEditor: JsonEditorComponent
    public editorOptions3: JsonEditorOptions = new JsonEditorOptions()
    jsonData: any = {}
    databaseConnections: Observable<Array<DatabaseConnection>>;
    hintText = ''
    isHintClicked = false
    mapWorkflowInputs: MapWorkflowInput[] = []
    
    page$: Observable<Array<ShortPageModel>>;
    _redirectionTypes = StaticResources.redirectionTypes()
    redirectionType = RedirectType

    hideDatabaseOption = false
    hideHttpOption = false
    hideWorkflowOption = false
    hideRedirectionOption = false

    constructor(
        private fb: FormBuilder,
        private databaseClient: DatabasesClient,
        private clipboardService: ClipboardService,
        private shortcutUtil: ShortcutUtil,
        private pagesClient: PagesClient
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
        
        this.actionCommandOptions = ObjectUtils.clone(this.actionCommandOptions)
        this.currentActionType = this.actionCommandOptions.actionType
        this.initDatabaseOptions()
        this.initHttpOptions()
        this.initWorkflowOptions()
        this.initRedirectionOptions()
        this.initNotificationOptions()

        this.databaseConnections = this.databaseClient.getAll()
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
                isSameDomain: true,
                passParams: '',
                redirectType: RedirectType.ThroughPage,
                targetPageId: ''
            }
        }

        this.page$ = this.pagesClient.getAllShortPages()
        this.redirectionOptionsForm = this.fb.group({
            redirectionType : [this.actionCommandOptions.redirectOptions.redirectType, Validators.required],
            isSameDomain: [this.actionCommandOptions.redirectOptions.isSameDomain],
            targetPageId: [this.actionCommandOptions.redirectOptions.targetPageId],
            passParams: [this.actionCommandOptions.redirectOptions.passParams],
            redirectUrl: [this.actionCommandOptions.redirectOptions.redirectUrl]
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

    initDatabaseOptions(){
        if(!this.actionCommandOptions.databaseOptions){
            this.actionCommandOptions.databaseOptions = {
                databaseConnectionId: '',
                query: '',
                entityName: ''
            }
        }
        
        this.editorOptions3.mode = 'code'
        this.jsonData = this.actionCommandOptions.databaseOptions.query ? JSON.parse(this.actionCommandOptions.databaseOptions.query) : {}
        this.editorOptions3.onChange = () => {
            try {
                const jsonStr = JSON.stringify(this.jsonDataEditor.get())
                if (!!jsonStr && jsonStr !== '{}') {
                    this.databaseOptionsForm.get('query').setValue(jsonStr)
                }
            }
            catch{

            }
        }

        this.databaseOptionsForm = this.fb.group({
            databaseConnectionId: [this.actionCommandOptions.databaseOptions.databaseConnectionId, Validators.required],
            query: [this.actionCommandOptions.databaseOptions.query, Validators.required]
        })

        this.databaseOptionsForm.valueChanges.subscribe(newValue => {

        })
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
            httpJsonPayload: [this.actionCommandOptions.httpServiceOptions.jsonBody, Validators.required],
            httpOutputProjection: [this.actionCommandOptions.httpServiceOptions.outputProjection, Validators.required],
            httpSuccessCode: [this.actionCommandOptions.httpServiceOptions.httpSuccessCode, Validators.required]
        })
    }

    openQueryHint() {
        let query = '{\r\n  \"$query\":{\r\n    \"{{options.entityname}}\":[\r\n        {\r\n          \"$match\": {\r\n            \"_id\": \"ObjectId(\'{{data.id}}\')\"\r\n          }\r\n        }\r\n      ]\r\n  }\r\n}'
        this.hintText = query
        this.isHintClicked = true
    }

    openInsertHint() {
        let insert = '{\"$insert\":{\"{{options.entityname}}\":{ \"$data\": \"{{data}}\"}}}'
        this.hintText = insert
        this.isHintClicked = true
    }

    openUpdateHint() {
        let update = '{\r\n  \"$update\": {\r\n    \"{{options.entityname}}\": {\r\n      \"$data\": \"{{data}}\",\r\n      \"$where\": {\r\n        \"_id\": \"ObjectId(\'{{data.id}}\')\"\r\n      }\r\n    }\r\n  }\r\n}'
        this.hintText = update
        this.isHintClicked = true
    }

    openUpdatePartsHint(){
        let updatePart = '{\r\n  \"$update\": {\r\n    \"{{options.entityname}}\": {\r\n      \"$data\": {\r\n        \"name\": \"{{data.name}}\"\r\n      },\r\n      \"$where\": {\r\n        \"_id\": \"ObjectId(\'{{data.id}}\')\"\r\n      }\r\n    }\r\n  }\r\n}'
        this.hintText = updatePart
        this.isHintClicked = true
    }

    openDeleteHint() {
        let deleteText = '{\r\n  \"$delete\":{\r\n    \"{{options.entityname}}\": {\r\n      \"$where\": {\r\n        \"_id\": \"{{data.id}}\"\r\n      }\r\n    }\r\n  }\r\n}'
        this.hintText = deleteText
        this.isHintClicked = true
    }

    copy(){
        this.shortcutUtil.toastMessage('Copied content', ToastType.Info)
        this.clipboardService.copyFromContent(this.hintText)
    }

    isValid(){
        switch(this.currentActionType){
            case ActionType.ExecuteDatabase:
                return this.databaseOptionsForm.valid && this.notificationOptionsForm.valid
            case ActionType.CallHttpService:
                return this.httpOptionsForm.valid && this.notificationOptionsForm.valid
            case ActionType.Redirect:
                return this.redirectionOptionsForm.valid
        }
    }

    get(): ActionCommandOptions{
        switch(this.currentActionType){
            case ActionType.ExecuteDatabase:
                let databaseFormValue = this.databaseOptionsForm.value
                return {
                    isEnable: this.isEnable,
                    actionType: this.currentActionType,
                    databaseOptions: {
                        databaseConnectionId: databaseFormValue.databaseConnectionId,
                        query: databaseFormValue.query                        
                    },
                    notificationOptions: this.getNotification()
                }
            case ActionType.CallHttpService:
                let httpFormValue = this.httpOptionsForm.value
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
                    notificationOptions: this.getNotification()
                }            
            case ActionType.Redirect:
                let redirectFormValue = this.redirectionOptionsForm.value
                return {
                    isEnable: this.isEnable,
                    actionType: this.currentActionType,
                    redirectOptions: {
                        redirectType: redirectFormValue.redirectionType,
                        redirectUrl: redirectFormValue.redirectUrl,
                        isSameDomain: redirectFormValue.isSameDomain,
                        passParams: redirectFormValue.passParams,
                        targetPageId: redirectFormValue.targetPageId
                    }
                }
        }
    }

    private getNotification(): NotificationOptions{
        let notificationFormValues =  this.notificationOptionsForm.value
        return {
            completeMessage: notificationFormValues.completeMessage,
            failedMessage: notificationFormValues.failedMessage
        }
    }
}
