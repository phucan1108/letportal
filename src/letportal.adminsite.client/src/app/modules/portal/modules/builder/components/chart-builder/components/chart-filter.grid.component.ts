import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ArrayUtils } from 'app/core/utils/array-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { DatasourceOptionsDialogComponent } from 'portal/shared/datasourceopts/datasourceopts.component';
import { ChartFilter, DatasourceControlType, FilterType } from 'services/portal.service';
import { ChartFilterDialogComponent } from './chart-filter.dialog.component';

@Component({
    selector: 'let-chart-filter-grid',
    templateUrl: './chart-filter.grid.component.html'
})
export class ChartFilterGridComponent implements OnInit {
    @Input()
    chartFilters: Array<ChartFilter>

    @Output()
    chartFiltersChanged = new EventEmitter<any>()

    currentIndex = 0
    isSmallDevice = false
    isAddDialog = true

    constructor(
        private dialog: MatDialog,
        private breakpointObserver: BreakpointObserver,
        private cd: ChangeDetectorRef,
        private logger: NGXLogger
    ) {
        this.breakpointObserver.observe([
            Breakpoints.HandsetPortrait,
            Breakpoints.HandsetLandscape
        ]).subscribe(result => {
            if (result.matches) {
                this.isSmallDevice = true
                this.logger.debug('Small device on chart filter', this.isSmallDevice)
            }
            else {
                this.isSmallDevice = false
                this.logger.debug('Small device on chart filter', this.isSmallDevice)
            }
        });
    }

    ngOnInit(): void {
        if(!ObjectUtils.isNotNull(this.chartFilters)){
            this.chartFilters = []
        }
    }

    enableDatasource(row: ChartFilter){
        return row.type === FilterType.Select && !row.isHidden
    }
    add(){
        const newFilter: ChartFilter = {
            name: '',
            displayName: '',
            allowDefaultValue: false,
            defaultValue: '',
            isHidden: false,
            isMultiple: false,
            rangeValue: '',
            type: FilterType.Select
        }
        this.isAddDialog = true
        const dialogRef = this.dialog.open(ChartFilterDialogComponent, {
            disableClose: true,
            data: newFilter
        })

        dialogRef.afterClosed().subscribe(result => {
            if(!result){
                return
            }

            this.chartFilters.push(result)
            this.refreshTable()
        })
    }

    editDatasource(row: ChartFilter){
        let newDatasource = row.datasourceOptions
        this.logger.debug('editting datasource', newDatasource)
        if (!newDatasource) {
            newDatasource = {
                type: DatasourceControlType.StaticResource
            }
        }
        const dialogRef = this.dialog.open(DatasourceOptionsDialogComponent, {
            disableClose: true,
            data: {
                datasourceOption: newDatasource
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                row.datasourceOptions = result
                this.logger.debug('after changed datasource', this.chartFilters)
                this.refreshTable()
            }
        })
    }

    editFilter(row: ChartFilter){
        this.currentIndex = this.chartFilters.indexOf(row)
        this.isAddDialog = false
        const dialogRef = this.dialog.open(ChartFilterDialogComponent, {
            disableClose: true,
            data: row
        })

        dialogRef.afterClosed().subscribe(result => {
            if(!result){
                return
            }
            this.chartFilters = ArrayUtils.updateOneItemByIndex(this.chartFilters, result, this.currentIndex)
            this.refreshTable()
        })
    }

    deleteFilter(row: ChartFilter){
        const index = this.chartFilters.indexOf(row)
        this.chartFilters.splice(index, 1)
        this.refreshTable()
    }

    refreshTable(){
        this.cd.detectChanges()
        this.chartFiltersChanged.emit(this.chartFilters)
    }
}
