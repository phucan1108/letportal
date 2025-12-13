import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ExtendedFilterOption, ExtendedRenderFilterField } from '../models/extended.model';
import { FilterOperator, FilterChainOperator, FieldValueType } from 'services/portal.service';
 
import { NGXLogger } from 'ngx-logger';
import { ObjectUtils } from 'app/core/utils/object-util';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'let-advanced-filer-dialog',
    templateUrl: './advancedfilter-dialog.component.html'
})
export class AdvancedFilterDialogComponent implements OnInit {

    filterOption: ExtendedFilterOption
    filters: Array<ExtendedRenderFilterField>;

    fieldValueType = FieldValueType

    containsText = 'Contains'
    equalText = 'Equal'
    noneText = 'None'
    andText = 'And'
    orText = 'Or'

    constructor(
        public dialogRef: MatDialogRef<any>,
        private translate: TranslateService,
        private logger: NGXLogger,
        private cd: ChangeDetectorRef,
        @Inject(MAT_DIALOG_DATA) public data: any
    ) {
        this.filterOption = this.data.filterOption
        this.logger.debug('Default filter option', this.filterOption)
        this.filters = this.data.filters
    }

    ngOnInit(): void { 
        this.translate.get('pageRender.dynamicList.advancedFilter.containsText').subscribe(
            text => {
                this.containsText = text
            }
        )
        this.translate.get('pageRender.dynamicList.advancedFilter.equalText').subscribe(
            text => {
                this.equalText = text
            }
        )
        this.translate.get('pageRender.dynamicList.advancedFilter.noneText').subscribe(
            text => {
                this.noneText = text
            }
        )
        this.translate.get('pageRender.dynamicList.advancedFilter.andText').subscribe(
            text => {
                this.andText = text
            }
        )
        this.translate.get('pageRender.dynamicList.advancedFilter.orText').subscribe(
            text => {
                this.orText = text
            }
        )
    }

    onFieldNameSelected(filterOption: ExtendedFilterOption) {
        this.filters?.forEach((element) => {
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
                    { name: this.containsText, value: FilterOperator.Contains },
                    { name: this.equalText, value: FilterOperator.Equal }
                ]
            case FieldValueType.Checkbox:
            case FieldValueType.Slide:
            case FieldValueType.Select:
                return [
                    { name: this.equalText , value: FilterOperator.Equal }
                ]
            case FieldValueType.Number:
                return [
                    { name: this.equalText, value: FilterOperator.Equal },
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
                    { name: this.equalText, value: FilterOperator.Equal }
                ]
        }
    }
}
