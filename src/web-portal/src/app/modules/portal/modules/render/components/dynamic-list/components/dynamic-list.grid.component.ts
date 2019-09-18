import { Component, OnInit, Input, ChangeDetectorRef, ViewChild, EventEmitter, Output } from '@angular/core';
import { DynamicList, ColumndDef, SortType, CommandButtonInList, CommandPositionType,  DynamicListClient, FieldValueType, DynamicListFetchDataModel } from 'services/portal.service';
import { MatDialog, MatTable, MatPaginator, MatSort } from '@angular/material';
import { BehaviorSubject, of, merge, Observable } from 'rxjs';
import * as _ from 'lodash';
import { tap, catchError, finalize, debounceTime, distinctUntilChanged, map, filter } from 'rxjs/operators';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { CommandClicked, DatasourceCache } from '../models/commandClicked';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { DynamicListDataDialogComponent } from './dynamic-list-data-dialog.component';
import { DatasourceOptionsService } from 'services/datasourceopts.service';
import { ExtendedColDef } from '../models/extended.model';
import { Guid } from 'guid-typescript';
import { NGXLogger } from 'ngx-logger';
import { ListOptions, ExtendedFilterField } from 'portal/modules/models/dynamiclist.extended.model';

@Component({
    selector: 'dynamic-list-grid',
    templateUrl: './dynamic-list.grid.component.html',
    styleUrls: ['./dynamic-list.grid.component.scss']
})
export class DynamicListGridComponent implements OnInit {

    @ViewChild('matTable')
    private matTable: MatTable<any>;
    @ViewChild(MatPaginator)
    private paginator: MatPaginator;
    @ViewChild(MatSort)
    private sort: MatSort;

    @Input()
    dynamicList: DynamicList;

    @Output()
    onClick = new EventEmitter<CommandClicked>();

    listOptions: ListOptions = ListOptions.DefaultListOptions

    dataSource: Array<any> = [];
    dataSource$: BehaviorSubject<Array<any>> = new BehaviorSubject([]);
    headers: Array<ExtendedColDef> = [];
    displayedColumns: Array<string> = [];

    loading$: BehaviorSubject<boolean> = new BehaviorSubject(false);
    isAlreadyFetchedTotal = false;
    totalItems = 0;

    datasourceCache: Array<DatasourceCache> = []
    defaultSortColumn = '';

    readyToRender = false
    commandsInList: Array<CommandButtonInList> = new Array<CommandButtonInList>();
    filters: ExtendedFilterField[] = []
    private fetchDataQuery: DynamicListFetchDataModel = {
        dynamicListId: null,
        filledParameterOptions: null,
        filterGroupOptions: null,
        paginationOptions: null,
        sortOptions: null,
        textSearch: null
    };

    isHandset = false
    isContructedGrid = false;
    hasDetailCols = false
    constructor(
        private datasoureOptsService: DatasourceOptionsService,
        private breakpointObserver: BreakpointObserver,
        private dynamicClient: DynamicListClient,
        private logger: NGXLogger,
        private cd: ChangeDetectorRef,
        public dialog: MatDialog,
        private shortcutUtil: ShortcutUtil) {
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

    ngOnInit(): void {
        this.constructGrid();
    }

    private constructGrid() {
        this.logger.debug('Dynamic list options', this.dynamicList.options)
        this.listOptions = ListOptions.getListOptions(this.dynamicList.options)

        // Extract some parts form Dynamic List 
        let counterInDetailMode = 0
        _.forEach(this.dynamicList.columnsList.columndDefs, colDef => {
            this.headers.push({
                ...colDef,
                datasourceId: '',
                inDetailMode: counterInDetailMode > this.listOptions.maximumColumns
            })
            if (!this.hasDetailCols) {
                this.hasDetailCols = counterInDetailMode > this.listOptions.maximumColumns
            }
            counterInDetailMode++
        })

        // Fetch all ready datasources and ready for filters
        let filters: ExtendedFilterField[] = []
        let counter = this.headers.length
        _.forEach(this.headers, (colDef: ExtendedColDef) => {
            // We will fetch datasource and then passing its for filter
            if (colDef.searchOptions.fieldValueType === FieldValueType.Select) {
                this.datasoureOptsService
                    .executeDatasourceOptions(colDef.datasourceOptions, null)
                    .subscribe(
                        res => {
                            let datasource = {
                                data: res,
                                datasourceId: Guid.create.toString()
                            }
                            this.datasourceCache.push(datasource)
                            colDef.datasourceId = datasource.datasourceId

                            let filter: ExtendedFilterField = {
                                name: colDef.name,
                                displayName: colDef.displayName,
                                allowInAdvancedMode: colDef.searchOptions.allowInAdvancedMode,
                                allowTextSearch: colDef.searchOptions.allowTextSearch,
                                fieldValueType: colDef.searchOptions.fieldValueType,
                                isHidden: !colDef.searchOptions.allowInAdvancedMode,
                                jsonData: res,
                                datasourceId: datasource.datasourceId
                            }
                            filters.push(filter)
                            counter--
                            if (counter == 0) {
                                this.filters = filters
                                this.isContructedGrid = true
                            }
                        }
                    )
            }
            else {
                let filter: ExtendedFilterField = {
                    name: colDef.name,
                    displayName: colDef.displayName,
                    allowInAdvancedMode: colDef.searchOptions.allowInAdvancedMode,
                    allowTextSearch: colDef.searchOptions.allowTextSearch,
                    fieldValueType: colDef.searchOptions.fieldValueType,
                    isHidden: !colDef.searchOptions.allowInAdvancedMode,
                    jsonData: null,
                    datasourceId: null
                }
                filters.push(filter)
                counter--
                if (counter == 0) {
                    this.filters = filters
                    this.isContructedGrid = true
                }
            }
        })
        this.defaultSortColumn = (_.find(this.headers, (element: ColumndDef) => {
            return element.allowSort;
        })).name

        this.commandsInList = _.filter(this.dynamicList.commandsList.commandButtonsInList, (element: CommandButtonInList) => {
            return element.commandPositionType === CommandPositionType.InList
        })


        _.forEach(this.headers, (element) => {
            if (!element.isHidden && !element.inDetailMode)
                this.displayedColumns.push(element.name)
        })
        // Check we have any command in list        
        if (this.commandsInList.length > 0) {
            this.displayedColumns.push('actions')
        }

        // Trigger loading data form sortChange and page event
        if (this.listOptions.enablePagination) {
            this.cd.detectChanges()
            merge(this.sort.sortChange, this.paginator.page)
                .pipe(
                    tap(() => {
                        this.fetchDataQuery = this.getFetchDataQuery();
                        this.fetchData();
                    })
                )
                .subscribe();
        }
        else {
            this.sort.sortChange
                .pipe(
                    tap(() => {
                        this.fetchDataQuery = this.getFetchDataQuery();
                        this.fetchData();
                    })
                )
                .subscribe();
        }

    }

    openDialogData(data) {
        if (this.isHandset) {
            const dialogRef = this.dialog.open(DynamicListDataDialogComponent, {
                data: {
                    headers: this.headers,
                    rawData: data,
                    datasource: this.datasourceCache
                }
            });

        }
    }

    openDialogDataForButton(data) {
        const dialogRef = this.dialog.open(DynamicListDataDialogComponent, {
            data: {
                headers: this.headers,
                rawData: data,
                datasource: this.datasourceCache
            }
        });
    }

    onSubmittingSearch($event) {
        let fetchQuery = $event as DynamicListFetchDataModel
        this.fetchDataQuery = this.getFetchDataQuery();
        this.fetchDataQuery.filledParameterOptions = fetchQuery.filledParameterOptions;
        this.fetchDataQuery.filterGroupOptions = fetchQuery.filterGroupOptions;
        this.fetchDataQuery.textSearch = fetchQuery.textSearch;
        this.fetchData();
    }

    private onCommandClick(commandClicked: CommandClicked) {
        this.onClick.emit(commandClicked)
    }

    private getFetchDataQuery() {
        this.fetchDataQuery.dynamicListId = this.dynamicList.id
        this.fetchDataQuery.paginationOptions = {
            pageNumber: this.listOptions.enablePagination ? this.paginator.pageIndex : 0,
            pageSize: this.listOptions.enablePagination ? this.paginator.pageSize : this.listOptions.defaultPageSize,
            needTotalItems: !this.isAlreadyFetchedTotal
        }
        this.fetchDataQuery.sortOptions = {
            sortableFields: [
                { fieldName: this.sort.active, sortType: this.sort.direction === 'asc' ? SortType.Asc : SortType.Desc }
            ]
        }

        return this.fetchDataQuery
    }

    private fetchData() {
        this.loading$.next(true)
        this.dynamicClient.executeQuery(this.dynamicList.id, this.fetchDataQuery)
            .pipe(
                tap(res => {
                    this.logger.debug('Reponse from query', res)
                    let readyData = res.data;
                    this.logger.debug('table data', readyData)
                    this.dataSource$.next(readyData)
                    if (!this.isAlreadyFetchedTotal) {
                        this.totalItems = res.totalItems;
                    }
                    this.readyToRender = true
                    this.logger.debug('Unblock loading for table')
                    this.loading$.next(false)
                }),
                catchError(err => of([])),
                finalize(() => this.loading$.next(false))
            ).subscribe();
    }

    private combineJsonStringToObject(arrayJsonStrings: Array<string>) {
        let objects: Array<any> = new Array<any>()

        _.forEach(arrayJsonStrings, (json) => {
            objects.push(JSON.parse(json))
        })

        return objects;
    }

    private translateData(renderingData: any, currentColumn: ExtendedColDef) {
        if (currentColumn.name === 'id' || currentColumn.name === '_id') {
            let checkData = renderingData['id']
            if (!checkData) {
                return renderingData['_id']
            }
            else {
                return checkData
            }
        }
        let displayData = null
        if (currentColumn.name.indexOf('.') > 0) {
            let extractData = new Function('data', `return data.${currentColumn.name}`)
            displayData = extractData(renderingData)
        }
        else {
            displayData = renderingData[currentColumn.name]
        }

        if (currentColumn.searchOptions.fieldValueType === FieldValueType.Select) {
            const datasource = _.find(this.datasourceCache, (elem: DatasourceCache) => elem.datasourceId === currentColumn.datasourceId);
            return _.find(datasource.data, elem => elem.value === displayData).name
        }
        return displayData
    }

    private isDisplayedAsHtml(currentColumn: ExtendedColDef) {
        return currentColumn.displayFormatAsHtml
    }

    private isBooleanAsHtml(currentColumn: ExtendedColDef) {
        return currentColumn.searchOptions.fieldValueType === FieldValueType.Checkbox
            || currentColumn.searchOptions.fieldValueType === FieldValueType.Slide
    }

    private isSelectAsHtml(currentColumn: ExtendedColDef) {
        return currentColumn.searchOptions.fieldValueType === FieldValueType.Select
    }

    private isTextAsHtml(currentColumn: ExtendedColDef) {
        return !this.isBooleanAsHtml(currentColumn) && !this.isSelectAsHtml(currentColumn)
    }

    private getBooleanValue(renderingData: any, currentColumn: ExtendedColDef) {
        if (currentColumn.name === 'id' || currentColumn.name === '_id') {
            let checkData = renderingData['id']
            if (!checkData) {
                return renderingData['_id']
            }
            else {
                return checkData
            }
        }
        let displayData = null
        if (currentColumn.name.indexOf('.') > 0) {
            let extractData = new Function('data', `return data.${currentColumn.name}`)
            displayData = extractData(renderingData)
        }
        else {
            displayData = renderingData[currentColumn.name]
        }
        switch (displayData.toString().toLowerCase()) {
            case "true":
            case "1":
            case "yes":
                return true;
            default:
                return false;
        }
    }

    getClassForChip(renderingData: any, currentColumn: ExtendedColDef) {
        let displayData = renderingData[currentColumn.name]
        const datasource = _.find(this.datasourceCache, (elem: DatasourceCache) => elem.datasourceId === currentColumn.datasourceId);
        let foundIndex = _.findIndex(datasource.data, (elem: any) => elem.value === displayData)
        return 'mat-chip-' + (foundIndex % 4)
    }

    private renderInnerHtml(renderingData: any, currentColumn: ExtendedColDef) {
        if (currentColumn.name === 'id' || currentColumn.name === '_id') {
            let checkData = renderingData['id']
            if (!checkData) {
                return renderingData['_id']
            }
            else {
                return checkData
            }
        }
        let displayData = null
        if (currentColumn.name.indexOf('.') > 0) {
            let extractData = new Function('data', `return data.${currentColumn.name}`)
            displayData = extractData(renderingData)
        }
        else {
            displayData = renderingData[currentColumn.name]
        }

        if (currentColumn.searchOptions.fieldValueType === FieldValueType.Select) {
            const datasource = _.find(this.datasourceCache, (elem: DatasourceCache) => elem.datasourceId === currentColumn.datasourceId);
            return _.find(datasource.data, elem => elem.value === displayData).name
        }
        return displayData
    }

    submitGrid() {
        this.fetchData();
    }
}
