import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ExtendedPageControl } from 'app/core/models/extended.models';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { Guid } from 'guid-typescript';
import { NGXLogger } from 'ngx-logger';
import { BehaviorSubject, Observable } from 'rxjs';
import { PageService } from 'services/page.service';
import { App, AppsClient, CompositeControl, CompositeControlsClient } from 'services/portal.service';

@Component({
    selector: 'let-composite-builder',
    templateUrl: './composite-builder.page.html',
    styleUrls: ['./composite-builder.page.scss']
})
export class CompositeBuilderPage implements OnInit {
    compositeControl: CompositeControl
    formGroup: FormGroup
    controls: ExtendedPageControl[] = []
    controls$: BehaviorSubject<ExtendedPageControl[]> = new BehaviorSubject([])
    apps$: Observable<App[]>
    isEditMode = false
    constructor(
        private translate: TranslateService,
        private appsClient: AppsClient,
        private fb: FormBuilder,
        private activatedRoute: ActivatedRoute,
        private router: Router,
        private compositeClient: CompositeControlsClient,
        private shortcutUtil: ShortcutUtil,
        private pageService: PageService,
        private logger: NGXLogger
    ) { 
        this.compositeControl = this.activatedRoute.snapshot.data.control
        this.isEditMode = !!this.compositeControl
        this.apps$ = this.appsClient.getAll()
        if (!this.isEditMode) {
            this.compositeControl = {
                id: Guid.create().toString(),
                controls: [],
                name: '',
                displayName: '',
                appId: ''
            }
        }
        else {
            this.compositeControl.controls?.forEach( control => {
                this.controls.push({
                    ...control,
                    value: '',
                    readOnlyMode: true
                })
            })
            this.controls$.next(this.controls)
        }
    }

    ngOnInit(): void { 
        this.pageService.init('composite-control-builder').subscribe()
        this.initFormGroup()
    }

    initFormGroup() {
        this.logger.debug('Composite control', this.compositeControl)
        if (this.isEditMode) {
            this.formGroup = this.fb.group({
                name: new FormControl({ value:this.compositeControl.name, disabled: true }, [Validators.required, Validators.maxLength(250)]),
                displayName: [this.compositeControl.displayName, [Validators.required, Validators.maxLength(250)]],
                app: [this.compositeControl.appId, [Validators.required]]
            })
        }
        else {

            this.formGroup = this.fb.group({
                name: [this.compositeControl.name, [Validators.required, Validators.maxLength(250)]],
                displayName: [this.compositeControl.displayName, [Validators.required, Validators.maxLength(250)]],
                app: [this.compositeControl.appId, [Validators.required]]
            })
        }

        // Auto-generated name and url path
        this.formGroup.get('displayName').valueChanges.subscribe(newValue => {
            if (newValue && !this.isEditMode) {
                // Apply this change to list name and url path
                const listNameValue = (newValue as string).toLowerCase().replace(/\s/g, '')
                this.formGroup.get('name').setValue(listNameValue)
            }
        })
    }

    onChanged($event: ExtendedPageControl[]) {
        this.controls = $event
        this.controls$.next(this.controls)
        this.logger.debug('Current controls', this.controls)
    }

    onSubmit() {
        if (this.formGroup.valid) {
            this.combineCompositeControl()
            this.logger.debug('Current standard info', this.compositeControl)
            if (!this.isEditMode) {
                this.compositeClient.create(this.compositeControl).subscribe(
                    result => {
                        this.shortcutUtil.toastMessage(this.translate.instant('common.createSuccessfully'), ToastType.Success)
                        this.router.navigateByUrl('portal/page/composite-controls')
                    },
                    err => {

                    }
                )
            }
            else {
                this.compositeClient.update(this.compositeControl.id, this.compositeControl).subscribe(
                    result => {
                        this.shortcutUtil.toastMessage(this.translate.instant('common.updateSuccessfully'), ToastType.Success)
                    },
                    err => {

                    }
                )
            }
        }
    }

    onCancel() {
        this.router.navigateByUrl('portal/page/composite-controls')
    }
    
    private combineCompositeControl() {
        const formValues = this.formGroup.value
        this.compositeControl.name = this.isEditMode ? this.compositeControl.name :  formValues.name
        this.compositeControl.displayName = formValues.displayName        
        this.compositeControl.controls = this.controls
        this.compositeControl.appId = formValues.app        
    }

}
