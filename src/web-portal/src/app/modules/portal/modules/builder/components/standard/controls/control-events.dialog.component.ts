import { Component, OnInit, Inject, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material';
import { ControlsGridComponent } from './controls-grid.component';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NGXLogger } from 'ngx-logger';
import { ExtendedPageControl, ExtendedPageControlEvent } from 'app/core/models/extended.models';
import { EventActionType, PageControlEvent, HttpServiceOptions, PageControl, ControlType } from 'services/portal.service';
import { Guid } from 'guid-typescript';
import * as _ from 'lodash';
import { BehaviorSubject } from 'rxjs';
import { JsonEditorOptions } from 'ang-jsoneditor';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { tap } from 'rxjs/operators';

@Component({
    selector: 'let-controlevents-dialog',
    templateUrl: './control-events.dialog.component.html',
    styleUrls: ['./control-events.dialog.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ControlEventsDialogComponent implements OnInit {
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

    isHttpOptionsValid: boolean = false
    httpOptions: HttpServiceOptions

    availabelEvents: string[] = []
    availableBoundDatas: string[] = []

    _eventActionTypes = [
        { name: 'Trigger Events', value: EventActionType.TriggerEvent },
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
            boundData: [],
            triggerEventsList: []
        })
    }

    prepareEventsList(control: PageControl): ExtendedPageControlEvent[] {
        let events: ExtendedPageControlEvent[] = []
        _.forEach(control.pageControlEvents, event => {
            events.push({
                ...event,
                id: Guid.create().toString()
            })
        })

        return events;
    }

    translateEventActionType(event: ExtendedPageControlEvent) {
        return _.find(this._eventActionTypes, e => e.value === event.eventActionType).name
    }

    loadEventFormBySelectedEvent() {
        this.eventForm.clearValidators()
        this.eventForm.reset()

        this.eventForm = this.fb.group({
            eventName: [this.selectedEvent.eventName, Validators.required],
            eventActionType: [this.selectedEvent.eventActionType, Validators.required],
            boundData: [this.selectedEvent.httpServiceOptions.boundData],
            triggerEventsList: [this.selectedEvent.triggerEventOptions.eventsList]
        })

        this.httpOptions = this.selectedEvent.httpServiceOptions
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
        _.forEach(this.events, event => {
            if(event.id === this.selectedEvent.id){
                event.eventActionType = this.selectedEvent.eventActionType
                event.triggerEventOptions = this.selectedEvent.triggerEventOptions
                event.httpServiceOptions = this.selectedEvent.httpServiceOptions
                return false
            }
        })
        this.events$.next(this.events)
        this.isShowEditForm = false
    }

    combineEvent(): ExtendedPageControlEvent{
        const formValues = this.eventForm.value
        let event: ExtendedPageControlEvent = {
            id: this.selectedEvent.id,
            eventActionType: formValues.eventActionType,
            eventName: this.selectedEvent.eventName,
            httpServiceOptions: this.httpOptions ? this.httpOptions : this.selectedEvent.httpServiceOptions,
            triggerEventOptions: {
                eventsList: formValues.triggerEventsList
            }
        }

        event.httpServiceOptions.boundData = formValues.boundData
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
