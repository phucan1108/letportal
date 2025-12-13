import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { ChangeDetectorRef, Component, EventEmitter, HostListener, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSelectChange } from '@angular/material/select';
import { TranslateService } from '@ngx-translate/core';
import { NGXLogger } from 'ngx-logger';
import { BehaviorSubject } from 'rxjs';
import { DatasourceOptionsService } from 'services/datasourceopts.service';
import { DynamicListFetchDataModel, FieldValueType, FilledParameter, FilterChainOperator, FilterGroup, FilterOperator } from 'services/portal.service';
import { DatasourceCache } from '../models/commandClicked';
import { ExtendedFilterOption, ExtendedRenderFilterField } from '../models/extended.model';
import { AdvancedFilterDialogComponent } from './advancedfilter-dialog.component';
 
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
        private translate: TranslateService,
        private logger: NGXLogger) {
            this.breakpointObserver.observe([
                Breakpoints.HandsetPortrait,
                Breakpoints.HandsetLandscape
            ]).subscribe(result => {
                if (result.matches) {
                    this.isSmallDevice = true
                    this.cd.markForCheck()
                    this.logger.debug('Small device on filter of list', this.isSmallDevice)
                }
                else {
                    this.isSmallDevice = false
                    this.cd.markForCheck()
                    this.logger.debug('Small device on filter of list', this.isSmallDevice)
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

    _operators = function(){
        return [
            { name: this.containsText, value: FilterOperator.Contains },
            { name: this.equalText, value: FilterOperator.Equal },
            { name: '>', value: FilterOperator.Great },
            { name: '<', value: FilterOperator.Less },
            { name: '>=', value: FilterOperator.Greater },
            { name: '<=', value: FilterOperator.Lesser }
        ]
    } 

    combineOperators$: BehaviorSubject<any[]> = new BehaviorSubject([])

    _combineOperators = function() {
        return [
            { name: this.noneText, value: FilterChainOperator.None },
            { name: this.andText, value: FilterChainOperator.And },
            { name: this.orText, value: FilterChainOperator.Or }
        ]
    } 

    filterChainOperatorType = FilterChainOperator
    isSmallDevice = false
    fieldValueType = FieldValueType

    containsText = 'Contains'
    equalText = 'Equal'
    noneText = 'None'
    andText = 'And'
    orText = 'Or'

    @HostListener('window:keydown.enter',  ['$event'])
    handleEnterPress(event: KeyboardEvent){
        this.logger.debug('Hit key down', event)
        this.onSearchClick()
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
                this.combineOperators$.next(this._combineOperators())
            }
        )
        this.translate.get('pageRender.dynamicList.advancedFilter.andText').subscribe(
            text => {
                this.andText = text
                this.combineOperators$.next(this._combineOperators())
            }
        )
        this.translate.get('pageRender.dynamicList.advancedFilter.orText').subscribe(
            text => {
                this.orText = text
                this.combineOperators$.next(this._combineOperators())
            }
        )
        this.filters = this.filters.filter((element: ExtendedRenderFilterField) => {
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
            if (this.filterOptions[this.filterOptions.length - 1].filterChainOperator !== FilterChainOperator.None) {
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
        this.filters?.forEach((element) => {
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
            this.filterOptions?.forEach((element, index) => {
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
        return this._operators().find(opt => opt.value === operator).name
    }

    translateHeader(fieldName) {
        return this.filters.find(filter => filter.name === fieldName).displayName
    }

    translateChainOperator(chainOperator) {
        return this._combineOperators().find(opt => opt.value === chainOperator).name
    }

    translateFieldValue(filterOption: ExtendedFilterOption, fieldValue) {
        if (!!filterOption.filterDataSource && filterOption.filterDataSource.length > 0) {
            const found = filterOption.filterDataSource.find( source => source.value === fieldValue)
            return found.name
        }
        return fieldValue
    }

    addFilterAnd(filterOption: ExtendedFilterOption) {        
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
                filterOption.filterChainOperator = FilterChainOperator.And
                this.filterOptions.push(result)
                this.cd.markForCheck()
            }
        })
    }

    addFilterOr(filterOption: ExtendedFilterOption) {        
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
                filterOption.filterChainOperator = FilterChainOperator.Or
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
                    { name: this.containsText, value: FilterOperator.Contains },
                    { name: this.equalText, value: FilterOperator.Equal }
                ]
            case FieldValueType.Checkbox:
            case FieldValueType.Slide:
            case FieldValueType.Select:
                return [
                    { name: this.equalText, value: FilterOperator.Equal }
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
                    { name: this.equalText, value: FilterOperator.Equal },
                    { name: '>', value: FilterOperator.Great },
                    { name: '<', value: FilterOperator.Less },
                    { name: '>=', value: FilterOperator.Greater },
                    { name: '<=', value: FilterOperator.Lesser }
                ]
        }
    }
}
