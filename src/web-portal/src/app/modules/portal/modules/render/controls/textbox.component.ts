import { Component, OnInit, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ExtendedPageControl } from '../../../../../core/models/extended.models';

@Component({
    selector: 'let-textbox',
    templateUrl: './textbox.component.html'
})
export class TextboxComponent implements OnInit {

    @Input()
    form: FormGroup

    @Input()
    formControl: ExtendedPageControl

    @Input()
    isPreviewMode: boolean

    constructor() { }

    ngOnInit(): void { }
}
