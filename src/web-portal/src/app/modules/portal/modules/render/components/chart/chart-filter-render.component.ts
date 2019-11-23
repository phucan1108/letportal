import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Chart, ChartsClient, FilterType } from 'services/portal.service';
import { NGXLogger } from 'ngx-logger';
import { ChartOptions, ExtendedChartFilter } from 'portal/modules/models/chart.extended.model';
import * as _ from 'lodash';
import { PageService } from 'services/page.service';
import { DateUtils } from 'app/core/utils/date-util';

@Component({
    selector: 'chart-filter-render',
    templateUrl: 'chart-filter-render.component.html'
})

export class ChartFilterRenderComponent implements OnInit {
    @Input()
    chart: Chart

    @Input()
    chartOptions: ChartOptions

    @Output()
    applied = new EventEmitter<any>()

    filterType = FilterType
    chartFilters: ExtendedChartFilter[] = []
    
    constructor(
        private pageService: PageService,
        private chartsClient: ChartsClient,
        private logger: NGXLogger
    ) { }

    ngOnInit() {
        if (this.chart.chartFilters && this.chart.chartFilters.length > 0) {
            _.forEach(this.chart.chartFilters, c => {
                let temp: ExtendedChartFilter = <ExtendedChartFilter>c
                temp.defaultObj = this.getDefaultObject(c.defaultValue, c.type, c.isMultiple)
                switch (c.type) {
                    case FilterType.Select:
                        temp.datasource = this.pageService.fetchDatasourceOptions(c.datasourceOptions)
                        break
                    case FilterType.NumberPicker:
                        temp.datasource = this.generateDatasourceForNumberRange(c.rangeValue, c.isMultiple)
                        break
                    case FilterType.DatePicker:
                        if (c.rangeValue) {
                            let tempDates: string[] = JSON.parse(c.rangeValue)
                            if (tempDates.length == 1) {
                                // Only min date
                                temp.minDate = this.convertNowWords(tempDates[0], false)
                            }
                            else if (tempDates.length == 2) {
                                temp.minDate = this.convertNowWords(tempDates[0], false)
                                temp.maxDate = this.convertNowWords(tempDates[1], false)
                            }
                        }
                        break
                    case FilterType.MonthYearPicker:
                        if (c.rangeValue) {
                            let tempDates: string[] = JSON.parse(c.rangeValue)
                            if (tempDates.length == 1) {
                                // Only min date
                                temp.minDate = this.convertNowWords(tempDates[0], true)
                            }
                            else if (tempDates.length == 2) {
                                temp.minDate = this.convertNowWords(tempDates[0], true)
                                temp.maxDate = this.convertNowWords(tempDates[1], true)
                            }
                        }
                        break
                }

                this.chartFilters.push(temp)
            })
        }
    }

    filterChanged($event){
       this.logger.debug('Filter changed', $event) 
    }

    private convertNowWords(nowStr: string, isMonthPicker: boolean) {
        let nowIndex = nowStr.indexOf('Now')
        if (nowIndex > -1) {
            let nowDate = new Date()
            if (nowStr.length === 3) {
                return nowDate
            }
            else {
                let splitMinus = nowStr.split('-')
                let splitPlus = nowStr.split('+')
                if (splitMinus.length > 1) {
                    if(isMonthPicker){
                        nowDate.setMonth(nowDate.getMonth() - parseInt(splitMinus[1]))
                    }
                    else{
                        nowDate.setDate(nowDate.getDate() - parseInt(splitMinus[1]))
                    }
                }
                else if (splitPlus.length > 1) {
                    if(isMonthPicker){
                        nowDate.setMonth(nowDate.getMonth() + parseInt(splitMinus[1]))
                    }
                    else{
                        nowDate.setDate(nowDate.getDate() + parseInt(splitMinus[1]))
                    }                    
                }
                return nowDate
            }
        }
        else {
            if(isMonthPicker){
                let splitted = nowStr.split('/')
                return new Date(parseInt(splitted[1]), parseInt(splitted[0]), 1)
            }
            else{
                return new Date(nowStr)
            }
        }
    }

    private generateDatasourceForNumberRange(numberRange: string, isMultiple: boolean) {
        const hasDoublePoints = numberRange.indexOf('..') > 0
        if (hasDoublePoints) {
            let count = (numberRange.match(/../g) || []).length;
            if (count == 2) {
                // Case [10..2..20]
                let splitted = numberRange.substr(1, numberRange.length - 2).split('..')
                let startNum = parseInt(splitted[0])
                let endNum = parseInt(splitted[2])
                let stepNum = parseInt(splitted[1])
                if (startNum <= endNum) {
                    let ds: any[] = []
                    for (let i = startNum; i <= endNum; i += stepNum) {
                        ds.push({
                            name: i.toString(),
                            value: i
                        })
                    }

                    return ds
                }
                else {
                    let ds: any[] = []
                    for (let i = endNum; i <= startNum; i += stepNum) {
                        ds.push({
                            name: i.toString(),
                            value: i
                        })
                    }
                    return ds
                }
            }
            else {
                // Case [10..20]
                let splitted = numberRange.substr(1, numberRange.length - 2).split('..')
                let startNum = parseInt(splitted[0])
                let endNum = parseInt(splitted[1])
                if (startNum <= endNum) {
                    let ds: any[] = []
                    for (let i = startNum; i <= endNum; i++) {
                        ds.push({
                            name: i.toString(),
                            value: i
                        })
                    }

                    return ds
                }
                else {
                    let ds: any[] = []
                    for (let i = endNum; i <= startNum; i++) {
                        ds.push({
                            name: i.toString(),
                            value: i
                        })
                    }
                    return ds
                }
            }
        }
        else {
            // return as number[]
            let arrayNum: number[] = JSON.parse(numberRange)
            return arrayNum.map(a => ({
                name: a.toString(),
                value: a
            }))
        }
    }

    private getDefaultObject(defaultValue: string, type: FilterType, isMultiple: boolean) {
        if (defaultValue) {
            switch (type) {
                case FilterType.Checkbox:
                    return defaultValue === 'true'
                case FilterType.Select:
                    if (isMultiple) {
                        return JSON.parse(defaultValue)
                    }
                    else {
                        return defaultValue
                    }
                case FilterType.NumberPicker:
                    if (isMultiple) {
                        return JSON.parse(defaultValue)
                    }
                    else {
                        return parseInt(defaultValue)
                    }
                case FilterType.DatePicker:
                    if (isMultiple) {
                        let tempDates: Date[] = []
                        let data = JSON.parse(defaultValue)
                        tempDates.push(new Date(data[0]))
                        tempDates.push(new Date(data[1]))
                        return tempDates
                    }
                    else {
                        return new Date(defaultValue)
                    }
                default:
                    return defaultValue
            }
        }
        return null
    }
}