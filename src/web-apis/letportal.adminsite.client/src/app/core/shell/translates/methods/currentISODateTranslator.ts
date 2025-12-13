import { MethodTranslator } from '../../decorators/method.decorator';
import { ShellMethod } from './shellmethod';
import { ObjectUtils } from 'app/core/utils/object-util';

@MethodTranslator({
    name: 'currentISODate',
    replaceDQuote: false
})
export class currentISODateTranslator implements ShellMethod  {
    execute(...params: any[]) {
        const now = new Date()
        if(ObjectUtils.isNotNull(params[0])){
            const seconds = parseInt(params[0])
            now.setSeconds(now.getSeconds() + seconds);
        }
        return now.toISOString()
    }
}
