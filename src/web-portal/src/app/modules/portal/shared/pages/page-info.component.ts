import { Component, OnInit, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { PagesClient, App, AppsClient } from 'services/portal.service';
import { Observable } from 'rxjs';

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

    apps$: Observable<App[]>

    constructor(
        private appsClient: AppsClient,
        private pageClient: PagesClient
    ) { }

    ngOnInit(): void {
        this.apps$ = this.appsClient.getAll()
    }
}
