import { Component, OnInit, Input, OnDestroy, ChangeDetectorRef, Optional } from '@angular/core';
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
import { filter, tap, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ChangeControlValueEvent } from 'stores/pages/page.actions';
import { PageControlEventStateModel } from 'stores/pages/pagecontrol.state';
import { PageService } from 'services/page.service';
import { NGXLogger } from 'ngx-logger';
import { EventsProvider } from 'app/core/events/event.provider';
import { UploadFileService } from 'services/uploadfile.service';
import { MarkdownService } from 'ngx-markdown';

@Component({
    selector: 'let-general-control',
    templateUrl: './general-control.component.html',
    styleUrls: ['./general-control.component.scss']
})
export class GeneralControlComponent implements OnInit, OnDestroy {

    @Input()
    formGroup: FormGroup

    @Input()
    control: PageRenderedControl<DefaultControlOptions>

    @Input()
    section: ExtendedPageSection

    controlFullName: string
    controlType = ControlType
    maxLength = -1
    validators: Array<ExtendedControlValidator> = []
    validatorTypes = StaticResources.formValidatorTypes();

    optionsList: Observable<any>
    defaultData: any
    pageControlState$: Observable<PageControlEventStateModel>
    controlEventSubscription: Subscription

    asyncValidators: PageControlAsyncValidator[] = []
    currentAsyncErrorName: string
    hasAsyncInvalid: boolean = false

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
        private uploadService: UploadFileService,
        private markdownService: MarkdownService,
        private logger: NGXLogger,
        private cd: ChangeDetectorRef
    ) {
    }

    ngOnInit(): void {
        this.controlEventSubscription = this.pageService.listenTriggeringControlEvent$().pipe(
            filter(state => state && (state.controlFullName === this.controlFullName)),
            tap(
                state => {
                    let tempData = ObjectUtils.clone(state.data)
                    // Implement some function events there based on control type
                    switch (state.eventType) {

                        case 'change':
                            this.formGroup.get(this.control.name).setValue(tempData)
                            break
                        case 'reset':
                            this.formGroup.get(this.control.name).setValue(this.defaultData)
                            break
                        case 'clean':
                            this.formGroup.get(this.control.name).setValue('')
                            break
                        case 'rebound':
                            const foundData = this.pageService.getDataByBindName(this.control.defaultOptions.bindname)
                            this.formGroup.get(this.control.name).setValue(foundData)
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
                                eventExecution.execute(
                                    this.formGroup.get(this.control.name) as FormControl,
                                    this.formGroup,
                                    this.pageService,
                                    this.control.defaultOptions.bindname,
                                    this.defaultData,
                                    this.pageService.getDataByBindName(this.control.defaultOptions.bindname),
                                    state.data
                                )
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
            this.generateOptions()
        }

        if (this.control.type === ControlType.MarkdownEditor) {
            this.markdownContent = this.defaultData
            this.markdownValue$.pipe(
                debounceTime(500),
                distinctUntilChanged(),
                tap(
                    res => {
                        this.pageService.changeControlValue(this.controlFullName, res)
                    }
                )
            ).subscribe()
        }
        this.asyncValidators = this.control.asyncValidators ? this.control.asyncValidators : []
        this.controlFullName = this.section.name + '_' + this.control.name
        this.notifyControlValueChange()
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
                    if (this.control.type == ControlType.Checkbox || this.control.type == ControlType.Slide) {
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
                            case EventActionType.WebService:
                                break
                        }
                    })
                }
            )
        ).subscribe()
    }

    generateOptions() {
        switch (this.control.datasourceOptions.type) {
            case DatasourceControlType.StaticResource:
                this.optionsList = of(JSON.parse(this.control.datasourceOptions.datasourceStaticOptions.jsonResource))
                break
            case DatasourceControlType.Database:
                this.optionsList = this.pageService.fetchDatasource(this.control.datasourceOptions.databaseOptions.databaseConnectionId, this.control.datasourceOptions.databaseOptions.query)
                break
            case DatasourceControlType.WebService:
                break
        }
    }

    isInvalid(validatorName: string): boolean {
        return this.formGroup.get(this.control.name).hasError(validatorName)
    }

    getErrorMessage(validatorName: string) {
        return _.find(this.validators, validator => validator.validatorName === validatorName).validatorErrorMessage
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
}
