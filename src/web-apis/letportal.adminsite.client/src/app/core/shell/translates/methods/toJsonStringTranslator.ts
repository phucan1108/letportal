import { MethodTranslator } from '../../decorators/method.decorator';
import { ShellMethod } from './shellmethod';

@MethodTranslator({
    name: 'toJsonString',
    replaceDQuote: true
})
export class toJsonStringTranslator implements ShellMethod  {
    execute(...params: any[]) {
        return JSON.stringify(params[0])
    }
}
