import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { SectionContructionType, StandardComponentClient, DynamicListClient, ChartsClient, PageSectionLayoutType } from 'services/portal.service';
import { ExtendedPageSection } from 'app/core/models/extended.models';
import { NGXLogger } from 'ngx-logger';
import { tap } from 'rxjs/operators';
import { Store } from '@ngxs/store';
import { RenderingPageSectionAction, RenderedPageSectionAction } from 'stores/pages/page.actions';
import { RenderingSectionState } from 'app/core/models/page.model';
import { Observable, Subscription } from 'rxjs';
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
        private logger: NGXLogger
    ) {
    }
    @Input()
    pageSection: ExtendedPageSection

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
        switch (this.pageSection.constructionType) {
            case SectionContructionType.Standard:
                this.standardClient.getOneForRender(this.pageSection.componentId).pipe(
                    // delay(5000),
                    tap(
                        standard => {
                            this.sectionClass = this.getSectionClass(standard.layoutType)
                            this.pageSection.relatedStandard = standard
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
                break
            case SectionContructionType.DynamicList:
                this.dynamicsClient.getOne(this.pageSection.componentId).pipe(
                    // delay(5000),
                    tap(
                        dynamicList => {
                            this.sectionClass = this.getSectionClass(dynamicList.layoutType)
                            this.pageSection.relatedDynamicList = dynamicList
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
                        chart =>{
                            this.sectionClass = this.getSectionClass(chart.layoutType)
                            this.pageSection.relatedChart = chart
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

    private getSectionClass(layoutType: PageSectionLayoutType){
        switch(layoutType){
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
}
