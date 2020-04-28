import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NGXLogger } from 'ngx-logger';
import { LanguageKey, PagesClient } from 'services/portal.service';
import { DataTable } from 'momentum-table';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { StaticResources } from 'portal/resources/static-resources';
import { BehaviorSubject, Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { Subscription } from 'rxjs'

@Component({
    selector: 'let-localization-page',
    templateUrl: './localization.page.html',
    styleUrls: ['./localization.page.scss']
})
export class LocalizationPage implements OnInit {
    @ViewChild('mtable', { static: true })
    mtable: DataTable;

    formGroup: FormGroup
    languageKeys: LanguageKey[] = []
    localeTags$: Observable<any[]>
    localeTags = StaticResources.localeTags()

    pageId: string
    localeId: string
    isEditMode = false
    constructor(
        private pagesClient: PagesClient,
        private fb: FormBuilder,
        private logger: NGXLogger,
        private activedRoute: ActivatedRoute
    ) { }

    ngOnInit(): void {
        this.activedRoute.queryParamMap.subscribe(
            queryParam => {
                this.pageId = queryParam.get('pageId')
                this.pagesClient.generateLanguages(this.pageId).subscribe(keys => {
                    this.languageKeys = keys
                })
                
                this.formGroup = this.fb.group({
                    language: [this.isEditMode ? this.localeId : '', Validators.required]
                })
            }
        )
        this.activedRoute.paramMap.subscribe(
            param => {
                if (param.get('localeId')) {
                    this.localeId = param.get('localeId')
                    this.isEditMode = true
                }
                
                this.formGroup = this.fb.group({
                    language: [this.isEditMode ? this.localeId : '', Validators.required]
                })
            }
        )

        this.localeTags$ = this.formGroup.get('language').valueChanges.pipe(
            startWith(''),
            map(value => this._filterTag(value))
        )
    }

    editComplete($event) {

    }

    private _filterTag(choosingTagValue: string): Array<any> {
        return this.localeTags.filter(op => op.name.toLowerCase().includes(choosingTagValue))
    }
}
