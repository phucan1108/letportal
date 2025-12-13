import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { App, AppsClient, PagesClient } from 'services/portal.service';

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
