import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Store } from '@ngxs/store';
import { JsonEditorOptions } from 'ang-jsoneditor';
import { SimpleBoundControl } from 'app/core/context/bound-control';
import { StandardBoundSection } from 'app/core/context/bound-section';
import { ExtendedPageSection, GroupControls } from 'app/core/models/extended.models';
import { DefaultControlOptions, PageLoadedDatasource, PageRenderedControl } from 'app/core/models/page.model';
import { FormUtil } from 'app/core/utils/form-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { Observable, Subscription } from 'rxjs';
import { filter, tap } from 'rxjs/operators';
import { ControlType, PageSectionLayoutType } from 'services/portal.service';
import { AddSectionBoundData, BeginBuildingBoundData, EndRenderingPageSectionsAction, GatherSectionValidations, SectionValidationStateAction } from 'stores/pages/page.actions';
import { PageStateModel } from 'stores/pages/page.state';
import { StandardSharedService } from './services/standard-shared.service';

@Component({
    selector: 'divided-columns',
    templateUrl: './divided-columns-section.component.html'
})
export class DividedColumnsSectionComponent implements OnInit, OnDestroy {

    @Input()
    section: ExtendedPageSection

    @Input()
    boundSection: StandardBoundSection

    standardBoundSection: StandardBoundSection
    builderFormGroup: FormGroup
    queryparams: any
    options: any
    data: any
    controlsGroups: Array<GroupControls>
    controls: Array<PageRenderedControl<DefaultControlOptions>>
    datasources: PageLoadedDatasource[]
    controlType = ControlType
    _numberOfColumns = 2;
    _labelClass = 'col-lg-2 col-form-label'
    _boundedClass = 'col-lg-4'
    jsonOptions = new JsonEditorOptions();
    queryJsonData: any = '';
    pageState$: Observable<PageStateModel>
    subscription: Subscription
    readyToRender = false

    storeName: string
    constructor(
        private logger: NGXLogger,
        private fb: FormBuilder,
        private store: Store,
        private standardSharedService: StandardSharedService
    ) { }

    ngOnInit(): void {
        this.logger.debug('Init divided columns')
        this.pageState$ = this.store.select<PageStateModel>(state => state.page)
        this.storeName =
            ObjectUtils.isNotNull(this.section.sectionDatasource.dataStoreName) 
                ? this.section.sectionDatasource.dataStoreName
                    : (this.section.sectionDatasource.datasourceBindName === 'data' ? 'data' : this.section.name)
        this.subscription = this.pageState$.pipe(
            filter(state => state.filterState
                && (state.filterState === EndRenderingPageSectionsAction
                    || state.filterState === BeginBuildingBoundData
                    || state.filterState === GatherSectionValidations)),
            tap(
                state => {
                    switch (state.filterState) {
                        case EndRenderingPageSectionsAction:
                            this.logger.debug('Rendered hit')
                            this.options = state.options
                            this.queryparams = state.queryparams
                            this.datasources = state.datasources
                            this.data = this.standardSharedService
                                        .buildSectionBoundData(
                                            this.section.sectionDatasource.datasourceBindName,
                                            this.datasources)
                            this.prepareRender()
                            this.dividedControls()
                            break
                        case BeginBuildingBoundData:
                            this.buildFormControls()                                                        
                            this.readyToRender = true
                            break
                        case GatherSectionValidations:
                            if (state.specificValidatingSection === this.section.name
                                || !ObjectUtils.isNotNull(state.specificValidatingSection)) {
                                FormUtil.triggerFormValidators(this.builderFormGroup)                                
                                if (this.builderFormGroup.invalid) {
                                    this.logger.debug('Section errors', this.collectAllControlsState(this.builderFormGroup.controls))
                                }
                                this.store.dispatch(new SectionValidationStateAction(this.section.name, this.builderFormGroup.valid))
                            }

                            break
                    }
                }
            )
        ).subscribe()
        this.builderFormGroup = this.fb.group({})
        this.jsonOptions.mode = 'code';
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe()
    }
    getBoundControl = (control: PageRenderedControl<DefaultControlOptions>) => this.standardBoundSection.get(control.name)

    prepareRender() {
        // By default, OneColumn must be separated two columns for all controls
        // another layout must be kept one col
        switch (this.section.relatedStandard.layoutType) {
            case PageSectionLayoutType.OneColumn:
                this._numberOfColumns = 2
                this._labelClass = 'col-lg-2 col-form-label'
                this._boundedClass = 'col-lg-4'
                break;
            default:
                this._numberOfColumns = 1
                this._labelClass = 'col-lg-2 col-form-label'
                this._boundedClass = 'col-lg-10'
                break
        }
    }

    dividedControls() {
        const filteredControls = this.standardSharedService
            .buildControlOptions(
                this.section.relatedStandard.controls as PageRenderedControl<DefaultControlOptions>[],
                this.data)
            .filter(control => {
                return control.defaultOptions.checkRendered
            })

        this.section.relatedStandard.controls = filteredControls
        this.controls = filteredControls
        this.controlsGroups = this.standardSharedService
                .buildControlsGroup(
                    filteredControls, this._numberOfColumns)
    }

    buildFormControls() {
        const sectionBoundData = this.data        
        this.standardBoundSection = this.standardSharedService.buildBoundSection(
            this.section.name,
            this.storeName,
            this.section.relatedStandard,
            sectionBoundData,
            null,
            (data, sectionsMap) => {
                this.store.dispatch(new AddSectionBoundData({
                    storeName: this.storeName,
                    data: data
                }, sectionsMap))
            }
        ) as StandardBoundSection

        this.boundSection.load(this.standardBoundSection.getAll())
        this.boundSection.loadFormGroup(this.standardBoundSection.getFormGroup())

        // Change 0.9.0: We need to set boundcontrol into PageControl
        if(ObjectUtils.isNotNull(this.controls)){
            this.controls.forEach(control => {
                let foundBoundControl = this.standardBoundSection.get(control.name)
                if(ObjectUtils.isNotNull(foundBoundControl)){
                    control.boundControl = foundBoundControl
                    control.boundControl.hide = control.defaultOptions.checkedHidden
                }
                else{
                    control.boundControl = new SimpleBoundControl(control.name, control.type, null)
                    control.boundControl.hide = control.defaultOptions.checkedHidden
                }            
            })
        }
        
        this.builderFormGroup = this.standardBoundSection.getFormGroup()
    }

    private collectAllControlsState(controls: any) {
        const controlStates = new Object()
        Object.keys(controls)?.forEach(key => {
            const control = controls[key] as FormControl
            controlStates[key] = {
                valid: control.valid,
                errors: control.errors
            }
        })

        return controlStates
    }
}
