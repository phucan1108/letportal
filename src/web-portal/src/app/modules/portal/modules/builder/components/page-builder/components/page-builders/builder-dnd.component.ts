import { Component, OnInit, Input, ChangeDetectorRef } from '@angular/core';
import { ExtendedPageSection, ExtendedPageControl, ExtendedStandardComponent } from 'app/core/models/extended.models';
import { FormGroup } from '@angular/forms';
import * as _ from 'lodash';
import { MatDialog } from '@angular/material';
import { SectionDialogComponent } from './section-dialog.component';
import { Guid } from 'guid-typescript';
import { Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { filter, tap, combineLatest } from 'rxjs/operators';
import { ShellContants } from 'app/core/shell/shell.contants';
import { NGXLogger } from 'ngx-logger';
import { InitEditPageBuilderAction, GeneratePageBuilderInfoAction, NextToWorkflowAction, UpdatePageBuilderInfoAction, UpdateAvailableEvents, UpdateAvailableShells, NextToDatasourceAction, GatherAllChanges, UpdateAvailableBoundDatas } from 'stores/pages/pagebuilder.actions';
import { SectionContructionType, PageSection, StandardComponent, PageSectionLayoutType, StandardComponentClient, DynamicListClient, PageControl, DynamicList } from 'services/portal.service';
import { ArrayUtils } from 'app/core/utils/array-util';
import { ObjectUtils } from 'app/core/utils/object-util';

@Component({
    selector: 'let-builder-dnd',
    templateUrl: './builder-dnd.component.html'
})
export class BuilderDnDComponent implements OnInit {

    pageSections: Array<ExtendedPageSection> = []
    constructionType = SectionContructionType

    constructor(
        private standardsClient: StandardComponentClient,
        private dynamicListsClient: DynamicListClient,
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
                                _.forEach(tempEdit, (section: ExtendedPageSection) => {
                                    this.pageSections.push({
                                        ...section
                                    })
                                })
                                _.forEach(this.pageSections, (section: ExtendedPageSection) => {
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
                            _.forEach(temp, (section: ExtendedPageSection) => {

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
                                new UpdateAvailableShells(this.generateAvailableFormShells())
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
        let newSection: ExtendedPageSection = {
            id: Guid.create().toString(),
            name: '',
            displayName: '',
            constructionType: SectionContructionType.Standard,
            componentId: '',
            overrideOptions: [],
            order: this.pageSections ? this.pageSections.length : 0,
            sectionDatasource: {
                datasourceBindName: 'data'
            },
            relatedStandard: null,
            relatedDynamicList: null,
            isLoaded: false
        }
        const dialogRef = this.dialog.open(SectionDialogComponent, { data: newSection });
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
        const dialogRef = this.dialog.open(SectionDialogComponent, { data: choosenSection });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                _.forEach(this.pageSections, (section) => {
                    if (section.id === result.id) {
                        section.name = result.name
                        section.displayName = result.displayName
                    }
                })
                this.refreshTable()
            }
        })
    }

    dropSection($event) {
        let droppedSectionId = '';

        _.forEach(this.pageSections, section => {
            if (section.order === $event.previousIndex) {
                section.order = $event.currentIndex
                droppedSectionId = section.id
            }

            if (section.order === $event.currentIndex && section.id !== droppedSectionId) {
                section.order = $event.previousIndex
            }
        })
        this.logger.debug('current order before', this.pageSections)
        this.pageSections = _.sortBy(this.pageSections, [function (section: ExtendedPageSection) { return section.order }])
        this.logger.debug('current order', this.pageSections)
        //[this.formSections[$event.previousIndex], this.formSections[$event.currentIndex]] = [this.formSections[$event.currentIndex], this.formSections[$event.previousIndex]] ;
        this.refreshTable()
    }

    refreshTable() {
        let shallowCopy = ObjectUtils.clone(this.pageSections)
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
        let events: Array<string> = []

        _.forEach(this.pageSections, (section: ExtendedPageSection) => {
            switch (section.constructionType) {
                case SectionContructionType.Standard:
                    _.forEach(section.relatedStandard.controls, (control: PageControl) => {
                        _.forEach(control.pageControlEvents, e => {
                            events.push(`${section.name}_${e.eventName}`)
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
        let boundDatas: string[] = []
        _.forEach(this.pageSections, (section: ExtendedPageSection) => {
            switch (section.constructionType) {
                case SectionContructionType.Standard:
                    _.forEach(section.relatedStandard.controls, (control: PageControl) => {
                        boundDatas.push(`${section.name}_${control.name}`)
                    })
                    break
            }
        })

        return boundDatas
    }

    generateAvailableFormShells(): Array<string> {
        let shellVars: Array<string> = []
        _.forEach(this.pageSections, (section: ExtendedPageSection) => {
            // _.forEach(section.formControls, (control: ExtendedFormControl) => {
            //     let shellVar = `${ShellContants.FORM_DATA}.${section.name.toLowerCase()}`
            //     shellVar += `.${control.name.toLowerCase()}`
            //     shellVars.push(shellVar)
            // })
        })
        this.logger.debug('Generating Shells', shellVars)
        return shellVars
    }
}
