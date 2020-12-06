import { Component, OnInit, ViewChild, Input, Output, EventEmitter, AfterViewInit } from '@angular/core';
import { JsonEditorComponent, JsonEditorOptions } from 'ang-jsoneditor';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Observable, BehaviorSubject } from 'rxjs';
 
import { NGXLogger } from 'ngx-logger';
import StringUtils from 'app/core/utils/string-util';
import { MatDialog } from '@angular/material/dialog';
import { ParamsDialogComponent } from './params-dialog.component';
import { DynamicListClient, DatabasesClient, EntitySchemasClient, DatabaseConnection, EntitySchema, Parameter, ColumnDef, CommandButtonInList, CommandPositionType, FilledParameter, ColumnField, ActionType, SharedDatabaseOptions, ExecuteParamModel } from 'services/portal.service';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { ObjectUtils } from 'app/core/utils/object-util';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'let-database-opt',
    templateUrl: './database-opt.component.html'
})
export class DatabaseOptionComponent implements OnInit, AfterViewInit {

    constructor(
        private translate: TranslateService,
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
    @ViewChild('jsonEditorQuery', { static: true }) editor: JsonEditorComponent;
    jsonOptions = new JsonEditorOptions();
    queryJsonData: any = {};
    isJsonEditorValid = true;

    @Input()
    heading: string

    @Input()
    databaseOptions: SharedDatabaseOptions

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
    changed = new EventEmitter<SharedDatabaseOptions>();

    databaseConnections$: Observable<Array<DatabaseConnection>>;
    databaseConnections: Array<DatabaseConnection>
    selectedDatabaseConnection: DatabaseConnection
    ismongodb = false
    isReadyToRender = false

    code = ''
    entities: BehaviorSubject<Array<EntitySchema>> = new BehaviorSubject([]);
    shallowedEntitySchemas: Array<EntitySchema>;

    isRefreshClicked = false
    isAutoPopulate = false

    isEnablePopulate = false

    private params: ExecuteParamModel[] = []
    ngAfterViewInit(): void {

    }

    ngOnInit(): void {
        this.databaseConnections$ = this.databaseClient.getAll()
        this.jsonOptions.mode = 'code';
        this.jsonOptions.onChange = () => {
            try {
                this.onJsonEditorChange(this.editor.get())
                this.isJsonEditorValid = true
            }
            catch {
                this.isJsonEditorValid = false;
            }
        }
        const subscriber = this.databaseConnections$.subscribe(res => {
            this.databaseConnections = res

            if (this.databaseOptions.databaseConnectionId) {
                this.selectedDatabaseConnection = this.databaseConnections.find(a => a.id == this.databaseOptions.databaseConnectionId)
                this.ismongodb = this.selectedDatabaseConnection.databaseConnectionType == 'mongodb'
            }

            this.isReadyToRender = true
            if (this.ismongodb) {

                if(ObjectUtils.isNotNull(this.databaseOptions.query)){
                    this.queryJsonData = JSON.parse(this.databaseOptions.query.replace(/(\r\n|\n|\r)/gm,' '))
                }
                else{
                    this.queryJsonData = {}
                }

                this.logger.debug('current json data', this.queryJsonData)
                if (this.isEditMode) {
                    this.queryJsonData = JSON.parse(this.databaseOptionForm.get('query').value)
                }
            }
            else {
                this.code = this.databaseOptions.query
            }

            if (this.isEditMode) {
                this.entityClient.getAllFromOneDatabase(this.databaseOptionForm.get('databaseId').value).subscribe(result => {
                    this.shallowedEntitySchemas = result
                    this.entities.next(result)
                })

                this.enablePopulateButton()
            }

            subscriber.unsubscribe()
        })

        this.databaseOptionForm = this.fb.group({
            databaseId: [this.databaseOptions.databaseConnectionId, Validators.required],
            entityName: [this.databaseOptions.entityName],
            query: [this.databaseOptions.query]
        })

        this.onValueChanges()
    }

    private enablePopulateButton() {
        if (ObjectUtils.isNotNull(this.databaseOptionForm.get('query').value)) {
            this.isEnablePopulate = true
        }
        else {
            this.isEnablePopulate = false
        }
    }

    private onJsonEditorChange($event) {
        this.databaseOptionForm.get('query').setValue(JSON.stringify($event))
    }

    onPopulatingQueryBody() {
        const command = {
            rawQuery: this.databaseOptionForm.get('query').value as string,
            databaseId: this.databaseOptionForm.get('databaseId').value as string
        }

        this.params = []
        const tempDcurly = StringUtils.getContentByDCurlyBrackets(command.rawQuery)
        const callDialog = !!tempDcurly && tempDcurly.length > 0
        if (callDialog) {
            tempDcurly?.forEach((param) => {
                if (this.params.findIndex(a => a.name === param) < 0 && !StringUtils.isAllUpperCase(param))
                    this.params.push({
                        name: param,
                        replaceValue: ''
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

    private extractingQuery(query: string, databaseId: string, params: ExecuteParamModel[]) {
        this.logger.debug('Filled parameters', params)
        this.databaseClient.extractingQuery(databaseId, {
            content: query,
            parameters: params
        }).subscribe(
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
                this.ismongodb = this.selectedDatabaseConnection.databaseConnectionType === 'mongodb'
                this.logger.debug('Selected database option', this.selectedDatabaseConnection)
                this.logger.debug('Is selecting mongodb', this.ismongodb)
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
                this.shallowedEntitySchemas?.forEach((element) => {
                    if (element.name === newValue) {
                        const elementName =
                            this.selectedDatabaseConnection.databaseConnectionType.toLowerCase() === 'postgresql'
                                || this.selectedDatabaseConnection.databaseConnectionType.toLowerCase() === 'sqlserver' ? `"${element.name}"` : `\`${element.name}\``
                        const defaultQuery =
                            this.ismongodb ?
                                `{ "$query": { "${element.name}": [ ] } }` :
                                `SELECT * FROM ${elementName}`

                        if (this.ismongodb) {
                            this.editor.set(JSON.parse(defaultQuery))
                        }
                        else {
                            this.code = defaultQuery
                        }
                        this.databaseOptionForm.get('query').setValue(defaultQuery)

                        if (this.isAutoPopulate) {
                            this.dynamicListClient.extractingQuery({ query: defaultQuery, databaseId: this.databaseOptionForm.get('databaseId').value, parameters: [] }).subscribe(
                                result => {
                                    this.logger.debug('Parsing response', result)
                                    this.afterSelectingEntity.emit(result)
                                },
                                err => {
                                    this.shortcutUtil.toastMessage(this.translate.instant('common.somethingWentWrong'), ToastType.Error)
                                }
                            )
                        }

                        return false;
                    }
                })
            }
        })

        this.databaseOptionForm.get('query').valueChanges.subscribe(newValue => {
            if (!this.ismongodb) {
                this.code = newValue
            }
            this.enablePopulateButton()
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
                keptSameName: false
            }).subscribe(
                result => {
                    this.shallowedEntitySchemas = result;
                    this.entities.next(result)
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
