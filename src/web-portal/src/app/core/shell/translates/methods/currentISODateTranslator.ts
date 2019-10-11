import { MethodTranslator } from '../../decorators/method.decorator';
import { ShellMethod } from './shellmethod';

@MethodTranslator({
    name: 'currentISODate',
    replaceDQuote: false
})
export class currentISODateTranslator implements ShellMethod  {
    execute(...params: any[]) {
        return (new Date()).toISOString()
    }
}
