import { Component, OnInit, ChangeDetectorRef, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { BuilderDnDComponent } from './builder-dnd.component';
import { ExtendedPageSection } from 'app/core/models/extended.models';
import { StaticResources } from 'portal/resources/static-resources';
import { DynamicListClient, DynamicList, SectionContructionType, StandardComponent, StandardComponentClient, ChartsClient, Chart } from 'services/portal.service';
import { Observable } from 'rxjs';
 
import { FormUtil } from 'app/core/utils/form-util';

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
    arrayStandards$: Observable<StandardComponent[]>;
    treeStandards$: Observable<StandardComponent[]>
    isEditMode = false;
    availableSectionNames: string[] = []
    _sectionLayouts = StaticResources.sectionLayoutTypes()
    _constructionTypes = StaticResources.constructionTypes()
    standards: StandardComponent[]
    arrayStandards: StandardComponent[]
    treeStandards: StandardComponent[]
    dynamicLists: DynamicList[]
    charts: Chart[]
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
        this.currentExtendedFormSection = this.data.section
        this.availableSectionNames = this.data.sectionNames
        this.isEditMode = this.currentExtendedFormSection.name ? true : false
        this.initialSectionForm()
        this.populatedFormValues()
        this.dynamicLists$ = this.dyanmicListsClient.getAll()
        this.standards$ = this.standardsClient.getManys('')
        this.charts$ = this.chartsClient.getMany()     
        this.arrayStandards$ = this.standardsClient.getArrayStandards('')
        this.treeStandards$ = this.standardsClient.getTreeStandards('')
        this.standards$.subscribe(standards => {
            this.standards = standards
        })

        this.arrayStandards$.subscribe(standards => {
            this.arrayStandards = standards
        })

        this.treeStandards$.subscribe(trees => {
            this.treeStandards = trees
        })

        this.dynamicLists$.subscribe(dynamicLists => {
            this.dynamicLists = dynamicLists
        })

        this.charts$.subscribe(charts => {
            this.charts = charts
        })
    }

    initialSectionForm() {
        this.sectionForm = this.fb.group({
            name: [this.currentExtendedFormSection.name, [Validators.required, FormUtil.isExist(this.availableSectionNames, this.currentExtendedFormSection.name)]],
            displayName: [this.currentExtendedFormSection.displayName, Validators.required],
            constructionType: [this.currentExtendedFormSection.constructionType, Validators.required],
            componentId: [this.currentExtendedFormSection.componentId, Validators.required],
            hidden: [this.currentExtendedFormSection.hidden, Validators.required],
            rendered: [this.currentExtendedFormSection.rendered, Validators.required]
        })
    }

    populatedFormValues() {
        this.sectionForm.get('displayName').valueChanges.subscribe(newValue => {
            const sectionFormNameValue = (newValue as string).toLowerCase().replace(/\s/g, '').replace(/[$&+,:;=?@#|'<>.^*()%!-]/g, '')
            this.sectionForm.get('name').setValue(sectionFormNameValue)
            this.cd.markForCheck()
        })

        this.sectionForm.get('componentId').valueChanges.subscribe(newValue => {
            switch(this.sectionForm.value.constructionType){
                case SectionContructionType.Standard:
                    const selectedStandard = this.standards.find(a => a.id === newValue)
                    this.sectionForm.get('displayName').setValue(selectedStandard.displayName)
                    break
                case SectionContructionType.Array:
                    const selectedArrayStandard = this.arrayStandards.find(a => a.id === newValue)
                    this.sectionForm.get('displayName').setValue(selectedArrayStandard.displayName)
                    break
                case SectionContructionType.Chart:
                    const selectedChart = this.charts.find(a => a.id === newValue)
                    this.sectionForm.get('displayName').setValue(selectedChart.displayName)
                    break
                case SectionContructionType.DynamicList:
                    const selectedDynamicList = this.dynamicLists.find(a => a.id === newValue)
                    this.sectionForm.get('displayName').setValue(selectedDynamicList.displayName)
                    break
                case SectionContructionType.Tree:
                    const selectedTree = this.treeStandards.find(a => a.id === newValue)
                    this.sectionForm.get('displayName').setValue(selectedTree.displayName)
                    break
            }
        })
    }

    generateFormSection(): ExtendedPageSection {
        const formValues = this.sectionForm.value
        const extendedFormSection: ExtendedPageSection = {
            id: this.currentExtendedFormSection.id,
            name: formValues.name,
            displayName: formValues.displayName,
            componentId: formValues.componentId,
            hidden: formValues.hidden,
            rendered: formValues.rendered,
            constructionType: formValues.constructionType,
            order: this.currentExtendedFormSection.order,
            overrideOptions: this.currentExtendedFormSection.overrideOptions,
            sectionDatasource: this.currentExtendedFormSection.sectionDatasource,
            relatedDynamicList: !!formValues.componentId 
                        ? 
                        this.dynamicLists.find(
                            dynamicList => dynamicList.id === formValues.componentId) 
                            : this.currentExtendedFormSection.relatedDynamicList,
            relatedStandard: !!formValues.componentId 
                        ?
                        this.standards.find( 
                            standard => standard.id === formValues.componentId) 
                            : this.currentExtendedFormSection.relatedStandard,
            relatedChart: !!formValues.componentId 
                        ? 
                        this.charts.find( 
                            chart => chart.id === formValues.componentId) 
                            : this.currentExtendedFormSection.relatedChart,
            relatedArrayStandard: !!formValues.componentId
                        ? 
                        this.arrayStandards.find(
                            standard => standard.id === formValues.componentId)
                            : this.currentExtendedFormSection.relatedArrayStandard,
            relatedTreeStandard: !!formValues.componentId
                        ? 
                        this.treeStandards.find(
                            standard => standard.id === formValues.componentId)
                            : this.currentExtendedFormSection.relatedTreeStandard,
                            
            relatedButtons: [],
            isLoaded: false,
            isBroken: false
        }

        if (this.isEditMode) {
            // extendedFormSection.formControls = this.currentExtendedFormSection.formControls
        }

        return extendedFormSection
    }

    onSubmittingSection() {
        FormUtil.triggerFormValidators(this.sectionForm)
        if(this.sectionForm.valid){
            this.dialogRef.close(this.generateFormSection())
        }
    }
}
