import { Component, OnInit, ChangeDetectorRef, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { BuilderDnDComponent } from './builder-dnd.component';
import { ExtendedPageSection } from 'app/core/models/extended.models';
import { StaticResources } from 'portal/resources/static-resources';
import { DynamicListClient, DynamicList, SectionContructionType, StandardComponent, StandardComponentClient, ChartsClient, Chart } from 'services/portal.service';
import { Observable } from 'rxjs';
import * as _ from 'lodash';

@Component({
    selector: 'let-section-dialog',
    templateUrl: './section-dialog.component.html'
})
export class SectionDialogComponent implements OnInit {

    sectionForm: FormGroup
    currentExtendedFormSection: ExtendedPageSection

    dynamicLists$: Observable<DynamicList[]>;
    standards$: Observable<StandardComponent[]>;
    charts$: Observable<Chart[]>;
    isEditMode = false;

    _sectionLayouts = StaticResources.sectionLayoutTypes()
    _constructionTypes = StaticResources.constructionTypes()
    standards: StandardComponent[]
    dynamicLists: DynamicList[]
    constructionType = SectionContructionType
    constructor(
        public dialogRef: MatDialogRef<BuilderDnDComponent>,
        private dyanmicListsClient: DynamicListClient,
        private standardsClient: StandardComponentClient,
        private chartsClient: ChartsClient,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef
    ) { }

    ngOnInit(): void {
        this.currentExtendedFormSection = this.data
        this.isEditMode = this.currentExtendedFormSection.name ? true : false
        this.initialSectionForm()
        this.populatedFormValues()        
        this.dynamicLists$ = this.dyanmicListsClient.getAll();
        this.standards$ = this.standardsClient.getManys('');
        this.charts$ = this.chartsClient.getMany();

        this.standards$.subscribe(standards => {
            this.standards = standards
        })

        this.dynamicLists$.subscribe(dynamicLists => {
            this.dynamicLists = dynamicLists
        })
    }

    initialSectionForm() {
        this.sectionForm = this.fb.group({
            name: [this.currentExtendedFormSection.name, Validators.required],
            displayName: [this.currentExtendedFormSection.displayName, Validators.required],
            constructionType: [this.currentExtendedFormSection.constructionType, Validators.required],
            componentId: [this.currentExtendedFormSection.componentId, Validators.required]
        })
    }

    populatedFormValues() {
        this.sectionForm.get('displayName').valueChanges.subscribe(newValue => {
            const sectionFormNameValue = (<string>newValue).toLowerCase().replace(/\s/g, '')
            this.sectionForm.get('name').setValue(sectionFormNameValue)
            this.cd.markForCheck()
        })
    }

    generateFormSection(): ExtendedPageSection {
        const formValues = this.sectionForm.value
        let extendedFormSection: ExtendedPageSection = {
            id: this.currentExtendedFormSection.id,
            name: formValues.name,
            displayName: formValues.displayName,
            componentId: formValues.componentId,
            constructionType: formValues.constructionType,
            order: this.currentExtendedFormSection.order,
            overrideOptions: this.currentExtendedFormSection.overrideOptions,
            sectionDatasource: this.currentExtendedFormSection.sectionDatasource,
            relatedDynamicList: !!formValues.componentId ? _.find(this.dynamicLists, dynamicList => dynamicList.id === formValues.componentId) : this.currentExtendedFormSection.relatedDynamicList,
            relatedStandard: !!formValues.componentId ? _.find(this.standards, standard => standard.id === formValues.componentId) : this.currentExtendedFormSection.relatedStandard,
            isLoaded: false
        }

        if (this.isEditMode) {
            //extendedFormSection.formControls = this.currentExtendedFormSection.formControls            
        }

        return extendedFormSection
    }

    onSubmittingSection() {
        if(this.sectionForm.valid){
            this.dialogRef.close(this.generateFormSection())
        }        
    }
}
