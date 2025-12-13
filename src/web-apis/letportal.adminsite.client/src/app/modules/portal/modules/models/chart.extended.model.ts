import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import { ShellOption, ChartFilter } from 'services/portal.service';
 
import StringUtils from 'app/core/utils/string-util';

export class ChartOptions {

    public static AllowRealTime: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Allow real-time data for Chart. Default: false',
        key: 'allowrealtime',
        value: 'false'
    }

    public static TimeToRefresh: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Time to refresh data in Chart, Allow Real-time must be enable, in second. Default: 60',
        key: 'timetorefresh',
        value: '60'
    }

    public static CompareRealTimeField: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'By default, if this field is blank, chart with real-time just refreshes a data. But if we set it to one field, this field (Must be DateTime type) will be compared between (Now - timetorefresh) and Now',
        key: 'comparerealtimefield',
        value: ''
    }

    public static DataRange: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'By default, if this field is blank. We can set a min-max value for xAxis and yAxis as x=[0,100];y=[0,200]. It supports number only',
        key: 'datarange',
        value: ''
    }

    public static XFormatDate: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'This format will use momentjs for displaying date value in X axis. Default: empty',
        key: 'xformatdate',
        value: 'YYYY-MM-DD'
    }

    public static Colors: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Colors list will be used to style a chart [\'red\',\'blue\',\'yellow\'] or style name (horizon|ocean|neons|vivid|cool|nightLights). Default: [\'horizon\']',
        key: 'colors',
        value: '["horizon"]'
    }

    public static DefaultListOptions: ChartOptions = {
        allowrealtime: false,
        timetorefresh: 60,
        comparerealtimefield: '',
        datarange: '',
        xformatdate: '',
        colors: ['horizon']
    }

    allowrealtime: boolean
    timetorefresh: number
    comparerealtimefield: string
    datarange: string
    xformatdate: string
    colors: string[]

    public static getChartOptions(options: ShellOption[]): ChartOptions {
        const comparedField = options.find(opt => opt.key === 'comparerealtimefield')
        const datarange = options.find(opt => opt.key === 'datarange')
        const xformatDate = options.find(opt => opt.key === 'xformatdate')
        return {
            allowrealtime: JSON.parse(options.find(opt => opt.key === 'allowrealtime').value),
            timetorefresh: JSON.parse(options.find(opt => opt.key === 'timetorefresh').value),
            comparerealtimefield: comparedField.value ? comparedField.value : '',
            datarange: datarange ? datarange.value : '',
            xformatdate: xformatDate ? xformatDate.value : '',
            colors: JSON.parse(StringUtils.replaceAllOccurences(options.find(opt => opt.key === 'colors').value, '\'', '"'))
        }
    }

    public static getDefaultShellOptionsForChart(): ExtendedShellOption[] {
        return [
            this.AllowRealTime,
            this.TimeToRefresh,
            this.CompareRealTimeField,
            this.DataRange,
            this.XFormatDate,
            this.Colors
        ]
    }

    public static combinedDefaultShellOptions(opts: ExtendedShellOption[]) {
        const defaultOpts = this.getDefaultShellOptionsForChart()
        opts?.forEach(a => {
            const found = defaultOpts.find(b => b.key === a.key)
            if (found) {
                a.description = found.description
                a.allowDelete = found.allowDelete
                a.id = found.id
            }
        })
    }
}

export interface ExtendedChartFilter extends ChartFilter{
   datasource: any
   minDate: any
   maxDate: any
   defaultObj: any
}