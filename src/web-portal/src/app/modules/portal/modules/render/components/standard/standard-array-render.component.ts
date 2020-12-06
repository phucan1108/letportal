import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatTable } from '@angular/material/table';
import { Store } from '@ngxs/store';
import { SelectBoundControl } from 'app/core/context/bound-control';
import { ArrayBoundSection, StandardBoundSection } from 'app/core/context/bound-section';
import { ExtendedPageSection, GroupControls } from 'app/core/models/extended.models';
import { DefaultControlOptions, MapDataControl, PageLoadedDatasource, PageRenderedControl } from 'app/core/models/page.model';
import { ArrayUtils } from 'app/core/utils/array-util';
import { FormUtil } from 'app/core/utils/form-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { ArrayStandardOptions } from 'portal/modules/models/standard.extended.model';
import { Observable, of, Subscription } from 'rxjs';
import { filter, tap } from 'rxjs/operators';
import { PageService } from 'services/page.service';
import { ControlType, DatasourceControlType, StandardComponent } from 'services/portal.service';
import { AddSectionBoundDataForStandardArray, BeginBuildingBoundData, CloseDialogForStandardArray, EndRenderingPageSectionsAction, GatherSectionValidations, InsertOneItemForStandardArray, OpenInsertDialogForStandardArray, RemoveOneItemForStandardArray, SectionValidationStateAction, UpdateOneItemForStandardArray } from 'stores/pages/page.actions';
import { PageStateModel } from 'stores/pages/page.state';
import { StandardArrayTableHeader } from './models/standard-array.models';
import { StandardSharedService } from './services/standard-shared.service';
import { StandardArrayDialog } from './standard-array-dialog.component';

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

    @Input()
    boundSection: ArrayBoundSection
    controlsGroups: Array<GroupControls>
    controls: Array<PageRenderedControl<DefaultControlOptions>>

    standard: StandardComponent
    pageState$: Observable<PageStateModel>
    subscription: Subscription
    datasources: PageLoadedDatasource[]
    readyToRender = false
    standardArrayOptions: ArrayStandardOptions

    headers: StandardArrayTableHeader[] = []
    displayedColumns: string[] = []
    tableData: any = new Object()

    idField: string
    ids: any[]
    cloneOneItem: any
    sectionMap: MapDataControl[] = []
    formGroup: FormGroup

    storeName: string
    constructor(
        private dialog: MatDialog,
        private logger: NGXLogger,
        private fb: FormBuilder,
        private store: Store,
        private pageService: PageService,
        private standardSharedService: StandardSharedService
    ) { }

    ngOnInit(): void {
        this.standardArrayOptions = ArrayStandardOptions.getStandardOptions(this.section.relatedArrayStandard.options)
        this.standard = this.section.relatedArrayStandard
        this.controls = this.standardSharedService
            .buildControlOptions(this.section.relatedArrayStandard.controls as PageRenderedControl<DefaultControlOptions>[])
            .filter(control => {
                return control.defaultOptions.checkRendered
            })
        this.section.relatedArrayStandard.controls = this.controls
        this.storeName =
            ObjectUtils.isNotNull(this.section.sectionDatasource.dataStoreName) ?
                this.section.sectionDatasource.dataStoreName
                : this.section.name
        this.pageState$ = this.store.select<PageStateModel>(state => state.page)
        this.subscription = this.pageState$.pipe(
            filter(state => state.filterState
                && (state.filterState === EndRenderingPageSectionsAction
                    || state.filterState === BeginBuildingBoundData
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
                            const openedSection = this.standardSharedService
                                .buildBoundSection(
                                    this.section.name,
                                    // We should use data mode because storeName can't be applied
                                    null,
                                    this.standard,
                                    this.cloneOneItem,
                                    null,
                                    (builtData, sectionMap) => {
                                        this.sectionMap = sectionMap
                                    }) as StandardBoundSection

                            this.boundSection.setOpenedSection(openedSection)
                            
                            openedSection.getAll().forEach(control => {
                                if (control.type === ControlType.Select || control.type === ControlType.AutoComplete || control.type === ControlType.Radio) {
                                    let foundControl = this.controls.find(a => a.name === control.name)
                                    this.generateOptions(foundControl, this.section.name).pipe(
                                        tap(res => {
                                            const boundControl = (control as SelectBoundControl)
                                            boundControl.bound(res)
                                            foundControl.boundControl = boundControl
                                        })
                                    ).subscribe()
                                }
                            })
                            this.formGroup = this.boundSection.getOpenedSection().getFormGroup()
                            this.controlsGroups = this.standardSharedService
                                .buildControlsGroup(
                                    this.controls,
                                    2)
                            break
                        case BeginBuildingBoundData:
                            this.store.dispatch(new AddSectionBoundDataForStandardArray({
                                storeName: this.storeName,
                                data: this.tableData,
                                allowUpdateParts: this.standardArrayOptions.allowupdateparts
                            }))
                            this.readyToRender = true
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

    generateOptions(control: PageRenderedControl<DefaultControlOptions>, sectionName: string): Observable<any> {
        switch (control.datasourceOptions.type) {
            case DatasourceControlType.StaticResource:
                return of(JSON.parse(control.datasourceOptions.datasourceStaticOptions.jsonResource))
            case DatasourceControlType.Database:
                const parameters = this.pageService.retrieveParameters(control.datasourceOptions.databaseOptions.query)
                return this.pageService
                    .fetchControlSelectionDatasource(
                        sectionName, 
                        control.name, 
                        control.compositeControlRefId,
                        ObjectUtils.isNotNull(control.compositeControlId),
                        parameters)
            case DatasourceControlType.WebService:
                return this.pageService.executeHttpWithBoundData(control.datasourceOptions.httpServiceOptions)
        }
    }

    buildArrayTableHeaders() {
        if (ObjectUtils.isNotNull(this.standardArrayOptions.namefield)) {
            const arrayColumns = this.standardArrayOptions.namefield.split(';')
            arrayColumns?.forEach(colName => {
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

    translateData(data: any, headerName: string) {
        const control = this.controls.find(a => a.defaultOptions.bindname === headerName)
        if (!!control && control.type === (ControlType.Select || ControlType.AutoComplete || ControlType.Radio)) {
            const boundControl = (control.boundControl as SelectBoundControl)
            return boundControl.getDs().find(a => a.value === data).name
        }
        return data
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
            this.boundSection.setOpenedSection(this.standardSharedService
                .buildBoundSection(
                    this.section.name,
                    null,
                    this.standard,
                    this.cloneOneItem,
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
                    null) as StandardBoundSection)
            this.formGroup = this.boundSection.getOpenedSection().getFormGroup()
        }
        else {
            this.boundSection.setOpenedSection(this.standardSharedService
                .buildBoundSection(
                    this.section.name,
                    null,
                    this.standard,
                    this.cloneOneItem,
                    null,
                    null) as StandardBoundSection)
            this.formGroup = this.boundSection.getOpenedSection().getFormGroup()
        }

        const dialogRef = this.dialog.open(StandardArrayDialog, {
            data: {
                controlsGroups: this.controlsGroups,
                formGroup: this.formGroup,
                section: this.section,
                isEdit: false,
                ids: this.ids,
                uniqueControl: this.standardArrayOptions.identityfield,
                boundSection: this.boundSection
            }
        })
        dialogRef.afterClosed().subscribe(res => {
            if (!!res) {
                this.store.dispatch(new InsertOneItemForStandardArray({
                    sectionName: this.section.name,
                    storeName: this.storeName,
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
            this.boundSection.setOpenedSection(this.standardSharedService
                .buildBoundSection(
                    this.section.name,
                    null,
                    this.standard,
                    element,
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
                    null) as StandardBoundSection)
            this.formGroup = this.boundSection.getOpenedSection().getFormGroup()
        }
        else {
            this.boundSection.setOpenedSection(this.standardSharedService
                .buildBoundSection(
                    this.storeName,
                    null,
                    this.standard,
                    element,
                    null,
                    null) as StandardBoundSection)
            this.formGroup = this.boundSection.getOpenedSection().getFormGroup()
        }

        const dialogRef = this.dialog.open(StandardArrayDialog, {
            data: {
                controlsGroups: this.controlsGroups,
                formGroup: this.formGroup,
                section: this.section,
                isEdit: true,
                ids: this.ids,
                uniqueControl: this.standardArrayOptions.identityfield,
                boundSection: this.boundSection
            }
        })
        dialogRef.afterClosed().subscribe(res => {
            if (!!res) {
                this.store.dispatch(new UpdateOneItemForStandardArray({
                    sectionName: this.section.name,
                    identityKey: this.idField,
                    storeName: this.storeName,
                    allowUpdateParts: this.standardArrayOptions.allowupdateparts
                }))
            }

            this.store.dispatch(new CloseDialogForStandardArray())
        })
    }

    delete($event, element: any) {
        this.store.dispatch(new RemoveOneItemForStandardArray({
            sectionName: this.section.name,
            removedItem: element,
            identityKey: this.idField,
            storeName: this.storeName,
            removeItemKey: element[this.idField],
            allowUpdateParts: this.standardArrayOptions.allowupdateparts
        }))

        this.tableData = ArrayUtils.removeOneItem(ObjectUtils.clone(this.tableData), a => a[this.idField] === element[this.idField])
        this.matTable.renderRows()
    }
}
