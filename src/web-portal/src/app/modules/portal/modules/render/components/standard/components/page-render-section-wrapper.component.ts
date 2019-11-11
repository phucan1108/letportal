import { Component, OnInit, Input, ViewChildren, QueryList, OnDestroy } from '@angular/core';
import { SectionTemplate } from './section-template.directive';
import { PageSection, SectionContructionType, StandardComponentClient, DynamicListClient, ChartsClient } from 'services/portal.service';
import { ExtendedPageSection } from 'app/core/models/extended.models';
import { NGXLogger } from 'ngx-logger';
import { debounceTime, tap, delay, filter } from 'rxjs/operators';
import { Store } from '@ngxs/store';
import { RenderingPageSectionAction, BeginRenderingPageSectionsAction, RenderedPageSectionAction, AddSectionBoundData } from 'stores/pages/page.actions';
import { RenderingSectionState } from 'app/core/models/page.model';
import { Observable, Subscription } from 'rxjs';
import { PageStateModel } from 'stores/pages/page.state';

@Component({
    selector: 'let-section-wrapper',
    templateUrl: './page-render-section-wrapper.component.html'
})
export class PageRenderSectionWrapperComponent implements OnInit, OnDestroy {
    ngOnDestroy(): void {
        
    }
    @Input()
    pageSection: ExtendedPageSection

    constructionType = SectionContructionType
    readyToRender = false

    pageState$: Observable<PageStateModel>
    subcription$: Subscription
    constructor(
        private chartsClient: ChartsClient,
        private store: Store,
        private standardClient: StandardComponentClient,
        private dynamicsClient: DynamicListClient,
        private logger: NGXLogger
    ) {
    }

    ngOnInit(): void {
        this.logger.debug('Render section hit')
        this.store.dispatch(new RenderingPageSectionAction({
            sectionName: this.pageSection.name,
            state: RenderingSectionState.Rendering
        }))
        switch (this.pageSection.constructionType) {
            case SectionContructionType.Standard:
                this.standardClient.getOne(this.pageSection.componentId).pipe(
                    //delay(5000),
                    tap(
                        standard => {
                            this.pageSection.relatedStandard = standard
                            this.pageSection.isLoaded = true
                            this.readyToRender = true
                            this.store.dispatch(new RenderedPageSectionAction({
                                sectionName: this.pageSection.name,
                                state: RenderingSectionState.Complete
                            }))
                        },
                        err => {

                        }
                    )
                ).subscribe()
                break
            case SectionContructionType.Array:
                break
            case SectionContructionType.DynamicList:
                this.dynamicsClient.getOne(this.pageSection.componentId).pipe(
                    //delay(5000),
                    tap(
                        dynamicList => {
                            this.pageSection.relatedDynamicList = dynamicList
                            this.pageSection.isLoaded = true
                            this.readyToRender = true
                            this.store.dispatch(new RenderedPageSectionAction({
                                sectionName: this.pageSection.name,
                                state: RenderingSectionState.Complete
                            }))
                        },
                        err => {

                        }
                    )
                ).subscribe()
                break
            case SectionContructionType.Chart:
                this.chartsClient.getOne(this.pageSection.componentId).pipe(
                    tap(
                        chart =>{
                            this.pageSection.relatedChart = chart
                            this.pageSection.isLoaded = true
                            this.readyToRender = true
                            this.store.dispatch(new RenderedPageSectionAction({
                                sectionName: this.pageSection.name,
                                state: RenderingSectionState.Complete
                            }))
                        },
                        err => {

                        }
                    )
                ).subscribe()
                break
        }
    }
}
