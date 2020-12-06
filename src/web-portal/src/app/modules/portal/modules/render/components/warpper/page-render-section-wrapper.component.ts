import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngxs/store';
import { BoundSection } from 'app/core/context/bound-section';
import { ExtendedPageButton, ExtendedPageSection } from 'app/core/models/extended.models';
import { RenderingSectionState } from 'app/core/models/page.model';
import { EnumUtils } from 'app/core/utils/enum-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { Observable, Subscription } from 'rxjs';
import { tap } from 'rxjs/operators';
import { LocalizationService } from 'services/localization.service';
import { PageService } from 'services/page.service';
import { ChartsClient, DynamicListClient, Page, PageButton, PageSectionLayoutType, SectionContructionType, StandardComponent, StandardComponentClient, ValidatorType } from 'services/portal.service';
import { RenderedPageSectionAction, RenderingPageSectionAction } from 'stores/pages/page.actions';
import { PageStateModel } from 'stores/pages/page.state';

@Component({
    selector: 'let-section-wrapper',
    templateUrl: './page-render-section-wrapper.component.html'
})
export class PageRenderSectionWrapperComponent implements OnInit, OnDestroy {
    constructor(
        private chartsClient: ChartsClient,
        private store: Store,
        private standardClient: StandardComponentClient,
        private dynamicsClient: DynamicListClient,
        private pageService: PageService,
        private localizationService: LocalizationService,
        private logger: NGXLogger
    ) {
    }

    @Input()
    page: Page

    @Input()
    pageSection: ExtendedPageSection

    boundSection: BoundSection
    constructionType = SectionContructionType
    readyToRender = false

    pageState$: Observable<PageStateModel>
    subcription$: Subscription

    sectionClass = 'col-lg-12'
    ngOnDestroy(): void {

    }

    ngOnInit(): void {
        this.logger.debug('Render section hit')
        this.store.dispatch(new RenderingPageSectionAction({
            sectionClass: this.sectionClass,
            sectionName: this.pageSection.name,
            state: RenderingSectionState.Rendering
        }))
        this.boundSection = this.pageService.context.get(this.pageSection.name)
        switch (this.pageSection.constructionType) {
            case SectionContructionType.Standard:
                this.standardClient.getOneForRender(this.pageSection.componentId).pipe(
                    // delay(5000),
                    tap(
                        standard => {
                            standard = this.standardLocalization(standard)
                            this.sectionClass = this.getSectionClass(standard.layoutType)
                            this.pageSection.relatedStandard = standard
                            this.pageSection.relatedButtons = []

                            this.pageSection.relatedButtons = this.getButtons(this.pageSection.id, this.page.commands)
                            this.pageSection.isLoaded = true
                            this.readyToRender = true
                            this.store.dispatch(new RenderedPageSectionAction({
                                sectionClass: this.sectionClass,
                                sectionName: this.pageSection.name,
                                state: RenderingSectionState.Complete
                            }))
                        },
                        () => {

                        }
                    )
                ).subscribe()
                break
            case SectionContructionType.Array:
                this.standardClient.getOneForRender(this.pageSection.componentId).pipe(
                    // delay(5000),
                    tap(
                        standard => {
                            standard = this.standardLocalization(standard)
                            this.sectionClass = this.getSectionClass(standard.layoutType)
                            this.pageSection.relatedArrayStandard = standard
                            this.pageSection.isLoaded = true
                            this.pageSection.relatedButtons = this.getButtons(this.pageSection.id, this.page.commands)

                            this.readyToRender = true
                            this.store.dispatch(new RenderedPageSectionAction({
                                sectionClass: this.sectionClass,
                                sectionName: this.pageSection.name,
                                state: RenderingSectionState.Complete
                            }))
                        },
                        () => {

                        }
                    )
                ).subscribe()
                break
            case SectionContructionType.Tree:
                this.standardClient.getOneForRender(this.pageSection.componentId).pipe(
                    // delay(5000),
                    tap(
                        standard => {
                            standard = this.standardLocalization(standard)
                            this.sectionClass = this.getSectionClass(standard.layoutType)
                            this.pageSection.relatedTreeStandard = standard
                            this.pageSection.isLoaded = true
                            this.pageSection.relatedButtons = this.getButtons(this.pageSection.id, this.page.commands)
                            this.readyToRender = true
                            this.store.dispatch(new RenderedPageSectionAction({
                                sectionClass: this.sectionClass,
                                sectionName: this.pageSection.name,
                                state: RenderingSectionState.Complete
                            }))
                        },
                        () => {

                        }
                    )
                ).subscribe()
                break
            case SectionContructionType.DynamicList:
                this.dynamicsClient.getOneForRender(this.pageSection.componentId).pipe(
                    // delay(5000),
                    tap(
                        dynamicList => {
                            this.sectionClass = this.getSectionClass(dynamicList.layoutType)
                            this.pageSection.relatedDynamicList = dynamicList
                            this.pageSection.relatedButtons = this.getButtons(this.pageSection.id, this.page.commands)
                            this.pageSection.isLoaded = true
                            this.readyToRender = true
                            this.store.dispatch(new RenderedPageSectionAction({
                                sectionClass: this.sectionClass,
                                sectionName: this.pageSection.name,
                                state: RenderingSectionState.Complete
                            }))
                        },
                        () => {

                        }
                    )
                ).subscribe()
                break
            case SectionContructionType.Chart:
                this.chartsClient.getOne(this.pageSection.componentId).pipe(
                    tap(
                        chart => {
                            this.sectionClass = this.getSectionClass(chart.layoutType)
                            this.pageSection.relatedChart = chart
                            this.pageSection.relatedButtons = this.getButtons(this.pageSection.id, this.page.commands)
                            this.pageSection.isLoaded = true
                            this.readyToRender = true
                            this.store.dispatch(new RenderedPageSectionAction({
                                sectionClass: this.sectionClass,
                                sectionName: this.pageSection.name,
                                state: RenderingSectionState.Complete
                            }))
                        },
                        () => {

                        }
                    )
                ).subscribe()
                break
        }
    }

    private getSectionClass(layoutType: PageSectionLayoutType) {
        switch (layoutType) {
            case PageSectionLayoutType.OneColumn:
                return 'col-lg-12'
            case PageSectionLayoutType.TwoColumns:
                return 'col-lg-6'
            case PageSectionLayoutType.ThreeColumns:
                return 'col-lg-4'
            case PageSectionLayoutType.FourColumns:
                return 'col-lg-3'
            default:
                return 'col-lg-12'
        }
    }

    private getButtons(sectionId: string, controls: PageButton[]) {
        let relatedButtons: ExtendedPageButton[] = []
        if (!ObjectUtils.isNotNull(controls)) {
            return []
        }
        controls?.forEach(a => {
            if (a.placeSectionId === sectionId) {
                let cloneControl: ExtendedPageButton = {
                    ...a,
                    isHidden: this.pageService.evaluatedExpression(a.allowHidden)
                }
                relatedButtons.push(cloneControl)
            }
        })

        return relatedButtons
    }

    private standardLocalization(standard: StandardComponent) {
        if (this.localizationService.allowTranslate) {

            const standardName = this.localizationService.getText(`standardComponents.${standard.name}.options.displayName`)
            if (ObjectUtils.isNotNull(standardName)) {
                standard.displayName = standardName
            }

            if (ObjectUtils.isNotNull(standard.controls)) {
                standard.controls?.forEach(control => {
                    const hidden = control.options.find(a => a.key === 'hidden')
                    if (hidden !== 'true') {
                        const labelName = this.localizationService.getText(`standardComponents.${standard.name}.${control.name}.options.label`)
                        if (ObjectUtils.isNotNull(labelName)) {
                            let label = control.options.find(a => a.key === 'label')
                            label.value = labelName
                        }

                        const placeholderName = this.localizationService.getText(`standardComponents.${standard.name}.${control.name}.options.label`)
                        if (ObjectUtils.isNotNull(placeholderName)) {
                            let placeholder = control.options.find(a => a.key === 'placeholder')
                            placeholder.value = placeholderName
                        }

                        if (ObjectUtils.isNotNull(control.validators)) {
                            control.validators?.forEach(validator => {
                                if (validator.isActive) {
                                    const validatorMessage = this.localizationService.getText(`standardComponents.${standard.name}.${control.name}.validators.${EnumUtils.getEnumKeyByValue(ValidatorType, validator.validatorType)}`)
                                    if (ObjectUtils.isNotNull(validatorMessage)) {
                                        validator.validatorMessage = validatorMessage
                                    }
                                }
                            })
                        }

                        if (ObjectUtils.isNotNull(control.asyncValidators)) {
                            control.asyncValidators?.forEach(validator => {
                                if (validator.isActive) {
                                    const validatorMessage = this.localizationService.getText(`standardComponents.${standard.name}.${control.name}.asyncValidators.${validator.validatorName}`)
                                    if (ObjectUtils.isNotNull(validatorMessage)) {
                                        validator.validatorMessage = validatorMessage
                                    }
                                }
                            })
                        }
                    }
                })
            }
        }
        return standard
    }
}
