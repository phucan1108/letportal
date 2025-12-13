import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { JsonEditorComponent, JsonEditorOptions } from 'ang-jsoneditor';
import { Observable } from 'rxjs';
import { DatabaseConnection, DatabasesClient, SharedDatabaseOptions } from 'services/portal.service';
import { ClipboardService } from 'ngx-clipboard';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'let-databaseoptions',
    templateUrl: './databaseoptions.component.html'
})
export class DatabaseOptionsComponent implements OnInit {
    @Input()
    databaseOptions: SharedDatabaseOptions

    @Input()
    options: DatabaseFormOptions

    databaseOptionsForm: FormGroup
    selectedDatabase: DatabaseConnection
    databases: Array<DatabaseConnection>

    isMongoDb = true
    code = ''

    @ViewChild('jsonDataEditor', { static: false }) jsonDataEditor: JsonEditorComponent
    public editorOptions3: JsonEditorOptions = new JsonEditorOptions()
    jsonData: any = {}
    databaseConnections$: Observable<Array<DatabaseConnection>>;
    hintText = ''
    isHintClicked = false

    constructor(
        private translate: TranslateService,
        private fb: FormBuilder,
        private databaseClient: DatabasesClient,
        private clipboardService: ClipboardService,
        private shortcutUtil: ShortcutUtil
    ) { }

    ngOnInit(): void {
        this.initDatabaseOptions()
        this.databaseConnections$ = this.databaseClient.getAll()
        this.databaseConnections$.subscribe(res => {
            this.databases = res
            this.selectedDatabase = this.databaseOptions.databaseConnectionId ? this.databases.find(a => a.id == this.databaseOptions.databaseConnectionId ) : null
            if(this.selectedDatabase){
                this.code = this.selectedDatabase.databaseConnectionType == 'mongodb' ? '' : this.databaseOptions.query
            }
        })
    }

    initDatabaseOptions(){
        if(!this.databaseOptions){
            this.databaseOptions = {
                databaseConnectionId: '',
                query: '',
                entityName: ''
            }
        }

        this.editorOptions3.mode = 'code'
        this.jsonData = this.databaseOptions.query ? JSON.parse(this.databaseOptions.query) : {}
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
            databaseConnectionId: [this.databaseOptions.databaseConnectionId, Validators.required],
            query: [this.databaseOptions.query, Validators.required]
        })

        this.databaseOptionsForm.get('databaseConnectionId').valueChanges.subscribe(newValue => {
            this.selectedDatabase = this.databases.find(a => a.id == newValue)
            if(this.selectedDatabase.databaseConnectionType == 'mongodb'){
                this.isMongoDb = true
            }
            else{
                this.isMongoDb = false
            }
        })
        this.databaseOptionsForm.get('query').valueChanges.subscribe(newValue => {
            this.code = newValue
        })
    }

    allowDisplayHint(hint: string){
        return this.options.allowHints.indexOf(hint) > -1
    }

    openQueryHint() {
        const query = this.isMongoDb ?
                    '{\r\n  \"$query\":{\r\n    \"{{options.entityname}}\":[\r\n        {\r\n          \"$match\": {\r\n            \"_id\": \"ObjectId(\'{{data.id}}\')\"\r\n          }\r\n        }\r\n      ]\r\n  }\r\n}'
                    : 'Select * From {Your_table} Where id={{data.id}}'
        this.hintText = query
        this.isHintClicked = true
    }

    openInsertHint() {
        const insert =  this.isMongoDb ? '{\"$insert\":{\"{{options.entityname}}\":{ \"$data\": \"{{data}}\"}}}' :
                        'Insert into {Your_table}(id) values ({{data.id}})'
        this.hintText = insert
        this.isHintClicked = true
    }

    openUpdateHint() {
        const update = this.isMongoDb ? '{\r\n  \"$update\": {\r\n    \"{{options.entityname}}\": {\r\n      \"$data\": \"{{data}}\",\r\n      \"$where\": {\r\n        \"_id\": \"ObjectId(\'{{data.id}}\')\"\r\n      }\r\n    }\r\n  }\r\n}' :
                        'Update {Your_table} Set name={{data.name}} Where id={{data.id}}'
        this.hintText = update
        this.isHintClicked = true
    }

    openUpdatePartsHint(){
        const updatePart = this.isMongoDb ? '{\r\n  \"$update\": {\r\n    \"{{options.entityname}}\": {\r\n      \"$data\": {\r\n        \"name\": \"{{data.name}}\"\r\n      },\r\n      \"$where\": {\r\n        \"_id\": \"ObjectId(\'{{data.id}}\')\"\r\n      }\r\n    }\r\n  }\r\n}'
                        : 'Update {Your_table} Set name={{data.name}} Where id={{data.id}}'
        this.hintText = updatePart
        this.isHintClicked = true
    }

    openDeleteHint() {
        const deleteText = this.isMongoDb ? '{\r\n  \"$delete\":{\r\n    \"{{options.entityname}}\": {\r\n      \"$where\": {\r\n        \"_id\": \"{{data.id}}\"\r\n      }\r\n    }\r\n  }\r\n}'
                        : 'Delete From {Your_table} Where id={{data.id}}'
        this.hintText = deleteText
        this.isHintClicked = true
    }

    copy(){
        this.shortcutUtil.toastMessage(this.translate.instant('common.copiedContent'), ToastType.Info)
        this.clipboardService.copyFromContent(this.hintText)
    }

    get(): SharedDatabaseOptions{
        if(this.databaseOptionsForm.valid){
            const formValues = this.databaseOptionsForm.value
            return {
                databaseConnectionId: formValues.databaseConnectionId,
                query: formValues.query
            }
        }
        return null
    }

    isValid(): boolean{
        return this.databaseOptionsForm.valid
    }
}

export interface DatabaseFormOptions{
    allowHints: string[]
}