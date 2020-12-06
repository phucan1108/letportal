import { ShellOption } from 'services/portal.service';
 

export default class PageUtils{
    public static getControlOptions<T>(options: ShellOption[]){
        const convertOption = new Object()
        options?.forEach(opt => {
            if(opt.value.toLowerCase() === 'true' || opt.value.toLowerCase() === 'false')
                convertOption[opt.key] = opt.value.toLowerCase() === 'true'
            else
                convertOption[opt.key] = opt.value
        })

        return convertOption as T
    }
}