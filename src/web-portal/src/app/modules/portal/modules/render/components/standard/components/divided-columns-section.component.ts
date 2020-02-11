import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import * as _ from 'lodash';
import { ExtendedPageSection, GroupControls, ExtendedPageControl } from 'app/core/models/extended.models';
import { FormGroup, FormControl, ValidatorFn, Validators, FormBuilder, AsyncValidatorFn } from '@angular/forms';
import { JsonEditorOptions } from 'ang-jsoneditor';
import { ShellConfigProvider } from 'app/core/shell/shellconfig.provider';
import { ControlType, PageSectionLayoutType, PageControl, PageControlValidator, ValidatorType, PageControlAsyncValidator, DatabasesClient } from 'services/portal.service';
import { Store } from '@ngxs/store';
import { PageStateModel } from 'stores/pages/page.state';
import { Observable, Subscription } from 'rxjs';
import { filter, tap } from 'rxjs/operators';
import { PageReadyAction, EndRenderingPageSectionsAction, UpdateDatasourceAction, AddSectionBoundData, GatherSectionValidations, SectionValidationStateAction } from 'stores/pages/page.actions';
import { DefaultControlOptions, PageRenderedControl, PageLoadedDatasource, MapDataControl } from 'app/core/models/page.model';
import PageUtils from 'app/core/utils/page-util';
import { CustomValidators } from 'ngx-custom-validators';
import { NGXLogger } from 'ngx-logger';
import { PageService } from 'services/page.service';
import { PortalValidators } from 'app/core/validators/portal.validators';
import { CustomHttpService } from 'services/customhttp.service';
import { ObjectUtils } from 'app/core/utils/object-util';

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
    _labelClass = "col-lg-2 col-form-label"
    _boundedClass = "col-lg-4"

    jsonOptions = new JsonEditorOptions();
    queryJsonData: any = '';

    pageState$: Observable<PageStateModel>
    subscription: Subscription
    readyToRender = false
    constructor(
        private logger: NGXLogger,
        private fb: FormBuilder,
        private store: Store,
        private databasesClient: DatabasesClient,
        private customHttpService: CustomHttpService,
        private pageService: PageService
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
                            this.store.dispatch(new SectionValidationStateAction(this.section.name, this.builderFormGroup.valid))
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
                this._labelClass = "col-lg-2 col-form-label"
                this._boundedClass = "col-lg-4"
                break;
            default:
                this._numberOfColumns = 1
                this._labelClass = "col-lg-2 col-form-label"
                this._boundedClass = "col-lg-10"
                break
        }
    }

    dividedControls() {
        this.controlsGroups = [];

        let counterFlag = 0;
        let tempGroupControls: GroupControls = {
            controlsList: [],
            numberOfColumns: this._numberOfColumns,
            isLineBreaker: false
        }

        let filteredControls = _.filter<PageRenderedControl<DefaultControlOptions>>(this.section.relatedStandard.controls as PageRenderedControl<DefaultControlOptions>[], (control: PageRenderedControl<DefaultControlOptions>) => {
            control.defaultOptions = PageUtils.getControlOptions<DefaultControlOptions>(control.options)
            control.defaultOptions.checkedHidden = this.pageService.evaluatedExpression(control.defaultOptions.hidden)
            control.defaultOptions.checkDisabled = this.pageService.evaluatedExpression(control.defaultOptions.disabled)
            return !control.defaultOptions.checkedHidden
        })
        this.controls = filteredControls
        this.logger.debug('Rendering controls', this.controls)
        let counterControls = 0
        _.forEach(filteredControls, (control: PageRenderedControl<DefaultControlOptions>) => {
            // These controls must be separated group
            if (control.type === ControlType.LineBreaker) {
                counterFlag = 0;
                this.controlsGroups.push(tempGroupControls)
                this.controlsGroups.push({
                    controlsList: [],
                    numberOfColumns: 0,
                    isLineBreaker: true
                })
                tempGroupControls = {
                    controlsList: [],
                    numberOfColumns: this._numberOfColumns,
                    isLineBreaker: false
                }
            }
            else if (control.type === ControlType.RichTextEditor) {
                counterFlag = 0;
                this.controlsGroups.push(tempGroupControls)
                let standaloneGroup: GroupControls = {
                    controlsList: [],
                    numberOfColumns: 1,
                    isLineBreaker: false
                }
                standaloneGroup.controlsList.push(control)
                this.controlsGroups.push(standaloneGroup)
                tempGroupControls = {
                    controlsList: [],
                    numberOfColumns: this._numberOfColumns,
                    isLineBreaker: false
                }
            }
            else if (control.type === ControlType.MarkdownEditor) {
                counterFlag = 0;
                this.controlsGroups.push(tempGroupControls)
                let standaloneGroup: GroupControls = {
                    controlsList: [],
                    numberOfColumns: 1,
                    isLineBreaker: false
                }
                standaloneGroup.controlsList.push(control)
                this.controlsGroups.push(standaloneGroup)
                tempGroupControls = {
                    controlsList: [],
                    numberOfColumns: this._numberOfColumns,
                    isLineBreaker: false
                }
            }
            else {

                tempGroupControls.controlsList.push(control)
                if (counterFlag === this._numberOfColumns - 1 || counterControls === filteredControls.length - 1) {
                    counterFlag = 0;
                    this.controlsGroups.push(tempGroupControls)
                    tempGroupControls = {
                        controlsList: [],
                        numberOfColumns: this._numberOfColumns,
                        isLineBreaker: false
                    }
                } else {
                    counterFlag++;
                }
            }

            counterControls++
        })
    }

    buildFormControls() {
        let formControls: any = []
        let tempSectionData = new Object()
        let foundDatasource: PageLoadedDatasource
        let sectionBoundData = {}
        // When a 'data' is bind name, so we need to keep a default structure of data. Ex: data.id, data.name
        // If bind name isn't 'data', we need to add sectioname to binding data.Ex: data.section1.id
        let isKeepDataSection = this.section.sectionDatasource.datasourceBindName === 'data'
        let sectionsMap: MapDataControl[] = []
        let hasDatasources = this.datasources.length > 0

        if (hasDatasources &&
            this.section.sectionDatasource.datasourceBindName) {

            let foundDatasource: any;

            if ((this.section.sectionDatasource.datasourceBindName.indexOf('data.') === 0
                || isKeepDataSection)) {
                foundDatasource = _.find(this.datasources, ds => ds.name === 'data')
                const execute = new Function('data', 'return ' + this.section.sectionDatasource.datasourceBindName)
                sectionBoundData = execute(foundDatasource.data)
            }
            else {
                foundDatasource = _.find(this.datasources, ds => ds.name === this.section.sectionDatasource.datasourceBindName)
                const execute = new Function(this.section.sectionDatasource.datasourceBindName, 'return ' + this.section.sectionDatasource.datasourceBindName)
                sectionBoundData = execute(foundDatasource.data)
            }
        }

        if (!isKeepDataSection) {
            tempSectionData[this.section.name] = new Object()
        }
        _.forEach(this.section.relatedStandard.controls as PageRenderedControl<DefaultControlOptions>[], control => {
            let controlData = this.getInitDataOfControl(sectionBoundData, control.defaultOptions.bindname, control)

            let mapDataControl: MapDataControl
            if (isKeepDataSection) {
                mapDataControl = {
                    controlFullName: this.section.name + '_' + control.name,
                    sectionMapName: null,
                    bindName: control.defaultOptions.bindname
                }
                sectionsMap.push(mapDataControl)
                tempSectionData[control.name] = controlData
            }
            else {
                tempSectionData[this.section.name][control.name] = controlData
                mapDataControl = {
                    controlFullName: this.section.name + '_' + control.name,
                    sectionMapName: this.section.name,
                    bindName: control.defaultOptions.bindname
                }
                sectionsMap.push(mapDataControl)
            }

            formControls[control.name] = new FormControl({
                value: this.getFormValue(control, controlData),
                disabled: control.defaultOptions.checkDisabled
            },
                this.generateFormValidators(control.validators, formControls),
                this.generateFormAsyncValidators(control.asyncValidators, mapDataControl.bindName, mapDataControl.controlFullName, controlData, formControls)
            )
        })
        this.logger.debug('section data', tempSectionData)
        this.store.dispatch(new AddSectionBoundData({
            name: this.section.name,
            isKeptDataName: isKeepDataSection,
            data: tempSectionData
        }, sectionsMap))
        this.builderFormGroup = new FormGroup(formControls)
    }

    private getInitDataOfControl(data: any, controlBindName: string, control: PageRenderedControl<DefaultControlOptions>): any {
        let controlData = null
        if (controlBindName === 'id' || controlBindName === '_id') {
            const boundData = data['_id']
            if (!boundData) {
                controlData = data['id']
            }
            else {
                controlData = boundData
            }
        }
        else {
            controlData = data[controlBindName]
        }

        // Depends on control type, we need to doublecheck data and set default value if data is null
        if (control.type == ControlType.AutoComplete || control.type == ControlType.Select) {
            if (controlData) {
                if (!ObjectUtils.isArray(controlData)) {
                    try {
                        let temp = JSON.parse(controlData)
                        controlData = temp
                    }
                    catch{
                        controlData = []
                    }
                }
            }
            else {
                controlData = []
            }
        }
        else if (control.type == ControlType.Checkbox || control.type == ControlType.Slide) {
            if (controlData == 0 || controlData == 1) {
                control.defaultOptions.allowZero = true
            }
            else if (controlData == 'Y' || controlData == 'N') {
                control.defaultOptions.allowYesNo = true
            }
            else {
                if (!ObjectUtils.isNotNull(controlData)) {
                    controlData = false
                }
            }
        }

        controlData = ObjectUtils.isNotNull(controlData) ? controlData : null
        return controlData
    }

    private getFormValue(control: PageRenderedControl<DefaultControlOptions>, controlData: any) {
        if (control.type == ControlType.Checkbox || control.type == ControlType.Slide) {
            if (control.defaultOptions.allowYesNo) {
                return controlData == 'Y'
            }
            else if (control.defaultOptions.allowZero) {
                return controlData == 1
            }
        }
        return controlData
    }

    private generateFormValidators(validators: Array<PageControlValidator>, availableControls: any): ValidatorFn[] {
        let formValidators: Array<ValidatorFn> = []
        _.forEach(validators, (validator: PageControlValidator) => {
            if (validator.isActive) {
                switch (validator.validatorType) {
                    case ValidatorType.Required:
                        formValidators.push(Validators.required)
                        break
                    case ValidatorType.Equal:
                        formValidators.push(CustomValidators.equal(JSON.parse(validator.validatorOption)))
                        break
                    case ValidatorType.EqualTo:
                        const foundEqualToControl = availableControls[validator.validatorOption]
                        if (!!foundEqualToControl) {
                            formValidators.push(CustomValidators.equalTo(foundEqualToControl))
                        }
                        break
                    case ValidatorType.Email:
                        formValidators.push(CustomValidators.email)
                        break
                    case ValidatorType.Number:
                        formValidators.push(CustomValidators.number)
                        break
                    case ValidatorType.NumberRange:
                        formValidators.push(CustomValidators.range(JSON.parse(validator.validatorOption)))
                        break
                    case ValidatorType.DateTime:
                        formValidators.push(CustomValidators.date)
                        break
                    case ValidatorType.MinDate:
                        formValidators.push(CustomValidators.minDate(validator.validatorOption))
                        break
                    case ValidatorType.MaxDate:
                        formValidators.push(CustomValidators.maxDate(validator.validatorOption))
                        break
                    case ValidatorType.Regex:
                        formValidators.push(Validators.pattern(validator.validatorOption))
                        break
                    case ValidatorType.MinLength:
                        formValidators.push(Validators.minLength(parseInt(validator.validatorOption)))
                        break
                    case ValidatorType.MaxLength:
                        formValidators.push(Validators.maxLength(parseInt(validator.validatorOption)))
                        break
                    case ValidatorType.Json:
                        formValidators.push(CustomValidators.json)
                        break
                }
            }
        })
        return formValidators
    }

    private generateFormAsyncValidators(
        validators: Array<PageControlAsyncValidator>,
        controlBindName: string,
        controlFullName: string,
        defaultValue: any,
        availableControls: any): AsyncValidatorFn[] {
        let asyncValidatorFns: AsyncValidatorFn[] = []

        _.forEach(validators, validator => {
            asyncValidatorFns.push(PortalValidators.addAsyncValidator(validator, controlBindName, controlFullName, defaultValue, this.databasesClient, this.pageService, this.customHttpService))
        })

        return asyncValidatorFns
    }
}
