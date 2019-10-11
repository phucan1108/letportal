import { MethodTranslator } from '../../decorators/method.decorator';
import { ShellMethod } from './shellmethod';

@MethodTranslator({
    name: 'currentDate',
    replaceDQuote: false
})
export class currentDateTranslator implements ShellMethod  {
    execute(...params: any[]) {
        return (new Date()).toUTCString()
    }
}
