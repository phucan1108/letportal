import { Component, OnInit, Input, Output, EventEmitter, Inject, ViewChild } from '@angular/core';
import { DatasourceOptions, DatasourceControlType, DatabaseConnection, EntitySchema, DatabasesClient, EntitySchemasClient } from 'services/portal.service';
import { NGXLogger } from 'ngx-logger';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { StaticResources } from 'portal/resources/static-resources';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ControlsGridComponent } from 'portal/modules/builder/components/standard/controls/controls-grid.component';
import { JsonEditorOptions, JsonEditorComponent } from 'ang-jsoneditor';
import { Observable, BehaviorSubject } from 'rxjs';
 

@Component({
    selector: 'let-datasourceopts',
    templateUrl: './datasourceopts.component.html',
    styleUrls: ['./datasourceopts.component.scss']
})
export class DatasourceOptionsDialogComponent implements OnInit {

    @ViewChild('jsonEditorQuery', { static: false }) editor: JsonEditorComponent;
    jsonOptions = new JsonEditorOptions();
    queryJsonData: any = {};
    isJsonEditorValid = true;

    @ViewChild('jsonStaticEditorQuery', { static: false }) staticEditor: JsonEditorComponent;
    jsonStaticOptions = new JsonEditorOptions();
    staticJsonData: any = {};
    isJsonStaticEditorValid = true;

    @ViewChild('jsonBodyEditorQuery', { static: false }) httpBodyEditor: JsonEditorComponent;
    jsonHttpBodyOptions = new JsonEditorOptions();
    httpBodyJsonData: any = {};
    isJsonHttpBodyEditorValid = true;

    datasourceOption: DatasourceOptions

    datasourceForm: FormGroup

    datasourceType = DatasourceControlType
    _datasourceTypes = StaticResources.datasourceTypes()
    _httpMethods = ['GET', 'POST', 'PUT', 'DELETE']
    databaseConnections$: Observable<Array<DatabaseConnection>>;
    entities: BehaviorSubject<Array<EntitySchema>> = new BehaviorSubject([]);
    shallowedEntitySchemas: Array<EntitySchema>;

    isRefreshClicked = false

    constructor(
        private fb: FormBuilder,
        private databaseClient: DatabasesClient,
        private entityClient: EntitySchemasClient,
        private logger: NGXLogger,
        public dialogRef: MatDialogRef<ControlsGridComponent>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: any
    ) {
        this.datasourceOption = {
            ...this.data.datasourceOption
        }

        this.initEditor()
        this.initDatasourceForm()
    }

    ngOnInit(): void {
        this.databaseConnections$ = this.databaseClient.getAll()
    }

    initEditor(){
        if(!this.datasourceOption.databaseOptions){
            this.datasourceOption.databaseOptions = {
                databaseConnectionId: '',
                entityName: '',
                query: ''
            }
        }

        if(!this.datasourceOption.datasourceStaticOptions){
            this.datasourceOption.datasourceStaticOptions = {
                jsonResource: ''
            }
        }

        if(!this.datasourceOption.httpServiceOptions){
            this.datasourceOption.httpServiceOptions = {
                httpMethod: 'Get',
                httpServiceUrl: '',
                httpSuccessCode: '200',
                jsonBody: '',
                outputProjection: ''
            }
        }
        this.jsonOptions.mode = 'code'
        this.queryJsonData = this.datasourceOption.databaseOptions.query ? JSON.parse(this.datasourceOption.databaseOptions.query) : {}
        this.jsonOptions.onChange = () => {
            try
            {
                this.jsonEditorQueryChanged(this.editor.get())
            }
            catch{

            }
        }

        this.jsonStaticOptions.mode = 'code'
        this.staticJsonData = this.datasourceOption.datasourceStaticOptions.jsonResource ? JSON.parse(this.datasourceOption.datasourceStaticOptions.jsonResource) : {}
        this.jsonStaticOptions.onChange = () => {
            try{
                this.jsonEditorChanged(this.staticEditor.get())
            }
            catch{

            }
        }

        this.jsonHttpBodyOptions.mode = 'code'
        this.httpBodyJsonData = this.datasourceOption.httpServiceOptions.jsonBody ? JSON.parse(this.datasourceOption.httpServiceOptions.jsonBody) : {}
        this.jsonHttpBodyOptions.onChange = () => {
            try{
                this.jsonWSBodyChanged(this.httpBodyEditor.get())
            }
            catch{

            }
        }
    }

    initDatasourceForm() {
        this.datasourceForm = this.fb.group({
            type: [this.datasourceOption.type, Validators.required],
            jsonquery: [this.datasourceOption.datasourceStaticOptions.jsonResource],
            databaseId: [this.datasourceOption.databaseOptions.databaseConnectionId, Validators.required],
            entityName: [this.datasourceOption.databaseOptions.entityName],
            query: [this.datasourceOption.databaseOptions.query],
            httpServiceUrl: [this.datasourceOption.httpServiceOptions.httpServiceUrl],
            httpMethod: [this.datasourceOption.httpServiceOptions.httpMethod],
            jsonBody: [this.datasourceOption.httpServiceOptions.jsonBody],
            httpOutputProjection: [this.datasourceOption.httpServiceOptions.outputProjection]
        })

        if(this.datasourceOption.databaseOptions.entityName){
            this.entityClient.getAllFromOneDatabase(this.datasourceOption.databaseOptions.entityName).subscribe(entitySchemas => {
                this.shallowedEntitySchemas = entitySchemas;
                this.entities.next(this.shallowedEntitySchemas)
            })
        }

        this.datasourceForm.get('databaseId').valueChanges.subscribe(newValue => {
            if (newValue) {
                this.entityClient.getAllFromOneDatabase(newValue).subscribe(entitySchemas => {
                    this.shallowedEntitySchemas = entitySchemas;
                    this.entities.next(this.shallowedEntitySchemas)
                })
            }
            else {
                this.datasourceForm.get('entityName').setValue('');
            }
        })

        // Auto-populated the filter options
        this.datasourceForm.get('entityName').valueChanges.subscribe(newValue => {
            if (newValue) {
                this.shallowedEntitySchemas?.forEach((element) => {
                    if (element.name === newValue) {
                        const defaultQuery = `{ "$query": { "${element.name}": [ ] } }`
                        this.datasourceForm.get('query').setValue(defaultQuery)
                        this.queryJsonData = JSON.parse(defaultQuery)
                    }
                })
            }
        })
    }

    combineDatasourceOption(): DatasourceOptions{
        const formValues = this.datasourceForm.value
        return {
            type: formValues.type,
            datasourceStaticOptions: {
                jsonResource: formValues.jsonquery
            },
            databaseOptions:{
                databaseConnectionId: formValues.databaseId,
                entityName: formValues.entityName,
                query: formValues.query
            },
            httpServiceOptions:{
                httpMethod: formValues.httpMethod,
                httpServiceUrl: formValues.httpServiceUrl,
                httpSuccessCode: '200',
                outputProjection: formValues.httpOutputProjection,
                jsonBody: formValues.jsonBody
            }
        }
    }

    onSubmit() {
        this.dialogRef.close(this.combineDatasourceOption())
    }
    refreshEntities() {
        if (this.datasourceForm.get('databaseId').value) {
            this.isRefreshClicked = false
            this.entityClient.flushOneDatabase({
                databaseId: this.datasourceForm.get('databaseId').value as string,
                keptSameName: true
            }).subscribe(
                result => {
                    this.entityClient.getAllFromOneDatabase(this.datasourceForm.get('databaseId').value).subscribe(result => {
                        this.entities.next(result)
                    })
                },
                err => {

                }
            )
        }
        else {
            this.isRefreshClicked = true
        }
    }

    jsonEditorChanged($event) {
        this.datasourceForm.get('jsonquery').setValue(JSON.stringify($event))
    }

    jsonEditorQueryChanged($event) {
        this.datasourceForm.get('query').setValue(JSON.stringify($event))
    }

    jsonWSBodyChanged($event) {
        this.datasourceForm.get('jsonBody').setValue(JSON.stringify($event))
    }
}
