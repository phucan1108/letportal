import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ExtendedFormValidator, ExtendedPageControl } from 'app/core/models/extended.models';
import { ControlOptions } from 'app/core/models/page.model';
import { DateUtils } from 'app/core/utils/date-util';
import { NGXLogger } from 'ngx-logger';
import { StaticResources } from 'portal/resources/static-resources';
import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CompositeControl, CompositeControlsClient, ControlType, EventActionType, PageControl, PageControlEvent, ValidatorType } from 'services/portal.service';
import { ControlsGridComponent } from './controls-grid.component';
 

@Component({
    selector: 'let-control-dialog',
    templateUrl: './control-dialog.component.html',
    styleUrls: ['./control-dialog.component.scss']
})
export class ControlDialogComponent implements OnInit {

    displayedValidatorListColumns = ['isActive', 'displayName', 'validatorMessage', 'validatorOption']
    controlForm: FormGroup
    currentExtendedFormControl: ExtendedPageControl
    isEditMode = false;

    _controlTypes = StaticResources.controlTypes()
    currentControlType: ControlType
    controlType = ControlType
    validators$: BehaviorSubject<Array<ExtendedFormValidator>> = new BehaviorSubject([])
    validators: Array<ExtendedFormValidator> = []

    compositeControls$: Observable<CompositeControl[]>

    validatorTypes = StaticResources.formValidatorTypes();

    shellOptions: ExtendedShellOption[] = []
    shellOptions$: BehaviorSubject<ExtendedShellOption[]> = new BehaviorSubject([])
    isHandset = false
    names: string[] = []
    constructor(
        public dialogRef: MatDialogRef<ControlsGridComponent>,
        public dialog: MatDialog,
        private breakpointObserver: BreakpointObserver,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef,
        private logger: NGXLogger,
        private compositeControlsClient: CompositeControlsClient
    ) {
        this.breakpointObserver.observe(Breakpoints.Handset)
            .pipe(
                tap(result => {
                    if (result.matches) {
                        this.isHandset = true
                        this.cd.markForCheck()
                    }
                    else {
                        this.isHandset = false
                        this.cd.markForCheck()
                    }
                })
            ).subscribe()
    }

    ngOnInit(): void {
        this.currentExtendedFormControl = this.data.control
        this.logger.debug('Passing control', this.currentExtendedFormControl)
        this.names = this.data.names
        this.isEditMode = this.currentExtendedFormControl.name ? true : false
        this.shellOptions = this.generateShellOptions(this.currentExtendedFormControl, true)
        this.shellOptions$.next(this.shellOptions)
        this.currentControlType = this.currentExtendedFormControl.type
        this.compositeControls$ = this.compositeControlsClient.getAll('')
        this.initialControlForm()
        this.populatedFormValues()
        this.convertValidatorTypeToFormValidator(this.currentExtendedFormControl.type)
    }

    private convertValidatorTypeToFormValidator(type: ControlType) {
        const formValidators: Array<ExtendedFormValidator> = []
        const allowedValidatorTypes = this.getValidatorsByControlType(type)
        this.validatorTypes?.forEach(validator => {
            if (allowedValidatorTypes.indexOf(validator.value) > -1) {
                let validatorForm;
                if (this.isEditMode) {
                    const foundValidatorForm = this.currentExtendedFormControl.validators.find(validatorTemp => validatorTemp.validatorType === validator.value)
                    validatorForm = {
                        validatorType: foundValidatorForm ? foundValidatorForm.validatorType : validator.value,
                        displayName: validator.name,
                        isActive: foundValidatorForm ? foundValidatorForm.isActive : false,
                        validatorOption: foundValidatorForm ? foundValidatorForm.validatorOption : '',
                        validatorOptionPlaceholder: '',
                        validatorMessage: foundValidatorForm ? foundValidatorForm.validatorMessage : '',
                        hideOption: this.getHideOption(foundValidatorForm ? foundValidatorForm.validatorType : validator.value)
                    };
                }
                else {
                    validatorForm = {
                        validatorType: validator.value,
                        displayName: validator.name,
                        isActive: false,
                        validatorOption: '',
                        validatorOptionPlaceholder: '',
                        validatorMessage: '',
                        hideOption: this.getHideOption(validator.value)
                    };
                }
                switch (validatorForm.validatorType) {
                    case ValidatorType.Required:
                        validatorForm.validatorMessage =
                            validatorForm.validatorMessage ? validatorForm.validatorMessage : this.generateRequiredMessage(this.currentControlType)
                        break
                    case ValidatorType.MinLength:
                        validatorForm.validatorMessage = validatorForm.validatorMessage ? validatorForm.validatorMessage : 'This field requires at least {{option}} characters'
                        validatorForm.validatorOptionPlaceholder = 'Input minimum length'
                        break
                    case ValidatorType.MaxLength:
                        validatorForm.validatorMessage = validatorForm.validatorMessage ? validatorForm.validatorMessage : 'This field requires maximum {{option}} characters'
                        validatorForm.validatorOptionPlaceholder = 'Input maximum length'
                        break
                    case ValidatorType.Regex:
                        validatorForm.validatorMessage = validatorForm.validatorMessage ? validatorForm.validatorMessage : 'This field\'s format does not match'
                        validatorForm.validatorOptionPlaceholder = 'Input regex pattern'
                        break
                    case ValidatorType.Equal:
                        validatorForm.validatorMessage = validatorForm.validatorMessage ? validatorForm.validatorMessage : 'This field must be equal {{option}}'
                        validatorForm.validatorOptionPlaceholder = 'Input matching value'
                        break
                    case ValidatorType.Email:
                        validatorForm.validatorMessage = validatorForm.validatorMessage ? validatorForm.validatorMessage : 'Please input correct email'
                        break
                    case ValidatorType.EqualTo:
                        validatorForm.validatorMessage = validatorForm.validatorMessage ? validatorForm.validatorMessage : 'This field doesn\'t match with {{option}} field'
                        validatorForm.validatorOptionPlaceholder = 'Input matching field'
                        break
                    case ValidatorType.Number:
                        validatorForm.validatorMessage = validatorForm.validatorMessage ? validatorForm.validatorMessage : 'Please input correct number format'
                        break
                    case ValidatorType.NumberRange:
                        validatorForm.validatorMessage = validatorForm.validatorMessage ? validatorForm.validatorMessage : 'This field must be in range {{option}}'
                        validatorForm.validatorOption = validatorForm.validatorOption ? validatorForm.validatorOption : '[-999999999,999999999]'
                        break
                    case ValidatorType.Json:
                        validatorForm.validatorMessage = validatorForm.validatorMessage ? validatorForm.validatorMessage : 'Please input correct json format'
                        break
                    case ValidatorType.DateTime:
                        validatorForm.validatorMessage = validatorForm.validatorMessage ? validatorForm.validatorMessage : 'Please input correct datetime {{option}} format'
                        validatorForm.validatorOptionPlaceholder = 'Input datetime format. Default: mm/dd/yyyy HH:MM:SS'
                        validatorForm.validatorOption = validatorForm.validatorOption ? validatorForm.validatorOption : 'mm/dd/yyyy HH:MM:SS'
                        break
                    case ValidatorType.MinDate:
                        validatorForm.validatorMessage = validatorForm.validatorMessage ? validatorForm.validatorMessage : 'This field must be greater than {{option}}'
                        validatorForm.validatorOptionPlaceholder = 'Input min date'
                        validatorForm.validatorOption = validatorForm.validatorOption ? validatorForm.validatorOption : '01/01/1970 00:00:00'
                        break;
                    case ValidatorType.MaxDate:
                        validatorForm.validatorMessage = validatorForm.validatorMessage ? validatorForm.validatorMessage : 'This field must be less than {{option}}'
                        validatorForm.validatorOptionPlaceholder = 'Input max date'
                        validatorForm.validatorOption = validatorForm.validatorOption ? validatorForm.validatorOption : DateUtils.toDateMMDDYYYYString(new Date())
                        break;
                    case ValidatorType.FileExtensions:
                        validatorForm.validatorMessage = validatorForm.validatorMessage ? validatorForm.validatorMessage : 'File must have an extension in {{option}}'
                        validatorForm.validatorOptionPlaceholder = 'Input file extensions'
                        validatorForm.validatorOption = validatorForm.validatorOption ? validatorForm.validatorOption : 'jpg;jpeg;png;gif'
                        break
                    case ValidatorType.FileMaximumFiles:
                        validatorForm.validatorMessage = validatorForm.validatorMessage ? validatorForm.validatorMessage : 'Number of files must be less than {{option}}'
                        validatorForm.validatorOptionPlaceholder = 'Input number of files'
                        validatorForm.validatorOption = validatorForm.validatorOption ? validatorForm.validatorOption : '5'
                        break
                    case ValidatorType.FileMaximumSize:
                        validatorForm.validatorMessage = validatorForm.validatorMessage ? validatorForm.validatorMessage : 'File must have size less than {{option}} Mb'
                        validatorForm.validatorOptionPlaceholder = 'Input file size (in Mb)'
                        validatorForm.validatorOption = validatorForm.validatorOption ? validatorForm.validatorOption : '10'
                        break
                }

                formValidators.push(validatorForm)
            }
        })

        this.validators = formValidators
        this.validators$.next(this.validators)
    }

    picked($event) {
        this.logger.debug('Current Validators', this.validators)
    }

    trackByValidatorIndex(index, validator) {
        return validator.validatorType
    }

    initialControlForm() {
        this.controlForm = this.fb.group({
            key: [this.currentExtendedFormControl.name],
            name: [
                this.currentExtendedFormControl.name, 
                [
                    Validators.pattern('^[a-zA-Z]+'), 
                    Validators.required, 
                    Validators.maxLength(100), 
                    // 0.9.0: Because we allow 'rendered' mode so that we accept the case is duplicate name
                    //FormUtil.isExist(this.names, this.currentExtendedFormControl.name)
                ]],
            controlType: [this.currentExtendedFormControl.type],
            compositeControl: [this.currentExtendedFormControl.compositeControlId]
        })
    }

    populatedFormValues() {
        this.controlForm.get('controlType').valueChanges.subscribe(newValue => {
            this.currentExtendedFormControl.type = newValue
            this.currentControlType = newValue
            if (this.currentExtendedFormControl.type === ControlType.LineBreaker) {
                this.controlForm.get('name').setValue('lineBreaker')
            }
            this.shellOptions = this.generateShellOptions(this.currentExtendedFormControl)
            this.shellOptions$.next(this.shellOptions)
            this.currentExtendedFormControl.pageControlEvents = this.generateEventsList(this.currentExtendedFormControl)
            this.convertValidatorTypeToFormValidator(newValue)
        })

        this.controlForm.get('name').valueChanges.subscribe(newValue => {
            const listNameValue = (newValue as string).toLowerCase().replace(/\s/g, '')
            this.controlForm.get('key').setValue(listNameValue)
        })
    }

    onJsonEditorHtmlAttributeChange($event) {
        this.controlForm.get('htmlAttributes').setValue($event)
    }

    combiningControl(): ExtendedPageControl {
        this.logger.debug('Current validators', this.validators)
        const formValues = this.controlForm.value
        const combiningControl: ExtendedPageControl = {
            id: this.currentExtendedFormControl.id,
            name: formValues.name,
            type: formValues.controlType,
            order: this.currentExtendedFormControl.order,
            readOnlyMode: this.currentExtendedFormControl.readOnlyMode,
            validators: this.validators,
            options: this.shellOptions,
            value: '',
            isActive: this.currentExtendedFormControl.isActive,
            datasourceOptions: this.currentExtendedFormControl.datasourceOptions,
            pageControlEvents: this.currentExtendedFormControl.pageControlEvents,
            compositeControlId: formValues.controlType === ControlType.Composite ? formValues.compositeControl : null
        }
        return combiningControl
    }

    onSubmittingControl() {
        if (this.controlForm.valid) {
            this.dialogRef.close(this.combiningControl())
        }
    }

    onChangingOptions($event) {
        this.shellOptions = $event
        this.logger.debug('current control options', this.shellOptions)
    }

    isCompositeControl(){
        return this.controlForm.get('controlType').value === ControlType.Composite
    }

    private generateEventsList(control: PageControl): PageControlEvent[] {
        switch (control.type) {
            case ControlType.Label:
            case ControlType.LineBreaker:
            case ControlType.Composite:
                return []
            case ControlType.AutoComplete:
                return [
                    {
                        eventName: `${control.name}_change`, eventActionType: EventActionType.TriggerEvent, triggerEventOptions: { eventsList: [] },
                        eventHttpServiceOptions: {
                            httpServiceUrl: '',
                            httpMethod: 'Get',
                            boundData: [],
                            httpSuccessCode: '200',
                            jsonBody: '',
                            outputProjection: ''
                        },
                        eventDatabaseOptions: {
                            databaseConnectionId: '',
                            entityName: '',
                            outputProjection: '',
                            boundData: [],
                            query: ''
                        }
                    }
                ]
            default:
                return [
                    {
                        eventName: `${control.name}_change`, eventActionType: EventActionType.TriggerEvent, triggerEventOptions: { eventsList: [] },
                        eventHttpServiceOptions: {
                            httpServiceUrl: '',
                            httpMethod: 'Get',
                            boundData: [],
                            httpSuccessCode: '200',
                            jsonBody: '',
                            outputProjection: ''
                        },
                        eventDatabaseOptions: {
                            databaseConnectionId: '',
                            entityName: '',
                            outputProjection: '',
                            boundData: [],
                            query: ''
                        }
                    }
                ]
        }
    }

    private generateShellOptions(control: PageControl, isOnLoad: boolean = false): ExtendedShellOption[] {
        let defaultOptions: ExtendedShellOption[] = ControlOptions.getDefaultControlOptions(control)
        switch (control.type) {
            case ControlType.Select:
            case ControlType.AutoComplete:
                defaultOptions.push(ControlOptions.MultipleOptions)
                break
            case ControlType.Textarea:
                defaultOptions.push(ControlOptions.TextareaRowsOption)
                break
            case ControlType.LineBreaker:
                defaultOptions = []
                break
            case ControlType.Uploader:
            case ControlType.MultiUploader:
                defaultOptions.push(ControlOptions.UploaderAllowFileUrl)
                defaultOptions.push(ControlOptions.UploaderSaveOnChange)
                break
            default:
                break
        }
        if (!!control.options && isOnLoad) {
            defaultOptions?.forEach(opt => {
                const found = control.options.find(controlOpt => controlOpt.key === opt.key)
                if (!!found)
                    opt.value = found.value
            })
        }

        return defaultOptions

    }

    private getHideOption(type: ValidatorType) {
        switch (type) {
            case ValidatorType.Required:
            case ValidatorType.Email:
            case ValidatorType.Json:
            case ValidatorType.Number:
                return true
            default:
                return false
        }
    }


    private getValidatorsByControlType(type: ControlType): ValidatorType[] {
        switch (type) {
            case ControlType.Label:
            case ControlType.LineBreaker:
                return []
            case ControlType.Composite:
                return []
            case ControlType.Textbox:
            case ControlType.Textarea:
                return [
                    ValidatorType.Required,
                    ValidatorType.Regex,
                    ValidatorType.Email,
                    ValidatorType.Equal,
                    ValidatorType.EqualTo,
                    ValidatorType.MaxLength,
                    ValidatorType.MinLength,
                    ValidatorType.Regex
                ]
            case ControlType.Select:
                return [
                    ValidatorType.Required
                ]
            case ControlType.AutoComplete:
                return [
                    ValidatorType.Required
                ]
            case ControlType.DateTime:
                return [
                    ValidatorType.Required,
                    ValidatorType.Equal,
                    ValidatorType.EqualTo,
                    ValidatorType.MinDate,
                    ValidatorType.MaxDate
                ]
            case ControlType.Number:
                return [
                    ValidatorType.Required,
                    ValidatorType.Number,
                    ValidatorType.NumberRange
                ]
            case ControlType.Uploader:
                this.logger.debug('Choose upload')
                return [
                    ValidatorType.Required,
                    ValidatorType.FileExtensions,
                    ValidatorType.FileMaximumSize
                ]
            case ControlType.MultiUploader:
                return [
                    ValidatorType.Required,
                    ValidatorType.FileExtensions,
                    ValidatorType.FileMaximumSize,
                    ValidatorType.FileMaximumFiles
                ]
            case ControlType.Email:
                return [
                    ValidatorType.Required,
                    ValidatorType.MaxLength,
                    ValidatorType.MinLength,
                    ValidatorType.Equal,
                    ValidatorType.EqualTo,
                    ValidatorType.Regex,
                    ValidatorType.Email
                ]
            case ControlType.Password:
                return [
                    ValidatorType.Required,
                    ValidatorType.MaxLength,
                    ValidatorType.MinLength,
                    ValidatorType.Equal,
                    ValidatorType.EqualTo,
                    ValidatorType.Regex
                ]
            default:
                return [
                    ValidatorType.Required
                ]
        }
    }

    private generateRequiredMessage(controlType: ControlType){
        switch(controlType){
            case ControlType.Uploader:
                return 'Please upload one file'
            case ControlType.MultiUploader:
                return 'Please upload at least one file'
            case ControlType.Select:
            case ControlType.AutoComplete:
            case ControlType.Radio:
            case ControlType.IconPicker:
                return 'Please select one option'
            case ControlType.DateTime:
                return 'Please choose one date'
            default:
                return 'Please fill out this field'
        }
    }
}
