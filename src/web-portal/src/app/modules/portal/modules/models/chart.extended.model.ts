import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import { ShellOption } from 'services/portal.service';
import * as _ from 'lodash';

export class ChartOptions{

    allowrealtime: boolean

    public static getChartOptions(options: ShellOption[]): ChartOptions {
        return {
            allowrealtime: JSON.parse(_.find(options, opt => opt.key === 'sizeoptions').value)
        }
    }

    public static AllowRealTime: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Allow real-time data for Chart. Default: false',
        key: 'allowrealtime',
        value: 'false'
    }

    public static getDefaultShellOptionsForChart(): ExtendedShellOption[] {
        return [
            this.AllowRealTime
        ]
    }

    public static combinedDefaultShellOptions(opts: ExtendedShellOption[]){
        const defaultOpts = this.getDefaultShellOptionsForChart()
        opts.forEach(a => {
            const found = defaultOpts.find(b => b.key === a.key)
            if(found){
                a.description = found.description
                a.allowDelete = found.allowDelete 
                a.id = found.id
            }                       
        })
    }    

    public static DefaultListOptions: ChartOptions =  {
        allowrealtime: false
    }
}