import { Component, OnInit, Input, ChangeDetectorRef, Output, EventEmitter } from '@angular/core';
import { ExtendedPageSection, ExtendedPageControl, ExtendedStandardComponent } from 'app/core/models/extended.models';
import { FormGroup } from '@angular/forms';
 
import { MatDialog } from '@angular/material/dialog';
import { SectionDialogComponent } from './section-dialog.component';
import { Guid } from 'guid-typescript';
import { Store } from '@ngxs/store';
import { Observable, BehaviorSubject } from 'rxjs';
import { filter, tap, combineLatest } from 'rxjs/operators';
import { ShellContants } from 'app/core/shell/shell.contants';
import { NGXLogger } from 'ngx-logger';
import { InitEditPageBuilderAction, GeneratePageBuilderInfoAction, NextToWorkflowAction, UpdatePageBuilderInfoAction, UpdateAvailableEvents, UpdateAvailableShells, NextToDatasourceAction, GatherAllChanges, UpdateAvailableBoundDatas, UpdateAvailableTriggerEventsList } from 'stores/pages/pagebuilder.actions';
import { SectionContructionType, PageSection, StandardComponent, PageSectionLayoutType, StandardComponentClient, DynamicListClient, PageControl, DynamicList, ControlType, ChartsClient } from 'services/portal.service';
import { ArrayUtils } from 'app/core/utils/array-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'let-builder-dnd',
    templateUrl: './builder-dnd.component.html'
})
export class BuilderDnDComponent implements OnInit {

    pageSections: Array<ExtendedPageSection> = []
    constructionType = SectionContructionType

    @Output()
    onSectionsChanged: EventEmitter<any> = new EventEmitter<any>()

    constructor(
        private translate: TranslateService,
        private standardsClient: StandardComponentClient,
        private shortcutUtil: ShortcutUtil,
        private dynamicListsClient: DynamicListClient,
        private chartsClient: ChartsClient,
        public dialog: MatDialog,
        private cd: ChangeDetectorRef,
        private store: Store,
        private logger: NGXLogger
    ) { }

    ngOnInit(): void {
        this.store.select(state => state.pagebuilder)
            .pipe(
                filter(result => result.filterState && (
                    result.filterState === InitEditPageBuilderAction
                    || result.filterState === GeneratePageBuilderInfoAction
                    || result.filterState === NextToDatasourceAction
                    || result.filterState === NextToWorkflowAction
                    || result.filterState === GatherAllChanges)),
                tap(result => {
                    this.logger.debug('hitting')
                    switch (result.filterState) {
                        case InitEditPageBuilderAction:
                            if (!!result.processPage.builder) {
                                // Important: Because we are using ngxs, so all objects have immutable, we should create a shallow copy instead of using directly
                                const tempEdit = result.processPage.builder.sections as Array<ExtendedPageSection>

                                this.pageSections = []
                                tempEdit?.forEach((section: ExtendedPageSection) => {
                                    this.pageSections.push(ObjectUtils.clone(section))
                                })
                                this.pageSections?.forEach((section: ExtendedPageSection) => {
                                    if (!section.sectionDatasource) {
                                        section.sectionDatasource = {
                                            datasourceBindName: 'data'
                                        }
                                    }
                                    switch (section.constructionType) {
                                        case SectionContructionType.Standard:
                                            this.standardsClient.getOne(section.componentId).pipe(
                                                tap(
                                                    standard => {
                                                        section.relatedStandard = standard
                                                    },
                                                    err => {
                                                        this.shortcutUtil.toastMessage(this.translate.instant('pageBuilder.builderDnd.messages.sectionBroken', { sectionName: section.displayName }), ToastType.Error)
                                                        section.isBroken = true
                                                    }
                                                )
                                            ).subscribe()
                                            break
                                        case SectionContructionType.Array:
                                            this.standardsClient.getOne(section.componentId).pipe(
                                                tap(
                                                    standard => {
                                                        section.relatedArrayStandard = standard
                                                    },
                                                    err => {
                                                        this.shortcutUtil.toastMessage(this.translate.instant('pageBuilder.builderDnd.messages.sectionBroken', { sectionName: section.displayName }), ToastType.Error)
                                                        section.isBroken = true
                                                    }
                                                )
                                            ).subscribe()
                                            break
                                        case SectionContructionType.Tree:
                                            this.standardsClient.getOne(section.componentId).pipe(
                                                tap(
                                                    standard => {
                                                        section.relatedTreeStandard = standard
                                                    },
                                                    err => {
                                                        this.shortcutUtil.toastMessage(this.translate.instant('pageBuilder.builderDnd.messages.sectionBroken', { sectionName: section.displayName }), ToastType.Error)
                                                        section.isBroken = true
                                                    }
                                                )
                                            ).subscribe()
                                            break
                                        case SectionContructionType.DynamicList:
                                            this.dynamicListsClient.getOne(section.componentId).pipe(
                                                tap(
                                                    dynamicList => {
                                                        section.relatedDynamicList = dynamicList
                                                    },
                                                    err => {
                                                        this.shortcutUtil.toastMessage(this.translate.instant('pageBuilder.builderDnd.messages.sectionBroken', { sectionName: section.displayName }), ToastType.Error)
                                                        section.isBroken = true
                                                    }
                                                )
                                            ).subscribe()
                                            break
                                        case SectionContructionType.Chart:
                                            this.chartsClient.getOne(section.componentId).pipe(
                                                tap(
                                                    chart => {
                                                        section.relatedChart = chart
                                                    },
                                                    err => {
                                                        this.shortcutUtil.toastMessage(this.translate.instant('pageBuilder.builderDnd.messages.sectionBroken', { sectionName: section.displayName }), ToastType.Error)
                                                        section.isBroken = true
                                                    }
                                                )
                                            ).subscribe()
                                            break
                                    }
                                })
                                this.cd.markForCheck()
                            }
                            break
                        case GeneratePageBuilderInfoAction:
                            // Important: Because we are using ngxs, so all objects have immutable, we should create a shallow copy instead of using directly
                            const temp = result.processDynamicForm.formBuilder.formSections as Array<ExtendedPageSection>

                            this.pageSections = []
                            temp?.forEach((section: ExtendedPageSection) => {

                                this.pageSections.push({
                                    ...section
                                })
                            })
                            this.cd.markForCheck()
                            break
                        case NextToWorkflowAction:
                        case NextToDatasourceAction:
                            this.store.dispatch([
                                new UpdatePageBuilderInfoAction({
                                    sections: ObjectUtils.clone(this.pageSections)
                                }),
                                new UpdateAvailableEvents(this.generateAvailableEvents()),
                                new UpdateAvailableBoundDatas(this.generateBoundDatas()),
                                new UpdateAvailableShells(this.generateAvailableFormShells()),
                                new UpdateAvailableTriggerEventsList(this.generateAvailableTriggerEvents())
                            ])
                            break
                        case GatherAllChanges:
                            this.store.dispatch(
                                new UpdatePageBuilderInfoAction({
                                    sections: ObjectUtils.clone(this.pageSections)
                                }),
                            )
                            break
                    }

                })
            ).subscribe()
    }

    onRemove(formSection: ExtendedPageSection) {
        this.pageSections = ArrayUtils.removeOneItem(this.pageSections, a => a.id === formSection.id)
        this.logger.debug('after deleting section', this.pageSections)
        this.cd.detectChanges()
    }

    addNewSection() {
        const newSection: ExtendedPageSection = {
            id: Guid.create().toString(),
            name: '',
            displayName: '',
            constructionType: SectionContructionType.Standard,
            componentId: '',
            overrideOptions: [],
            hidden: 'false',
            rendered: 'true',
            order: this.pageSections ? this.pageSections.length : 0,
            sectionDatasource: {
                datasourceBindName: 'data'
            },
            relatedStandard: null,
            relatedDynamicList: null,
            relatedChart: null,
            relatedArrayStandard: null,
            relatedTreeStandard: null,
            relatedButtons: [],
            isLoaded: false,
            isBroken: false
        }
        const availableSectionNames = this.pageSections.map(a => a.name)
        const dialogRef = this.dialog.open(SectionDialogComponent, {
            data: {
                section: newSection,
                sectionNames: availableSectionNames
            }
         });
        const pageSectionsRef = this.pageSections
        dialogRef.afterClosed().subscribe((result: ExtendedPageSection) => {
            if (result) {
                this.logger.debug('is extensible', Object.isExtensible(result))
                pageSectionsRef.push({
                    ...result
                })
                this.logger.debug('is extensible after pushing', Object.isExtensible(pageSectionsRef))
                this.logger.debug('new section', this.pageSections)
                this.refreshTable()
            }
        })
    }

    editSection(choosenSection: ExtendedPageSection) {
        const availableSectionNames = this.pageSections.map(a => a.name)
        const dialogRef = this.dialog.open(SectionDialogComponent, {
            data: {
                section: choosenSection,
                sectionNames: availableSectionNames
            }
         });
        dialogRef.afterClosed().subscribe((result:ExtendedPageSection) => {
            if (result) {
                this.pageSections?.forEach((section) => {
                    if (section.id === result.id) {
                        section.name = result.name
                        section.displayName = result.displayName
                        section.hidden = result.hidden
                        section.rendered = result.rendered
                        section.constructionType = result.constructionType
                    }
                })
                this.refreshTable()
            }
        })
    }

    dropSection($event) {
        let droppedSectionId = '';

        this.pageSections?.forEach(section => {
            if (section.order === $event.previousIndex) {
                section.order = $event.currentIndex
                droppedSectionId = section.id
            }

            if (section.order === $event.currentIndex && section.id !== droppedSectionId) {
                section.order = $event.previousIndex
            }
        })
        this.logger.debug('current order before', this.pageSections)
        this.pageSections = this.pageSections.sort(section => section.order)
        this.logger.debug('current order', this.pageSections)        
        this.refreshTable()
    }

    refreshTable() {
        const shallowCopy = ObjectUtils.clone(this.pageSections)
        this.onSectionsChanged.emit(shallowCopy)
        this.store.dispatch(new UpdatePageBuilderInfoAction({
            sections: shallowCopy
        }))
        this.cd.markForCheck()
    }

    controlsChange($event) {
        this.store.dispatch(new UpdatePageBuilderInfoAction({
            sections: this.pageSections
        }))
    }

    generateAvailableEvents(): Array<string> {
        const events: Array<string> = []

        this.pageSections?.forEach((section: ExtendedPageSection) => {
            switch (section.constructionType) {
                case SectionContructionType.Standard:
                    section.relatedStandard.controls?.forEach((control: PageControl) => {
                        control.pageControlEvents?.forEach(e => {
                            events.push(`${section.name}_${control.name}_change`)
                        })
                    })
                    break
                case SectionContructionType.DynamicList:
                    events.push(`${section.name}_${section.relatedDynamicList.name}_search`)
                    break
            }
        })

        return events
    }

    generateAvailableTriggerEvents(): Array<string>{
        const events: Array<string> = []

        this.pageSections?.forEach((section: ExtendedPageSection) => {
            switch (section.constructionType) {
                case SectionContructionType.Standard:
                    section.relatedStandard.controls?.forEach( (control: PageControl) => {
                        control.pageControlEvents?.forEach( e => {
                            switch(control.type){
                                case ControlType.Select:
                                case ControlType.AutoComplete:
                                    events.push(`${section.name}_${control.name}_rebounddatasource`)
                                    events.push(`${section.name}_${control.name}_rebounddatasource`)
                                default:
                                    events.push(`${section.name}_${control.name}_change`)
                                    events.push(`${section.name}_${control.name}_rebound`)
                                    events.push(`${section.name}_${control.name}_clean`)
                                    events.push(`${section.name}_${control.name}_reset`)
                                    break
                            }
                        })
                    })
                    break
                case SectionContructionType.DynamicList:
                    events.push(`${section.name}_${section.relatedDynamicList.name}_refetch`)
                    break
            }
        })

        return events
    }

    generateBoundDatas(): string[] {
        const boundDatas: string[] = []
        this.pageSections?.forEach((section: ExtendedPageSection) => {
            switch (section.constructionType) {
                case SectionContructionType.Standard:
                    section.relatedStandard.controls?.forEach((control: PageControl) => {
                        boundDatas.push(`${section.name}_${control.name}`)
                    })
                    break
            }
        })

        return boundDatas
    }

    generateAvailableFormShells(): Array<string> {
        const shellVars: Array<string> = []
        this.logger.debug('Generating Shells', shellVars)
        return shellVars
    }
}
