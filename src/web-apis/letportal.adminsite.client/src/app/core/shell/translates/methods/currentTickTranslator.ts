import { ShellMethod } from './shellmethod';
import { MethodTranslator } from '../../decorators/method.decorator';

@MethodTranslator({
    name: 'currentTick',
    replaceDQuote: false
})
export class currentTickTranslator implements ShellMethod  {
    execute(...params: any[]) {
        return ((new Date().getTimezoneOffset() * 10000) + 621355968000000000)
    }
}
