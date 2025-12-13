import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NGXLogger } from 'ngx-logger';
import { ExtendedFilterField } from 'portal/modules/models/dynamiclist.extended.model';
import { FieldValueType, FilterField } from 'services/portal.service';

@Component({
    selector: 'let-filter-option',
    templateUrl: './filter-option.component.html'
})
export class FilterOptionComponent implements OnInit {
    @Input()
    filterOptions: Array<ExtendedFilterField>

    @Output()
    filtersChanged = new EventEmitter<any>()

    _fieldTypes = [
        { name: 'Text', value: 'string' },
        { name: 'Select', value: 'string-select' },
        { name: 'Slide', value: 'boolean' },
        { name: 'Date Picker', value: 'datetime' },
        { name: 'Check List', value: 'list' },
        { name: 'Number', value: 'number' },
        { name: 'Json', value: 'document' }
    ]
    isSmallDevice = false

    fieldValueType = FieldValueType

    constructor(
        private dialog: MatDialog,
        private breakpointObserver: BreakpointObserver,
        private logger: NGXLogger
    ) {
        this.breakpointObserver.observe([
            Breakpoints.HandsetPortrait,
            Breakpoints.HandsetLandscape
        ]).subscribe(result => {
            if (result.matches) {
                this.isSmallDevice = true
                this.logger.debug('Small device on filter option of list', this.isSmallDevice)
            }
            else{
                this.isSmallDevice = false
                this.logger.debug('Small device on filter option of list', this.isSmallDevice)
            }
        });
    }

    ngOnInit(): void {
    }

    trackFilterName(index, filter) {
        return filter ? filter.name : undefined;
    }

    openDatasourceDialogForFilter(currentFilter: FilterField) {
        // if (currentFilter.filterValueType === FieldValueType.Select) {
            // if(!currentFilter.datasourceOptions){
            //     currentFilter.datasourceOptions = {
            //         datasourceStaticOptions: {
            //             jsonResource: null
            //         },
            //         type: DatasourceControlType.StaticResource
            //     }
            // }
            // const dialogRef = this.dialog.open(DatasourceOptionsDialogComponent, {
            //     disableClose: true,
            //     data: {
            //         datasourceOption: currentFilter.datasourceOptions
            //     }
            // });
            // dialogRef.afterClosed().subscribe(result => {
            //     if (result) {
            //         currentFilter.datasourceOptions = result
            //         this.logger.debug('after changed datasource', currentFilter.datasourceOptions)
            //         this.notifyFiltersChanged()
            //     }
            // })
        // }
    }

    onFieldTypeSelectChange(currentFilter: FilterField) {
    }

    deleteFilter(filter: ExtendedFilterField) {
        const index = this.filterOptions.indexOf(filter);
        this.filterOptions.splice(index, 1);
        this.notifyFiltersChanged()
    }

    onHiddenFilter(hiddenFilter: FilterField) {
        hiddenFilter.allowTextSearch = false
        hiddenFilter.allowInAdvancedMode = false
    }

    notifyFiltersChanged() {
        this.filtersChanged.emit(this.filterOptions)
    }
}