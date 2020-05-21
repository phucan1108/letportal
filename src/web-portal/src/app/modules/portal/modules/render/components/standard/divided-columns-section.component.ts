import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import * as _ from 'lodash';
import { ExtendedPageSection, GroupControls } from 'app/core/models/extended.models';
import { FormGroup, FormControl, ValidatorFn, Validators, FormBuilder, AsyncValidatorFn } from '@angular/forms';
import { JsonEditorOptions } from 'ang-jsoneditor';
import { ControlType, PageSectionLayoutType, PageControl, PageControlValidator, ValidatorType, PageControlAsyncValidator, DatabasesClient } from 'services/portal.service';
import { Store } from '@ngxs/store';
import { PageStateModel } from 'stores/pages/page.state';
import { Observable, Subscription } from 'rxjs';
import { filter, tap } from 'rxjs/operators';
import { EndRenderingPageSectionsAction, AddSectionBoundData, GatherSectionValidations, SectionValidationStateAction } from 'stores/pages/page.actions';
import { DefaultControlOptions, PageRenderedControl, PageLoadedDatasource, MapDataControl } from 'app/core/models/page.model';
import PageUtils from 'app/core/utils/page-util';
import { CustomValidators } from 'ngx-custom-validators';
import { NGXLogger } from 'ngx-logger';
import { PageService } from 'services/page.service';
import { PortalValidators } from 'app/core/validators/portal.validators';
import { CustomHttpService } from 'services/customhttp.service';
import { ObjectUtils } from 'app/core/utils/object-util';
import { StandardSharedService } from './services/standard-shared.service';
import { FormUtil } from 'app/core/utils/form-util';
import { LocalizationService } from 'services/localization.service';

@Component({
    selector: 'divided-columns',
    templateUrl: './divided-columns-section.component.html'
})
export class DividedColumnsSectionComponent implements OnInit, OnDestroy {

    @Input()
    section: ExtendedPageSection

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
    constructor(
        private logger: NGXLogger,
        private fb: FormBuilder,
        private store: Store,
        private standardSharedService: StandardSharedService
    ) { }

    ngOnInit(): void {
        this.logger.debug('Init divided columns')
        this.pageState$ = this.store.select<PageStateModel>(state => state.page)
        this.subscription = this.pageState$.pipe(
            filter(state => state.filterState
                && (state.filterState === EndRenderingPageSectionsAction
                    || state.filterState === GatherSectionValidations)),
            tap(
                state => {
                    switch (state.filterState) {
                        case EndRenderingPageSectionsAction:
                            this.logger.debug('Rendered hit')
                            this.options = state.options
                            this.queryparams = state.queryparams
                            this.datasources = state.datasources
                            this.prepareRender()
                            this.dividedControls()
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
            .buildControlOptions(this.section.relatedStandard.controls as PageRenderedControl<DefaultControlOptions>[])
            .filter(control => {
                return !control.defaultOptions.checkedHidden
            })
        this.controlsGroups = this.standardSharedService.buildControlsGroup(filteredControls, this._numberOfColumns)
    }

    buildFormControls() {
        const sectionBoundData = this.standardSharedService
            .buildSectionBoundData(
                this.section.sectionDatasource.datasourceBindName,
                this.datasources)
        this.builderFormGroup = this.standardSharedService.buildFormGroups(
            this.section.name,
            this.section.relatedStandard,
            sectionBoundData,
            this.section.sectionDatasource.datasourceBindName === 'data',
            null,
            (data, keptDataSection, sectionsMap) => {
                this.store.dispatch(new AddSectionBoundData({
                    name: this.section.name,
                    isKeptDataName: keptDataSection,
                    data: data
                }, sectionsMap))
            }
        )
    }

    private collectAllControlsState(controls: any) {
        const controlStates = new Object()
        Object.keys(controls).forEach(key => {
            const control = controls[key] as FormControl
            controlStates[key] = {
                valid: control.valid,
                errors: control.errors
            }
        })

        return controlStates
    }
}
