import { Component, OnInit, Inject, ChangeDetectorRef, ChangeDetectionStrategy, ViewChild } from '@angular/core';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ControlsGridComponent } from './controls-grid.component';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NGXLogger } from 'ngx-logger';
import { ExtendedPageControl, ExtendedPageControlEvent } from 'app/core/models/extended.models';
import { EventActionType, PageControlEvent, HttpServiceOptions, PageControl, ControlType, SharedDatabaseOptions } from 'services/portal.service';
import { Guid } from 'guid-typescript';
 
import { BehaviorSubject } from 'rxjs';
import { JsonEditorOptions } from 'ang-jsoneditor';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { tap } from 'rxjs/operators';
import { DatabaseFormOptions, DatabaseOptionsComponent } from 'portal/shared/databaseoptions/databaseoptions.component';

@Component({
    selector: 'let-controlevents-dialog',
    templateUrl: './control-events.dialog.component.html',
    styleUrls: ['./control-events.dialog.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ControlEventsDialogComponent implements OnInit {
    @ViewChild(DatabaseOptionsComponent, { static: false }) dbOptionsComponent: DatabaseOptionsComponent
    public editorOptions: JsonEditorOptions
    query: any
    control: ExtendedPageControl

    events$: BehaviorSubject<ExtendedPageControlEvent[]> = new BehaviorSubject([])
    events: ExtendedPageControlEvent[] = []

    displayedColumns = ['eventName', 'eventActionType', 'actions']

    eventForm: FormGroup
    selectedEvent: ExtendedPageControlEvent

    eventActionType = EventActionType

    isShowEditForm = false

    isHttpOptionsValid = false
    httpOptions: HttpServiceOptions

    databaseOptions: SharedDatabaseOptions
    dbOptions: DatabaseFormOptions = {
        allowHints: ['query']
    }
    availabelEvents: string[] = []
    availableBoundDatas: string[] = []

    _eventActionTypes = [
        { name: 'Trigger Events', value: EventActionType.TriggerEvent },
        { name: 'Query Database', value: EventActionType.QueryDatabase },
        { name: 'Web Service', value: EventActionType.WebService }
    ]

    isHandset = false
    constructor(
        public dialogRef: MatDialogRef<ControlsGridComponent>,
        public dialog: MatDialog,
        private breakpointObserver: BreakpointObserver,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef,
        private logger: NGXLogger) {

        this.control = this.data.control
        this.availabelEvents = this.data.availabelEvents
        if(!this.availabelEvents){
            this.availabelEvents = []
        }
        this.availableBoundDatas = this.data.availableBoundDatas
        if(!this.availableBoundDatas){
            this.availableBoundDatas = []
        }

        this.logger.debug('current control', this.control)

        this.editorOptions = new JsonEditorOptions()
        this.editorOptions.mode = 'code'
        this.query = {}

        this.breakpointObserver.observe([Breakpoints.HandsetLandscape, Breakpoints.HandsetPortrait])
            .pipe(
                tap(result => {
                    if (result.matches) {
                        this.isHandset = true
                        this.cd.markForCheck()
                    }
                    else {
                        this.isHandset = false
                        this.cd.markForCheck()
                    }
                })
            ).subscribe()
    }

    ngOnInit(): void {
        this.initEventForm()
        this.events = this.prepareEventsList(this.control)
        this.events$.next(this.events)
    }

    initEventForm() {
        this.eventForm = this.fb.group({
            eventName: ['', Validators.required],
            eventActionType: [EventActionType.TriggerEvent, Validators.required],
            outputProjectionDatabase: [''],
            boundDataHttp: [],
            boundDataDatabase: [],
            triggerEventsList: []
        })
    }

    prepareEventsList(control: PageControl): ExtendedPageControlEvent[] {
        const events: ExtendedPageControlEvent[] = []
        control.pageControlEvents?.forEach(event => {
            events.push({
                ...event,
                id: Guid.create().toString()
            })
        })

        return events;
    }

    translateEventActionType(event: ExtendedPageControlEvent) {
        return this._eventActionTypes.find(e => e.value === event.eventActionType).name
    }

    loadEventFormBySelectedEvent() {
        this.eventForm.clearValidators()
        this.eventForm.reset()

        this.eventForm = this.fb.group({
            eventName: [this.selectedEvent.eventName, Validators.required],
            eventActionType: [this.selectedEvent.eventActionType, Validators.required],
            outputProjectionDatabase: [this.selectedEvent.eventDatabaseOptions.outputProjection],
            boundDataHttp: [this.selectedEvent.eventHttpServiceOptions.boundData],
            boundDataDatabase: [this.selectedEvent.eventDatabaseOptions.boundData],
            triggerEventsList: [this.selectedEvent.triggerEventOptions.eventsList]
        })

        this.httpOptions = this.selectedEvent.eventHttpServiceOptions
        this.databaseOptions = this.selectedEvent.eventDatabaseOptions
    }

    editEvent(event: ExtendedPageControlEvent) {
        this.selectedEvent = event
        this.isShowEditForm = true
        this.loadEventFormBySelectedEvent()
    }

    cancelEditEvent() {
        this.isShowEditForm = false
    }

    saveEvent() {
        this.selectedEvent = this.combineEvent()
        let formValid = false
        if(this.eventForm.valid){
            if(this.selectedEvent.eventActionType === EventActionType.QueryDatabase
                && this.dbOptionsComponent.isValid()){

                    const currentDbOptions = this.dbOptionsComponent.get()
                    this.selectedEvent.eventDatabaseOptions = {
                        ...this.selectedEvent.eventDatabaseOptions,
                        databaseConnectionId: currentDbOptions.databaseConnectionId,
                        query: currentDbOptions.query
                    }

                    formValid = true
                }
            else if(this.selectedEvent.eventActionType === EventActionType.WebService
                && this.isHttpOptionsValid){
                formValid = true
            }
            else if(this.selectedEvent.eventActionType === EventActionType.TriggerEvent){
                formValid = true
            }
        }

        if(formValid){
            this.events?.forEach(event => {
                if(event.id === this.selectedEvent.id){
                    event.eventActionType = this.selectedEvent.eventActionType
                    event.triggerEventOptions = this.selectedEvent.triggerEventOptions
                    event.eventHttpServiceOptions = this.selectedEvent.eventHttpServiceOptions
                    event.eventDatabaseOptions = this.selectedEvent.eventDatabaseOptions
                    return false
                }
            })
            this.events$.next(this.events)
            this.isShowEditForm = false
        }
    }

    combineEvent(): ExtendedPageControlEvent{
        const formValues = this.eventForm.value
        const event: ExtendedPageControlEvent = {
            id: this.selectedEvent.id,
            eventActionType: formValues.eventActionType,
            eventName: this.selectedEvent.eventName,
            eventHttpServiceOptions: this.httpOptions ? this.httpOptions : this.selectedEvent.eventHttpServiceOptions,
            eventDatabaseOptions: this.databaseOptions ? this.databaseOptions : this.selectedEvent.eventDatabaseOptions,
            triggerEventOptions: {
                eventsList: formValues.triggerEventsList
            }
        }

        event.eventHttpServiceOptions.boundData = formValues.boundDataHttp
        event.eventDatabaseOptions.boundData = formValues.boundDataDatabase
        event.eventDatabaseOptions.outputProjection = formValues.outputProjectionDatabase
        return event
    }

    onChangeHttpOptions($event) {
        this.httpOptions = $event
    }

    onCheckingHttpOptionsValid($event) {
        this.isHttpOptionsValid = $event
    }

    onSave() {
        this.dialogRef.close(this.events)
    }
}
