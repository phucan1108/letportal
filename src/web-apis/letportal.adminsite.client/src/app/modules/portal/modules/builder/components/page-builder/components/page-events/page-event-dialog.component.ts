import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { PageEventGridComponent } from './page-event-grid.component';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PageEvent, EventActionType, HttpServiceOptions } from 'services/portal.service';
import { Observable, BehaviorSubject } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
    selector: 'let-page-event-dialog',
    templateUrl: './page-event-dialog.component.html'
})
export class PageEventDialogComponent implements OnInit {

    currentEvent: PageEvent

    eventForm: FormGroup

    isEditMode = false
    isHttpOptionsValid = false
    httpOptions: HttpServiceOptions

    availableEvents$: Observable<Array<string>>
    availableEvents: string[] = []
    availableTriggerEvents: string[] = []
    availableBoundDatas: string[] = []
    _eventActionTypes = [
        { name: 'Trigger Events', value: EventActionType.TriggerEvent },
        { name: 'Web Service', value: EventActionType.WebService }
    ]
    eventActionType = EventActionType
    constructor(
        public dialogRef: MatDialogRef<PageEventGridComponent>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef
    ) { }

    ngOnInit(
    ): void {
        this.currentEvent = this.data.event
        this.isEditMode = this.data.isEdit
        this.availableEvents = this.data.availableEvents
        if(!this.availableEvents){
            this.availableEvents = []
        }
        this.availableBoundDatas = this.data.availableBoundDatas
        if(!this.availableBoundDatas){
            this.availableBoundDatas = []
        }

        this.availableTriggerEvents = this.data.availableTriggerEvents
        if(!this.availableTriggerEvents){
            this.availableTriggerEvents = []
        }
        this.httpOptions = this.currentEvent.httpServiceOptions
        this.initialEventForm()
        this.populateValueChanges()
    }

    initialEventForm() {
        this.eventForm = this.fb.group({
            eventName: [this.currentEvent.eventName, Validators.required],
            eventActionType: [this.currentEvent.eventActionType, Validators.required],
            boundData: [this.currentEvent.httpServiceOptions.boundData],
            triggerEventsList: [this.currentEvent.triggerEventOptions.eventsList]
        })
    }

    populateValueChanges() {
        this.availableEvents$ = this.eventForm.get('eventName').valueChanges
            .pipe(
                startWith(''),
                map(value => this._filter(value))
            )
    }

    private _filter(value: string): Array<string> {
        const filterValue = value.toLowerCase();

        return this.availableEvents.filter(option => option.toLowerCase().includes(filterValue))
    }

    eventSelected($event: MatAutocompleteSelectedEvent) {
    }

    onSubmit() {
        const formValues = this.eventForm.value
        if((this.isHttpOptionsValid && formValues.eventActionType === EventActionType.WebService) || this.eventForm.valid){
            this.dialogRef.close(this.combinePageEvent())
        }
    }

    combinePageEvent(){
        const formValues = this.eventForm.value
        const formEvent: PageEvent = {
            eventName: formValues.eventName,
            eventActionType: formValues.eventActionType,
            triggerEventOptions: {
                eventsList: formValues.triggerEventsList
            },
            httpServiceOptions: this.isHttpOptionsValid && formValues.eventActionType === EventActionType.WebService ?
            {
                ...this.httpOptions,
                boundData: formValues.boundData
            } : {
                ...this.currentEvent.httpServiceOptions,
                boundData: formValues.boundData
            }
        }
        return formEvent
    }

    onCheckingHttpOptionsValid($event) {
        this.isHttpOptionsValid = $event
    }

    onChangeHttpOptions($event) {
        this.httpOptions = $event
    }
}
