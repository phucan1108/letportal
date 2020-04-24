import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { ExtendedPageSection, GroupControls } from 'app/core/models/extended.models';
import { NGXLogger } from 'ngx-logger';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Store } from '@ngxs/store';
import { CustomHttpService } from 'services/customhttp.service';
import { PageService } from 'services/page.service';
import { PageStateModel } from 'stores/pages/page.state';
import { Observable, Subscription } from 'rxjs';
import { filter, tap } from 'rxjs/operators';
import { EndRenderingPageSectionsAction, GatherSectionValidations, SectionValidationStateAction, AddSectionBoundData, BeginBuildingBoundData, AddSectionBoundDataForStandardArray, OpenInsertDialogForStandardArray, CloseDialogForStandardArray, InsertOneItemForStandardArray, UpdateOneItemForStandardArray, RemoveOneItemForStandardArray } from 'stores/pages/page.actions';
import { PageLoadedDatasource, PageRenderedControl, DefaultControlOptions, MapDataControl } from 'app/core/models/page.model';
import { StandardOptions } from 'portal/modules/models/standard.extended.model';
import { ObjectUtils } from 'app/core/utils/object-util';
import { StandardArrayTableHeader } from './models/standard-array.models';
import { StandardComponent } from 'services/portal.service';
import { StandardSharedService } from './services/standard-shared.service';
import { MatDialog } from '@angular/material/dialog';
import { MatTable } from '@angular/material/table'
import { StandardArrayDialog } from './standard-array-dialog.component';
import { ArrayUtils } from 'app/core/utils/array-util';
import { FormUtil } from 'app/core/utils/form-util';

@Component({
    selector: 'let-standard-array-render',
    templateUrl: './standard-array-render.component.html',
    styleUrls: ['./standard-array-render.component.scss']
})
export class StandardArrayRenderComponent implements OnInit {
    @ViewChild('matTable', { static: false })
    private matTable: MatTable<any>;

    @Input()
    section: ExtendedPageSection

    controlsGroups: Array<GroupControls>
    controls: Array<PageRenderedControl<DefaultControlOptions>>

    standard: StandardComponent
    pageState$: Observable<PageStateModel>
    subscription: Subscription
    datasources: PageLoadedDatasource[]
    readyToRender = false
    standardArrayOptions: StandardOptions

    headers: StandardArrayTableHeader[] = []
    displayedColumns: string[] = []
    tableData: any = new Object()

    idField: string
    ids: any[]
    cloneOneItem: any
    sectionMap: MapDataControl[] = []
    formGroup: FormGroup

    constructor(
        private dialog: MatDialog,
        private logger: NGXLogger,
        private fb: FormBuilder,
        private store: Store,
        private pageService: PageService,
        private standardSharedService: StandardSharedService
    ) { }

    ngOnInit(): void {
        this.standardArrayOptions = StandardOptions.getStandardOptions(this.section.relatedArrayStandard.options)
        this.standard = this.section.relatedArrayStandard
        this.controls = this.standardSharedService
            .buildControlOptions(this.section.relatedArrayStandard.controls as PageRenderedControl<DefaultControlOptions>[])
            .filter(control => {
                return !control.defaultOptions.checkedHidden
            })
        this.pageState$ = this.store.select<PageStateModel>(state => state.page)
        this.subscription = this.pageState$.pipe(
            filter(state => state.filterState
                && (state.filterState === EndRenderingPageSectionsAction
                    || state.filterState === GatherSectionValidations
                    || state.filterState === UpdateOneItemForStandardArray
                    || state.filterState === InsertOneItemForStandardArray
                    || state.filterState === RemoveOneItemForStandardArray)),
            tap(
                state => {
                    switch (state.filterState) {
                        case EndRenderingPageSectionsAction:
                            this.logger.debug('Rendered hit')
                            this.datasources = state.datasources
                            let boundData = this.standardSharedService
                                .buildDataArray(this.section.sectionDatasource.datasourceBindName, this.datasources)
                            this.tableData = this.standardSharedService
                                .buildDataArrayForTable(
                                    boundData,
                                    this.standard,
                                    this.standardArrayOptions,
                                    (idField, ids, cloneData) => {
                                        this.idField = idField,
                                            this.ids = ids
                                        this.cloneOneItem = cloneData
                                    })
                            this.buildArrayTableHeaders()
                            this.formGroup = this.standardSharedService
                                .buildFormGroups(
                                    this.section.name,
                                    this.standard,
                                    this.cloneOneItem,
                                    true,
                                    null,
                                    (builtData, keptData, sectionMap) => {
                                        this.sectionMap = sectionMap
                                    })
                            this.controlsGroups = this.standardSharedService
                                .buildControlsGroup(
                                    this.controls,
                                    2)
                            this.readyToRender = true
                            this.store.dispatch(new AddSectionBoundDataForStandardArray({
                                name: this.section.name,
                                isKeptDataName: true,
                                data: this.tableData,
                                allowUpdateParts: this.standardArrayOptions.allowupdateparts
                            }))
                            break
                        case GatherSectionValidations:
                            if (state.specificValidatingSection === this.section.name
                                || !ObjectUtils.isNotNull(state.specificValidatingSection)) {
                                this.store.dispatch(new SectionValidationStateAction(this.section.name, true))
                            }
                            break
                        case UpdateOneItemForStandardArray:
                            if (state.lastStandardArrayItem.sectionName === this.section.name) {
                                this.tableData = ArrayUtils.updateOneItem(
                                    ObjectUtils.clone(
                                        this.tableData),
                                    state.lastStandardArrayItem.data,
                                    a => a[this.idField] === state.lastStandardArrayItem.data[this.idField])
                                this.matTable.renderRows()
                            }
                            break
                        case InsertOneItemForStandardArray:
                            if (state.lastStandardArrayItem.sectionName === this.section.name) {
                                this.tableData = ObjectUtils.clone(this.tableData)
                                this.tableData.push(state.lastStandardArrayItem.data)
                                this.ids.push(state.lastStandardArrayItem.data[this.idField])
                                this.matTable.renderRows()
                            }
                            break
                        case RemoveOneItemForStandardArray:
                            if (state.lastStandardArrayItem.sectionName === this.section.name) {
                                ArrayUtils.removeOneItem(this.ids, a => a === state.lastStandardArrayItem.data[this.idField])
                            }
                            break
                    }
                }
            )
        ).subscribe()
    }

    buildArrayTableHeaders() {
        if (ObjectUtils.isNotNull(this.standardArrayOptions.namefield)) {
            const arrayColumns = this.standardArrayOptions.namefield.split(';')
            arrayColumns.forEach(colName => {
                try {
                    const control = this.controls.find(a => a.name === colName)
                    const displayName = control.defaultOptions.label
                    this.headers.push({
                        name: control.defaultOptions.bindname,
                        displayName: displayName
                    })
                    this.displayedColumns.push(colName)
                }
                catch (ex) {
                    this.logger.error('Error with name field ' + colName, ex)
                }
            })

            if (this.standardArrayOptions.allowadjustment) {
                this.displayedColumns.push('actions')
            }
        }
    }

    add() {
        this.store.dispatch(new OpenInsertDialogForStandardArray({
            sectionName: this.section.name,
            data: this.cloneOneItem,
            identityKey: this.idField,
            sectionMap: this.sectionMap,
            allowUpdateParts: this.standardArrayOptions.allowupdateparts
        }))
        this.formGroup = null
        if (this.idField !== 'uniq_id') {
            this.formGroup = this.standardSharedService
                .buildFormGroups(
                    this.section.name,
                    this.standard,
                    this.cloneOneItem,
                    true,
                    [
                        {
                            controlName: this.standardArrayOptions.identityfield,
                            validators: [
                                FormUtil.isExist(this.ids, null)
                            ],
                            customErrorMessages: [
                                {
                                    errorName: 'isExist',
                                    errorMessage: 'This value has been used by another'
                                }
                            ]
                        }
                    ],
                    null)
        }
        else {
            this.formGroup = this.standardSharedService
                .buildFormGroups(
                    this.section.name,
                    this.standard,
                    this.cloneOneItem,
                    true,
                    null,
                    null)
        }

        const dialogRef = this.dialog.open(StandardArrayDialog, {
            data: {
                controlsGroups: this.controlsGroups,
                formGroup: this.formGroup,
                section: this.section,
                isEdit: false,
                ids: this.ids,
                uniqueControl: this.standardArrayOptions.identityfield
            }
        })
        dialogRef.afterClosed().subscribe(res => {
            if (!!res) {
                this.store.dispatch(new InsertOneItemForStandardArray({
                    sectionName: this.section.name,
                    isKeptDataName: true,
                    allowUpdateParts: this.standardArrayOptions.allowupdateparts
                }))
            }
            this.store.dispatch(new CloseDialogForStandardArray())
        })
    }

    edit($event, element: any) {
        this.store.dispatch(new OpenInsertDialogForStandardArray({
            sectionName: this.section.name,
            data: element,
            identityKey: this.idField,
            sectionMap: this.sectionMap,
            allowUpdateParts: this.standardArrayOptions.allowupdateparts
        }))
        this.formGroup = null
        if (this.idField !== 'uniq_id') {
            this.formGroup = this.standardSharedService
                .buildFormGroups(
                    this.section.name,
                    this.standard,
                    element,
                    true,
                    [
                        {
                            controlName: this.standardArrayOptions.identityfield,
                            validators: [
                                FormUtil.isExist(this.ids, element[this.idField])
                            ],
                            customErrorMessages: [
                                {
                                    errorName: 'isExist',
                                    errorMessage: 'This value has been used by another'
                                }
                            ]
                        }
                    ],
                    null)
        }
        else {
            this.formGroup = this.standardSharedService
                .buildFormGroups(
                    this.section.name,
                    this.standard,
                    element,
                    true,
                    null,
                    null)
        }

        const dialogRef = this.dialog.open(StandardArrayDialog, {
            data: {
                controlsGroups: this.controlsGroups,
                formGroup: this.formGroup,
                section: this.section,
                isEdit: true,
                ids: this.ids,
                uniqueControl: this.standardArrayOptions.identityfield
            }
        })
        dialogRef.afterClosed().subscribe(res => {
            if (!!res) {
                this.store.dispatch(new UpdateOneItemForStandardArray({
                    sectionName: this.section.name,
                    identityKey: this.idField,
                    isKeptDataName: true,
                    allowUpdateParts: this.standardArrayOptions.allowupdateparts
                }))
            }

            this.store.dispatch(new CloseDialogForStandardArray())
        })
    }

    delete($event, element: any) {
        this.store.dispatch(new RemoveOneItemForStandardArray({
            sectionName: this.section.name,
            identityKey: this.idField,
            isKeptDataName: true,
            removeItemKey: element[this.idField],
            allowUpdateParts: this.standardArrayOptions.allowupdateparts
        }))

        this.tableData = ArrayUtils.removeOneItem(ObjectUtils.clone(this.tableData), a => a[this.idField] === element[this.idField])
        this.matTable.renderRows()
    }
}
