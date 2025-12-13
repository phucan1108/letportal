import { ShellMethod } from './shellmethod';
import { MethodTranslator } from '../../decorators/method.decorator';
import { Guid } from 'guid-typescript';

@MethodTranslator({
    name: 'guid',
    replaceDQuote: false
})
export class guidTranslator implements ShellMethod  {
    execute(...params: any[]) {
        return Guid.create().toString()
    }
}
