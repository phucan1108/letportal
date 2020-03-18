import { Injectable } from '@angular/core';
import { FormGroup, FormControl, ValidatorFn, Validators, AsyncValidatorFn } from '@angular/forms';
import { PageLoadedDatasource, MapDataControl, PageRenderedControl, DefaultControlOptions } from 'app/core/models/page.model';
import { StandardComponent, ControlType, PageControlValidator, ValidatorType, PageControlAsyncValidator, PageControl } from 'services/portal.service';
import { ObjectUtils } from 'app/core/utils/object-util';
import * as _ from 'lodash';
import { CustomValidators } from 'ngx-custom-validators';
import { PortalValidators } from 'app/core/validators/portal.validators';
import { PageService } from 'services/page.service';
import { CustomHttpService } from 'services/customhttp.service';
import PageUtils from 'app/core/utils/page-util';
import { GroupControls } from 'app/core/models/extended.models';
import { StandardOptions } from 'portal/modules/models/standard.extended.model';
import { Guid } from 'guid-typescript';

/**
 * This service is used to share common logic between Standard and Array Standard
 * Due to complexity, we should warp it all into one service and improve code quality
 */
@Injectable()
export class StandardSharedService {

    constructor(
        private pageService: PageService,
        private customHttpService: CustomHttpService
    ) { }


    public buildControlsGroup(
        filteredControls: PageRenderedControl<DefaultControlOptions>[],
        numberOfColumns: number
    ): GroupControls[] {
        let controlGroups: GroupControls[] = []
        let counterFlag = 0;
        let tempGroupControls: GroupControls = {
            controlsList: [],
            numberOfColumns: numberOfColumns,
            isLineBreaker: false
        }

        let counterControls = 0
        _.forEach(filteredControls, (control: PageRenderedControl<DefaultControlOptions>) => {
            // These controls must be separated group
            if (control.type === ControlType.LineBreaker) {
                counterFlag = 0;
                controlGroups.push(tempGroupControls)
                controlGroups.push({
                    controlsList: [],
                    numberOfColumns: 0,
                    isLineBreaker: true
                })
                tempGroupControls = {
                    controlsList: [],
                    numberOfColumns: numberOfColumns,
                    isLineBreaker: false
                }
            }
            else if (control.type === ControlType.RichTextEditor) {
                counterFlag = 0;
                controlGroups.push(tempGroupControls)
                const standaloneGroup: GroupControls = {
                    controlsList: [],
                    numberOfColumns: 1,
                    isLineBreaker: false
                }
                standaloneGroup.controlsList.push(control)
                controlGroups.push(standaloneGroup)
                tempGroupControls = {
                    controlsList: [],
                    numberOfColumns: numberOfColumns,
                    isLineBreaker: false
                }
            }
            else if (control.type === ControlType.MarkdownEditor) {
                counterFlag = 0;
                controlGroups.push(tempGroupControls)
                const standaloneGroup: GroupControls = {
                    controlsList: [],
                    numberOfColumns: 1,
                    isLineBreaker: false
                }
                standaloneGroup.controlsList.push(control)
                controlGroups.push(standaloneGroup)
                tempGroupControls = {
                    controlsList: [],
                    numberOfColumns: numberOfColumns,
                    isLineBreaker: false
                }
            }
            else {

                tempGroupControls.controlsList.push(control)
                if (counterFlag === numberOfColumns - 1 || counterControls === filteredControls.length - 1) {
                    counterFlag = 0;
                    controlGroups.push(tempGroupControls)
                    tempGroupControls = {
                        controlsList: [],
                        numberOfColumns: numberOfColumns,
                        isLineBreaker: false
                    }
                } else {
                    counterFlag++;
                }
            }

            counterControls++
        })

        return controlGroups
    }

    public buildControlOptions(controls: PageRenderedControl<DefaultControlOptions>[]): PageRenderedControl<DefaultControlOptions>[] {
        controls.forEach(control => {
            control.defaultOptions = PageUtils.getControlOptions<DefaultControlOptions>(control.options)
            control.defaultOptions.checkedHidden = this.pageService.evaluatedExpression(control.defaultOptions.hidden)
            control.defaultOptions.checkDisabled = this.pageService.evaluatedExpression(control.defaultOptions.disabled)
        })
        return controls
    }

    public buildSectionBoundData(
        datasourceName: string,
        datasources: PageLoadedDatasource[]
    ): any{
        return this.getSectionBoundData(datasourceName, datasources)
    }

    public buildFormGroups(
        sectionName: string,
        standard: StandardComponent,
        boundData: any,
        isKeepDataSection: boolean,
        onComplete: (builtData: any, keptDataSection: boolean, sectionMap: MapDataControl[]) => void
    ): FormGroup {
        const formControls: any = []
        const tempSectionData = new Object()
        // When a 'data' is bind name, so we need to keep a default structure of data. Ex: data.id, data.name
        // If bind name isn't 'data', we need to add sectioname to binding data.Ex: data.section1.id
        const sectionsMap: MapDataControl[] = []
        let sectionBoundData = boundData

        if (!isKeepDataSection) {
            tempSectionData[sectionName] = new Object()
        }
        (standard.controls as PageRenderedControl<DefaultControlOptions>[]).forEach(control => {
            const controlData = this.getInitDataOfControl(sectionBoundData, control.defaultOptions.bindname, control)

            let mapDataControl: MapDataControl
            if (isKeepDataSection) {
                mapDataControl = {
                    controlFullName: sectionName + '_' + control.name,
                    sectionMapName: null,
                    bindName: control.defaultOptions.bindname
                }
                sectionsMap.push(mapDataControl)
                tempSectionData[control.name] = controlData
            }
            else {
                tempSectionData[sectionName][control.name] = controlData
                mapDataControl = {
                    controlFullName: sectionName + '_' + control.name,
                    sectionMapName: sectionName,
                    bindName: control.defaultOptions.bindname
                }
                sectionsMap.push(mapDataControl)
            }

            formControls[control.name] = new FormControl({
                value: this.getFormValue(control, controlData),
                disabled: control.defaultOptions.checkDisabled
            },
                this.generateFormValidators(control.validators, formControls),
                this.generateFormAsyncValidators(
                    control.asyncValidators,
                    mapDataControl.bindName,
                    mapDataControl.controlFullName,
                    sectionName,
                    control.name,
                    controlData,
                    formControls)
            )
        })
        if (onComplete) {
            onComplete(tempSectionData, isKeepDataSection, sectionsMap)
        }
        return new FormGroup(formControls)
    }

    public buildDataArray(
        datasourceName: string,
        datasources: PageLoadedDatasource[]
    ): any[] {
        let sectionBoundData = this.getSectionBoundData(datasourceName, datasources)

        // Ensure section bound data is array
        if (ObjectUtils.isArray(sectionBoundData)) {
            return sectionBoundData as any[]
        }
        else {
            return []
        }
    }

    public buildDataArrayForTable(
        arrayData: any[],
        standard: StandardComponent,
        options: StandardOptions,
        onComplete: (idField: string, ids: any[], cloneData: any) => void
    ): any[] {
        let arrayTableData = []
        let idKey = ''
        let ids: any[] = []
        let cloneData: any = new Object()
        if (arrayData.length > 0) {
            arrayData.forEach(element => {
                let tempElementObject = new Object();
                (standard.controls as PageRenderedControl<DefaultControlOptions>[])
                    .forEach(control => {
                        const foundKey = Object.keys(element).find(a => a === control.defaultOptions.bindname)
                        if (ObjectUtils.isNotNull(foundKey)) {
                            tempElementObject[control.defaultOptions.bindname] = element[foundKey]
                        }
                        else {
                            // Ensure object has property as control name
                            tempElementObject[control.defaultOptions.bindname] = null
                        }
                    })

                // If identityfield is empty, we should create an identity field
                if (!ObjectUtils.isNotNull(options.identityfield)
                    || Object.keys(tempElementObject).indexOf(options.identityfield) < 0) {
                    tempElementObject['uniq_id'] = Guid.create().toString()
                    ids.push(tempElementObject['uniq_id'])
                    idKey = 'uniq_id'
                }
                else {
                    ids.push(tempElementObject[options.identityfield])
                    idKey = options.identityfield
                }

                arrayTableData.push(tempElementObject)
            })

            cloneData = ObjectUtils.clone(arrayTableData[0])
            Object.keys(cloneData).forEach(e => {
                cloneData[e] = null
            })
        }
        else {
            // If there are no data, we need to create clone object for inserting
            (standard.controls as PageRenderedControl<DefaultControlOptions>[])
                .forEach(control => {
                    cloneData[control.defaultOptions.bindname] = null
                })

            if (!ObjectUtils.isNotNull(options.identityfield)) {
                cloneData['uniq_id'] = null
                idKey = 'uniq_id'
            }
            else {
                idKey = options.identityfield
            }
        }


        if (ObjectUtils.isNotNull(onComplete)) {
            onComplete(idKey, ids, cloneData)
        }

        return arrayTableData
    }

    private getSectionBoundData(datasourceName: string, datasources: PageLoadedDatasource[]) {
        let sectionBoundData = {}
        // When a 'data' is bind name, so we need to keep a default structure of data. Ex: data.id, data.name
        // If bind name isn't 'data', we need to add sectioname to binding data.Ex: data.section1.id
        const isKeepDataSection = datasourceName === 'data'
        const hasDatasources = datasources.length > 0

        if (hasDatasources &&
            datasourceName) {

            let foundDatasource: PageLoadedDatasource;

            if ((datasourceName.indexOf('data.') === 0
                || isKeepDataSection)) {
                foundDatasource = datasources.find(ds => ds.name === 'data')
                const execute = new Function('data', 'return ' + datasourceName)
                sectionBoundData = execute(foundDatasource.data)
            }
            else {
                const splittedDs = datasourceName.split('.')
                let hasDot = splittedDs.length > 1
                if (hasDot) {
                    foundDatasource = datasources.find(ds => ds.name === splittedDs[0])
                }
                else {
                    foundDatasource = datasources.find(ds => ds.name === datasourceName)
                }

                const execute = new Function(datasourceName, 'return ' + datasourceName)
                sectionBoundData = execute(foundDatasource.data)
            }
        }
        return sectionBoundData
    }

    private getInitDataOfControl(
        data: any,
        controlBindName: string,
        control: PageRenderedControl<DefaultControlOptions>): any {
        let controlData = null
        if (controlBindName === 'id' || controlBindName === '_id') {
            const boundData = data._id
            if (!boundData) {
                controlData = data.id
            }
            else {
                controlData = boundData
            }
        }
        else {
            controlData = data[controlBindName]
        }

        // Depends on control type, we need to doublecheck data and set default value if data is null
        if ((control.type == ControlType.AutoComplete
            || control.type == ControlType.Select)
            && control.defaultOptions.multiple) {
            if (controlData) {
                // If the data isn't an array, maybe it is array string, try to parse
                if (!ObjectUtils.isArray(controlData)) {
                    try {
                        const temp = JSON.parse(controlData)
                        controlData = temp
                    }
                    catch{
                        controlData = [controlData]
                    }
                }
            }
            else {
                controlData = []
            }
        }
        else if ((control.type == ControlType.Checkbox || control.type == ControlType.Slide) && controlData) {
            if (controlData.toString() == '0' || controlData.toString() == '1') {
                control.defaultOptions.allowZero = true
            }
            else if (controlData == 'Y' || controlData == 'N') {
                control.defaultOptions.allowYesNo = true
            }
            else {
                // Checkbox or slide must be true/false, there shouldn't be null
                controlData = ObjectUtils.isNotNull(controlData)
                return controlData
            }
        }

        // Otherwise, normal cases can return value or null
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

    private generateFormValidators(
        validators: Array<PageControlValidator>,
        availableControls: any): ValidatorFn[] {
        const formValidators: Array<ValidatorFn> = []
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
        sectionName: string,
        controlName: string,
        defaultValue: any,
        availableControls: any): AsyncValidatorFn[] {
        const asyncValidatorFns: AsyncValidatorFn[] = []


        _.forEach(validators, validator => {
            asyncValidatorFns.push(PortalValidators
                .addAsyncValidator(
                    validator,
                    controlBindName,
                    controlFullName,
                    sectionName,
                    controlName,
                    defaultValue,
                    this.pageService,
                    this.customHttpService))
        })

        return asyncValidatorFns
    }
}