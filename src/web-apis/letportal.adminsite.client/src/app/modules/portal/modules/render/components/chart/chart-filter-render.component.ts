import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Chart, ChartsClient, FilterType, ChartFilterValue } from 'services/portal.service';
import { NGXLogger } from 'ngx-logger';
import { ChartOptions, ExtendedChartFilter } from 'portal/modules/models/chart.extended.model';
 
import { PageService } from 'services/page.service';
import { DateUtils } from 'app/core/utils/date-util';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import StringUtils from 'app/core/utils/string-util';
import { ObjectUtils } from 'app/core/utils/object-util';

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

    formGroup: FormGroup
    filterType = FilterType
    chartFilters: ExtendedChartFilter[] = []

    filterValues: ChartFilterValue[] = []

    constructor(
        private pageService: PageService,
        private fb: FormBuilder,
        private chartsClient: ChartsClient,
        private logger: NGXLogger
    ) { }

    ngOnInit() {
        if (this.chart.chartFilters && this.chart.chartFilters.length > 0) {
            const formControls: any = []
            this.chart.chartFilters?.forEach(c => {
                let tempName = c.name
                // Note: in mongodb, we allow to have '.' in name, so we need to remove this
                if(tempName.indexOf('.') > 0){
                    tempName = StringUtils.replaceAllOccurences(tempName,'.','')
                }
                const temp: ExtendedChartFilter = c as ExtendedChartFilter
                temp.defaultObj = this.getDefaultObject(c.defaultValue, c.type, c.isMultiple)
                switch (c.type) {
                    case FilterType.Checkbox:
                        formControls[tempName] = new FormControl(
                            ObjectUtils.isNotNull(c.defaultValue) && c.allowDefaultValue ?
                                c.defaultValue.toUpperCase() === 'TRUE' : false)
                        break
                    case FilterType.Select:
                        temp.datasource = this.pageService.fetchDatasourceOptions(c.datasourceOptions)
                        let defaultValSelect: any
                        defaultValSelect = ObjectUtils.isNotNull(c.defaultValue) && c.allowDefaultValue ?
                            (c.isMultiple ? JSON.parse(c.defaultValue) : c.defaultValue)
                            : ''
                        formControls[tempName] = new FormControl(defaultValSelect, Validators.required)
                        break
                    case FilterType.NumberPicker:
                        temp.datasource = this.generateDatasourceForNumberRange(c.rangeValue, c.isMultiple)
                        let defaultValNum: any
                        const tempDefault = StringUtils.replaceAllOccurences(c.defaultValue, '\'','"')
                        defaultValNum = ObjectUtils.isNotNull(c.defaultValue) && c.allowDefaultValue ?
                            (c.isMultiple ? JSON.parse(tempDefault) : parseInt(c.defaultValue))
                            : 0
                        formControls[tempName] = new FormControl(defaultValNum, Validators.required)
                        break
                    case FilterType.DatePicker:
                        if (ObjectUtils.isNotNull(c.rangeValue)) {
                            const tempStr = StringUtils.replaceAllOccurences(c.rangeValue, '\'', '"')
                            const tempDates: string[] = JSON.parse(tempStr)
                            if (tempDates.length == 1) {
                                // Only min date
                                // Accept some values: ['2020-02-02'] or ['Now'] or ['Now-5'] or ['Now+5']
                                temp.minDate = this.convertNowWords(tempDates[0], false)

                                const currentYear = (new Date()).getFullYear()
                                temp.maxDate = new Date(currentYear + 30, 1, 1)
                            }
                            else if (tempDates.length == 2) {
                                temp.minDate = this.convertNowWords(tempDates[0], false)
                                temp.maxDate = this.convertNowWords(tempDates[1], false)
                            }
                        }
                        else{
                            temp.minDate = new Date(1970,1,1)
                            temp.maxDate = new Date()
                        }

                        if (c.isMultiple) {
                            let defaultMinDate: any = new Date()
                            let defaultMaxDate: any = new Date()
                            if(ObjectUtils.isNotNull(c.defaultValue)){
                                const tempStr = StringUtils.replaceAllOccurences(c.defaultValue, '\'', '"')
                                const tempDates: string[] = JSON.parse(tempStr)
                                if (tempDates.length == 1) {
                                    defaultMinDate = this.convertNowWords(tempDates[0], false)
                                }
                                else if (tempDates.length == 2) {
                                    defaultMinDate = this.convertNowWords(tempDates[0], false)
                                    defaultMaxDate = this.convertNowWords(tempDates[1], false)
                                }
                            }
                            else{
                                // If we don't set Default, set it to Now
                                formControls[tempName + '_min'] = new FormControl(defaultMinDate, Validators.required)
                                formControls[tempName + '_max'] = new FormControl(defaultMaxDate, Validators.required)
                            }
                        }
                        else {
                            let defaultMinDate: any
                            defaultMinDate = ObjectUtils.isNotNull(c.defaultValue) && c.allowDefaultValue ? this.convertNowWords(c.defaultValue, false) : new Date()
                            formControls[tempName] = new FormControl(defaultMinDate.toISOString(), Validators.required)
                        }
                        break
                    case FilterType.MonthYearPicker:
                        if (ObjectUtils.isNotNull(c.rangeValue)) {
                            const tempStr = StringUtils.replaceAllOccurences(c.rangeValue, '\'', '"')
                            const tempDates: string[] = JSON.parse(tempStr)
                            if (tempDates.length == 1) {
                                // Only min date
                                temp.minDate = this.convertNowWords(tempDates[0], true)
                                const currentYear = (new Date()).getFullYear()
                                const currentMonth = (new Date()).getMonth()
                                temp.maxDate = new Date(currentYear + 30, currentMonth, 1)
                            }
                            else if (tempDates.length == 2) {
                                temp.minDate = this.convertNowWords(tempDates[0], true)
                                temp.maxDate = this.convertNowWords(tempDates[1], true)
                            }
                        }
                        else{
                            temp.minDate = new Date(1970,1,1)
                            temp.maxDate = new Date()
                        }
                        if (c.isMultiple) {
                            let defaultMinDate: any
                            let defaultMaxDate: any
                            const tempStr = StringUtils.replaceAllOccurences(c.defaultValue, '\'', '"')
                            const tempDates: string[] = JSON.parse(tempStr)
                            if (tempDates.length == 1) {
                                defaultMinDate = this.convertNowWords(tempDates[0], true)
                            }
                            else if (tempDates.length == 2) {
                                defaultMinDate = this.convertNowWords(tempDates[0], true)
                                defaultMaxDate = this.convertNowWords(tempDates[1], true)
                            }
                            formControls[tempName + '_min'] = new FormControl(defaultMinDate, Validators.required)
                            formControls[tempName + '_max'] = new FormControl(defaultMaxDate, Validators.required)
                        }
                        else {
                            let defaultMinDate: any
                            defaultMinDate = ObjectUtils.isNotNull(c.defaultValue) && c.allowDefaultValue ? this.convertNowWords(c.defaultValue, true) : new Date()
                            formControls[tempName] = new FormControl(defaultMinDate, Validators.required)
                        }
                        break
                }

                this.chartFilters.push(temp)
            })
            this.formGroup = new FormGroup(formControls)
        }
    }

    submit() {
        if (this.formGroup.valid) {
            this.filterValues = []
            this.chartFilters?.forEach(filter => {
                this.filterValues.push({
                    filterType: filter.type,
                    isMultiple: filter.isMultiple,
                    name: filter.name,
                    value: this.getFilterValue(filter, this.formGroup)
                })
            })
            this.applied.emit(this.filterValues)
        }
    }

    filterChanged($event) {
        this.logger.debug('Filter changed', $event)
        const filterChanged: ExtendedChartFilter = $event.filter
        switch (filterChanged.type) {
            case FilterType.Checkbox:
                break
            case FilterType.DatePicker:
                break
            case FilterType.MonthYearPicker:
                break
            case FilterType.NumberPicker:
                break
            case FilterType.Select:
                break
        }
    }

    private getFilterValue(filter: ExtendedChartFilter, formGroup: FormGroup) {
        const hasDot = filter.name.indexOf('.') > 0
        let tempName = filter.name
        if(hasDot){
            tempName = StringUtils.replaceAllOccurences(tempName, '.', '')
        }
        switch (filter.type) {
            case FilterType.Checkbox:
                return formGroup.get(tempName).value.toString()
            case FilterType.Select:
                return filter.isMultiple ? JSON.stringify(formGroup.get(tempName).value) : formGroup.get(tempName).value.toString()
            case FilterType.NumberPicker:
                return filter.isMultiple ? JSON.stringify(formGroup.get(tempName).value) : formGroup.get(tempName).value.toString()
            case FilterType.DatePicker:
                if (filter.isMultiple) {
                    const minDate = new Date(formGroup.get(tempName + '_min').value)
                    const maxDate = new Date(formGroup.get(tempName + '_max').value)
                    const groupDates: string[] = []
                    groupDates.push(minDate.toDateString())
                    groupDates.push(maxDate.toDateString())
                    return JSON.stringify(groupDates)
                }
                else {
                    const minDate = new Date(formGroup.get(tempName).value)
                    return minDate.toDateString()
                }
            case FilterType.MonthYearPicker:
                if (filter.isMultiple) {
                    const minDate = new Date(formGroup.get(tempName + '_min').value)
                    const maxDate = new Date(formGroup.get(tempName + '_max').value)
                    const groupDates: string[] = []
                    groupDates.push(minDate.toDateString())
                    groupDates.push(maxDate.toDateString())
                    return JSON.stringify(groupDates)
                }
                else {
                    const minDate = new Date(formGroup.get(tempName).value)
                    return minDate.toDateString()
                }
        }
        return null
    }

    private convertNowWords(nowStr: string, isMonthPicker: boolean) {
        const nowIndex = nowStr.toUpperCase().indexOf('NOW')
        if (nowIndex > -1) {
            const nowDate = new Date()
            if (nowStr.length === 3) {
                return nowDate
            }
            else {
                const splitMinus = nowStr.split('-')
                const splitPlus = nowStr.split('+')
                if (splitMinus.length > 1) {
                    if (isMonthPicker) {
                        nowDate.setMonth(nowDate.getMonth() - parseInt(splitMinus[1]))
                    }
                    else {
                        nowDate.setDate(nowDate.getDate() - parseInt(splitMinus[1]))
                    }
                }
                else if (splitPlus.length > 1) {
                    if (isMonthPicker) {
                        nowDate.setMonth(nowDate.getMonth() + parseInt(splitMinus[1]))
                    }
                    else {
                        nowDate.setDate(nowDate.getDate() + parseInt(splitMinus[1]))
                    }
                }
                return nowDate
            }
        }
        else {

            if (isMonthPicker) {
                // 2020-02
                const splitted = nowStr.split('-')
                return new Date(parseInt(splitted[1]), parseInt(splitted[0]), 1)
            }
            else {
                // 2020-02-02
                return new Date(nowStr)
            }
        }
    }

    private generateDatasourceForNumberRange(numberRange: string, isMultiple: boolean) {
        const hasDoublePoints = numberRange.indexOf('..') > 0
        const hasMinus = numberRange.indexOf('-') > 0
        if (hasDoublePoints) {
            const count = StringUtils.getNumberOccurencesOfStr(numberRange, '..')
            if (count == 2) {
                // Case [10..2..20]
                const splitted = numberRange.substr(1, numberRange.length - 2).split('..')
                const startNum = parseInt(splitted[0])
                const endNum = parseInt(splitted[2])
                const stepNum = parseInt(splitted[1])
                if (startNum <= endNum) {
                    const ds: any[] = []
                    for (let i = startNum; i <= endNum; i += stepNum) {
                        ds.push({
                            name: i.toString(),
                            value: i
                        })
                    }

                    return ds
                }
                else {
                    const ds: any[] = []
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
                const splitted = numberRange.substr(1, numberRange.length - 2).split('..')
                const startNum = parseInt(splitted[0])
                const endNum = parseInt(splitted[1])
                if (startNum <= endNum) {
                    const ds: any[] = []
                    for (let i = startNum; i <= endNum; i++) {
                        ds.push({
                            name: i.toString(),
                            value: i
                        })
                    }

                    return ds
                }
                else {
                    const ds: any[] = []
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
        else if (hasMinus) {
            const arrayStr: string[] = numberRange.slice(1, numberRange.length - 1).slice(0, numberRange.length - 2).split(',')
            return arrayStr.map(a => ({
                name: a,
                value: a
            }))
        }
        else {
            // return as number[]
            const temp = StringUtils.replaceAllOccurences(numberRange, '\'','"')
            const arrayNum: number[] = JSON.parse(temp)
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
                        const tempDates: Date[] = []
                        const data = JSON.parse(defaultValue)
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