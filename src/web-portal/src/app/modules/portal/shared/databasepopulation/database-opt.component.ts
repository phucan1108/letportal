import { Component, OnInit, ViewChild, Input, Output, EventEmitter, AfterViewInit } from '@angular/core';
import { JsonEditorComponent, JsonEditorOptions } from 'ang-jsoneditor';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Observable, BehaviorSubject } from 'rxjs';
import * as _ from 'lodash';
import { NGXLogger } from 'ngx-logger';
import StringUtils from 'app/core/utils/string-util';
import { MatDialog } from '@angular/material';
import { ParamsDialogComponent } from './params-dialog.component';
import { DynamicListClient, DatabasesClient, EntitySchemasClient, DatabaseConnection, EntitySchema, Parameter, ColumndDef, CommandButtonInList, CommandPositionType, FilledParameter, ColumnField, ActionType, DatabaseOptions } from 'services/portal.service';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';

@Component({
    selector: 'let-database-opt',
    templateUrl: './database-opt.component.html'
})
export class DatabaseOptionComponent implements OnInit, AfterViewInit {
    ngAfterViewInit(): void {

    }
    @ViewChild('jsonEditorQuery') editor: JsonEditorComponent;
    jsonOptions = new JsonEditorOptions();
    queryJsonData: any = {};
    isJsonEditorValid = true;

    @Input()
    heading: string

    @Input()
    databaseOptions: DatabaseOptions

    databaseOptionForm: FormGroup

    @Input()
    isEditMode: boolean

    @Output()
    afterPopulatingQuery = new EventEmitter<any>();

    @Output()
    afterSelectingEntity = new EventEmitter<any>();

    @Output()
    afterSelectingEntityName = new EventEmitter<any>();

    @Output()
    afterPopulatingParams = new EventEmitter<any>();

    @Output()
    changed = new EventEmitter<DatabaseOptions>();

    databaseConnections$: Observable<Array<DatabaseConnection>>;
    databaseConnections: Array<DatabaseConnection>
    selectedDatabaseConnection: DatabaseConnection
    ismongodb = false
    isReadyToRender = false

    code = ''
    entities: BehaviorSubject<Array<EntitySchema>> = new BehaviorSubject([]);
    shallowedEntitySchemas: Array<EntitySchema>;

    isRefreshClicked = false

    private params: FilledParameter[] = []

    constructor(
        private dynamicListClient: DynamicListClient,
        private databaseClient: DatabasesClient,
        private entityClient: EntitySchemasClient,
        private shortcutUtil: ShortcutUtil,
        private logger: NGXLogger,
        private dialog: MatDialog,
        private fb: FormBuilder
    ) {
        this.databaseOptionForm = this.fb.group({})
    }

    ngOnInit(): void {
        this.databaseConnections$ = this.databaseClient.getAll()
        this.databaseConnections$.subscribe(res => {
            this.databaseConnections = res

            if (this.databaseOptions.databaseConnectionId) {
                this.selectedDatabaseConnection = this.databaseConnections.find(a => a.id == this.databaseOptions.databaseConnectionId)
                this.ismongodb = this.selectedDatabaseConnection.databaseConnectionType == 'mongodb'
            }

            this.isReadyToRender = true

            if (this.ismongodb) {
                this.jsonOptions.mode = 'code';
                this.queryJsonData = this.databaseOptions.query ? JSON.parse(this.databaseOptions.query) : {}
                this.jsonOptions.onChange = () => {
                    try {
                        this.onJsonEditorChange(this.editor.get())
                        this.isJsonEditorValid = true
                    }
                    catch {
                        this.isJsonEditorValid = false;
                    }
                }
                this.logger.debug('current json data', this.queryJsonData)
                if (this.isEditMode) {
                    this.queryJsonData = JSON.parse(this.databaseOptionForm.get('query').value)
                }
            }
            else {
                this.code = this.databaseOptions.query
            }            
             
            if(this.isEditMode){
                this.entityClient.getAllFromOneDatabase(this.databaseOptionForm.get('databaseId').value).subscribe(result => {
                    this.entities.next(result)
                })
            }
        })

        this.databaseOptionForm = this.fb.group({
            databaseId: [this.databaseOptions.databaseConnectionId, Validators.required],
            entityName: [this.databaseOptions.entityName],
            query: [this.databaseOptions.query]
        })

        this.onValueChanges()
    }

    private onJsonEditorChange($event) {
        this.databaseOptionForm.get('query').setValue(JSON.stringify($event))
    }

    onPopulatingQueryBody() {
        let command = {
            rawQuery: this.databaseOptionForm.get('query').value as string,
            databaseId: this.databaseOptionForm.get('databaseId').value as string
        }

        this.params = []
        let tempDcurly = StringUtils.getContentByDCurlyBrackets(command.rawQuery)
        let callDialog = !!tempDcurly && tempDcurly.length > 0
        if (callDialog) {
            _.forEach(tempDcurly, (param) => {
                this.params.push({
                    name: param,
                    value: ''
                })
            })

            const dialogRef = this.dialog.open(ParamsDialogComponent, {
                data: {
                    params: this.params
                }
            });
            dialogRef.afterClosed().subscribe(result => {
                if (result) {
                    this.params = result
                    this.afterPopulatingParams.emit(this.params)
                    this.extractingQuery(command.rawQuery, command.databaseId, this.params)
                }
            })
        }
        else {
            this.extractingQuery(command.rawQuery, command.databaseId, this.params)
        }

    }

    private extractingQuery(query: string, databaseId: string, params: FilledParameter[]) {
        _.each(params, param => {
            query = query.replace("{{" + param.name + "}}", param.value);
        })
        this.databaseClient.extractingQuery(databaseId, query).subscribe(
            result => {
                this.afterPopulatingQuery.emit(result)
            },
            err => {

            }
        )
    }

    private onValueChanges() {

        // Auto-populated the entity fields
        this.databaseOptionForm.get('databaseId').valueChanges.subscribe(newValue => {
            if (newValue) {
                this.selectedDatabaseConnection = this.databaseConnections.find(a => a.id == newValue)
                this.ismongodb = this.selectedDatabaseConnection.databaseConnectionType == 'mongodb'
                console.log('Before Calling')
                this.entityClient.getAllFromOneDatabase(newValue).subscribe(entitySchemas => {
                    this.shallowedEntitySchemas = entitySchemas;
                    this.entities.next(this.shallowedEntitySchemas)
                })
                this.changed.emit({
                    databaseConnectionId: newValue,
                    entityName: this.databaseOptionForm.get('entityName').value,
                    query: this.databaseOptionForm.get('query').value
                })
            }
            else {
                this.databaseOptionForm.get('entityName').setValue('');
            }
        })

        // Auto-populated the filter options
        this.databaseOptionForm.get('entityName').valueChanges.subscribe(newValue => {
            this.logger.debug('Entity changed', newValue)
            if (newValue) {
                this.afterSelectingEntityName.emit(newValue)
                _.forEach(this.shallowedEntitySchemas, (element) => {
                    if (element.name === newValue) {
                        let defaultQuery = this.ismongodb ? `{ "$query": { "${element.name}": [ ] } }` : `SELECT * FROM ${element.name}`

                        if(this.ismongodb){
                            this.editor.set(JSON.parse(defaultQuery))
                        }
                        else{
                            this.code = defaultQuery
                        }
                        this.databaseOptionForm.get('query').setValue(defaultQuery)
                        this.dynamicListClient.extractingQuery({ query: defaultQuery, databaseId: this.databaseOptionForm.get('databaseId').value, parameters: [] }).subscribe(
                            result => {
                                this.logger.debug('Parsing response', result)
                                this.afterSelectingEntity.emit(result)
                            },
                            err => {
                                this.shortcutUtil.notifyMessage('Oops, we cannot populate a query, please check syntax again.', ToastType.Error)
                            }
                        )

                        return false;
                    }
                })
            }
        })

        this.databaseOptionForm.get('query').valueChanges.subscribe(newValue => {
            if(!this.ismongodb){
                this.code = newValue
            }
        })

        this.databaseOptionForm.valueChanges.subscribe(newValue => {
            this.notifyChanged()
        })
    }

    notifyChanged() {
        this.changed.emit({
            databaseConnectionId: this.databaseOptionForm.get('databaseId').value,
            entityName: this.databaseOptionForm.get('entityName').value,
            query: this.databaseOptionForm.get('query').value
        })
    }

    refreshEntities() {
        if (this.databaseOptionForm.valid) {
            this.isRefreshClicked = false
            this.entityClient.flushOneDatabase({
                databaseId: this.databaseOptionForm.get('databaseId').value as string,
                keptSameName: true
            }).subscribe(
                result => {
                    this.entityClient.getAllFromOneDatabase(this.databaseOptionForm.get('databaseId').value).subscribe(result => {
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
}
