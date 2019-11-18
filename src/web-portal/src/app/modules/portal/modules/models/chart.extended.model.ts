import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import { ShellOption } from 'services/portal.service';
import * as _ from 'lodash';

export class ChartOptions {

    allowrealtime: boolean
    timetorefresh: number
    comparerealtimefield: string
    colors: string[]

    public static getChartOptions(options: ShellOption[]): ChartOptions {
        let comparedField = _.find(options, opt => opt.key === 'comparerealtimefield')
        return {
            allowrealtime: JSON.parse(_.find(options, opt => opt.key === 'allowrealtime').value),
            timetorefresh: JSON.parse(_.find(options, opt => opt.key === 'timetorefresh').value),
            comparerealtimefield: comparedField.value ? comparedField.value : '',
            colors: JSON.parse(_.find(options, opt => opt.key === 'colors').value)
        }
    }

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
        value: '30'
    }

    public static Colors: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: "Colors list will be used to style a chart ['red','blue','yellow'] or style name (horizon|ocean|neons|vivid|cool|nightLights). Default: ['horizon']",
        key: 'colors',
        value: "['horizon']"
    }

    public static getDefaultShellOptionsForChart(): ExtendedShellOption[] {
        return [
            this.AllowRealTime,
            this.TimeToRefresh,
            this.CompareRealTimeField,
            this.Colors
        ]
    }

    public static combinedDefaultShellOptions(opts: ExtendedShellOption[]) {
        const defaultOpts = this.getDefaultShellOptionsForChart()
        opts.forEach(a => {
            const found = defaultOpts.find(b => b.key === a.key)
            if (found) {
                a.description = found.description
                a.allowDelete = found.allowDelete
                a.id = found.id
            }
        })
    }

    public static DefaultListOptions: ChartOptions = {
        allowrealtime: false,
        timetorefresh: 60,
        comparerealtimefield: '',
        colors: ['horizon']
    }
}