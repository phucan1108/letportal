import { Component, OnInit, Input, ChangeDetectorRef, ViewChild, Output, EventEmitter } from '@angular/core';
import { ExtendedPageSection, ExtendedPageControl, ExtendedStandardComponent } from 'app/core/models/extended.models';
import { SelectionModel } from '@angular/cdk/collections';
import { BehaviorSubject, Observable } from 'rxjs';
import * as _ from 'lodash';
import { Guid } from 'guid-typescript';
import { StaticResources } from 'portal/resources/static-resources';
import { MatDialog, MatTable } from '@angular/material';
import { ControlDialogComponent } from './control-dialog.component';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { ArrayUtils } from 'app/core/utils/array-util';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { MessageType, ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { NGXLogger } from 'ngx-logger';
import { ControlType, PageControl, StandardComponent, ShellOption, DatasourceControlType } from 'services/portal.service';
import { slideInRightOnEnterAnimation, slideOutRightOnLeaveAnimation, slideInLeftOnEnterAnimation, slideOutLeftOnLeaveAnimation } from 'angular-animations';
import { trigger, transition, state, style, animate } from '@angular/animations';
import { DatasourceOptionsDialogComponent } from 'portal/shared/datasourceopts/datasourceopts.component';
import { ControlEventsDialogComponent } from './control-events.dialog.component';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { tap } from 'rxjs/operators';
import { ObjectUtils } from 'app/core/utils/object-util';
import { AsyncValidatorDialogComponent } from './control-async-validator.dialog.component';

@Component({
    selector: 'let-controls-grid',
    templateUrl: './controls-grid.component.html',
    animations: [
        trigger('slideInRight', [
            state('in', style({ transform: 'translateX(0)' })),
            transition(':enter', [
                style({ transform: 'translateX(-100%)' }),
                animate(2000)
            ]),
            transition(':leave', [
                animate(2000, style({ transform: 'translateX(100%)' }))
            ])
        ]),
        slideInRightOnEnterAnimation(),
        slideOutRightOnLeaveAnimation(),
        slideInLeftOnEnterAnimation(),
        slideOutLeftOnLeaveAnimation()
    ]
})
export class ControlsGridComponent implements OnInit {
    @ViewChild('table') table: MatTable<ExtendedPageControl>;

    controls: Array<ExtendedPageControl>

    @Input()
    controls$: Observable<ExtendedPageControl[]>

    selection = new SelectionModel<ExtendedPageControl>(true, []);
    displayedControlsInListColumns = ['select', 'label', 'controlType', 'bindname', 'actions'];

    _controlTypes = StaticResources.controlTypes()

    @Output()
    changed = new EventEmitter<any>();

    isEditControl = false
    isHandset = false
    constructor(
        private shortcutUtil: ShortcutUtil,
        private cd: ChangeDetectorRef,
        private breakpointObserver: BreakpointObserver,
        private dialog: MatDialog,
        private logger: NGXLogger
    ) {
        this.breakpointObserver.observe(Breakpoints.Handset)
            .pipe(
                tap(result => {
                    if (result.matches) {
                        this.isHandset = true
                    }
                    else {
                        this.isHandset = false
                    }
                })
            ).subscribe()
    }

    ngOnInit(): void {
        this.controls$.subscribe(controls => {
            this.controls = controls
        })
    }

    addNewControl() {
        let newControl: ExtendedPageControl = {
            id: Guid.create().toString(),
            name: '',
            type: ControlType.Textbox,
            order: this.controls.length,
            isActive: true,
            options: [],
            validators: [],
            readOnlyMode: true,
            value: '',
            datasourceOptions: {
                type: DatasourceControlType.StaticResource,
                datasourceStaticOptions: {
                    jsonResource: '[]'
                },
                databaseOptions: {
                    databaseConnectionId: '',
                    entityName: '',
                    query: ''
                },
                httpServiceOptions: {
                    httpMethod: 'Get',
                    httpServiceUrl: '',
                    httpSuccessCode: '200',
                    outputProjection: '{}',
                    jsonBody: '{}'
                },
                triggeredEvents: ''
            }
        }
        this.isEditControl = false
        const dialogRef = this.dialog.open(ControlDialogComponent, {
            disableClose: true,
            data: {
                control: newControl,
                names: this.getAllAvailableControlNames()
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.controls.push(result)
                this.refreshControlTable()
            }
        })
    }

    /** SELECTION */
    isAllSelected(): boolean {
        const numSelected = this.selection.selected.length;
        const numRows = this.controls.length;
        return numSelected === numRows;
    }

    masterToggle() {
        if (this.selection.selected.length === this.controls.length) {
            this.selection.clear();
        } else {
            this.controls.forEach(row => this.selection.select(row));
        }
    }

    deleteSelectedControls() {
        const _title = "Delete Controls"
        const _description = "Are you sure to delete all selected controls?"
        const _waitDesciption = "Waiting..."
        const dialogRef = this.shortcutUtil.actionEntityElement(_title, _description, _waitDesciption);
        dialogRef.afterClosed().subscribe(res => {
            if (!res) {
                return;
            }

            for (let i = 0; i < this.selection.selected.length; i++) {
                this.controls = _.remove(this.controls, (elem: ExtendedPageControl) => {
                    return elem.id === this.selection.selected[i].id
                })
            }
            this.refreshControlTable();
        });
    }

    /** Table */
    translateControlType(controlType: ControlType) {
        let controlText = ''
        _.forEach(this._controlTypes, control => {
            if (control.value === controlType) {
                controlText = control.name
                return false;
            }
        })

        return controlText;
    }

    getBindName(options: ShellOption[]) {
        let found = _.find(options, opt => opt.key === 'bindname')
        if (!!found)
            return found.value
        return ''
    }

    allowEditDatasource(control: PageControl) {
        if (control.type === ControlType.AutoComplete
            || control.type === ControlType.Select
            || control.type === ControlType.Radio)
            return true
        return false
    }

    editControlDatasource(control: ExtendedPageControl) {
        let newDatasource = control.datasourceOptions
        this.logger.debug('editting datasource', newDatasource)
        if (!newDatasource) {
            newDatasource = {
                type: DatasourceControlType.StaticResource
            }
        }
        const dialogRef = this.dialog.open(DatasourceOptionsDialogComponent, {
            disableClose: true,
            data: {
                datasourceOption: newDatasource
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                control.datasourceOptions = result
                this.logger.debug('after changed datasource', this.controls)
            }
        })
    }

    ifHasEvents(control: ExtendedPageControl) {
        return control.pageControlEvents ? control.pageControlEvents.length > 0 : false
    }

    editEvents(control: ExtendedPageControl) {
        const dialogRef = this.dialog.open(ControlEventsDialogComponent, {
            disableClose: true,
            data: {
                control: control,
                availabelEvents: this.generateFunctionEventsList(this.controls),
                availableBoundDatas: this.getAvailableBoundData()
            }
        })
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                control.pageControlEvents = result
                this.logger.debug('after changed events', this.controls)
            }
        })
    }

    editAsyncValidators(control: ExtendedPageControl) {
        const dialogRef = this.dialog.open(AsyncValidatorDialogComponent, {
            disableClose: true,
            data: {
                control: control
            }
        })
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                control.asyncValidators = result
            }
        })
    }

    generateFunctionEventsList(controls: PageControl[]): string[] {
        let events: string[] = []

        _.forEach(controls, control => {
            switch (control.type) {
                case ControlType.Textbox:
                case ControlType.Number:
                case ControlType.DateTime:
                case ControlType.IconPicker:
                case ControlType.Select:
                case ControlType.AutoComplete:
                    events.push(`${control.name}_reset`)
                    events.push(`${control.name}_clean`)
                    events.push(`${control.name}_rebound`)
                    break
                case ControlType.Radio:
                case ControlType.Slide:
                    events.push(`${control.name}_reset`)
                    events.push(`${control.name}_rebound`)
                    break
                case ControlType.Label:
                    events.push(`${control.name}_rebound`)
                    break
            }
        })

        return events
    }

    getAvailableEvents(): string[] {
        let availableEvents: string[] = []
        _.forEach(this.controls, control => {
            _.forEach(control.pageControlEvents, event => {
                availableEvents.push(event.eventName)
            })
        })

        return availableEvents
    }

    getAvailableBoundData(): string[] {
        let availableBoundDatas: string[] = []
        _.forEach(this.controls, control => {
            availableBoundDatas.push(`${control.name}`)
        })

        return availableBoundDatas
    }

    onListDrop($event: CdkDragDrop<Array<ExtendedPageControl>>) {
        const prevIndex = this.controls.findIndex((control) => control === $event.item.data)
        moveItemInArray(this.controls, prevIndex, $event.currentIndex);
        this.refreshControlTable()
    }
    onListDropExpansion($event: CdkDragDrop<Array<ExtendedPageControl>>) {
        const prevIndex = this.controls.findIndex((control) => control === $event.item.data)
        moveItemInArray(this.controls, prevIndex, $event.currentIndex);
        this.changed.emit(this.controls)
    }

    editControl(control: ExtendedPageControl) {
        this.isEditControl = true
        const dialogRef = this.dialog.open(ControlDialogComponent, {
            disableClose: true,
            data: {
                control: ObjectUtils.clone(control),
                names: this.getAllAvailableControlNames()
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }
            this.controls = ArrayUtils.updateOneItem(this.controls, result, (control: ExtendedPageControl) => { return control.id === result.id })
            this.refreshControlTable()
        })
    }

    deleteControl(control: ExtendedPageControl) {
        this.controls = _.filter(this.controls, (elem) => {
            return elem.id !== control.id
        })
        this.shortcutUtil.notifyMessage('Delete control successfully!', ToastType.Success);
        this.refreshControlTable()
    }

    refreshControlTable() {
        if (!this.isHandset) {
            this.selection.clear();
            this.changed.emit(this.controls)
            this.table.renderRows()
        }
        else {

            this.changed.emit(this.controls)
        }
    }

    getAllAvailableControlNames(): string[] {
        let names: string[] = []
        _.forEach(this.controls, control => {
            names.push(control.name)
        })
        return names
    }
}
