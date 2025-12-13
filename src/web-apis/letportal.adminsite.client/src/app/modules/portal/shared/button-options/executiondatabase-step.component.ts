import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { DatabaseConnection, DatabaseExecutionStep } from 'services/portal.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { JsonEditorComponent, JsonEditorOptions } from 'ang-jsoneditor';
import { ClipboardService } from 'ngx-clipboard';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { NGXLogger } from 'ngx-logger';
import { ObjectUtils } from 'app/core/utils/object-util';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'let-executiondatabase-step',
    templateUrl: './executiondatabase-step.component.html'
})
export class ExecutionDatabaseStepComponent implements OnInit {
    @Input()
    databaseConnections: Array<DatabaseConnection>

    @Input()
    step: DatabaseExecutionStep

    @Output()
    changed = new EventEmitter<any>()

    databaseOptionsForm: FormGroup

    @ViewChild('jsonDataEditor', { static: false }) jsonDataEditor: JsonEditorComponent
    public editorOptions3: JsonEditorOptions = new JsonEditorOptions()
    jsonData: any = {}
    hintText = ''
    isHintClicked = false

    selectedDatabaseConnection: DatabaseConnection
    ismongodb = false
    isReadyToRender = false

    code = ''

    constructor(
        private translate: TranslateService,
        private fb: FormBuilder,
        private clipboardService: ClipboardService,
        private shortcutUtil: ShortcutUtil,
        private logger: NGXLogger
    ) { }

    ngOnInit(): void {
        this.logger.debug('Init execution database step')

        this.editorOptions3.mode = 'code'
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
            databaseConnectionId: [this.step.databaseConnectionId, Validators.required],
            query: [this.step.executeCommand, Validators.required],
            dataLoopKey: [this.step.dataLoopKey]
        })

        if(ObjectUtils.isNotNull(this.step.databaseConnectionId)){
            this.selectedDatabaseConnection = this.databaseConnections.find(a => a.id == this.step.databaseConnectionId)
            this.ismongodb = this.selectedDatabaseConnection.databaseConnectionType == 'mongodb'
        }

        if(this.ismongodb){
            this.jsonData = this.step.executeCommand ? JSON.parse(this.step.executeCommand) : {}
        }
        else{
            this.code = this.step.executeCommand
        }

        this.databaseOptionsForm.get('databaseConnectionId').valueChanges.subscribe(newValue => {
                this.selectedDatabaseConnection = this.databaseConnections.find(a => a.id == newValue)
                this.ismongodb = this.selectedDatabaseConnection.databaseConnectionType == 'mongodb'

                this.databaseOptionsForm.get('query').setValue('')
                this.jsonData = {}
        })

        this.databaseOptionsForm.get('query').valueChanges.subscribe(newValue => {
            if(!this.ismongodb){
                this.code = newValue
            }
        })

        this.databaseOptionsForm.valueChanges.subscribe(newValue => {
            this.changed.emit(this.getStep())
        })
        this.isReadyToRender = true
    }

    openQueryHint() {
        const query = this.ismongodb ?
                    '{\r\n  \"$query\":{\r\n    \"{{options.entityname}}\":[\r\n        {\r\n          \"$match\": {\r\n            \"_id\": \"ObjectId(\'{{data.id}}\')\"\r\n          }\r\n        }\r\n      ]\r\n  }\r\n}'
                    : 'Select * From {Your_table} Where id={{data.id}}'
        this.hintText = query
        this.isHintClicked = true
    }

    openInsertHint() {
        const insert =  this.ismongodb ? '{\"$insert\":{\"{{options.entityname}}\":{ \"$data\": \"{{data}}\"}}}' :
                        'Insert into {Your_table}(id) values ({{data.id}})'
        this.hintText = insert
        this.isHintClicked = true
    }

    openUpdateHint() {
        const update = this.ismongodb ? '{\r\n  \"$update\": {\r\n    \"{{options.entityname}}\": {\r\n      \"$data\": \"{{data}}\",\r\n      \"$where\": {\r\n        \"_id\": \"ObjectId(\'{{data.id}}\')\"\r\n      }\r\n    }\r\n  }\r\n}' :
                        'Update {Your_table} Set name={{data.name}} Where id={{data.id}}'
        this.hintText = update
        this.isHintClicked = true
    }

    openUpdatePartsHint(){
        const updatePart = this.ismongodb ? '{\r\n  \"$update\": {\r\n    \"{{options.entityname}}\": {\r\n      \"$data\": {\r\n        \"name\": \"{{data.name}}\"\r\n      },\r\n      \"$where\": {\r\n        \"_id\": \"ObjectId(\'{{data.id}}\')\"\r\n      }\r\n    }\r\n  }\r\n}'
                        : 'Update {Your_table} Set name={{data.name}} Where id={{data.id}}'
        this.hintText = updatePart
        this.isHintClicked = true
    }

    openDeleteHint() {
        const deleteText = this.ismongodb ? '{\r\n  \"$delete\":{\r\n    \"{{options.entityname}}\": {\r\n      \"$where\": {\r\n        \"_id\": \"{{data.id}}\"\r\n      }\r\n    }\r\n  }\r\n}'
                        : 'Delete From {Your_table} Where id={{data.id}}'
        this.hintText = deleteText
        this.isHintClicked = true
    }

    copy(){
        this.shortcutUtil.toastMessage(this.translate.instant('common.copiedContent'), ToastType.Info)
        this.clipboardService.copyFromContent(this.hintText)
    }

    private getStep(): DatabaseExecutionStep{
        const formValues = this.databaseOptionsForm.value
        return {
            databaseConnectionId: formValues.databaseConnectionId,
            executeCommand: formValues.query,
            dataLoopKey: formValues.dataLoopKey
        }
    }
}
