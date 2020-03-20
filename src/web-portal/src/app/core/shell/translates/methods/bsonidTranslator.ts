import { ShellMethod } from './shellmethod';
import { MethodTranslator } from '../../decorators/method.decorator';

const ObjectId = (rnd = (r16: number) => Math.floor(r16).toString(16)) =>
    rnd(Date.now()/1000) + ' '.repeat(16).replace(/./g, () => rnd(Math.random()*16));

@MethodTranslator({
    name: 'bsonid',
    replaceDQuote: false
})
export class bsonidTranslator implements ShellMethod  {

    execute(...params: any[]) {
        return ObjectId()
    }
}
