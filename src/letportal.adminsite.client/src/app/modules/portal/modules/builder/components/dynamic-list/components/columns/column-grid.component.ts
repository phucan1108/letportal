import { moveItemInArray } from '@angular/cdk/drag-drop';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NGXLogger } from 'ngx-logger';
import { StaticResources } from 'portal/resources/static-resources';
import { DatasourceOptionsDialogComponent } from 'portal/shared/datasourceopts/datasourceopts.component';
import { ActionType, ColumnDef, DatasourceControlType, FieldValueType } from 'services/portal.service';

@Component({
    selector: 'let-column-grid',
    templateUrl: './column-grid.component.html'
})
export class ColumnGridComponent implements OnInit {

    @Input()
    columnDefs: Array<ColumnDef>

    @Output()
    columnDefsChanged = new EventEmitter<any>()

    _fieldTypes = StaticResources.fieldValueTypes()

    _patterns = [
        { name: 'text', displayName: 'Text', format: '{0}' },
        { name: 'numberTwoDots', displayName: 'Number two digits', format: '{0:0.00}' }
    ]

    _patternGroups = [
        {
            name: 'Text',
            patterns: [
                { name: 'Text', format: '{0}' }
            ]
        },
        {
            name: 'Number',
            patterns: [
                { name: 'One digit', format: '{0:0.0}' },
                { name: 'Two digit', format: '{0:0.00}' },
                { name: 'Thousands separator', format: '{0:0,000.00}' },
                { name: 'Percent', format: '{0:0.00%}' },
                { name: 'Currency', format: '{0:c}' }
            ]
        },
        {
            name: 'Datetime',
            patterns: [
                { name: 'Day first', format: '{0:dd/MM/yyyy}' },
                { name: 'Month first', format: '{0:MM/dd/yyyy}' },
                { name: 'Long date', format: '{0:D}'},
                { name: 'Month year', format: '{0:Y}'},
                { name: 'Day first time', format: '{0:dd/MM/yyyy HH:mm}'},
                { name: 'Month first time', format: '{0:MM/dd/yyyy HH:mm}'}
            ]
        }
    ]
    fieldValueType = FieldValueType
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
                this.logger.debug('Small device on column grid', this.isSmallDevice)
            }
            else {
                this.isSmallDevice = false
                this.logger.debug('Small device on column grid', this.isSmallDevice)
            }
        });
    }

    ngOnInit(): void { }

    trackColumnName(index, columnDef) {
        return columnDef ? columnDef.name : undefined;
    }

    deleteColumn(column: ColumnDef) {
        const index = this.columnDefs.indexOf(column);
        this.columnDefs.splice(index, 1);
        this.notifyColumnDefsChanged()
    }

    translateCommandType(commandType: ActionType) {
        switch (commandType) {
            case ActionType.CallHttpService:
                return 'Http Service'
            case ActionType.Redirect:
                return 'Redirect'
        }
    }

    onHiddenColumn(columnDef: ColumnDef) {
        columnDef.allowSort = false;
        columnDef.searchOptions.allowInAdvancedMode = false
        columnDef.searchOptions.allowTextSearch = false
        columnDef.displayFormatAsHtml = false
    }

    onFieldTypeSelectChange(columnDef: ColumnDef){

    }

    onListDrop($event){
        const prevIndex = this.columnDefs.findIndex((colDef) => colDef === $event.item.data)
        moveItemInArray(this.columnDefs, prevIndex, $event.currentIndex);
        this.logger.debug('New columns after dragging', this.columnDefs)
    }

    openDatasourceDialogForColumn(columnDef: ColumnDef) {
        if (columnDef.searchOptions.fieldValueType === FieldValueType.Select) {
            if(!columnDef.datasourceOptions){
                columnDef.datasourceOptions = {
                    datasourceStaticOptions: {
                        jsonResource: null
                    },
                    type: DatasourceControlType.StaticResource
                }
            }
            const dialogRef = this.dialog.open(DatasourceOptionsDialogComponent, {
                disableClose: true,
                data: {
                    datasourceOption: columnDef.datasourceOptions
                }
            });
            dialogRef.afterClosed().subscribe(result => {
                if (result) {
                    columnDef.datasourceOptions = result
                    this.logger.debug('after changed datasource', columnDef.datasourceOptions)
                    this.notifyColumnDefsChanged()
                }
            })
        }
    }

    notifyColumnDefsChanged() {
        this.columnDefsChanged.emit(this.columnDefs)
    }
}
