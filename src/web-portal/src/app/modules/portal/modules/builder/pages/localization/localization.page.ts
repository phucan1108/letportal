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
import { FormUtil } from 'app/core/utils/form-util';
import { PortalValidators } from 'app/core/validators/portal.validators';
import { ArrayUtils } from 'app/core/utils/array-util';

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
                    // this.pagesClient.generateLanguages(this.pageId).subscribe(keys => {
                    //     this.languageKeys = keys
                    // })
                    
                    this.formGroup = this.fb.group({
                        language: ['', [Validators.required], [PortalValidators.localeUniqueName(this.localizationClient)]]
                    })
                }
            )
        }
        else{
            this.isEditMode = true
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
                                this.router.navigateByUrl('portal/page/localization-management')
                            }
                        )
                    }
                )

            }
            else{
                this.localizationClient.create(combineLocalization).subscribe(
                    res => {
                        this.shortcutUtil.toastMessage(`Create ${combineLocalization.localeId} language successfully`, ToastType.Success)
                        this.router.navigateByUrl('portal/page/localization-management')
                    }
                )
            }
        }
    }

    onCollect() {
        const sub = this.localizationClient.collectAll().pipe(
            tap(
                allKeys => {
                    if(this.isEditMode){
                        allKeys.forEach(text => {
                            if(!this.languageKeys.some(a => a.key === text.key)){
                                this.languageKeys.push(text)
                            }
                        })
                    }
                    else{
                        this.languageKeys = allKeys
                    }
                    sub.unsubscribe()
                }
            )
        ).subscribe()
    }
    cancel(){
        this.router.navigateByUrl('portal/page/localization-management')
    }
    private _filterTag(choosingTagValue: string): Array<any> {
        return this.localeTags.filter(op => op.name.toLowerCase().includes(choosingTagValue))
    }
}
