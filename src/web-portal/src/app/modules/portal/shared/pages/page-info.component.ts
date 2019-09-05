import { Component, OnInit, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { PagesClient } from 'services/portal.service';

@Component({
    selector: 'let-page-info',
    templateUrl: './page-info.component.html'
})
export class PageInfoComponent implements OnInit {

    @Input()
    pageInfoFormGroup: FormGroup

    @Input()
    heading: string

    @Input()
    isEditMode = false

    constructor(
        private pageClient: PagesClient
    ) { }

    ngOnInit(): void {
    }
}
