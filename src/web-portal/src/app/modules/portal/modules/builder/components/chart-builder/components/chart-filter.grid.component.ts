import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ChartFilter, FilterType, DatasourceControlType } from 'services/portal.service';
import { MatDialog } from '@angular/material';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { NGXLogger } from 'ngx-logger';
import { DatasourceOptionsDialogComponent } from 'portal/shared/datasourceopts/datasourceopts.component';
import { ChartFilterDialogComponent } from './chart-filter.dialog.component';
import { ArrayUtils } from 'app/core/utils/array-util';

@Component({
    selector: 'let-chart-filter-grid',
    templateUrl: './chart-filter.grid.component.html'
})
export class ChartFilterGridComponent implements OnInit {
    @Input()
    chartFilters: Array<ChartFilter>

    @Output()
    chartFiltersChanged = new EventEmitter<any>()

    isSmallDevice = false
    
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
                this.logger.debug('Small device', this.isSmallDevice)
            }
            else {
                this.isSmallDevice = false
                this.logger.debug('Small device', this.isSmallDevice)
            }
        });
    }

    ngOnInit(): void { }

    enableDatasource(row: ChartFilter){
        return row.type === FilterType.Select && !row.isHidden 
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
        const dialogRef = this.dialog.open(ChartFilterDialogComponent, {
            disableClose: true,
            data: row
        })
        
        dialogRef.afterClosed().subscribe(result => {
            if(!result){
                return
            }
            this.chartFilters = ArrayUtils.updateOneItem(this.chartFilters, result, (chartFilter: ChartFilter) => { return chartFilter.name === result.name})            
            this.refreshTable()
        })
    }

    refreshTable(){
        this.chartFiltersChanged.emit(this.chartFilters)
    }
}
