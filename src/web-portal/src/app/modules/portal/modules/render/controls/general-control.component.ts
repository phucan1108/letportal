import { AfterViewInit, ChangeDetectorRef, Component, Input, OnDestroy, OnInit, Optional, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { TranslateService } from '@ngx-translate/core';
import { BoundControl, SelectBoundControl, SimpleBoundControl } from 'app/core/context/bound-control';
import { StandardBoundSection } from 'app/core/context/bound-section';
import { EventsProvider } from 'app/core/events/event.provider';
import { DefaultControlOptions, PageRenderedControl } from 'app/core/models/page.model';
import { ObjectUtils } from 'app/core/utils/object-util';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { NGXLogger } from 'ngx-logger';
import { MarkdownService } from 'ngx-markdown';
import { MultipleDataSelection } from 'portal/modules/models/control.extended.model';
import { StaticResources } from 'portal/resources/static-resources';
import { BehaviorSubject, Observable, of, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, map, startWith, tap } from 'rxjs/operators';
import { PageService } from 'services/page.service';
import { ControlType, DatasourceControlType, EventActionType, PageControlAsyncValidator, ValidatorType } from 'services/portal.service';
import { PageControlEventStateModel } from 'stores/pages/pagecontrol.state';
import { ExtendedControlValidator, ExtendedPageSection } from '../../../../../core/models/extended.models';
import { AutocompleteMultipleComponent } from './autocomplete-multiple.component';


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

    @Input()
    boundSection: StandardBoundSection

    @Input()
    boundControl: BoundControl

    @ViewChild(AutocompleteMultipleComponent, { static: false }) autoComplete: AutocompleteMultipleComponent

    controlFullName: string
    controlType = ControlType
    maxLength = -1
    validators: Array<ExtendedControlValidator> = []
    validatorTypes = StaticResources.formValidatorTypes();

    formControlLink: FormControl = new FormControl()

    optionsList$: Observable<any> 
    optionsList: any[] = []
    autoOptionsList: Array<MultipleDataSelection> = []
    autoOptionsList$: BehaviorSubject<Array<MultipleDataSelection>> = new BehaviorSubject([])
    defaultData: any
    pageControlState$: Observable<PageControlEventStateModel>
    controlEventSubscription: Subscription

    asyncValidators: PageControlAsyncValidator[] = []
    currentAsyncErrorName: string
    hasAsyncInvalid = false
    selectDisabled = false
    sectionName = ''

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
        private translate: TranslateService,
        private cd: ChangeDetectorRef
    ) {
    }

    ngOnInit(): void {
        const isChildCompositeControl = this.control.type !== ControlType.Composite && !!this.control.compositeControlName
        this.asyncValidators = this.control.asyncValidators ? this.control.asyncValidators : []
        this.controlFullName = isChildCompositeControl ? this.section.name + '_' + this.control.compositeControlName + '.' + this.control.name : this.section.name + '_' + this.control.name
        this.sectionName = this.section.name
        this.controlEventSubscription = this.pageService.listenTriggeringControlEvent$().pipe(
            filter(state => state && (state.controlFullName === this.controlFullName)),
            tap(
                state => {
                    const tempData = ObjectUtils.clone(state.data)
                    // Implement some function events there based on control type
                    switch (state.eventType) {
                        case 'resetdatasource':
                            // Only for Auto and Select
                            this.reset()
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
                                        this.boundControl,
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
        this.selectDisabled = this.control.defaultOptions.checkDisabled
        if (this.control.type === ControlType.Select
            || this.control.type === ControlType.AutoComplete
            || this.control.type === ControlType.Radio) {

            // Important note: to prevent many unexpected data between multiple and single mode
            // We need to check a defaultData for ensuring this is matching with Single/Multiple mode
            if (ObjectUtils.isArray(this.defaultData) && !this.control.defaultOptions.multiple) {
                this.defaultData = this.defaultData[0]
            }

            this.optionsList$ = (this.boundControl as SelectBoundControl).connect()
            this.generateOptions()
                .pipe(
                    tap(
                        res => {
                            (this.boundControl as SelectBoundControl).bound(res)
                        }
                    )
                ).subscribe()

            this.optionsList$
                .pipe(
                    tap(
                        res => {
                            if (!ObjectUtils.isNotNull(res) || res.length === 0) {
                                return
                            }
                            if (this.control.type === ControlType.AutoComplete) {
                                // Available only for AutoComplete and Multiple mode
                                const arrayData: string[] = ObjectUtils.isArray(this.defaultData) ? this.defaultData : [this.defaultData]
                                this.optionsList$.subscribe(res => {
                                    res?.forEach(elem => {
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

        this.setupBoundControl()
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
                    this.changeValue(newValue)
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
                    .fetchControlSelectionDatasource(
                        this.sectionName,
                        this.control.name,
                        this.control.compositeControlRefId,
                        ObjectUtils.isNotNull(this.control.compositeControlId),
                        parameters)
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
        const selected = this.autoOptionsList.filter(a => a.selected)
        this.logger.debug('Current selected values', selected)
        this.setControlValue(selected.length > 0 ? selected.map(b => b.value) : null)
    }

    isInvalid(validatorName: string): boolean {
        const invalid = this.formGroup.get(this.control.name).hasError(validatorName)
        return invalid
    }

    getErrorMessage(validatorName: string) {
        const errorMessage = this.validators.find(validator => validator.validatorName === validatorName).validatorErrorMessage
        return errorMessage
    }

    isInvalidAsync(): boolean {
        let invalid = false
        this.asyncValidators?.forEach(validator => {
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
            return this.asyncValidators.find(validator => validator.validatorName === this.currentAsyncErrorName).validatorMessage
        }
    }

    getMaxLength(): number {
        const validatorMaxLength = this.control.validators.find(validator => validator.validatorType === ValidatorType.MaxLength)
        if (validatorMaxLength) {
            return parseInt(validatorMaxLength.validatorOption)
        }
        return -1
    }
    mardownChanged($event) {
        this.markdownValue$.next($event.getContent())
    }

    private generateValidators() {
        this.control.validators?.forEach(validator => {
            if (validator.isActive) {
                const replacedMessage = validator.validatorMessage.replace('{{option}}', validator.validatorOption)
                this.validators.push({
                    validatorName: this.validatorTypes.find(type => type.value === validator.validatorType).validatorName,
                    validatorErrorMessage: replacedMessage
                })
            }
        })

        // New change for custom error message
        this.control.customErrorMessages?.forEach(cusErr => {
            this.validators.push({
                validatorName: cusErr.errorName,
                validatorErrorMessage: cusErr.errorMessage
            })
        })
    }

    private notifyChangedByActionEvent(boundControls: string[], data: any) {
        const foundControls = this.section.relatedStandard.controls.filter(a => boundControls.some(bound => bound === a.name))

        foundControls?.forEach(control => {
            // Detach data by bind name
            const keyValue = control.options.find(a => a.key == 'bindname')
            const evaluted = Function('data', 'return data.' + keyValue.value)
            this.pageService.notifyTriggeringEvent(this.controlFullName + '_change', evaluted(data))
        })
    }

    private changeValue(newValue: any) {
        this.boundControl.value = newValue
        this.hasAsyncInvalid = this.isInvalidAsync()
        let allowChainingEvents = true
        if (this.control.type == ControlType.Checkbox
            || this.control.type == ControlType.Slide) {
            if (this.control.defaultOptions.allowZero) {
                allowChainingEvents = this.pageService.changeControlValue(this.controlFullName, newValue ? 1 : 0, this.boundSection, this.boundControl)
            }
            else if (this.control.defaultOptions.allowYesNo) {
                allowChainingEvents = this.pageService.changeControlValue(this.controlFullName, newValue ? 'Y' : 'N', this.boundSection, this.boundControl)
            }
            else {
                allowChainingEvents = this.pageService.changeControlValue(this.controlFullName, newValue, this.boundSection, this.boundControl)
            }
        }
        else {
            allowChainingEvents = this.pageService.changeControlValue(this.controlFullName, newValue, this.boundSection, this.boundControl)
        }
        this.logger.debug('allow chaining events', allowChainingEvents)
        if (allowChainingEvents) {
            // Check with chaining events must be notified
            this.control.pageControlEvents?.forEach(event => {
                switch (event.eventActionType) {
                    case EventActionType.TriggerEvent:
                        event.triggerEventOptions.eventsList?.forEach(eventOpt => {
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
                                        this.shortcutUtil.toastMessage(this.translate.instant('common.somethingWentWrong'), ToastType.Error)
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
                                        this.shortcutUtil.toastMessage(this.translate.instant('common.somethingWentWrong'), ToastType.Error)
                                    }
                                )
                            )
                        break
                }
            })
        }
    }

    private reset() {
        if (this.control.type === ControlType.Select
            || this.control.type === ControlType.AutoComplete
            || this.control.type === ControlType.Radio) {
            let generatedSub = this.generateOptions()
                .pipe(
                    tap(
                        res => {
                            (this.boundControl as SelectBoundControl).bound(res)
                            generatedSub.unsubscribe()
                        }
                    )
                ).subscribe()
        }
    }

    private setupBoundControl() {
        switch (this.control.type) {
            case ControlType.Select:
            case ControlType.AutoComplete:
            case ControlType.Radio:
                let selectBoundControl = this.boundControl as SelectBoundControl
                break
            default:
                let simpleBoundControl = this.boundControl as SimpleBoundControl
                break
        }
    }
}
