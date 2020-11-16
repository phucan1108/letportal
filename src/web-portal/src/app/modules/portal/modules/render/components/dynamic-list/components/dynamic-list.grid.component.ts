import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { AfterViewInit, ChangeDetectorRef, Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import { ObjectUtils } from 'app/core/utils/object-util';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { Guid } from 'guid-typescript';
import { NGXLogger } from 'ngx-logger';
import { ExtendedFilterField, ListOptions } from 'portal/modules/models/dynamiclist.extended.model';
import { BehaviorSubject, of, Subscription } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { DatasourceOptionsService } from 'services/datasourceopts.service';
import { PageService } from 'services/page.service';
import { ColumnDef, CommandButtonInList, CommandPositionType, DynamicList, DynamicListClient, DynamicListFetchDataModel, DynamicListSourceType, FieldValueType, FilledParameter, SortType } from 'services/portal.service';
import { CommandClicked, DatasourceCache } from '../models/commandClicked';
import { ExtendedColDef } from '../models/extended.model';
import { DynamicListDataDialogComponent } from './dynamic-list-data-dialog.component';


@Component({
    selector: 'dynamic-list-grid',
    templateUrl: './dynamic-list.grid.component.html',
    styleUrls: ['./dynamic-list.grid.component.scss']
})
export class DynamicListGridComponent implements OnInit, OnDestroy, AfterViewInit {

    @ViewChild('matTable', { static: false })
    private matTable: MatTable<any>;
    @ViewChild(MatPaginator, { static: true })
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
    needToWaitDatasource = false
    datasourceSub: Subscription
    numberOfActions = 0
    constructor(
        private datasoureOptsService: DatasourceOptionsService,
        private breakpointObserver: BreakpointObserver,
        private dynamicClient: DynamicListClient,
        private pageService: PageService,
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
    ngAfterViewInit(): void {
        if (!this.needToWaitDatasource) {
            this.logger.debug('Hit after view init fecthing data', this.sort, this.paginator)
            this.initFetchData()
        }      
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
        this.dynamicList.columnsList.columnDefs?.forEach(colDef => {
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
        this.needToWaitDatasource = false
        this.headers?.forEach((colDef: ExtendedColDef) => {
            // We will fetch datasource and then passing its for filter
            if (colDef.searchOptions.fieldValueType === FieldValueType.Select) {
                this.needToWaitDatasource = true
                this.datasoureOptsService
                    .executeDatasourceOptions(colDef.datasourceOptions, null)
                    .subscribe(
                        res => {
                            if (ObjectUtils.isNotNull(colDef.datasourceOptions.outputMapProjection)) {
                                res = ObjectUtils.projection(colDef.datasourceOptions.outputMapProjection, ObjectUtils.isArray(res) ? res : [res])
                            }
                            const datasource = {
                                data: ObjectUtils.isArray(res) ? res : [res],
                                datasourceId: Guid.create().toString()
                            }
                            // Very inconvient when value is number/boolean/datetime
                            let valueType = FieldValueType.Text
                            try {
                                const firstValue = datasource[0].value
                                const isNumber = ObjectUtils.isNumber(firstValue)
                                const isBoolean = ObjectUtils.isBoolean(firstValue)
                                if (isNumber) {
                                    valueType = FieldValueType.Number
                                }
                                if (isBoolean) {
                                    valueType = FieldValueType.Slide
                                }
                            }
                            catch
                            {

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
                                // Very import to call detectChanges because this data always comes ngViewInit
                                this.cd.detectChanges()
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
        const foundSort = this.headers.find((element: ColumnDef) => {
            return element.allowSort;
        });
        if (foundSort) {
            this.defaultSortColumn = foundSort.name
        }

        if (this.dynamicList.commandsList && this.dynamicList.commandsList.commandButtonsInList.length > 0) {
            this.commandsInList = this.dynamicList.commandsList.commandButtonsInList.filter((element: CommandButtonInList) => {
                return element.commandPositionType === CommandPositionType.InList
            })
        }

        this.headers?.forEach((element) => {
            if (!element.isHidden && !element.inDetailMode)
                this.displayedColumns.push(element.name)
        })
        // Check we have any command in list
        if (this.commandsInList.length > 0 || this.hasDetailCols) {
            this.displayedColumns.push('actions')
        }
        this.numberOfActions = this.commandsInList.length
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
        this.fetchDataQuery.filterGroupOptions = fetchQuery.filterGroupOptions;
        this.fetchDataQuery.textSearch = fetchQuery.textSearch;
        this.logger.debug('Hit submit search')
        this.fetchData();
    }

    private initFetchData() {
        // Trigger loading data form sortChange and page event
        if (this.listOptions.enablePagination) {
            this.paginator.page
                .pipe(
                    tap(() => {
                        this.logger.debug('Hit paginator change')
                        this.fetchDataQuery = this.getFetchDataQuery();
                        this.fetchData();
                    })
                ).subscribe()
        }
        
        this.sort.sortChange
        .pipe(
            tap(() => {
                this.logger.debug('Hit sort change')
                this.fetchDataQuery = this.getFetchDataQuery();
                this.fetchData();
            })
        )
        .subscribe()
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
        if (this.sort.active) {
            this.fetchDataQuery.sortOptions = {
                sortableFields: [
                    { fieldName: this.sort.active, sortType: this.sort.direction === 'asc' ? SortType.Asc : SortType.Desc }
                ]
            }
        }
        else {
            this.fetchDataQuery.sortOptions = {
                sortableFields: []
            }
        }

        this.fetchDataQuery.filledParameterOptions = {
            filledParameters: []
        }
        if (this.dynamicList.listDatasource.sourceType === DynamicListSourceType.Database) {
            const params = this.pageService.retrieveParameters(this.dynamicList.listDatasource.databaseConnectionOptions.query)
            if (ObjectUtils.isNotNull(params)) {
                this.fetchDataQuery.filledParameterOptions = {
                    filledParameters: params.map(a => <FilledParameter>{
                        name: a.name,
                        value: a.replaceValue
                    })
                }
            }
        }

        return this.fetchDataQuery
    }

    private fetchData() {
        this.logger.debug('Hit Fetch Data')
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

        arrayJsonStrings?.forEach((json) => {
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
            const datasource = this.datasourceCache.find((elem: DatasourceCache) => elem.datasourceId === currentColumn.datasourceId);
            if (ObjectUtils.isNotNull(datasource)) {
                const found = datasource.data.find(elem => elem ? elem.value.toString() === displayData : false)
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
        const datasource = this.datasourceCache.find((elem: DatasourceCache) => elem.datasourceId === currentColumn.datasourceId);
        const foundIndex = datasource.data.findIndex((elem: any) => elem.value.toString() === displayData)
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
            const datasource = this.datasourceCache.find((elem: DatasourceCache) => elem.datasourceId === currentColumn.datasourceId);
            if (ObjectUtils.isNotNull(datasource)) {
                const found = datasource.data.find(elem => elem ? elem.value.toString() === displayData : false)
                return found ? found.name : displayData
            }
        }
        return displayData
    }

    submitGrid() {
        this.fetchData();
    }
}
