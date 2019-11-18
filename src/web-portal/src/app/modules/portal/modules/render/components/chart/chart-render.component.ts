import { Component, OnInit, Input, Output, EventEmitter, AfterViewChecked, OnDestroy } from '@angular/core';
import { Chart, ChartsClient, ChartType, ExecuteParamModel, ChartParameterValue, PageSectionLayoutType } from 'services/portal.service';
import { ExtendedPageSection } from 'app/core/models/extended.models';
import { PageService } from 'services/page.service';
import { NGXLogger } from 'ngx-logger';
import { Store } from '@ngxs/store';
import { Observable, Subscription, BehaviorSubject } from 'rxjs';
import { PageStateModel } from 'stores/pages/page.state';
import { filter, tap } from 'rxjs/operators';
import { BeginBuildingBoundData, GatherSectionValidations, AddSectionBoundData, SectionValidationStateAction } from 'stores/pages/page.actions';
import { ChartOptions } from 'portal/modules/models/chart.extended.model';
import { DateUtils } from 'app/core/utils/date-util';

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

    lastComparedDate: Date
    colClass = 'col-lg-12'

    constructor(
        private pageService: PageService,
        private logger: NGXLogger,
        private chartsClient: ChartsClient,
        private store: Store
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

        this.chartOptions = ChartOptions.getChartOptions(this.chart.options)
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

    private setupOptions() {
        if (this.chartOptions.allowrealtime) {            
            this.interval = setInterval(() => {
                //this.readyToRender = false
                this.fetchDataForChart()
            }, this.chartOptions.timetorefresh * 1000)
        }
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
                this.chartData = res.isSuccess ? [...res.result] : null
                this.lastComparedDate = DateUtils.getUTCNow()
                setTimeout(() =>{
                    this.isDoneDefer = true
                }, this.deferRender)                
            }
        )
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
}
