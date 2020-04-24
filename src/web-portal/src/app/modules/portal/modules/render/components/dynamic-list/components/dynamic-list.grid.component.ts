import { Component, OnInit, Input, ChangeDetectorRef, ViewChild, EventEmitter, Output, OnDestroy } from '@angular/core';
import { DynamicList, ColumndDef, SortType, CommandButtonInList, CommandPositionType,  DynamicListClient, FieldValueType, DynamicListFetchDataModel } from 'services/portal.service';
import { MatDialog } from '@angular/material/dialog';
import { BehaviorSubject, of, merge, Observable, Subscription } from 'rxjs';
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
import { ObjectUtils } from 'app/core/utils/object-util';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';

@Component({
    selector: 'dynamic-list-grid',
    templateUrl: './dynamic-list.grid.component.html',
    styleUrls: ['./dynamic-list.grid.component.scss']
})
export class DynamicListGridComponent implements OnInit, OnDestroy {

    @ViewChild('matTable', { static: false })
    private matTable: MatTable<any>;
    @ViewChild(MatPaginator, { static: false })
    private paginator: MatPaginator;
    @ViewChild(MatSort, { static: true })
    private sort: MatSort;

    @Input()
    dynamicList: DynamicList;

    @Output()
    onClick = new EventEmitter<CommandClicked>();

    @Input()
    listOptions: ListOptions

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
    isContructedGrid = false
    hasDetailCols = false

    datasourceSub: Subscription
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
        this.datasourceSub = this.dataSource$.pipe(
            tap(
                res => {
                    this.dataSource = res
                }
            )
        ).subscribe()
        this.constructGrid();
    }

    ngOnDestroy(): void {
        this.datasourceSub.unsubscribe()
    }

    private constructGrid() {
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
        const filters: ExtendedFilterField[] = []
        let counter = this.headers.length
        let needToWaitDatasource = false
        _.forEach(this.headers, (colDef: ExtendedColDef) => {
            // We will fetch datasource and then passing its for filter
            if (colDef.searchOptions.fieldValueType === FieldValueType.Select) {
                needToWaitDatasource = true
                this.datasoureOptsService
                    .executeDatasourceOptions(colDef.datasourceOptions, null)
                    .subscribe(
                        res => {
                            const datasource = {
                                data: ObjectUtils.isArray(res) ? res : [res],
                                datasourceId: Guid.create().toString()
                            }
                            this.datasourceCache.push(datasource)
                            colDef.datasourceId = datasource.datasourceId

                            const filter: ExtendedFilterField = {
                                name: colDef.name,
                                displayName: colDef.displayName,
                                allowInAdvancedMode: colDef.searchOptions.allowInAdvancedMode,
                                allowTextSearch: colDef.searchOptions.allowTextSearch,
                                fieldValueType: colDef.searchOptions.fieldValueType,
                                isHidden: !colDef.searchOptions.allowInAdvancedMode,
                                jsonData: datasource.data,
                                datasourceId: datasource.datasourceId
                            }
                            filters.push(filter)
                            counter--
                            if (counter == 0) {
                                this.filters = filters
                                this.isContructedGrid = true
                                this.initFetchData()
                            }
                        }
                    )
            }
            else {
                const filter: ExtendedFilterField = {
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
        const foundSort = _.find(this.headers, (element: ColumndDef) => {
            return element.allowSort;
        });
        if(foundSort){
            this.defaultSortColumn = foundSort.name
        }

        if(this.dynamicList.commandsList && this.dynamicList.commandsList.commandButtonsInList.length > 0){
            this.commandsInList = _.filter(this.dynamicList.commandsList.commandButtonsInList, (element: CommandButtonInList) => {
                return element.commandPositionType === CommandPositionType.InList
            })
        }

        _.forEach(this.headers, (element) => {
            if (!element.isHidden && !element.inDetailMode)
                this.displayedColumns.push(element.name)
        })
        // Check we have any command in list
        if (this.commandsInList.length > 0 || this.hasDetailCols) {
            this.displayedColumns.push('actions')
        }

        if(!needToWaitDatasource){
            this.initFetchData()
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
        const fetchQuery = $event as DynamicListFetchDataModel
        this.fetchDataQuery = this.getFetchDataQuery();
        this.fetchDataQuery.filledParameterOptions = fetchQuery.filledParameterOptions;
        this.fetchDataQuery.filterGroupOptions = fetchQuery.filterGroupOptions;
        this.fetchDataQuery.textSearch = fetchQuery.textSearch;
        this.fetchData();
    }

    private initFetchData(){
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
        if(this.sort.active){
            this.fetchDataQuery.sortOptions = {
                sortableFields: [
                    { fieldName: this.sort.active, sortType: this.sort.direction === 'asc' ? SortType.Asc : SortType.Desc }
                ]
            }
        }
        else{
            this.fetchDataQuery.sortOptions = {
                sortableFields: [ ]
            }
        }


        return this.fetchDataQuery
    }

    private fetchData() {
        this.loading$.next(true)
        this.dynamicClient.executeQuery(this.dynamicList.id, this.fetchDataQuery)
            .pipe(
                tap(res => {
                    this.logger.debug('Reponse from query', res)
                    const readyData = res.data;
                    this.logger.debug('table data', readyData)
                    this.dataSource$.next(readyData)
                    if (!this.isAlreadyFetchedTotal) {
                        this.totalItems = res.totalItems;
                        this.isAlreadyFetchedTotal = true // stop getting another total items
                    }
                    this.readyToRender = true
                    this.logger.debug('Unblock loading for table')
                    this.loading$.next(false)
                    this.cd.detectChanges()
                }),
                catchError(err => of([])),
                finalize(() => this.loading$.next(false))
            ).subscribe();
    }

    private combineJsonStringToObject(arrayJsonStrings: Array<string>) {
        const objects: Array<any> = new Array<any>()

        _.forEach(arrayJsonStrings, (json) => {
            objects.push(JSON.parse(json))
        })

        return objects;
    }

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
            if(ObjectUtils.isNotNull(datasource)){
                const found = _.find(datasource.data, elem => elem ? elem.value.toString() === displayData : false)
                return found ? found.name : displayData
            }
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
        switch (displayData.toString().toLowerCase()) {
            case 'true':
            case '1':
            case 'yes':
                return true;
            default:
                return false;
        }
    }

    getClassForChip(renderingData: any, currentColumn: ExtendedColDef) {
        const displayData = renderingData[currentColumn.name]
        const datasource = _.find(this.datasourceCache, (elem: DatasourceCache) => elem.datasourceId === currentColumn.datasourceId);
        const foundIndex = _.findIndex(datasource.data, (elem: any) => elem.value.toString() === displayData)
        return 'mat-chip-' + (foundIndex % 4)
    }

    private renderInnerHtml(renderingData: any, currentColumn: ExtendedColDef) {
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
            if(ObjectUtils.isNotNull(datasource)){
                const found = _.find(datasource.data, elem => elem ? elem.value.toString()  === displayData : false)
                return found ? found.name : displayData
            }
        }
        return displayData
    }

    submitGrid() {
        this.fetchData();
    }
}
