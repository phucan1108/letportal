import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import StringUtils from 'app/core/utils/string-util';
import { tap } from 'rxjs/operators';
import { FieldValueType } from 'services/portal.service';
import { DatasourceCache } from '../models/commandClicked';
import { ExtendedColDef } from '../models/extended.model';

@Component({
    selector: 'let-dynamic-list-data-dialog',
    templateUrl: './dynamic-list-data-dialog.component.html',
    standalone: false
})
export class DynamicListDataDialogComponent implements OnInit {

  headers: ExtendedColDef[]
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

  translateData(renderingData: any, currentColumn: ExtendedColDef) {

    let refName = StringUtils.toCamelCase(currentColumn.name)
    // Bug: some reasons the data isn't Camel Case, 
    // we should check column name is upper case or lower case       
    if (StringUtils.startsWithCapital(currentColumn.name)) {
      refName = currentColumn.name
    }
    if (refName === 'id' || refName === '_id') {
      const checkData = renderingData.id
      if (!checkData) {
        return renderingData._id
      }
      else {
        return checkData
      }
    }
    let displayData = null
    if (refName.indexOf('.') > 0) {
      const extractData = new Function('data', `return data.${refName}`)
      displayData = extractData(renderingData)
    }
    else {
      displayData = renderingData[refName]
    }

    if (currentColumn.searchOptions.fieldValueType === FieldValueType.Select) {
      const datasource = this.datasourceCache.find((elem: DatasourceCache) => elem.datasourceId === currentColumn.datasourceId);
      return datasource.data.find(elem => elem.value.toString() === displayData).name
    }
    return displayData
  }
}
