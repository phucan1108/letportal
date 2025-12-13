import { Component, OnInit, Input, ViewChild, ChangeDetectorRef } from '@angular/core';
import { PageEventDialogComponent } from './page-event-dialog.component';
import { ArrayUtils } from 'app/core/utils/array-util';
import { Store } from '@ngxs/store';
import { filter, tap } from 'rxjs/operators';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { MessageType, ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
 
import { UpdateAvailableEvents, NextToWorkflowAction, NextToRouteAction, GeneratePageEventsAction, UpdatePageEventsAction, InitEditPageBuilderAction, GatherAllChanges, UpdateAvailableBoundDatas } from 'stores/pages/pagebuilder.actions';
import { PageEvent, EventActionType } from 'services/portal.service';
import { PageBuilderState, PageBuilderStateModel } from 'stores/pages/pagebuilder.state';
import { MatTable } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'let-page-event-grid',
    templateUrl: './page-event-grid.component.html'
})
export class PageEventGridComponent implements OnInit {
    @ViewChild('table', { static: true }) table: MatTable<PageEvent>;

    displayedListColumns = ['eventKey', 'sourceName', 'actions'];
    availableEvents: Array<string> = []
    availableTriggerEventsList: Array<string> = []
    availableBoundDatas: Array<string> = []

    currentEvents: Array<PageEvent> = []
    _eventActionTypes = [
        { name: 'Trigger Events', value: EventActionType.TriggerEvent },
        { name: 'Web Service', value: EventActionType.WebService }
    ]
    constructor(
        private translate: TranslateService,
        private shortcutUtil: ShortcutUtil,
        private cd: ChangeDetectorRef,
        public dialog: MatDialog,
        private store: Store
    ) { }

    ngOnInit(): void {
        this.store.select(state => state.pagebuilder)
            .pipe(
                filter(state => state.filterState && (
                    state.filterState === InitEditPageBuilderAction
                    || state.filterState === UpdateAvailableEvents
                    || state.filterState === GeneratePageEventsAction
                    || state.filterState === UpdateAvailableBoundDatas
                    || state.filterState === NextToWorkflowAction
                    || state.filterState === GatherAllChanges
                    || state.filterState === NextToRouteAction)),
                tap((state: PageBuilderStateModel) => {
                    this.availableEvents = state.availableEvents
                    this.availableBoundDatas = state.availableBoundDatas
                    this.availableTriggerEventsList = state.availableTriggerEventsList
                    switch (state.filterState) {
                        case NextToWorkflowAction:
                            this.currentEvents = []
                            state.processPage.events?.forEach(event => {
                                this.currentEvents.push({
                                    ...event
                                })
                            })

                            this.table.renderRows()
                            break
                        case GeneratePageEventsAction:
                            this.currentEvents = []
                            state.processPage.events?.forEach(event => {
                                this.currentEvents.push({
                                    ...event
                                })
                            })
                            this.table.renderRows()
                            break
                        case NextToRouteAction:
                        case GatherAllChanges:
                            this.store.dispatch(new UpdatePageEventsAction(
                                this.currentEvents
                            ))
                            break
                    }
                })
            ).subscribe()
    }

    addNewEvent() {
        const newEvent: PageEvent = {
            eventName: '',
            eventActionType: EventActionType.TriggerEvent,
            triggerEventOptions: {
                eventsList: []
            },
            httpServiceOptions: {
                boundData: [],
                httpMethod: 'Get',
                httpServiceUrl: '',
                httpSuccessCode: '200',
                jsonBody: '',
                outputProjection: ''
            }
        }

        const dialogRef = this.dialog.open(PageEventDialogComponent, {
            data: {
                event: newEvent,
                isEdit: false,
                availableEvents: this.availableEvents,
                availableBoundDatas: this.availableBoundDatas,
                availableTriggerEvents: this.availableTriggerEventsList
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.currentEvents.push(result)
                this.table.renderRows()
            }
        })
    }

    translateEventActionType(event: PageEvent) {
        return this._eventActionTypes.find(e => e.value === event.eventActionType).name
    }

    editEvent(event: PageEvent) {
        const dialogRef = this.dialog.open(PageEventDialogComponent, {
            data: {
                event,
                isEdit: true,
                availableEvents: this.availableEvents,
                availableBoundDatas: this.availableBoundDatas,
                availableTriggerEvents: this.availableTriggerEventsList
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }
            this.currentEvents = ArrayUtils.updateOneItem(this.currentEvents, result, (event: PageEvent) => { return event.eventName === result.eventKey })
            this.table.renderRows()
        })
    }

    deleteEvent(event: PageEvent) {
        this.currentEvents = this.currentEvents.filter((elem) => {
            return elem.eventName !== event.eventName
        })
        this.shortcutUtil.toastMessage(this.translate.instant('common.deleteSuccessfully'), ToastType.Success);

        this.table.renderRows()
    }


}
