import { Input, Directive } from '@angular/core';
@Directive({
    selector: '[sTemplate]'
})
export class SectionTemplate {
    @Input('sTemplate') type: string

    getType(): string{
        return this.type
    }
}