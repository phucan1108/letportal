import { Component, OnInit, Input, Output, EventEmitter, AfterViewChecked, OnDestroy, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { Chart, ChartsClient, ChartType, ExecuteParamModel, ChartParameterValue, PageSectionLayoutType } from 'services/portal.service';
import { ExtendedPageSection } from 'app/core/models/extended.models';
import { PageService } from 'services/page.service';
import { NGXLogger } from 'ngx-logger';
import { Store } from '@ngxs/store';
import { Observable, Subscription, BehaviorSubject } from 'rxjs';
import { PageStateModel } from 'stores/pages/page.state';
import { filter, tap, toArray } from 'rxjs/operators';
import { BeginBuildingBoundData, GatherSectionValidations, AddSectionBoundData, SectionValidationStateAction } from 'stores/pages/page.actions';
import { ChartOptions } from 'portal/modules/models/chart.extended.model';
import { DateUtils } from 'app/core/utils/date-util';
import * as _ from 'lodash';
import { ArrayUtils } from 'app/core/utils/array-util';
import * as moment from 'moment'
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { ObjectUtils } from 'app/core/utils/object-util';
@Component({
    selector: 'let-chart-render',
    templateUrl: './chart-render.component.html',
    styleUrls: ['./chart-render.component.scss']
})
export class ChartRenderComponent implements OnInit, AfterViewChecked, OnDestroy {

    @Input()
    chart: Chart

    @Input()
    displayName: string

    @Input()
    section: ExtendedPageSection

    @Output()
    onRendered = new EventEmitter()

    chartType = ChartType;
    pageState$: Observable<PageStateModel>
    subscription: Subscription

    chartData: any
    readyToRender = true

    chartOptions: ChartOptions
    chartColors: any
    interval: any

    deferRender = 500
    isDoneDefer = false
    isOpenningFilter = false

    lastComparedDate: Date
    colClass = 'col-lg-12'
    hasFilters = false
    isMultiData = false

    xScaleMin: any
    xScaleMax: any
    yScaleMin: any
    yScaleMax: any

    constructor(
        private pageService: PageService,
        private logger: NGXLogger,
        private chartsClient: ChartsClient,
        private shortcutUtil: ShortcutUtil,
        private store: Store,
        private cd: ChangeDetectorRef
    ) { }

    ngOnInit(): void {
        this.pageState$ = this.store.select<PageStateModel>(state => state.page)
        this.subscription = this.pageState$.pipe(
            filter(state => state.filterState
                && (state.filterState === BeginBuildingBoundData
                    || state.filterState === GatherSectionValidations)),
            tap(
                state => {
                    switch (state.filterState) {
                        case BeginBuildingBoundData:
                            this.store.dispatch(new AddSectionBoundData({
                                name: this.section.name,
                                isKeptDataName: true,
                                data: null
                            }, []))
                            break
                        case GatherSectionValidations:
                            this.store.dispatch(new SectionValidationStateAction(this.section.name, true))
                            break
                    }
                }
            )
        ).subscribe()

        this.hasFilters = this.chart.chartFilters && this.chart.chartFilters.length > 0
        this.chartOptions = ChartOptions.getChartOptions(this.chart.options)
        this.setupDataRange(this.chartOptions.datarange)
        this.isMultiData = this.chart.definitions.mappingProjection.indexOf('group=') > 0
        if (this.chartOptions.colors.length == 1) {
            this.chartColors = this.chartOptions.colors[0]
        }
        else {
            this.chartColors = {
                domain: this.chartOptions.colors
            }
        }

        switch (this.chart.layoutType) {
            case PageSectionLayoutType.OneColumn:
                this.colClass = 'col-lg-12'
                break
            case PageSectionLayoutType.TwoColumns:
                this.colClass = 'col-lg-6'
                break
            case PageSectionLayoutType.ThreeColumns:
                this.colClass = 'col-lg-4'
                break
            case PageSectionLayoutType.FourColumns:
                this.colClass = 'col-lg-3'
                break
            case PageSectionLayoutType.SixColumns:
                this.colClass = 'col-lg-2'
                break
        }

        this.fetchDataForChart()

        this.setupOptions()
    }

    onRefresh() {
        //this.readyToRender = false
        this.fetchDataForChart()
    }

    ngAfterViewChecked(): void {
        this.onRendered.emit()
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe()
        if (this.interval) {
            clearInterval(this.interval)
        }
    }

    applied($event){

    }

    openFilters(){
        this.isOpenningFilter = !this.isOpenningFilter
    }

    private setupDataRange(dataRange: string) {
        if (dataRange) {
            const splitted = dataRange.split(';')
            _.forEach(splitted, s => {
                const subSplitted = s.split('=')
                if (subSplitted[0] === 'x') {
                    const minMaxRange = JSON.parse(subSplitted[1])
                    this.xScaleMin = minMaxRange[0]
                    this.xScaleMax = minMaxRange[1]
                }

                if (subSplitted[0] === 'y') {
                    const minMaxRange = JSON.parse(subSplitted[1])
                    this.yScaleMin = minMaxRange[0]
                    this.yScaleMax = minMaxRange[1]
                }
            })
        }
    }

    private setupOptions() {
        if (this.chartOptions.allowrealtime) {
            this.interval = this.fetchInterval()
        }
    }

    private fetchInterval(){
        return setInterval(() => {
            //this.readyToRender = false
            this.fetchDataForChart()
        }, this.chartOptions.timetorefresh * 1000)
    }

    private fetchDataForChart() {
        const params = this.pageService.retrieveParameters(this.chart.databaseOptions.query)
        let executeParamModels: ChartParameterValue[] = []
        params.map(a => {
            executeParamModels.push({
                name: a.name,
                value: a.replaceValue,
                replaceDQuotes: a.removeQuotes
            })
        })

        this.chartsClient.execution({
            chartId: this.chart.id,
            chartFilterValues: [],
            chartParameterValues: executeParamModels,
            isRealTime: this.chartOptions.allowrealtime,
            realTimeField: this.chartOptions.comparerealtimefield,
            lastRealTimeComparedDate: this.lastComparedDate ? this.lastComparedDate : null
        }).subscribe(
            res => {
                if(res.isSuccess){
                    res.result = this.convertResNameToDate(res.result, this.isMultiData, this.chartOptions.xformatdate)
                }
                if (this.chartData) {
                    if (res.isSuccess && this.chartOptions.allowrealtime && this.chartOptions.comparerealtimefield) {
                        if (!this.isMultiData) {
                            _.forEach(res.result, a => {
                                this.chartData.push(a)
                            })
                            this.chartData = [...this.chartData]
                        }
                        else {
                            _.forEach(res.result, a => {
                                let found = _.find(this.chartData, b => b.name === a.name)
                                if (found) {
                                    _.forEach(a.series, b => {
                                        found.series.push(b)
                                    })
                                }
                                else {
                                    this.chartData.push(a)
                                }
                            })

                            this.chartData = [...this.chartData]
                        }
                    }
                    else {
                        this.chartData = res.isSuccess ? [...res.result] : [...this.chartData]
                    }
                }
                else {
                    this.chartData = res.isSuccess ? [...res.result] : null
                }
                if (res.isSuccess && res.result) {
                    this.lastComparedDate = DateUtils.getUTCNow()
                }
                if (!this.isDoneDefer) {
                    setTimeout(() => {
                        this.isDoneDefer = true
                    }, this.deferRender)
                }
            },
            err => {
                if (this.interval) {
                    clearInterval(this.interval)
                }

                this.shortcutUtil.notifyMessage("Oops! Something went wrong, please check data or refresh F5 again", ToastType.Error)
            }
        )
    }

    private convertResNameToDate(result: any, isMultiData: boolean, xFormatDate: string){
        if(!!xFormatDate){
            if(isMultiData){
                _.forEach(result, r => {
                    const isDate = moment(r.series[0].name).isValid()
                    if(isDate){
                        _.forEach(r.series, e => {
                            e.name = moment(e.name).format(xFormatDate)
                        })
                    }
                })
            }
            else{
                const isDate = moment(result[0].name).isValid()
                if(isDate){
                    _.forEach(result, e => {
                        e.name = moment(e.name).format(xFormatDate)
                    })
                }            
            }   
    
            return result
        }
        else{
            return result
        }        
    }
}
