import { Component, OnInit, Inject, ChangeDetectorRef, ViewChild } from '@angular/core';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ControlsGridComponent } from './controls-grid.component';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NGXLogger } from 'ngx-logger';
import { PageControlAsyncValidator, PageControl, AsyncValidatorType, HttpServiceOptions, SharedDatabaseOptions } from 'services/portal.service';
import { BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ExtendedPageControl } from 'app/core/models/extended.models';
import { FormUtil } from 'app/core/utils/form-util';
import { PortalValidators } from 'app/core/validators/portal.validators';
import { StaticResources } from 'portal/resources/static-resources';
import { DatabaseFormOptions, DatabaseOptionsComponent } from 'portal/shared/databaseoptions/databaseoptions.component';
import { ObjectUtils } from 'app/core/utils/object-util';

@Component({
    selector: 'let-async-validators',
    templateUrl: './control-async-validator.dialog.component.html'
})
export class AsyncValidatorDialogComponent implements OnInit {
    control: ExtendedPageControl
    displayedColumns = ['validatorName', 'validatorMessage', 'actions']
    validators$: BehaviorSubject<PageControlAsyncValidator[]> = new BehaviorSubject([])
    validators: PageControlAsyncValidator[] = []
    isHandset = false
    isShowEditForm = false

    asyncValidatorForm: FormGroup
    validatorTypes = StaticResources.asyncValidatorTypes()
    validatorType = AsyncValidatorType
    isHttpOptionsValid = false
    httpOptions: HttpServiceOptions
    databaseOptions: SharedDatabaseOptions
    dbOptions: DatabaseFormOptions = {
        allowHints: ['query']
    }

    currentValidatorType: AsyncValidatorType

    selectedValidator: PageControlAsyncValidator

    isEditMode = false

    @ViewChild(DatabaseOptionsComponent, { static: false }) dbOptionsComponent: DatabaseOptionsComponent

    constructor(
        public dialogRef: MatDialogRef<ControlsGridComponent>,
        public dialog: MatDialog,
        private breakpointObserver: BreakpointObserver,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef,
        private logger: NGXLogger
    ) {
        this.control = this.data.control
        this.breakpointObserver.observe([Breakpoints.HandsetLandscape, Breakpoints.HandsetPortrait])
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
        if (this.control.asyncValidators) {
            this.validators = this.control.asyncValidators
            this.validators$.next(this.validators)
        }

        this.initAsyncValidatorForm()
    }

    initAsyncValidatorForm() {
        this.asyncValidatorForm = this.fb.group({
            validatorName: ['', [Validators.pattern('^[a-zA-Z]+'), Validators.required, Validators.maxLength(100), Validators.required, FormUtil.isExist(this.getAvailableValidatorNames(), '')]],
            isActive: [true],
            validatorMessage: ['', Validators.required],
            validatorType: [AsyncValidatorType.DatabaseValidator, Validators.required],
            evaluatedExpression: ['', Validators.required]
        })

        this.currentValidatorType = AsyncValidatorType.DatabaseValidator
    }

    getAvailableValidatorNames() {
        const names = []
        this.validators?.forEach(validator => {
            names.push(validator.validatorName)
        })

        return names
    }

    addValidator() {
        this.asyncValidatorForm.clearValidators()
        this.asyncValidatorForm.reset()
        this.selectedValidator = {
            validatorName: '',
            isActive: true,
            validatorMessage: '',
            asyncValidatorOptions: {
                validatorType: AsyncValidatorType.DatabaseValidator,
                databaseOptions: {
                    databaseConnectionId: '',
                    entityName: '',
                    query: ''
                },
                httpServiceOptions: {
                    httpMethod: 'Get',
                    httpServiceUrl: '',
                    httpSuccessCode: '',
                    jsonBody: '',
                    outputProjection: ''
                },
                evaluatedExpression: ''
            }
        }

        this.reinitSelectedValidator()
        this.isShowEditForm = true
        this.isEditMode = false
    }

    editValidator(validator: PageControlAsyncValidator){
        this.asyncValidatorForm.clearValidators()
        this.asyncValidatorForm.reset()
        this.selectedValidator = validator
        this.reinitSelectedValidator()
        this.isShowEditForm = true
        this.isEditMode = true
    }

    reinitSelectedValidator() {
        if(!ObjectUtils.isNotNull(this.selectedValidator.asyncValidatorOptions.httpServiceOptions)){
            this.selectedValidator.asyncValidatorOptions.httpServiceOptions = {
                httpMethod: 'Get',
                httpServiceUrl: '',
                httpSuccessCode: '200',
                jsonBody: '',
                outputProjection: ''
            }
        }

        if(!ObjectUtils.isNotNull(this.selectedValidator.asyncValidatorOptions.databaseOptions)){
            this.selectedValidator.asyncValidatorOptions.databaseOptions = {
                databaseConnectionId: '',
                entityName: '',
                query: ''
            }
        }

        this.asyncValidatorForm = this.fb.group({
            validatorName: [this.selectedValidator.validatorName, [Validators.pattern('^[a-zA-Z]+'), Validators.required, Validators.maxLength(100), Validators.required, FormUtil.isExist(this.getAvailableValidatorNames(), this.selectedValidator.validatorName)]],
            isActive: [this.selectedValidator.isActive],
            validatorMessage: [this.selectedValidator.validatorMessage, Validators.required],
            validatorType: [this.selectedValidator.asyncValidatorOptions.validatorType, Validators.required],
            evaluatedExpression: [this.selectedValidator.asyncValidatorOptions.evaluatedExpression, Validators.required]
        })

        this.httpOptions = {
            httpServiceUrl: this.selectedValidator.asyncValidatorOptions.httpServiceOptions.httpServiceUrl,
            httpMethod: this.selectedValidator.asyncValidatorOptions.httpServiceOptions.httpMethod,
            httpSuccessCode: this.selectedValidator.asyncValidatorOptions.httpServiceOptions.httpSuccessCode,
            jsonBody: this.selectedValidator.asyncValidatorOptions.httpServiceOptions.jsonBody,
            outputProjection: this.selectedValidator.asyncValidatorOptions.httpServiceOptions.outputProjection
        }

        this.databaseOptions = {
            databaseConnectionId: this.selectedValidator.asyncValidatorOptions.databaseOptions.databaseConnectionId,
            entityName: this.selectedValidator.asyncValidatorOptions.databaseOptions.entityName,
            query: this.selectedValidator.asyncValidatorOptions.databaseOptions.query
        }
    }

    onSave() {
        this.dialogRef.close(this.validators)
    }

    cancelEdit() {
        this.isShowEditForm = false
    }

    saveValidator() {
        this.currentValidatorType = this.asyncValidatorForm.get('validatorType').value
        let validator: PageControlAsyncValidator
        if(this.currentValidatorType == AsyncValidatorType.DatabaseValidator){
            if(this.dbOptionsComponent.isValid() && this.asyncValidatorForm.valid){
                validator = this.combineAsyncValidator()
                this.saveValidatorToList(validator)
            }
        }
        else{
            if(this.isHttpOptionsValid && this.asyncValidatorForm.valid){
                validator = this.combineAsyncValidator()
                this.saveValidatorToList(validator)
            }
        }
    }

    private saveValidatorToList(validator: PageControlAsyncValidator){

        if(this.isEditMode){
            const selectedIndex = this.validators.indexOf(this.selectedValidator)
            this.validators[selectedIndex] = validator
        }
        else{
            this.validators.push(validator)
        }

        this.validators$.next(this.validators)
        this.isShowEditForm = false
    }

    combineAsyncValidator(): PageControlAsyncValidator{
        const formValues = this.asyncValidatorForm.value
        return {
            validatorName: formValues.validatorName,
            isActive: formValues.isActive,
            validatorMessage: formValues.validatorMessage,
            asyncValidatorOptions:{
                validatorType: this.currentValidatorType,
                evaluatedExpression: formValues.evaluatedExpression,
                databaseOptions: this.currentValidatorType == AsyncValidatorType.DatabaseValidator ? this.dbOptionsComponent.get() : { },
                httpServiceOptions: this.currentValidatorType == AsyncValidatorType.HttpValidator ? this.httpOptions : {}
            }
        }
    }

    onChangeHttpOptions($event) {
        this.httpOptions = $event
    }

    onCheckingHttpOptionsValid($event) {
        this.isHttpOptionsValid = $event
    }
}
