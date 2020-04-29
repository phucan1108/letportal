import { Component, OnInit, Input, ChangeDetectorRef, Output, EventEmitter, HostListener } from '@angular/core';
import { FilterGroup, FilterOption, FilterOperator, FilterChainOperator, FilledParameter, FieldValueType, DatabasesClient, DynamicListFetchDataModel } from 'services/portal.service';
import * as _ from 'lodash';
import { NGXLogger } from 'ngx-logger';
import { ExtendedFilterOption, ExtendedRenderFilterField } from '../models/extended.model';
import { AdvancedFilterDialogComponent } from './advancedfilter-dialog.component';
import { DatasourceOptionsService } from 'services/datasourceopts.service';
import { DatasourceCache } from '../models/commandClicked';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { MatDialog } from '@angular/material/dialog';
import { MatSelectChange } from '@angular/material/select';
import { PageService } from 'services/page.service';


@Component({
    selector: 'dynamic-list-filters',
    templateUrl: './dynamic-list.filters.component.html'
})
export class DynamicListFiltersComponent implements OnInit {
    constructor(
        private cd: ChangeDetectorRef,
        private datasourceOptsService: DatasourceOptionsService,
        private breakpointObserver: BreakpointObserver,
        public dialog: MatDialog,
        private logger: NGXLogger) {
            this.breakpointObserver.observe([
                Breakpoints.HandsetPortrait,
                Breakpoints.HandsetLandscape
            ]).subscribe(result => {
                if (result.matches) {
                    this.isSmallDevice = true
                    this.cd.markForCheck()
                    this.logger.debug('Small device', this.isSmallDevice)
                }
                else {
                    this.isSmallDevice = false
                    this.cd.markForCheck()
                    this.logger.debug('Small device', this.isSmallDevice)
                }
            });
        }

    @Input()
    filters: Array<ExtendedRenderFilterField>;

    @Input()
    datasourceCache: DatasourceCache[]

    @Input()
    fetchFirstTime: boolean

    @Input()
    enableSearch: boolean

    @Input()
    enableAdvancedSearch: boolean

    @Output()
    onSearch: EventEmitter<any> = new EventEmitter();

    textSearch = '';

    filterOptions: Array<ExtendedFilterOption> = [];

    isOpeningAdvancedMode = false;

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

    filterChainOperatorType = FilterChainOperator
    isSmallDevice = false
    fieldValueType = FieldValueType

    @HostListener('window:keydown.enter',  ['$event'])
    handleEnterPress(event: KeyboardEvent){
        this.logger.debug('Hit key down', event)
        this.onSearchClick()
    }

    ngOnInit(): void {
        this.filters = _.filter(this.filters, (element: ExtendedRenderFilterField) => {
            return element.allowInAdvancedMode;
        })

        if (this.fetchFirstTime) {
            // Avoid almost race condition
            setTimeout(() => {
                this.onSearchClick()
            }, 1000);
        }
    }

    onSearchClick() {
        const filterQuery: DynamicListFetchDataModel = {
            textSearch: this.textSearch,
            filledParameterOptions: {
                filledParameters: this.combineFilledParameters()
            },
            filterGroupOptions: {
                filterGroups: this.combineFilterGroups()
            }
        };
        this.onSearch.emit(filterQuery)
    }

    combineFilledParameters() {
        const filledParameters: Array<FilledParameter> = [];
        return filledParameters;
    }

    combineFilterGroups() {
        const filterGroups: Array<FilterGroup> = [];

        filterGroups.push({
            filterChainOperator: FilterChainOperator.None,
            filterOptions: this.filterOptions
        })

        return filterGroups;
    }

    onAdvancedSearchChange() {
        if (this.isOpeningAdvancedMode) {
            if (this.filterOptions.length === 0) {
                const addingFilter = {
                    fieldName: this.filters[0].name,
                    fieldValue: '',
                    filterChainOperator: FilterChainOperator.None,
                    filterOperator: this.getDefaultOperator(this.filters[0].fieldValueType),
                    filterValueType: this.filters[0].fieldValueType,
                    filterDataSource: this.filters[0].jsonData ? this.filters[0].jsonData : new Array<any>(),
                    filterOperators: this.getAllowedOperators(this.filters[0].fieldValueType)
                }

                this.filterOptions.push(addingFilter)
            }
        }
        else {
            this.filterOptions = [];
        }
    }

    onAdvancedSearchMobileChange() {
        if (this.isOpeningAdvancedMode) {
            if (this.filterOptions.length === 0) {
                const dialogRef = this.dialog.open(AdvancedFilterDialogComponent, {
                    data: {
                        filterOption: {
                            fieldName: this.filters[0].name,
                            fieldValue: '',
                            filterChainOperator: FilterChainOperator.None,
                            filterOperator: this.getDefaultOperator(this.filters[0].fieldValueType),
                            filterValueType: this.filters[0].fieldValueType,
                            filterDataSource: this.filters[0].jsonData ? this.filters[0].jsonData : new Array<any>(),
                            filterOperators: this.getAllowedOperators(this.filters[0].fieldValueType)
                        },
                        filters: this.filters
                    }
                })
                dialogRef.afterClosed().subscribe(result => {
                    if (result) {
                        this.filterOptions.push(result)
                        this.cd.markForCheck()
                        this.logger.debug('Filter options', this.filterOptions)
                    }
                    else{
                        if(this.filterOptions.length === 0){
                            this.isOpeningAdvancedMode = false
                            this.cd.markForCheck()
                        }
                    }
                })
            }
        }
        else {
            this.filterOptions = [];
        }
    }

    onCombineOperatorValueChange(valueChange: MatSelectChange) {
        if (valueChange.value === FilterChainOperator.None) {
            this.balanceFilterOptions();
        }
        else {
            // Check the last element is AND or OR -> add new filter
            if (_.last(this.filterOptions).filterChainOperator !== FilterChainOperator.None) {
                this.filterOptions.push({
                    fieldName: this.filters[0].name,
                    fieldValue: '',
                    filterChainOperator: FilterChainOperator.None,
                    filterOperator: this.getDefaultOperator(this.filters[0].fieldValueType),
                    filterValueType: this.filters[0].fieldValueType,
                    filterDataSource: this.filters[0].jsonData ? this.filters[0].jsonData : new Array<any>(),
                    filterOperators: this.getAllowedOperators(this.filters[0].fieldValueType)
                })
            }
        }
    }

    onFieldNameSelected(filterOption: ExtendedFilterOption) {
        _.forEach(this.filters, (element) => {
            if (element.name === filterOption.fieldName) {
                if (element.fieldValueType === FieldValueType.Select) {
                    filterOption.filterDataSource = element.jsonData
                    filterOption.fieldValue = element.jsonData.length == 1 ? element.jsonData[0].value : ''
                }
                else{
                    filterOption.fieldValue = ''
                }

                filterOption.filterValueType = element.fieldValueType
                filterOption.filterOperators = this.getAllowedOperators(filterOption.filterValueType)
                filterOption.filterOperator = this.getDefaultOperator(filterOption.filterValueType)
            }
        })
    }

    balanceFilterOptions() {
        // This method just allow balance the Filter Options for making sure 'None' is the last option
        let lastNoneIndex = -1;
        if (this.filterOptions.length === 1) {
            this.filterOptions[0].filterChainOperator = FilterChainOperator.None
        }
        else {
            _.forEach(this.filterOptions, (element, index) => {
                if (element.filterChainOperator === FilterChainOperator.None) {
                    lastNoneIndex = index;
                    return false;
                }
            })

            if (lastNoneIndex > -1 && this.filterOptions.length !== lastNoneIndex) {
                this.filterOptions = this.filterOptions.slice(0, lastNoneIndex + 1);
                this.cd.markForCheck();
            }
        }
    }

    translateOperator(operator) {
        return _.find(this._operators, opt => opt.value === operator).name
    }

    translateHeader(fieldName) {
        return _.find(this.filters, filter => filter.name === fieldName).displayName
    }

    translateChainOperator(chainOperator) {
        return _.find(this._combineOperators, opt => opt.value === chainOperator).name
    }

    translateFieldValue(filterOption: ExtendedFilterOption, fieldValue) {
        if (!!filterOption.filterDataSource && filterOption.filterDataSource.length > 0) {
            const found = _.find(filterOption.filterDataSource, source => source.value === fieldValue)
            return found.name
        }
        return fieldValue
    }

    addFilterAnd(filterOption: ExtendedFilterOption) {
        filterOption.filterChainOperator = FilterChainOperator.And
        const dialogRef = this.dialog.open(AdvancedFilterDialogComponent, {
            data: {
                filterOption: {
                    fieldName: this.filters[0].name,
                    fieldValue: '',
                    filterChainOperator: FilterChainOperator.None,
                    filterOperator: this.getDefaultOperator(this.filters[0].fieldValueType),
                    filterValueType: this.filters[0].fieldValueType,
                    filterDataSource: this.filters[0].jsonData ? this.filters[0].jsonData : new Array<any>(),
                    filterOperators: this.getAllowedOperators(this.filters[0].fieldValueType)
                },
                filters: this.filters
            }
        })
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.filterOptions.push(result)
                this.cd.markForCheck()
            }
        })
    }

    addFilterOr(filterOption: ExtendedFilterOption) {
        filterOption.filterChainOperator = FilterChainOperator.Or
        const dialogRef = this.dialog.open(AdvancedFilterDialogComponent, {
            data: {
                filterOption: {
                    fieldName: this.filters[0].name,
                    fieldValue: '',
                    filterChainOperator: FilterChainOperator.None,
                    filterOperator: this.getDefaultOperator(this.filters[0].fieldValueType),
                    filterValueType: this.filters[0].fieldValueType,
                    filterDataSource: this.filters[0].jsonData ? this.filters[0].jsonData : new Array<any>(),
                    filterOperators: this.getAllowedOperators(this.filters[0].fieldValueType)
                },
                filters: this.filters
            }
        })
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.filterOptions.push(result)
                this.cd.markForCheck()
            }
        })

    }
    removeFilter(filterOption: ExtendedFilterOption) {
        const index = this.filterOptions.indexOf(filterOption)
        this.filterOptions.splice(index, 1)
        if (this.filterOptions.length > 0) {
            this.balanceFilterOptions()
        }
        this.cd.markForCheck()
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
                    { name: 'Equal', value: FilterOperator.Equal },
                    { name: '>', value: FilterOperator.Great },
                    { name: '<', value: FilterOperator.Less },
                    { name: '>=', value: FilterOperator.Greater },
                    { name: '<=', value: FilterOperator.Lesser }
                ]
        }
    }
}
