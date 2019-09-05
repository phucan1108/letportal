import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { JsonEditorComponent, JsonEditorOptions } from 'ang-jsoneditor';
import { Observable } from 'rxjs';
import { DatabaseConnection, DatabasesClient, DatabaseOptions } from 'services/portal.service';
import { ClipboardService } from 'ngx-clipboard';
import { ShortcutUtil } from 'app/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/shared/components/shortcuts/shortcut.models';

@Component({
    selector: 'let-databaseoptions',
    templateUrl: './databaseoptions.component.html'
})
export class DatabaseOptionsComponent implements OnInit {
    @Input()
    databaseOptions: DatabaseOptions

    @Input()
    options: DatabaseFormOptions

    databaseOptionsForm: FormGroup

    @ViewChild('jsonDataEditor') jsonDataEditor: JsonEditorComponent
    public editorOptions3: JsonEditorOptions = new JsonEditorOptions()
    jsonData: any = {}
    databaseConnections: Observable<Array<DatabaseConnection>>;
    hintText = ''
    isHintClicked = false
    
    constructor(
        private fb: FormBuilder,
        private databaseClient: DatabasesClient,
        private clipboardService: ClipboardService,
        private shortcutUtil: ShortcutUtil
    ) { }

    ngOnInit(): void { 
        this.initDatabaseOptions()
        this.databaseConnections = this.databaseClient.getAll()
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

        this.databaseOptionsForm.valueChanges.subscribe(newValue => {

        })
    }

    allowDisplayHint(hint: string){
        return this.options.allowHints.indexOf(hint) > -1
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
        this.shortcutUtil.notifyMessage('Copied content', ToastType.Info)
        this.clipboardService.copyFromContent(this.hintText)
    }

    get(): DatabaseOptions{
        if(this.databaseOptionsForm.valid){
            let formValues = this.databaseOptionsForm.value
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