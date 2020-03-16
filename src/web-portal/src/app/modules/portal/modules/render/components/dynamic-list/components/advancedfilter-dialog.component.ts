import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ExtendedFilterOption, ExtendedRenderFilterField } from '../models/extended.model';
import { FilterOperator, FilterChainOperator, FieldValueType } from 'services/portal.service';
import * as _ from 'lodash';
import { NGXLogger } from 'ngx-logger';
import { ObjectUtils } from 'app/core/utils/object-util';

@Component({
    selector: 'let-advanced-filer-dialog',
    templateUrl: './advancedfilter-dialog.component.html'
})
export class AdvancedFilterDialogComponent implements OnInit {

    filterOption: ExtendedFilterOption
    filters: Array<ExtendedRenderFilterField>;
    _operators = [
        { name: 'Contains', value: FilterOperator.Contains },
        { name: 'Equal', value: FilterOperator.Equal },
        { name: '>', value: FilterOperator.Great },
        { name: '<', value: FilterOperator.Less },
        { name: '>=', value: FilterOperator.Greater },
        { name: '<=', value: FilterOperator.Lesser }
    ]

    _combineOperators = [
        { name: 'None', value: FilterChainOperator.None },
        { name: 'And', value: FilterChainOperator.And },
        { name: 'Or', value: FilterChainOperator.Or }
    ]

    fieldValueType = FieldValueType

    constructor(
        public dialogRef: MatDialogRef<any>,
        private logger: NGXLogger,
        private cd: ChangeDetectorRef,
        @Inject(MAT_DIALOG_DATA) public data: any
    ) {
        this.filterOption = this.data.filterOption
        this.logger.debug('Default filter option', this.filterOption)
        this.filters = this.data.filters
    }

    ngOnInit(): void { }

    onFieldNameSelected(filterOption: ExtendedFilterOption) {
        _.forEach(this.filters, (element) => {
            if (element.name === filterOption.fieldName) {
                if (element.fieldValueType === FieldValueType.Select) {
                    filterOption.filterDataSource = element.jsonData
                }

                filterOption.filterValueType = element.fieldValueType
                filterOption.filterOperators = this.getAllowedOperators(filterOption.filterValueType)
                filterOption.filterOperator = this.getDefaultOperator(filterOption.filterValueType)
                this.cd.markForCheck()
            }
        })
    }
    onFilterSubmit(){
        this.dialogRef.close(this.filterOption)
    }

    private getDefaultOperator(fieldType: FieldValueType): FilterOperator {
        switch (fieldType) {
            case FieldValueType.Text:
                return FilterOperator.Contains
            case FieldValueType.Slide:
            case FieldValueType.Checkbox:
            case FieldValueType.Select:
            case FieldValueType.Number:
            case FieldValueType.DatePicker:
                return FilterOperator.Equal
        }
    }

    private getAllowedOperators(fieldType: FieldValueType) {
        switch (fieldType) {
            case FieldValueType.Text:
                return [
                    { name: 'Contains', value: FilterOperator.Contains },
                    { name: 'Equal', value: FilterOperator.Equal }
                ]
            case FieldValueType.Checkbox:
            case FieldValueType.Slide:
            case FieldValueType.Select:
                return [
                    { name: 'Equal', value: FilterOperator.Equal }
                ]
            case FieldValueType.Number:
                return [
                    { name: 'Equal', value: FilterOperator.Equal },
                    { name: '>', value: FilterOperator.Great },
                    { name: '<', value: FilterOperator.Less },
                    { name: '>=', value: FilterOperator.Greater },
                    { name: '<=', value: FilterOperator.Lesser }
                ]
            case FieldValueType.DatePicker:
                return [
                    { name: '>', value: FilterOperator.Great },
                    { name: '<', value: FilterOperator.Less },
                    { name: '>=', value: FilterOperator.Greater },
                    { name: '<=', value: FilterOperator.Lesser },
                    { name: 'Equal', value: FilterOperator.Equal }
                ]
        }
    }
}
