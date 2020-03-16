import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { DatabaseOptions, ChartFilter, ColumnField, FilterType, ExtractingSchemaQueryModel, FilledParameter } from 'services/portal.service';
import { NGXLogger } from 'ngx-logger';
import * as _ from 'lodash';

@Component({
    selector: 'let-chart-datasource',
    templateUrl: './chart-datasource.component.html'
})
export class ChartDatasourceComponent implements OnInit {
    heading = 'Database Connection Info:'
    format = /[ !@#$%^&*()+\-=\[\]{};':"\\|,.<>\/?]/;

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

    private chartFilters: ChartFilter[] = []
    private selectedEntityname: string

    constructor(
        private logger: NGXLogger
    ) { }

    ngOnInit(): void { }

    onPopulatingQuery($event: ExtractingSchemaQueryModel) {
        this.chartFilters = this.populateChartFilters($event.columnFields)
        this.afterPopulatingQuery.emit({
            chartFilters: this.chartFilters
        })

    }

    onSelectingEntity($event: ExtractingSchemaQueryModel) {
        this.chartFilters = this.populateChartFilters($event.columnFields)
        this.afterSelectingEntity.emit({
            chartFilters: this.chartFilters
        })
    }

    databaseOptChanged($event: DatabaseOptions){
        this.changed.emit($event)
    }

    onSelectingEntityName($event: string) {
        this.selectedEntityname = $event
    }

    onExtractingParam($event: FilledParameter[]){
        // Do nothing
    }

    populateChartFilters(columnFields: ColumnField[]): Array<ChartFilter> {
        const chartFilters = new Array<ChartFilter>();

        _.forEach(columnFields, (element) => {
            // By default, we need to remove all fields that contain id or special char
            const fieldName = element.name.toLowerCase()
            if ((fieldName === 'id' || fieldName === '_id')
                && !this.format.test(fieldName)
                && !this.ignoreBsonFieldTypes(element.fieldType)) {
                // Ignore some special fields
                const chartFilter: ChartFilter = {
                    name: element.name,
                    displayName: element.displayName,
                    isHidden: false,
                    isMultiple: false,
                    allowDefaultValue: false,
                    defaultValue: '',
                    rangeValue: '',
                    type: this.convertFieldType(element.fieldType)
                }

                chartFilters.push(chartFilter)
            }
            else{
                this.logger.debug('Ignore chart field name:', fieldName)
            }
        })

        return chartFilters;
    }

    private convertFieldType(fieldType: string): FilterType{
        switch(fieldType){
            case 'string':
                return FilterType.Select
            case 'boolean':
                return FilterType.Checkbox
            case 'number':
                return FilterType.NumberPicker
            case 'datetime':
                return FilterType.DatePicker
            default:
                return FilterType.None
        }
    }

    private ignoreBsonFieldTypes(fieldType: string){
        return fieldType == 'list' || fieldType == 'document'
    }
}
