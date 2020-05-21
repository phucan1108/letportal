import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ColumndDef, FieldValueType } from 'services/portal.service';
import { DatasourceCache } from '../models/commandClicked';
import * as _ from 'lodash';
import { ExtendedColDef } from '../models/extended.model';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { tap } from 'rxjs/operators';

@Component({
    selector: 'let-dynamic-list-data-dialog',
    templateUrl: './dynamic-list-data-dialog.component.html'
})
export class DynamicListDataDialogComponent implements OnInit {

    headers: ColumndDef[]
    rawData: any
    datasourceCache: Array<DatasourceCache> = []
    isHandset = false
    constructor(
        public dialogRef: MatDialogRef<any>,
        private breakpointObserver: BreakpointObserver,
        @Inject(MAT_DIALOG_DATA) public data: any
    ) {
        this.headers = this.data.headers
        this.rawData = this.data.rawData
        this.datasourceCache = this.data.datasource
        this.breakpointObserver.observe(Breakpoints.Handset)
            .pipe(
                tap(result => {
                    if (result.matches) {
                        this.isHandset = true
                    }
                    else {
                        this.isHandset = false
                    }
                })
            ).subscribe()
    }

    ngOnInit(): void { }

    private translateData(renderingData: any, currentColumn: ExtendedColDef) {
        if (currentColumn.name === 'id' || currentColumn.name === '_id') {
            const checkData = renderingData.id
            if (!checkData) {
                return renderingData._id
            }
            else {
                return checkData
            }
        }
        let displayData = null
        if (currentColumn.name.indexOf('.') > 0) {
            const extractData = new Function('data', `return data.${currentColumn.name}`)
            displayData = extractData(renderingData)
        }
        else {
            displayData = renderingData[currentColumn.name]
        }

        if (currentColumn.searchOptions.fieldValueType === FieldValueType.Select) {
            const datasource = _.find(this.datasourceCache, (elem: DatasourceCache) => elem.datasourceId === currentColumn.datasourceId);
            return _.find(datasource.data, elem => elem.value.toString() === displayData).name
        }
        return displayData
    }
}
