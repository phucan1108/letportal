import { Component, OnInit, Input, OnDestroy, ChangeDetectorRef, Optional, ViewChild, AfterViewInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { StaticResources } from 'portal/resources/static-resources';
import { ExtendedFormValidator, ExtendedControlValidator, ExtendedPageSection } from '../../../../../core/models/extended.models';
import * as _ from 'lodash';
import { ObjectUtils } from 'app/core/utils/object-util';
import { PageControl, ControlType, ValidatorType, DatasourceControlType, EventActionType, PageControlAsyncValidator, FilesClient } from 'services/portal.service';
import { PageRenderedControl, DefaultControlOptions } from 'app/core/models/page.model';
import { Observable, of, Subscription, BehaviorSubject } from 'rxjs';
import { Store } from '@ngxs/store';
import { PageStateModel } from 'stores/pages/page.state';
import { filter, tap, debounceTime, distinctUntilChanged, startWith, map } from 'rxjs/operators';
import { ChangeControlValueEvent } from 'stores/pages/page.actions';
import { PageControlEventStateModel } from 'stores/pages/pagecontrol.state';
import { PageService } from 'services/page.service';
import { NGXLogger } from 'ngx-logger';
import { EventsProvider } from 'app/core/events/event.provider';
import { UploadFileService } from 'services/uploadfile.service';
import { MarkdownService } from 'ngx-markdown';
import { MultipleDataSelection } from 'portal/modules/models/control.extended.model';
import { AutocompleteMultipleComponent } from './autocomplete-multiple.component';
import { CustomHttpService } from 'services/customhttp.service';
import { MatAutocompleteSelectedEvent } from '@angular/material';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';

@Component({
    selector: 'let-general-control',
    templateUrl: './general-control.component.html',
    styleUrls: ['./general-control.component.scss']
})
export class GeneralControlComponent implements OnInit, OnDestroy, AfterViewInit {

    @Input()
    formGroup: FormGroup

    @Input()
    control: PageRenderedControl<DefaultControlOptions>

    @Input()
    section: ExtendedPageSection

    @ViewChild(AutocompleteMultipleComponent, { static: false }) autoComplete: AutocompleteMultipleComponent

    controlFullName: string
    controlType = ControlType
    maxLength = -1
    validators: Array<ExtendedControlValidator> = []
    validatorTypes = StaticResources.formValidatorTypes();

    formControlLink: FormControl = new FormControl()

    optionsList$: BehaviorSubject<any> = new BehaviorSubject([])
    optionsList: any[] = []
    autoOptionsList: Array<MultipleDataSelection> = []
    autoOptionsList$: BehaviorSubject<Array<MultipleDataSelection>> = new BehaviorSubject([])
    defaultData: any
    pageControlState$: Observable<PageControlEventStateModel>
    controlEventSubscription: Subscription

    asyncValidators: PageControlAsyncValidator[] = []
    currentAsyncErrorName: string
    hasAsyncInvalid: boolean = false

    sectionName: string = ''

    // Check full documentation of Markdown Editor: https://github.com/ghiscoding/angular-markdown-editor
    markdownContent: string
    // see full markdown options: https://github.com/ghiscoding/angular-markdown-editor/blob/master/src/lib/angular-markdown-editor/global-editor-options.ts
    markdownOptions = {
        iconlibrary: 'fa',
        fullscreen: {
            enable: false,
            icons: null
        },
        onChange: (e: any) => this.mardownChanged(e),
        parser: (val) => this.markdownService.compile(val.trim())
    }
    markdownValue$ = new BehaviorSubject<string>('');
    constructor(
        @Optional() private eventsProvider: EventsProvider,
        private pageService: PageService,
        private markdownService: MarkdownService,
        private shortcutUtil: ShortcutUtil,
        private logger: NGXLogger,
        private cd: ChangeDetectorRef
    ) {
    }

    ngOnInit(): void {
        this.asyncValidators = this.control.asyncValidators ? this.control.asyncValidators : []
        this.controlFullName = this.section.name + '_' + this.control.name
        this.sectionName = this.section.name
        this.controlEventSubscription = this.pageService.listenTriggeringControlEvent$().pipe(
            filter(state => state && (state.controlFullName === this.controlFullName)),
            tap(
                state => {
                    let tempData = ObjectUtils.clone(state.data)
                    // Implement some function events there based on control type
                    switch (state.eventType) {
                        case 'resetdatasource':
                            // Only for Auto and Select
                            if (this.control.type === ControlType.Select
                                || this.control.type === ControlType.AutoComplete
                                || this.control.type === ControlType.Radio) {
                                this.generateOptions()
                                    .pipe(
                                        tap(
                                            res => {
                                                this.optionsList$.next(res)
                                            }
                                        )
                                    ).subscribe()
                            }
                            break
                        case 'hasAsyncError':
                            this.hasAsyncInvalid = true
                            this.currentAsyncErrorName = state.data
                            break
                        case 'noAsyncError':
                            this.hasAsyncInvalid = false
                            break
                        default:
                            const eventExecution = this.eventsProvider.getEvent(state.eventType)
                            if (eventExecution != null) {
                                eventExecution
                                    .execute(
                                        this.control,
                                        this.pageService,
                                        <FormControl>this.formGroup.get(this.control.name),
                                        this.defaultData,
                                        ObjectUtils.isNotNull(tempData) ?
                                            tempData : this.pageService.getDataByBindName(this.control.defaultOptions.bindname))
                            }
                            break
                    }
                }
            )
        ).subscribe()

        this.generateValidators()
        this.maxLength = this.getMaxLength()
        this.defaultData = this.formGroup.get(this.control.name).value
        if (this.control.type === ControlType.Select
            || this.control.type === ControlType.AutoComplete
            || this.control.type === ControlType.Radio) {

            // Important note: to prevent many unexpected data between multiple and single mode
            // We need to check a defaultData for ensuring this is matching with Single/Multiple mode
            if (ObjectUtils.isArray(this.defaultData) && !this.control.defaultOptions.multiple) {
                this.defaultData = this.defaultData[0]
            }

            this.generateOptions()
                .pipe(
                    tap(
                        res => {
                            this.optionsList$.next(res)
                        }
                    )
                ).subscribe() 
                
                this.optionsList$
                .pipe(
                    tap(
                        res => {
                            if(!ObjectUtils.isNotNull(res) || res.length === 0){
                                return
                            }
                            if (this.control.type === ControlType.AutoComplete) {
                                // Available only for AutoComplete and Multiple mode
                                let arrayData: string[] = ObjectUtils.isArray(this.defaultData) ? this.defaultData : [this.defaultData]
                                this.optionsList$.subscribe(res => {                    
                                    res.forEach(elem => {
                                        this.autoOptionsList.push({
                                            name: elem.name,
                                            value: elem.value,
                                            selected: arrayData.some(a => elem.value === a)
                                        })
                                    })
                                    // Set default option for AutoComplete/Single
                                    if (!this.control.defaultOptions.multiple) {
                                        const defaultSelect = this.autoOptionsList.find(a => a.value === this.defaultData)
                                        this.formControlLink.setValue(defaultSelect ? defaultSelect : null)
                                        this.autoOptionsList$.next(this.autoOptionsList)
                                    }
                                })
                            }
                        }
                    )
                ).subscribe()  
        }

        if (this.control.type === ControlType.MarkdownEditor) {
            this.markdownContent = this.defaultData
            this.markdownValue$.pipe(
                debounceTime(500),
                distinctUntilChanged(),
                tap(
                    res => {
                        this.setControlValue(res)
                    }
                )
            ).subscribe()
        }
        this.notifyControlValueChange()
    }

    ngAfterViewInit(): void {
        if (this.control.type === ControlType.AutoComplete && this.control.defaultOptions.multiple) {
            setTimeout(() => {
                this.autoComplete.dropdownList = this.autoOptionsList
            }, 500)
        }
    }

    ngOnDestroy(): void {
        this.controlEventSubscription.unsubscribe()
    }

    notifyControlValueChange() {
        this.formGroup.get(this.control.name).valueChanges.pipe(
            debounceTime(500),
            distinctUntilChanged(),
            tap(
                newValue => {
                    this.hasAsyncInvalid = this.isInvalidAsync()
                    if (this.control.type == ControlType.Checkbox
                        || this.control.type == ControlType.Slide) {
                        if (this.control.defaultOptions.allowZero) {
                            this.pageService.changeControlValue(this.controlFullName, newValue ? 1 : 0)
                        }
                        else if (this.control.defaultOptions.allowYesNo) {
                            this.pageService.changeControlValue(this.controlFullName, newValue ? 'Y' : 'N')
                        }
                        else {
                            this.pageService.changeControlValue(this.controlFullName, newValue)
                        }
                    }
                    else {
                        this.pageService.changeControlValue(this.controlFullName, newValue)
                    }
                    // Check with chaining events must be notified
                    _.forEach(this.control.pageControlEvents, event => {
                        switch (event.eventActionType) {
                            case EventActionType.TriggerEvent:
                                _.forEach(event.triggerEventOptions.eventsList, eventOpt => {
                                    this.pageService.notifyTriggeringEvent(this.section.name + '_' + eventOpt)
                                })
                                break
                            case EventActionType.QueryDatabase:
                                this.pageService.executeActionEventOnDatabase(event, this.section.name, this.control.name)
                                    .pipe(
                                        tap(
                                            res => {
                                                this.notifyChangedByActionEvent(event.eventDatabaseOptions.boundData, res)
                                            },
                                            err => {
                                                this.shortcutUtil.toastMessage('Oops! Something went wrong, please try again.', ToastType.Error)
                                            }
                                        )
                                    ).subscribe()
                                break
                            case EventActionType.WebService:
                                this.pageService.executeActionEventOnWebService(event)
                                    .pipe(
                                        tap(
                                            res => {
                                                this.notifyChangedByActionEvent(event.eventHttpServiceOptions.boundData, res)
                                            },
                                            err => {
                                                this.shortcutUtil.toastMessage('Oops! Something went wrong, please try again.', ToastType.Error)
                                            }
                                        )
                                    )
                                break
                        }
                    })
                }
            )
        ).subscribe()

        // Only for AutoComplete with single mode
        if (this.control.type == ControlType.AutoComplete
            && !this.control.defaultOptions.multiple) {

            this.formControlLink.valueChanges.pipe(
                startWith(''),
                map(value => typeof value === 'string' ? value : value.name),
                tap(
                    res => {
                        // For one selection mode
                        const keyword = res.toLowerCase();
                        const filters = this.autoOptionsList.filter(a => a.name.toLowerCase().includes(keyword))
                        this.autoOptionsList$.next(filters)
                    }
                )
            ).subscribe()
        }
    }

    setControlValue(value: any) {
        this.formGroup.get(this.control.name).markAsTouched()
        this.formGroup.get(this.control.name).setValue(value)
    }

    generateOptions(): Observable<any> {
        switch (this.control.datasourceOptions.type) {
            case DatasourceControlType.StaticResource:
                return of(JSON.parse(this.control.datasourceOptions.datasourceStaticOptions.jsonResource))                
            case DatasourceControlType.Database:
                const parameters = this.pageService.retrieveParameters(this.control.datasourceOptions.databaseOptions.query)
                return this.pageService
                    .fetchControlSelectionDatasource(this.sectionName, this.control.name, parameters)               
            case DatasourceControlType.WebService:                
                return this.pageService.executeHttpWithBoundData(this.control.datasourceOptions.httpServiceOptions)
        }
    }

    displayFn(option: MultipleDataSelection): string {
        return option && option.name ? option.name : '';
    }

    onAutoCompleteSelected($event: MatAutocompleteSelectedEvent) {
        // only for AutoComplete with single mode
        const found = this.autoOptionsList.find(a => a.value === $event.option.value)
        this.formControlLink.setValue(found)
        this.setControlValue(found.value)
    }

    onAutoChanged() {
        // only for AutoComplett with multiple mode
        let selected = this.autoOptionsList.filter(a => a.selected)
        this.logger.debug('Current selected values', selected)
        this.setControlValue(selected.length > 0 ? selected.map(b => b.value) : null)
    }

    isInvalid(validatorName: string): boolean {
        const invalid = this.formGroup.get(this.control.name).hasError(validatorName)
        return invalid
    }

    getErrorMessage(validatorName: string) {
        const errorMessage = _.find(this.validators, validator => validator.validatorName === validatorName).validatorErrorMessage
        return errorMessage
    }

    isInvalidAsync(): boolean {
        let invalid = false
        _.forEach(this.asyncValidators, validator => {
            this.logger.debug('Is async validator error', this.formGroup.get(this.control.name).hasError(validator.validatorName))
            invalid = this.formGroup.get(this.control.name).hasError(validator.validatorName)
            if (invalid) {
                this.currentAsyncErrorName = validator.validatorName
                return false
            }
        })

        if (!invalid) {
            this.currentAsyncErrorName = null
        }

        return invalid
    }

    getAsyncErrorMessage() {
        if (this.currentAsyncErrorName) {
            return _.find(this.asyncValidators, validator => validator.validatorName === this.currentAsyncErrorName).validatorMessage
        }
    }

    getMaxLength(): number {
        let validatorMaxLength = _.find(this.control.validators, validator => validator.validatorType === ValidatorType.MaxLength)
        if (validatorMaxLength) {
            return parseInt(validatorMaxLength.validatorOption)
        }
        return -1
    }
    mardownChanged($event) {
        this.markdownValue$.next($event.getContent())
    }

    private generateValidators() {
        _.forEach(this.control.validators, validator => {
            if (validator.isActive) {
                let replacedMessage = validator.validatorMessage.replace("{{option}}", validator.validatorOption)
                this.validators.push({
                    validatorName: _.find(this.validatorTypes, type => type.value === validator.validatorType).validatorName,
                    validatorErrorMessage: replacedMessage
                })
            }
        })
    }

    private notifyChangedByActionEvent(boundControls: string[], data: any) {
        const foundControls = this.section.relatedStandard.controls.filter(a => boundControls.some(bound => bound === a.name))

        foundControls.forEach(control => {
            // Detach data by bind name
            const keyValue = control.options.find(a => a.key == 'bindname')
            const evaluted = Function('data', 'return data.' + keyValue.value)
            this.pageService.notifyTriggeringEvent(this.section.name + '_' + control.name + '_change', evaluted(data))
        })
    }
}
