import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { StandardComponentClient, StandardComponent, PageSectionLayoutType } from 'services/portal.service';
import { ExtendedPageControl } from 'app/core/models/extended.models';
import { ActivatedRoute, Router } from '@angular/router';
import { Guid } from 'guid-typescript';
import { NGXLogger } from 'ngx-logger';
import * as _ from 'lodash';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { StaticResources } from 'portal/resources/static-resources';
import { PortalValidators } from 'app/core/validators/portal.validators';
import { BehaviorSubject } from 'rxjs';
import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import { StandardOptions } from 'portal/modules/models/standard.extended.model';
import { ObjectUtils } from 'app/core/utils/object-util';

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
    constructor(
        private fb: FormBuilder,
        private activatedRoute: ActivatedRoute,
        private router: Router,
        private standardsClient: StandardComponentClient,
        private shortcutUtil: ShortcutUtil,
        private logger: NGXLogger
    ) {
        this.standardComponent = this.activatedRoute.snapshot.data.standard
        this.isEditMode = !!this.standardComponent
        if (!this.isEditMode) {
            this.standardComponent = {
                id: Guid.create().toString(),
                controls: [],
                name: '',
                displayName: '',
                layoutType: PageSectionLayoutType.OneColumn
            }
        }
        else {
            _.each(this.standardComponent.controls, control => {
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
                allowArrayData: [this.standardComponent.allowArrayData],
                layoutType: [this.standardComponent.layoutType.toString()],
                allowOverrideOptions: [this.standardComponent.allowOverrideOptions]
            })

            
            if(ObjectUtils.isNotNull(this.standardComponent.options)){
                this.shellOptions =  this.standardComponent.options as ExtendedShellOption[]
            }
            else{
                this.shellOptions = StandardOptions.getDefaultShellOptionsForStandard()
            }
            StandardOptions.combinedDefaultShellOptions(this.shellOptions)
            this.shellOptions$.next(this.shellOptions)
        }
        else {

            this.standardFormGroup = this.fb.group({
                name: [this.standardComponent.name, [Validators.required, Validators.maxLength(250)], [PortalValidators.standardUniqueName(this.standardsClient)]],
                displayName: [this.standardComponent.displayName, [Validators.required, Validators.maxLength(250)]],
                allowArrayData: [this.standardComponent.allowArrayData],
                layoutType: [this.standardComponent.layoutType.toString()],
                allowOverrideOptions: [this.standardComponent.allowOverrideOptions]
            })

            this.shellOptions = this.shellOptions.concat(StandardOptions.getDefaultShellOptionsForStandard())
            this.shellOptions$.next(this.shellOptions)
        }

        // Auto-generated name and url path
        this.standardFormGroup.get('displayName').valueChanges.subscribe(newValue => {
            if (newValue && !this.isEditMode) {
                // Apply this change to list name and url path
                const listNameValue = (<string>newValue).toLowerCase().replace(/\s/g, '')
                this.standardFormGroup.get('name').setValue(listNameValue)
            }
        })
    }

    combineStandardInfo() {
        const formValues = this.standardFormGroup.value
        this.standardComponent.name = this.isEditMode ? this.standardComponent.name :  formValues.name
        this.standardComponent.displayName = formValues.displayName
        this.standardComponent.allowArrayData = formValues.allowArrayData
        this.standardComponent.controls = this.controls
        this.standardComponent.layoutType = parseInt(formValues.layoutType)
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
                        this.shortcutUtil.toastMessage('Create standard successfully', ToastType.Success)
                        this.router.navigateByUrl('portal/page/standard-list-management')
                    },
                    err => {

                    }
                )
            }
            else {
                this.standardsClient.updateOne(this.standardComponent.id, this.standardComponent).subscribe(
                    result => {
                        this.shortcutUtil.toastMessage('Update standard successfully', ToastType.Success)
                    },
                    err => {

                    }
                )
            }
        }
    }

    onCancel() {

    }

    onChangingOptions($event){
        this.shellOptions = $event
        this.logger.debug('Options changed', this.shellOptions)
    }
}
