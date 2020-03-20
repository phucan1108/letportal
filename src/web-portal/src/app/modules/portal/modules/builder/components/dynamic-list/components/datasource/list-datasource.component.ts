import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ExtractingSchemaQueryModel, ColumnField, ColumndDef, CommandButtonInList, ActionType, CommandPositionType, FilledParameter, Parameter, DatabaseOptions, FieldValueType, DatasourceControlType } from 'services/portal.service';
import * as _ from 'lodash';
import { Constants } from 'portal/resources/constants';
import { NGXLogger } from 'ngx-logger';
import { Guid } from 'guid-typescript';

@Component({
    selector: 'let-list-datasource',
    templateUrl: './list-datasource.component.html'
})
export class ListDatasourceComponent implements OnInit {
    heading = 'Database Connection Info:'
    format = /[ !@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/;
    @Input()
    databaseOptions: DatabaseOptions

    @Input()
    isEditMode: boolean

    @Output()
    afterSelectingEntity = new EventEmitter<any>();

    @Output()
    afterPopulatingQuery = new EventEmitter<any>();

    @Output()
    changed = new EventEmitter<DatabaseOptions>()

    private params: FilledParameter[] = []
    private selectedEntityname: string

    constructor(
        private fb: FormBuilder,
        private logger: NGXLogger
    ) {
    }

    ngOnInit(): void { }

    onPopulatingQuery($event: ExtractingSchemaQueryModel) {
        const columnDefs = this.populateColumnDefs($event.columnFields)
        const parameters: Array<Parameter> = []
        _.forEach(this.params, param => {
            parameters.push({ name: param.name })
        })
        this.afterPopulatingQuery.emit({
            filters: [],
            columnDefs,
            parameters
        })

    }

    onSelectingEntity($event: ExtractingSchemaQueryModel) {
        const columnDefs = this.populateColumnDefs($event.columnFields)
        const commands = this.populateDefaultCommandsForEntityExecutionType(this.selectedEntityname)
        this.afterSelectingEntity.emit({
            filters: [],
            columnDefs,
            commands
        })
    }
    databaseOptChanged($event: DatabaseOptions){
        this.changed.emit($event)
    }
    onExtractingParam($event: FilledParameter[]) {
        this.params = $event
    }

    onSelectingEntityName($event: string) {
        this.selectedEntityname = $event
    }

    private convertFieldType(fieldType: string): FieldValueType{
        switch(fieldType){
            case 'string':
                return FieldValueType.Text
            case 'boolean':
                return FieldValueType.Slide
            case 'number':
                return FieldValueType.Number
            case 'datetime':
                return FieldValueType.DatePicker
            case 'list':
                return FieldValueType.Text
            case 'document':
                return FieldValueType.Text
        }
    }

    populateColumnDefs(columnFields: ColumnField[]): Array<ColumndDef> {
        const columnTempDefs = new Array<ColumndDef>();

        _.forEach(columnFields, (element) => {
            // By default, we need to remove all fields that contain id or special char
            const fieldName = element.name.toLowerCase()
            if (fieldName.indexOf('id') < 2
                && !this.format.test(fieldName)
                && !this.ignoreBsonFieldTypes(element.fieldType)) {
                // Ignore some special fields
                const columnDef: ColumndDef = {
                    name: (element.name === '_id' || element.name === 'id') ? 'id' : element.name,
                    displayName: this.getBeautifulName(element.displayName),
                    displayFormat: '{0}',
                    allowSort: !(element.name === '_id' || element.name === 'id' || (element.name.toLowerCase().indexOf('id') > 0)),
                    displayFormatAsHtml: false,
                    isHidden: (element.name === '_id' || element.name === 'id' || (element.name.toLowerCase().indexOf('id') > 0)),
                    searchOptions: {
                        allowInAdvancedMode: !(element.name === '_id' || element.name === 'id' || (element.name.toLowerCase().indexOf('id') > 0)),
                        allowTextSearch: !(element.name === '_id' || element.name === 'id' || (element.name.toLowerCase().indexOf('id') > 0)),
                        fieldValueType: this.convertFieldType(element.fieldType)
                    },
                    datasourceOptions: {
                        datasourceStaticOptions: {
                            jsonResource: ''
                        },
                        type: DatasourceControlType.StaticResource
                    },
                    htmlFunction: ''
                }

                columnTempDefs.push(columnDef)
            }
            else{
                this.logger.debug('Ignore column name:', fieldName)
            }
        })

        return columnTempDefs;
    }

    populateDefaultCommandsForEntityExecutionType(entityName: string): Array<CommandButtonInList> {
        // Auto-generate Create, Edit, Delete Button
        return [
            {
                id: Guid.create().toString(),
                order: 0,
                name: 'create',
                displayName: 'Create',
                color: 'primary',
                icon: '',
                commandPositionType: CommandPositionType.OutList,
                allowRefreshList: false,
                actionCommandOptions: {
                    actionType: ActionType.Redirect,
                    dbExecutionChains: {
                        steps: []
                    },
                    httpServiceOptions: {
                        httpMethod: 'GET',
                        httpServiceUrl: '',
                        httpSuccessCode: '200',
                        outputProjection: '{}',
                        jsonBody: null
                    },
                    redirectOptions: {
                        redirectUrl: `${Constants.PREFIX_FORM_URL}${entityName}-form`,
                        isSameDomain: true
                    },
                    workflowOptions: {
                        workflowId: '',
                    },
                    notificationOptions: {
                        completeMessage: Constants.COMPLETE_DEFAULT_MESSAGE,
                        failedMessage: Constants.FAILED_DEFAULT_MESSAGE
                    }
                }
            },
            {
                id: Guid.create().toString(),
                order: 1,
                name: 'edit',
                displayName: 'Edit',
                color: 'primary',
                icon: 'create',
                commandPositionType: CommandPositionType.InList,
                allowRefreshList: false,
                actionCommandOptions: {
                    actionType: ActionType.Redirect,
                    dbExecutionChains: {
                        steps: []
                    },
                    httpServiceOptions: {
                        httpMethod: 'GET',
                        httpServiceUrl: '',
                        httpSuccessCode: '200',
                        outputProjection: '{}',
                        jsonBody: null
                    },
                    redirectOptions: {
                        redirectUrl: `${Constants.PREFIX_FORM_URL}${entityName}-form?id={{data.id}}`,
                        isSameDomain: true
                    },
                    notificationOptions: {
                        completeMessage: Constants.COMPLETE_DEFAULT_MESSAGE,
                        failedMessage: Constants.FAILED_DEFAULT_MESSAGE
                    }
                }
            },
            {
                id: Guid.create().toString(),
                order: 2,
                name: 'delete',
                displayName: 'Delete',
                color: 'warn',
                icon: 'delete',
                commandPositionType: CommandPositionType.InList,
                allowRefreshList: false,
                actionCommandOptions: {
                    actionType: ActionType.Redirect,
                    dbExecutionChains: {
                        steps: []
                    },
                    httpServiceOptions: {
                        httpMethod: 'GET',
                        httpServiceUrl: '',
                        httpSuccessCode: '200',
                        outputProjection: '{}',
                        jsonBody: null
                    },
                    redirectOptions: {
                        isSameDomain: true,
                        redirectUrl: `${Constants.PREFIX_FORM_URL}${entityName}-entity?id={{data._id}}`
                    },
                    notificationOptions: {
                        completeMessage: Constants.COMPLETE_DEFAULT_MESSAGE,
                        failedMessage: Constants.FAILED_DEFAULT_MESSAGE
                    }
                }
            }
        ]
    }

    private ignoreBsonFieldTypes(fieldType: string){
        return fieldType == 'list' || fieldType == 'document'
    }

    private getBeautifulName(colName: string){
        const firstLetter = colName[0].toUpperCase()
        const subStr = colName.substr(1)
        const combine = firstLetter + subStr
        try{
            const splitted = combine.split(/(?=[A-Z])/);
            return splitted.join(' ').trim()
        }
        catch{
            return colName
        }
    }
}
