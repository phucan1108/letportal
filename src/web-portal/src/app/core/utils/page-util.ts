import { ShellOption } from 'services/portal.service';
import * as _ from 'lodash';

export default class PageUtils{
    public static getControlOptions<T>(options: ShellOption[]){
        let convertOption = new Object()
        _.forEach(options, opt => {
            if(opt.value.toLowerCase() === 'true' || opt.value.toLowerCase() === 'false')
                convertOption[opt.key] = opt.value.toLowerCase() === 'true'
            else 
                convertOption[opt.key] = opt.value
        })

        return convertOption as T
    }
}