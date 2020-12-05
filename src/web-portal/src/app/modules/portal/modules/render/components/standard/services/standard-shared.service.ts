import { Injectable } from '@angular/core';
import { AsyncValidatorFn, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { BoundControl, SelectBoundControl, SimpleBoundControl } from 'app/core/context/bound-control';
import { BoundSection, StandardBoundSection } from 'app/core/context/bound-section';
import { GroupControls } from 'app/core/models/extended.models';
import { DefaultControlOptions, MapDataControl, PageLoadedDatasource, PageRenderedControl } from 'app/core/models/page.model';
import { ObjectUtils } from 'app/core/utils/object-util';
import PageUtils from 'app/core/utils/page-util';
import { PortalValidators } from 'app/core/validators/portal.validators';
import { Guid } from 'guid-typescript';
import { CustomValidators } from 'ngx-custom-validators';
import { ArrayStandardOptions, TreeStandardOptions } from 'portal/modules/models/standard.extended.model';
import { CustomHttpService } from 'services/customhttp.service';
import { PageService } from 'services/page.service';
import { ControlType, PageControlAsyncValidator, PageControlValidator, StandardComponent, ValidatorType } from 'services/portal.service';
import { ExtendedFormControlValidators } from '../models/standard.models';


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


    /**
     * This method is used to group all controls which are fitted in layout
     * @param filteredControls all displayed controls
     * @param numberOfColumns one col, two cols, etc...
     * @returns controls group 
     */
    public buildControlsGroup(
        filteredControls: PageRenderedControl<DefaultControlOptions>[],
        numberOfColumns: number
    ): GroupControls[] {
        let controlGroups: GroupControls[] = []
        let counterFlag = 0;
        let tempGroupControls: GroupControls = {
            controlsList: [],
            numberOfColumns: numberOfColumns,
            isLineBreaker: false,
            isCompositeGroup: false,
            compositeGroupRef: null
        }

        // New 0.9.0: We need to prepare group of composite controls
        let compositeControlGroups: GroupControls[] = []
        const compositeControls = filteredControls
            .filter(a =>
                ObjectUtils.isNotNull(a.compositeControlId))

        compositeControls.forEach(control => {
            if (control.type === ControlType.Composite) {
                const childControls = compositeControls.filter(a => a.compositeControlId === control.name && a.type !== ControlType.Composite)
                childControls.forEach(child => {
                    child.compositeControlRefId = control.compositeControlId
                    child.compositeControlBindName = control.defaultOptions.bindname
                    child.compositeControlName = control.name
                    tempGroupControls.controlsList.push(child)
                })
                tempGroupControls.compositeGroupRef = control.name
                tempGroupControls.isCompositeGroup = true
                tempGroupControls.compositeGroupLabel = control.defaultOptions.label
                compositeControlGroups.push(tempGroupControls)
                tempGroupControls = {
                    controlsList: [],
                    numberOfColumns: numberOfColumns,
                    isLineBreaker: false,
                    isCompositeGroup: false,
                    compositeGroupRef: null
                }
            }
        })

        let counterControls = 0
        filteredControls?.forEach((control: PageRenderedControl<DefaultControlOptions>) => {
            if (control.type !== ControlType.Composite && ObjectUtils.isNotNull(control.compositeControlId)) {
                // Child control of Composite, skip
                return
            }

            // These controls must be separated group
            switch (control.type) {
                case ControlType.LineBreaker:
                    counterFlag = 0;
                    controlGroups.push({
                        controlsList: [],
                        numberOfColumns: 0,
                        isLineBreaker: true,
                        isCompositeGroup: false,
                        compositeGroupRef: null
                    })
                    tempGroupControls = {
                        controlsList: [],
                        numberOfColumns: numberOfColumns,
                        isLineBreaker: false,
                        isCompositeGroup: false,
                        compositeGroupRef: null
                    }
                    break
                case ControlType.RichTextEditor:
                    counterFlag = 0;
                    const standaloneRichtextGroup: GroupControls = {
                        controlsList: [],
                        numberOfColumns: 1,
                        isLineBreaker: false,
                        isCompositeGroup: false,
                        compositeGroupRef: null
                    }
                    standaloneRichtextGroup.controlsList.push(control)
                    controlGroups.push(standaloneRichtextGroup)
                    tempGroupControls = {
                        controlsList: [],
                        numberOfColumns: numberOfColumns,
                        isLineBreaker: false,
                        isCompositeGroup: false,
                        compositeGroupRef: null
                    }
                    break
                case ControlType.MarkdownEditor:
                    counterFlag = 0;
                    const standaloneGroup: GroupControls = {
                        controlsList: [],
                        numberOfColumns: 1,
                        isLineBreaker: false,
                        isCompositeGroup: false,
                        compositeGroupRef: null
                    }
                    standaloneGroup.controlsList.push(control)
                    controlGroups.push(standaloneGroup)
                    tempGroupControls = {
                        controlsList: [],
                        numberOfColumns: numberOfColumns,
                        isLineBreaker: false,
                        isCompositeGroup: false,
                        compositeGroupRef: ''
                    }
                    break
                case ControlType.Composite:
                    counterFlag = 0;
                    controlGroups.push(compositeControlGroups.find(a => a.compositeGroupRef === control.name))
                    tempGroupControls = {
                        controlsList: [],
                        numberOfColumns: numberOfColumns,
                        isLineBreaker: false,
                        isCompositeGroup: false,
                        compositeGroupRef: null
                    }
                    break
                default:
                    tempGroupControls.controlsList.push(control)
                    if (counterFlag === numberOfColumns - 1 || counterControls === filteredControls.length - 1) {
                        counterFlag = 0;
                        controlGroups.push(tempGroupControls)
                        tempGroupControls = {
                            controlsList: [],
                            numberOfColumns: numberOfColumns,
                            isLineBreaker: false,
                            compositeGroupRef: null,
                            isCompositeGroup: false
                        }
                    } else {
                        counterFlag++;
                    }
                    break
            }

            counterControls++
        })
        console.log('Controls have been grouped', controlGroups)
        return controlGroups
    }

    public buildControlOptions(
        controls: PageRenderedControl<DefaultControlOptions>[],
        initData: any = null): PageRenderedControl<DefaultControlOptions>[] {
        controls?.forEach(control => {
            control.defaultOptions = PageUtils.getControlOptions<DefaultControlOptions>(control.options)
            control.defaultOptions.checkedHidden = this.pageService.evaluatedExpression(control.defaultOptions.hidden, initData)
            control.defaultOptions.checkDisabled = this.pageService.evaluatedExpression(control.defaultOptions.disabled, initData)
            if (ObjectUtils.isNotNull(control.defaultOptions.rendered)) {
                control.defaultOptions.checkRendered = this.pageService.evaluatedExpression(control.defaultOptions.rendered, initData)
            }
            else {
                // Default rendered is true
                control.defaultOptions.checkRendered = true
            }
            if (ObjectUtils.isNotNull(control.defaultOptions.defaultvalue)) {
                control.defaultOptions.defaultvalue = this.pageService.translateData(control.defaultOptions.defaultvalue)
            }
            control.customErrorMessages = []
        })
        return controls
    }

    public buildSectionBoundData(
        datasourceName: string,
        datasources: PageLoadedDatasource[]
    ): any {
        return this.getSectionBoundData(datasourceName, datasources)
    }

    /**
     * Uses this method for constructing StandardBoundSection by passing StandardComponent
     * All types must call this method to build StandardBoundSection which is used in form or dialog
     * @param sectionName Section name
     * @param storeName Store name is inputted while constructing Page Builder, it can be null 
     * @param standard StandardComponent instance
     * @param boundData BoundData can be gotten from Datasource, use to set initial value and construct section data     
     * @param [extendedValidators] Passing some custom validators, inputted in Standard Builder page
     * @param onComplete Callback method to retrieve sectionData, sectionMap
     * @returns bound section Always returned StandardBoundSection
     */
    public buildBoundSection(
        sectionName: string,
        storeName: string,
        standard: StandardComponent,
        boundData: any,
        extendedValidators: ExtendedFormControlValidators[] = null,
        onComplete: (builtData: any, sectionMap: MapDataControl[]) => void
    ): BoundSection {
        const formControls: any = []
        const tempSectionData = new Object()
        // Changed from 0.9.0: introduces 'storeName' to indicate where data is stored
        // If 'storeName' != null, so we will store its in second level of 'data'.
        // Ex: 'storeName' = 'info', so data will be stored in 'data.info'
        // Else -> we will store in root level same with section name
        // Also, if dataBindName === 'data', mean all fields will stayed in root level
        // Ex: 'dataBindName' === 'data', => 'data.username', 'data.password' instead of 'data.info.username'
        // So in this case we will use 'storeName' = 'data' to apdapt this scenario
        // IMPORTANT NOTE: storeName is available for Standard, Array or Tree
        // However, we should pass storeName = null when calling this method on Array or Tree
        // Because Array/Tree doesn't update the data frequently
        const sectionsMap: MapDataControl[] = []
        let sectionBoundData = boundData
        let boundControls: BoundControl[] = []
        const isStoredInRootLevel = storeName === 'data'
        if (!isStoredInRootLevel) {
            tempSectionData[storeName] = new Object()
        }
        (standard.controls as PageRenderedControl<DefaultControlOptions>[])?.forEach(control => {
            // Skip if it is composite control
            if(control.type === ControlType.Composite){
                return
            }

            const controlData = this.getInitDataOfControl(sectionBoundData, control.defaultOptions.bindname, control)
            let mapDataControl: MapDataControl
            if (isStoredInRootLevel) {
                const isChildCompositeControl = !!control.compositeControlId
                mapDataControl = {
                    controlFullName: isChildCompositeControl ? sectionName + '_' + control.compositeControlName + '.' + control.name : sectionName + '_' + control.name,
                    sectionMapName: null,
                    bindName: control.defaultOptions.bindname,
                    compositeBindName: isChildCompositeControl ? control.compositeControlBindName : null,
                    isCompositeControl: isChildCompositeControl
                }
                sectionsMap.push(mapDataControl)
                
                if(isChildCompositeControl){
                    if(!ObjectUtils.isNotNull(tempSectionData[control.compositeControlBindName])){
                        tempSectionData[control.compositeControlBindName] = new Object()
                        
                    }
                    tempSectionData[control.compositeControlBindName][control.defaultOptions.bindname] = controlData 
                }
                else{
                    tempSectionData[control.defaultOptions.bindname] = controlData
                }                
            }
            else {
                const isChildCompositeControl = !!control.compositeControlId
                mapDataControl = {
                    controlFullName: isChildCompositeControl ? sectionName + '_' + control.compositeControlName + '.' + control.name : sectionName + '_' + control.name,
                    sectionMapName: storeName,
                    bindName: control.defaultOptions.bindname,
                    compositeBindName: isChildCompositeControl ? control.compositeControlBindName : null,
                    isCompositeControl: isChildCompositeControl
                }
                if(isChildCompositeControl){
                    if(!ObjectUtils.isNotNull(tempSectionData[storeName][control.compositeControlBindName])){
                        tempSectionData[storeName][control.compositeControlBindName] = new Object()                        
                    }
                    tempSectionData[storeName][control.compositeControlBindName][control.defaultOptions.bindname] = controlData 
                }
                else{
                    tempSectionData[storeName][control.defaultOptions.bindname] = controlData
                }    
                
                sectionsMap.push(mapDataControl)
            }

            const validators = this.generateFormValidators(control.validators, formControls)
            const asyncValidators = this.generateFormAsyncValidators(
                control.asyncValidators,
                mapDataControl.bindName,
                mapDataControl.controlFullName,
                sectionName,
                control.name,
                controlData,
                formControls)

            if (ObjectUtils.isNotNull(extendedValidators)) {
                extendedValidators?.forEach(c => {
                    if (c.controlName === control.name) {
                        c.validators?.forEach(v => {
                            validators.push(v)
                        })
                        c.customErrorMessages?.forEach(a => {
                            control.customErrorMessages.push(a)
                        })

                        return false
                    }
                })
            }

            let formValue = this.getFormValue(control, controlData)
            if (control.defaultOptions.checkDisabled && control.type === ControlType.Select) {
                formControls[control.name] = new FormControl(formValue,
                    validators,
                    asyncValidators
                )
            }
            else {
                formControls[control.name] = new FormControl({
                    value: formValue,
                    disabled: control.defaultOptions.checkDisabled
                },
                    validators,
                    asyncValidators
                )
            }

            switch (control.type) {
                case ControlType.Select:
                case ControlType.AutoComplete:
                    boundControls.push(new SelectBoundControl(control.name, control.type, formControls[control.name]))
                    break
                case ControlType.Checkbox:
                case ControlType.Slide:
                    boundControls.push(
                        new SimpleBoundControl(
                            control.name,
                            control.type,
                            formControls[control.name],
                            formValue,
                            control.defaultOptions.allowYesNo,
                            control.defaultOptions.allowZero))
                    break
                default:
                    boundControls.push(
                        new SimpleBoundControl(
                            control.name,
                            control.type,
                            formControls[control.name],
                            formValue))
                    break
            }
        })
        if (onComplete) {
            onComplete(tempSectionData, sectionsMap)
        }

        // Changed from 0.9.0: This method will return BoundSection which is very suitable for Event-Based
        // Whatever call this, it always returns a StandardBoundSection
        return new StandardBoundSection(
            sectionName,
            boundControls,
            new FormGroup(formControls))
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
        options: ArrayStandardOptions,
        onComplete: (idField: string, ids: any[], cloneData: any) => void
    ): any[] {
        let arrayTableData = []
        let idKey = ''
        let ids: any[] = []
        let cloneData: any = new Object()
        if (arrayData.length > 0) {
            arrayData?.forEach(element => {
                let tempElementObject = new Object();
                (standard.controls as PageRenderedControl<DefaultControlOptions>[])
                    ?.forEach(control => {
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
                // Identity Field is control name, we need to find a relate control
                if (!ObjectUtils.isNotNull(options.identityfield)) {
                    tempElementObject['uniq_id'] = Guid.create().toString()
                    ids.push(tempElementObject['uniq_id'])
                    idKey = 'uniq_id'
                }
                else {
                    const foundControl = <PageRenderedControl<DefaultControlOptions>>standard.controls.find(a => a.name === options.identityfield)
                    ids.push(tempElementObject[foundControl.defaultOptions.bindname])
                    idKey = foundControl.defaultOptions.bindname
                }

                arrayTableData.push(tempElementObject)
            })

            cloneData = ObjectUtils.clone(arrayTableData[0])
            Object.keys(cloneData)?.forEach(e => {
                cloneData[e] = null
            })
        }
        else {
            // If there are no data, we need to create clone object for inserting
            (standard.controls as PageRenderedControl<DefaultControlOptions>[])
                ?.forEach(control => {
                    cloneData[control.defaultOptions.bindname] = null
                })

            if (!ObjectUtils.isNotNull(options.identityfield)) {
                cloneData['uniq_id'] = null
                idKey = 'uniq_id'
            }
            else {
                const foundControl = <PageRenderedControl<DefaultControlOptions>>standard.controls.find(a => a.name === options.identityfield)
                idKey = foundControl.defaultOptions.bindname
            }
        }


        if (ObjectUtils.isNotNull(onComplete)) {
            onComplete(idKey, ids, cloneData)
        }

        return arrayTableData
    }

    public buildTreeData(
        dataSourceName: string,
        datasources: PageLoadedDatasource[],
        treeOptions: TreeStandardOptions): any[] {

        // Cause an exception if the data isn't array, but let it be
        let data: any[] = this.getSectionBoundData(dataSourceName, datasources) as any[]

        // TODO: We need to transform data from flat -> nest because Tree 
        if (treeOptions.indatastructure === 'flat') {
            return data
        }
        return data
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
        if (!ObjectUtils.isNotNull(data)) {
            return null
        }
        let controlData = null

        // New 0.9.0: Support Child Composite Control
        if(ObjectUtils.isNotNull(control.compositeControlRefId) 
            && control.type !== ControlType.Composite){
            if(!!data[control.compositeControlBindName]){
                controlData = data[control.compositeControlBindName][control.defaultOptions.bindname]
            }        
        }
        else if (controlBindName === 'id' || controlBindName === '_id') {
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

        // Set default value if it is happened
        if (!ObjectUtils.isNotNull(controlData) && ObjectUtils.isNotNull(control.defaultOptions.defaultvalue)) {
            controlData = control.defaultOptions.defaultvalue
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
                    catch {
                        controlData = [controlData]
                    }
                }
            }
            else {
                controlData = []
            }
        }
        else if (control.type == ControlType.Checkbox || control.type == ControlType.Slide) {
            if (ObjectUtils.isNotNull(controlData)) {
                if (controlData.toString() == '0' || controlData.toString() == '1') {
                    control.defaultOptions.allowZero = true
                }
                else if (controlData == 'Y' || controlData == 'N') {
                    control.defaultOptions.allowYesNo = true
                }
            }
            else {
                controlData = false
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
        validators?.forEach((validator: PageControlValidator) => {
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


        validators?.forEach(validator => {
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