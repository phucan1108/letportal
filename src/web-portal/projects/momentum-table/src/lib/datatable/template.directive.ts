import { Directive, Input, TemplateRef } from '@angular/core';

@Directive({
  selector: '[mTemplate]',
})
export class MomentumTemplate {
  @Input('mTemplate') type: string;

  constructor(public template: TemplateRef<any>) {}

  getType(): string {
    return this.type;
  }
}
