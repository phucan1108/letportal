import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NGXLogger } from 'ngx-logger';
import { LanguageKey, PagesClient, Localization, LocalizationContent, LocalizationClient } from 'services/portal.service';
import { DataTable } from 'momentum-table';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { StaticResources } from 'portal/resources/static-resources';
import { BehaviorSubject, Observable } from 'rxjs';
import { startWith, map, tap } from 'rxjs/operators';
import { Subscription } from 'rxjs'
import { ObjectUtils } from 'app/core/utils/object-util';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';

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
    localization: Localization
    constructor(
        private pagesClient: PagesClient,
        private localizationClient: LocalizationClient,
        private fb: FormBuilder,
        private router: Router,
        private shortcutUtil: ShortcutUtil,
        private logger: NGXLogger,
        private activedRoute: ActivatedRoute
    ) { }

    ngOnInit(): void {

        this.localization = this.activedRoute.snapshot.data.localization
        if(!ObjectUtils.isNotNull(this.localization)){
            this.activedRoute.queryParamMap.subscribe(
                queryParam => {
                    this.pageId = queryParam.get('pageId')
                    this.pagesClient.generateLanguages(this.pageId).subscribe(keys => {
                        this.languageKeys = keys
                    })
                    
                    this.formGroup = this.fb.group({
                        language: ['', Validators.required]
                    })
                }
            )
        }
        else{
            this.isEditMode = true
            this.pageId = this.localization.pageId
            this.localeId = this.localization.localeId
            this.formGroup = this.fb.group({
                language: [{value: this.localeId, disabled: true }, Validators.required]
            })

            this.languageKeys = this.localization.localizationContents.map(a => <LanguageKey>{
                key: a.key,
                value: a.text
            })
        }

        this.localeTags$ = this.formGroup.get('language').valueChanges.pipe(
            startWith(''),
            map(name => this._filterTag(name))
        )
    }

    editComplete($event) {

    }
    saveChanges(){
        if(this.formGroup.valid || this.isEditMode){
            const combineLocalization: Localization = {
                id: '',
                localeId: this.formGroup.get('language').value,
                pageId: this.pageId,
                localizationContents: this.languageKeys.map(a => <LocalizationContent>{
                    key: a.key,
                    text: a.value
                })
            }

            if(this.isEditMode){
                this.localizationClient.delete(this.localization.id).subscribe(
                    res => {
                        this.localizationClient.create(combineLocalization).subscribe(
                            res => {
                                this.shortcutUtil.toastMessage(`Update ${combineLocalization.localeId} language successfully`, ToastType.Success)
                                this.router.navigateByUrl('portal/page/localizations-management?pageId=' + this.pageId)
                            }
                        )
                    }
                )

            }
            else{
                this.localizationClient.create(combineLocalization).subscribe(
                    res => {
                        this.shortcutUtil.toastMessage(`Create ${combineLocalization.localeId} language successfully`, ToastType.Success)
                        this.router.navigateByUrl('portal/page/localizations-management?pageId=' + this.pageId)
                    }
                )
            }
        }
    }

    cancel(){
        this.router.navigateByUrl('portal/page/localizations-management?pageId=' + this.pageId)
    }
    private _filterTag(choosingTagValue: string): Array<any> {
        return this.localeTags.filter(op => op.name.toLowerCase().includes(choosingTagValue))
    }
}
