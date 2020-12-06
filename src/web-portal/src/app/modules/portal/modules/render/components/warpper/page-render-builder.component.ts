import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { AfterContentInit, AfterViewChecked, AfterViewInit, ChangeDetectorRef, Component, Input, OnDestroy, OnInit, QueryList, ViewChildren } from '@angular/core';
import { Store } from '@ngxs/store';
import { ExtendedPageButton } from 'app/core/models/extended.models';
import { RenderingPageSectionState, RenderingSectionState } from 'app/core/models/page.model';
import { ObjectUtils } from 'app/core/utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { Observable, of, Subscription } from 'rxjs';
import { delay, filter, tap } from 'rxjs/operators';
import { LocalizationService } from 'services/localization.service';
import { PageService } from 'services/page.service';
import { LocalizationClient, Page, PageButton, PageSection } from 'services/portal.service';
import { AddSectionBoundData, AddSectionBoundDataForStandardArray, AddSectionBoundDataForTree, BeginBuildingBoundData, BeginRenderingPageSectionsAction, EndBuildingBoundDataComplete, EndRenderingPageSectionsAction, PageReadyAction, RenderedPageSectionAction } from 'stores/pages/page.actions';
import { PageStateModel } from 'stores/pages/page.state';
import { PageRenderSectionWrapperComponent } from './page-render-section-wrapper.component';
 

@Component({
    selector: 'let-builder',
    templateUrl: './page-render-builder.component.html'
})
export class PageRenderBuilderComponent implements OnInit, AfterViewInit, AfterContentInit, AfterViewChecked, OnDestroy {

    @ViewChildren(PageRenderSectionWrapperComponent) renderSections: QueryList<PageRenderSectionWrapperComponent>

    @Input()
    page: Page

    isReadyToRender = false
    pageState$: Observable<PageStateModel>
    subscribe$: Subscription
    options: any
    queryparams: any
    counterRenderedSection = 0
    counterBuildSectionData = 0

    sectionClasses: string[] = []

    renderingSections: RenderingPageSectionState[] = []
    actionCommands: PageButton[] = []
    isSmallDevice = false

    sections: PageSection[] = []
    constructor(
        private localizationService: LocalizationService,
        private localizationClient: LocalizationClient,
        private store: Store,
        private logger: NGXLogger,
        private cd: ChangeDetectorRef,
        private pageService: PageService,
        private breakpointObserver: BreakpointObserver
    ) {
        this.pageState$ = this.store.select(state => state.page)
        this.breakpointObserver.observe([
            Breakpoints.HandsetPortrait,
            Breakpoints.HandsetLandscape
        ]).subscribe(result => {
            if (result.matches) {
                this.isSmallDevice = true
                this.cd.markForCheck()
            }
            else{
                this.isSmallDevice = false
                this.cd.markForCheck()
            }
        });
    }
    data: any
    readyToRenderButtons = false
    readyToRenderAllSections = false
    ngOnInit(): void {
        this.logger.debug('Init render builder')
        this.localization()
        this.page.builder.sections?.forEach(sec =>{
            this.sectionClasses.push('col-lg-12')
        })
        this.sections = ObjectUtils.clone(this.page.builder.sections)
        this.actionCommands = this.page.commands ? this.page.commands.filter(a => !ObjectUtils.isNotNull(a.placeSectionId)) : []
        this.actionCommands = ObjectUtils.clone(this.actionCommands)
        const sub$ = this.pageService.listenDataChange$().subscribe(
            data => {
                this.data = data
                this.actionCommands?.forEach((command: ExtendedPageButton) => {
                    command.isHidden = this.pageService.evaluatedExpression(command.allowHidden)
                })
                this.readyToRenderButtons = true
                sub$.unsubscribe()
            }
        )
        this.subscribe$ = this.pageState$.pipe(
            filter(state => state.filterState &&
                (state.filterState === PageReadyAction
                    || state.filterState === RenderedPageSectionAction
                    || state.filterState === AddSectionBoundDataForTree
                    || state.filterState === AddSectionBoundData
                    || state.filterState === AddSectionBoundDataForStandardArray)),
            tap(
                pageState => {
                    switch (pageState.filterState) {
                        case PageReadyAction:
                            this.options = pageState.options
                            this.queryparams = pageState.queryparams

                            //  We need to filter which section must be rendered
                            this.sections?.forEach(section => {
                                if(ObjectUtils.isNotNull(section.rendered) && section.rendered !== 'true'){
                                    const checkRendered = this.pageService.evaluatedExpression(section.rendered)
                                    section.rendered = checkRendered ? 'true' : 'false'
                                }
                            })

                            this.sections = this.sections.filter(a => a.rendered === 'true')
                            this.counterRenderedSection = this.sections.length
                            this.counterBuildSectionData = this.sections.length                            
                            this.isReadyToRender = true
                            this.store.dispatch(new BeginRenderingPageSectionsAction(this.prepareRenderingPageSectionsStates(this.page)))
                            break
                        case RenderedPageSectionAction:
                            this.counterRenderedSection--
                            if (this.counterRenderedSection === 0) {
                                const timer$ = of(true).pipe(
                                    delay(500),
                                    tap(
                                        () => {
                                            this.readyToRenderAllSections = true
                                            pageState.renderingSections?.forEach(sec => {
                                                const index = this.sections.findIndex(a => a.name === sec.sectionName)
                                                this.sectionClasses[index] = sec.sectionClass
                                            })
                                            this.store.dispatch(new EndRenderingPageSectionsAction())
                                            this.store.dispatch(new BeginBuildingBoundData())
                                            timer$.unsubscribe()
                                        }
                                    )
                                ).subscribe()
                            }
                            break
                        case AddSectionBoundData:
                        case AddSectionBoundDataForStandardArray:
                        case AddSectionBoundDataForTree:                             
                            this.counterBuildSectionData--
                            this.logger.debug('Hit counter bound section data', this.counterBuildSectionData)                           
                            if (this.counterBuildSectionData === 0) {
                                const timer$ = of(true).pipe(
                                    delay(200),
                                    tap(
                                        () => {
                                            this.store.dispatch(new EndBuildingBoundDataComplete())
                                            timer$.unsubscribe()
                                        }
                                    )
                                ).subscribe()
                            }
                            break

                    }
                }
            )
        ).subscribe()
    }

    ngAfterContentInit(): void {
    }
    ngAfterViewInit(): void {
    }

    ngAfterViewChecked(): void {
    }
    ngOnDestroy(): void {
        this.subscribe$.unsubscribe()
    }

    prepareRenderingPageSectionsStates(page: Page) {
        this.sections?.forEach(section => {
            this.renderingSections.push({
                sectionName: section.name,
                sectionClass: 'col-lg-12',
                state: RenderingSectionState.Init
            })
        })

        return this.renderingSections
    }

    onCommandClick(command: ExtendedPageButton) {
        this.pageService.executeCommand(command)
    }

    getSectionClass(section: PageSection){
        if(this.readyToRenderAllSections){
            return this.renderingSections.find(a => a.sectionName == section.name).sectionClass
        }
        else{
            return 'hidden'
        }
    }

    private localization(){
        if(this.localizationService.allowTranslate){
            const pageName = this.localizationService.getText(`pages.${this.page.name}.options.displayName`)
            if(ObjectUtils.isNotNull(pageName)){
                this.page.displayName = pageName
            }

            if(ObjectUtils.isNotNull(this.sections)){
                    this.sections?.forEach(section => {
                        const sectionName = this.localizationService.getText(`pages.${this.page.name}.sections.${section.name}.options.displayName`)
                        if(ObjectUtils.isNotNull(sectionName)){
                            section.displayName = sectionName
                        }
                    })
                }
            if(ObjectUtils.isNotNull(this.page.commands)){
                this.page.commands?.forEach(command => {
                    const commandName = this.localizationService.getText(`pages.${this.page.name}.commands.${command.name}.options.name`)
                    if(ObjectUtils.isNotNull(commandName)){
                        command.name = commandName
                    }

                    if(ObjectUtils.isNotNull(command.buttonOptions.actionCommandOptions.confirmationOptions)){
                        const confirmation = this.localizationService.getText(`pages.${this.page.name}.commands.${command.name}.options.confirmation`)
                        if(ObjectUtils.isNotNull(confirmation)){
                            command.buttonOptions.actionCommandOptions.confirmationOptions.confirmationText = confirmation
                        }
                    }

                    if(ObjectUtils.isNotNull(command.buttonOptions.actionCommandOptions.notificationOptions)){
                        const complete = this.localizationService.getText(`pages.${this.page.name}.commands.${command.name}.options.success`)
                        const failed = this.localizationService.getText(`pages.${this.page.name}.commands.${command.name}.options.failed`)

                        if(ObjectUtils.isNotNull(complete)){
                            command.buttonOptions.actionCommandOptions.notificationOptions.completeMessage = complete
                        }

                        if(ObjectUtils.isNotNull(failed)){
                            command.buttonOptions.actionCommandOptions.notificationOptions.failedMessage = failed
                        }
                    }
                })
            }
        }
    }
}
