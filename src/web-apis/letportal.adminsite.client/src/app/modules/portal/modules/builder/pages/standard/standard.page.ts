import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { StandardComponentClient, StandardComponent, PageSectionLayoutType, App, AppsClient, StandardType } from 'services/portal.service';
import { ExtendedPageControl } from 'app/core/models/extended.models';
import { ActivatedRoute, Router } from '@angular/router';
import { Guid } from 'guid-typescript';
import { NGXLogger } from 'ngx-logger';
 
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { StaticResources } from 'portal/resources/static-resources';
import { PortalValidators } from 'app/core/validators/portal.validators';
import { BehaviorSubject, Observable } from 'rxjs';
import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import { ArrayStandardOptions, TreeStandardOptions } from 'portal/modules/models/standard.extended.model';
import { ObjectUtils } from 'app/core/utils/object-util';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'let-standard-page',
    templateUrl: './standard.page.html',
    styleUrls:['./standard.page.scss']
})
export class StandardPagePage implements OnInit {

    standardFormGroup: FormGroup
    standardComponent: StandardComponent
    controls: ExtendedPageControl[] = []
    controls$: BehaviorSubject<ExtendedPageControl[]> = new BehaviorSubject([])
    shellOptions$: BehaviorSubject<Array<ExtendedShellOption>> = new BehaviorSubject([])
    shellOptions: Array<ExtendedShellOption> = []
    isCanSubmit = false
    isEditMode = false

    _layoutTypes = StaticResources.sectionLayoutTypes()
    _standardTypes = StaticResources.standardTypes()
    apps$: Observable<App[]>
    constructor(
        private translate: TranslateService,
        private appsClient: AppsClient,
        private fb: FormBuilder,
        private activatedRoute: ActivatedRoute,
        private router: Router,
        private standardsClient: StandardComponentClient,
        private shortcutUtil: ShortcutUtil,
        private logger: NGXLogger
    ) {
        this.standardComponent = this.activatedRoute.snapshot.data.standard
        this.isEditMode = !!this.standardComponent
        this.apps$ = this.appsClient.getAll()
        if (!this.isEditMode) {
            this.standardComponent = {
                id: Guid.create().toString(),
                controls: [],
                name: '',
                displayName: '',
                appId: '',
                layoutType: PageSectionLayoutType.OneColumn
            }
        }
        else {
            this.standardComponent.controls?.forEach( control => {
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
        this.initFormGroup()
    }

    initFormGroup() {
        this.logger.debug('Standard component', this.standardComponent)
        if (this.isEditMode) {
            this.standardFormGroup = this.fb.group({
                name: new FormControl({ value:this.standardComponent.name, disabled: true }, [Validators.required, Validators.maxLength(250)], [PortalValidators.standardUniqueName(this.standardsClient)]),
                displayName: [this.standardComponent.displayName, [Validators.required, Validators.maxLength(250)]],
                app: [this.standardComponent.appId, [Validators.required]],
                type: [{ value: this.standardComponent.type, disabled: true}],
                layoutType: [this.standardComponent.layoutType.toString()],
                allowOverrideOptions: [this.standardComponent.allowOverrideOptions]
            })


            if(ObjectUtils.isNotNull(this.standardComponent.options)){
                this.shellOptions =  this.standardComponent.options as ExtendedShellOption[]
            }
            else{
                switch(this.standardComponent.type){
                    case StandardType.Standard:
                        this.shellOptions = []
                        break
                    case StandardType.Array:
                        this.shellOptions = ArrayStandardOptions.getDefaultShellOptionsForArray()
                        break
                    case StandardType.Tree:
                        this.shellOptions = TreeStandardOptions.getDefaultShellOptionsForTreeStandard()
                        break
                }
               
            }
            ArrayStandardOptions.combinedDefaultShellOptions(this.shellOptions)
            this.shellOptions$.next(this.shellOptions)
        }
        else {

            this.standardFormGroup = this.fb.group({
                name: [this.standardComponent.name, [Validators.required, Validators.maxLength(250)], [PortalValidators.standardUniqueName(this.standardsClient)]],
                displayName: [this.standardComponent.displayName, [Validators.required, Validators.maxLength(250)]],
                app: [this.standardComponent.appId, [Validators.required]],
                type: [this.standardComponent.type],
                layoutType: [this.standardComponent.layoutType.toString()],
                allowOverrideOptions: [this.standardComponent.allowOverrideOptions]
            })

            this.shellOptions = []
            this.shellOptions$.next(this.shellOptions)
        }

        // Auto-generated name and url path
        this.standardFormGroup.get('displayName').valueChanges.subscribe(newValue => {
            if (newValue && !this.isEditMode) {
                // Apply this change to list name and url path
                const listNameValue = (newValue as string).toLowerCase().replace(/\s/g, '')
                this.standardFormGroup.get('name').setValue(listNameValue)
            }
        })

        this.standardFormGroup.controls.type.valueChanges.subscribe((newValue : StandardType) => {
            switch(newValue){
                case StandardType.Standard:
                    this.shellOptions = []
                    this.shellOptions$.next(this.shellOptions)
                    break
                case StandardType.Array:
                    this.shellOptions = ArrayStandardOptions.getDefaultShellOptionsForArray()
                    this.shellOptions$.next(this.shellOptions)
                    break
                case StandardType.Tree:
                    this.shellOptions = TreeStandardOptions.getDefaultShellOptionsForTreeStandard()
                    this.shellOptions$.next(this.shellOptions)
                    break
            }
        })
    }

    combineStandardInfo() {
        const formValues = this.standardFormGroup.value
        this.standardComponent.name = this.isEditMode ? this.standardComponent.name :  formValues.name
        this.standardComponent.displayName = formValues.displayName
        this.standardComponent.type =  this.isEditMode ? this.standardComponent.type : parseInt(formValues.type)
        this.standardComponent.controls = this.controls
        this.standardComponent.layoutType = parseInt(formValues.layoutType)
        this.standardComponent.options = this.shellOptions
        this.standardComponent.appId = formValues.app        
    }

    onPopulatedControls($event: ExtendedPageControl[]) {
        this.controls = $event
        this.controls$.next(this.controls)
    }

    onChanged($event: ExtendedPageControl[]) {
        this.controls = $event
        this.controls$.next(this.controls)
        this.logger.debug('Current controls', this.controls)
    }

    onSubmit() {
        if (this.standardFormGroup.valid) {
            this.combineStandardInfo()
            this.logger.debug('Current standard info', this.standardComponent)
            if (!this.isEditMode) {
                this.standardsClient.createOne(this.standardComponent).subscribe(
                    result => {
                        this.shortcutUtil.toastMessage(this.translate.instant('common.createSuccessfully'), ToastType.Success)
                        this.router.navigateByUrl('portal/page/standard-list-management')
                    },
                    err => {

                    }
                )
            }
            else {
                this.standardsClient.updateOne(this.standardComponent.id, this.standardComponent).subscribe(
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
        this.router.navigateByUrl('portal/page/standard-list-management')
    }

    onChangingOptions($event){
        this.shellOptions = $event
        this.logger.debug('Options changed', this.shellOptions)
    }
}
