import { Component, OnInit, Input, Output, EventEmitter, AfterViewChecked, OnDestroy } from '@angular/core';
import { Chart, ChartsClient, ChartType, ExecuteParamModel, ChartParameterValue } from 'services/portal.service';
import { ExtendedPageSection } from 'app/core/models/extended.models';
import { PageService } from 'services/page.service';
import { NGXLogger } from 'ngx-logger';
import { Store } from '@ngxs/store';
import { Observable, Subscription, BehaviorSubject } from 'rxjs';
import { PageStateModel } from 'stores/pages/page.state';
import { filter, tap } from 'rxjs/operators';
import { BeginBuildingBoundData, GatherSectionValidations, AddSectionBoundData, SectionValidationStateAction } from 'stores/pages/page.actions';
import { ChartOptions } from 'portal/modules/models/chart.extended.model';

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
    readyToRender = false
    
    chartOptions: ChartOptions

    interval: any

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
        this.fetchDataForChart()

        this.setupOptions()
    }

    private setupOptions(){
        if(this.chartOptions.allowrealtime){
            this.interval = setInterval(() => {
                this.readyToRender = false
                this.fetchDataForChart()
            }, this.chartOptions.timetorefresh * 1000)
        }
    }

    private fetchDataForChart(){
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
            chartParameterValues: executeParamModels  
        }).subscribe(
            res => {
                this.chartData = res.isSuccess ? res.result : null
                this.readyToRender = true
            }
        )
    }

    onRefresh(){
        this.readyToRender = false
        this.fetchDataForChart()
    }
    
    ngAfterViewChecked(): void {
        this.onRendered.emit()
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe()
        if(this.interval){
            clearInterval(this.interval)
        }
    }
}
