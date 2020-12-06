import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { FormUtil } from 'app/core/utils/form-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import { PortalValidators } from 'app/core/validators/portal.validators';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { DataTable } from 'app/modules/thirdparties/momentum-table/datatable/datatable';
import { NGXLogger } from 'ngx-logger';
import { StaticResources } from 'portal/resources/static-resources';
import { Observable } from 'rxjs';
import { map, startWith, tap } from 'rxjs/operators';
import { ExportService } from 'services/export.service';
import { PageService } from 'services/page.service';
import { LanguageKey, Localization, LocalizationClient, LocalizationContent } from 'services/portal.service';

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
    appId: string

    hasRestored = false
    constructor(
        private translate: TranslateService,
        private exportService: ExportService,
        private localizationClient: LocalizationClient,
        private fb: FormBuilder,
        private router: Router,
        private shortcutUtil: ShortcutUtil,
        private logger: NGXLogger,
        private activedRoute: ActivatedRoute,
        private pageService: PageService
    ) { }

    ngOnInit(): void {
        this.pageService.init('localization-management').subscribe()
        this.localization = this.activedRoute.snapshot.data.localization
        if (!ObjectUtils.isNotNull(this.localization)) {
            this.activedRoute.paramMap.subscribe(
                param => {
                    if (param.get('appId')) {
                        this.appId = param.get('appId')
                    }

                    this.formGroup = this.fb.group({
                        language: ['', [Validators.required], [PortalValidators.localeUniqueName(this.localizationClient, this.appId)]]
                    })
                }
            )
        }
        else {
            this.isEditMode = true
            this.localeId = this.localization.localeId
            this.appId = this.localization.appId
            this.formGroup = this.fb.group({
                language: [{ value: this.localeId, disabled: true }, Validators.required]
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
    saveChanges() {
        if (this.formGroup.valid || this.isEditMode) {
            const combineLocalization: Localization = {
                id: '',
                localeId: this.formGroup.get('language').value,
                appId: this.appId,
                localizationContents: this.languageKeys.map(a => <LocalizationContent>{
                    key: a.key,
                    text: a.value
                })
            }

            if (this.isEditMode) {
                this.localizationClient.delete(this.localization.id).subscribe(
                    res => {
                        this.localizationClient.create(combineLocalization).subscribe(
                            res => {
                                this.shortcutUtil.toastMessage(this.translate.instant('common.updateSuccessfully'), ToastType.Success)
                                this.router.navigateByUrl('portal/page/localization-management?appId=' + this.appId)
                            }
                        )
                    }
                )

            }
            else {
                this.localizationClient.create(combineLocalization).subscribe(
                    res => {
                        this.shortcutUtil.toastMessage(this.translate.instant('common.createSuccessfully'), ToastType.Success)
                        this.router.navigateByUrl('portal/page/localization-management?appId=' + this.appId)
                    }
                )
            }
        }
    }

    onCollect() {
        const sub = this.localizationClient.collectAll(this.appId).pipe(
            tap(
                allKeys => {
                    if (this.isEditMode) {
                        allKeys?.forEach(text => {
                            if (!this.languageKeys.some(a => a.key === text.key)) {
                                this.languageKeys.push(text)
                            }
                        })

                        // Remove all non-existed
                        let removedItems: LanguageKey[] = []
                        this.languageKeys?.forEach(lang => {
                            if (!allKeys.some(a => a.key === lang.key)) {
                                removedItems.push(lang)
                            }
                        })

                        this.languageKeys = this.languageKeys.filter(a => !removedItems.includes(a))

                    }
                    else {
                        allKeys?.forEach(text => {
                            if (this.hasRestored){
                                const found = this.languageKeys.find(a => a.key === text.key)
                                if(found){
                                    text.value = found.value
                                }
                            }
                        })
                        this.languageKeys = allKeys
                    }
                    sub.unsubscribe()
                }
            )
        ).subscribe()
    }
    cancel() {
        this.router.navigateByUrl('portal/page/localization-management?appId=' + this.appId)
    }

    onBackup() {
        if (this.isEditMode) {
            const jsonString = JSON.stringify(this.localization)
            this.exportService.exportJsonFile(jsonString, this.localization.localeId + '.json')
        }
    }

    onFileChange($event) {
        const latestFile: File = $event.target.files[$event.target.files.length - 1]
        if (ObjectUtils.isNotNull(latestFile)) {
            if (latestFile.name.indexOf('.json') < 0) {
                window.alert('Please upload json file')
            }
            else {
                var reader = new FileReader()
                reader.onload = (e) => {
                    try {
                        this.localization = JSON.parse(<string>reader.result)
                        this.formGroup.get('language').setValue(this.localization.localeId)
                        FormUtil.triggerFormValidators(this.formGroup)
                        this.languageKeys = this.localization.localizationContents.map(a => <LanguageKey>{
                            key: a.key,
                            value: a.text
                        })
                        this.hasRestored = true
                    }
                    catch (ex) {
                        window.alert('Something went wrong: ' + ex.message)
                    }
                }
                reader.readAsText(latestFile)
            }
        }
    }

    private _filterTag(choosingTagValue: string): Array<any> {
        return this.localeTags.filter(op => op.name.toLowerCase().includes(choosingTagValue))
    }
}
