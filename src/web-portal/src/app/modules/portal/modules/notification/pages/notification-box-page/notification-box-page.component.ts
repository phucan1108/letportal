import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';
import { ChangeDetectorRef, Component, NgZone, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSelectionList } from '@angular/material/list';
import { TranslateService } from '@ngx-translate/core';
import { Actions, ofActionSuccessful, Store } from '@ngxs/store';
import { MessageGroup, NotificationMessage, NotificationType, OnlineSubcriber } from 'app/core/models/notification.model';
import { DateUtils } from 'app/core/utils/date-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { BehaviorSubject, Subject, Subscription, timer } from 'rxjs';
import { debounceTime, filter, map, pairwise, tap, throttleTime } from 'rxjs/operators';
import { NotificationService } from 'services/notification.service';
import { PageService } from 'services/page.service';
import { SubcribeToServer } from 'stores/notifications/notification.actions';
import { NOTIFICATION_STATE_TOKEN } from 'stores/notifications/notification.state';

@Component({
    selector: 'notification-box-page',
    templateUrl: './notification-box-page.component.html',
    styleUrls: ['./notification-box-page.component.scss']
})
export class NotificationBoxPage implements OnInit, OnDestroy {

    @ViewChild('messageGroupsSelection', { static: true })
    private messageGroupsSelection: MatSelectionList
    @ViewChild('scroller', { static: true }) scroller: CdkVirtualScrollViewport
    private formGroup: FormGroup

    private messageGroup$: BehaviorSubject<MessageGroup[]> = new BehaviorSubject<MessageGroup[]>([])
    private notificationMessages$: BehaviorSubject<NotificationMessage[]> = new BehaviorSubject<NotificationMessage[]>([])
    private onlineSubcriber: OnlineSubcriber
    private selectedGroup: MessageGroup
    private checkedInfo = true
    private checkedWarn = true
    private checkedCritical = true
    private disabled = false
    private fechingSub$ = new Subject<boolean>()

    private selectedNotificationType = NotificationType.Info
    private notificationTypes: NotificationType[] = [NotificationType.Info, NotificationType.Warning, NotificationType.Critical]    
    private selectedNotificationTypes: NotificationType[] = [NotificationType.Info, NotificationType.Warning, NotificationType.Critical] 
    private loading = false
    // Use this var for counting the scroll top behavior, it is in px
    private messageWidthSpace = 70
    // When user scrolls to top, this is a number of messages which allows to call to fetch more messages
    // It counts from top to the current item (in scrolling)
    private minimumMessagesForFetching = 3

    sup: Subscription = new Subscription()
    constructor(
        private notificationService: NotificationService,
        private pageService: PageService,
        private translate: TranslateService,
        private logger: NGXLogger,
        private cd: ChangeDetectorRef,
        private fb: FormBuilder,
        private store: Store,
        private actions$: Actions,
        private ngZone: NgZone
    ) {
    }
    ngOnDestroy(): void {
        this.sup.unsubscribe()
    }

    ngOnInit(): void {
        this.formGroup = this.fb.group({
            groupSearch: ['', [Validators.required]]
        })
        this.sup.add(
            this.actions$.pipe(
                ofActionSuccessful(SubcribeToServer),
                tap(res => {
                    this.onlineSubcriber = this.store.selectSnapshot(NOTIFICATION_STATE_TOKEN).onlineSubcriber
                    this.messageGroup$.next(this.onlineSubcriber.groups)
                })
            ).subscribe()
        )

        this.sup.add(this.scroller.elementScrolled().pipe(
            map(() => this.scroller.measureScrollOffset('top')),
            pairwise(),
            filter(([y1, y2]) => (y2 < y1 && y2 < (this.messageWidthSpace * this.minimumMessagesForFetching))), // y1: last YPos, y2: current YPos, in case scroll to top, only allow
            throttleTime(1000)
        ).subscribe(
            () => {
                this.ngZone.run(() => {
                    const currentLength = this.notificationMessages$.getValue().length
                    // Ensure we already have the data
                    if (currentLength > 0) {
                        this.loading = true
                        const subcriber = this.notificationService
                            .getMessages({
                                subcriberId: this.onlineSubcriber.subcriberId,
                                messageGroupId: this.selectedGroup.id,
                                lastFectchedTs: this.notificationMessages$.getValue()[0].receivedDateTs,
                                selectedTypes: this.getSelectedNotificationTypes()
                            })
                            .pipe(
                                tap(res => {
                                    if (!!res && res.length > 0) {
                                        const timerSubcriber = timer(1000).subscribe(() => {
                                            this.loading = false
                                            this.notificationMessages$.next([
                                                ...res,
                                                ...this.notificationMessages$.getValue(),
                                            ])
                                            setTimeout(() => {
                                                this.scroller.scrollToIndex(res.length + this.minimumMessagesForFetching, 'smooth')
                                            }, 200)
                                            timerSubcriber.unsubscribe()
                                        })
                                    }
                                    else {
                                        this.loading = false
                                    }
                                    subcriber.unsubscribe()
                                })
                            ).subscribe()
                    }
                });
            }
        ))

        this.sup.add(this.fechingSub$.pipe(
            debounceTime(500),
            tap(pull => {
                if (pull) {
                    this.disabled = true
                    this.fetchedMessages(
                        this.onlineSubcriber.subcriberId,
                        this.selectedGroup,
                        this.getSelectedNotificationTypes(),
                        (messages) => {
                            if (ObjectUtils.isNotNull(messages)) {
                                this.notificationMessages$.next([...messages, this.selectedGroup.lastMessage])
                                setTimeout(() => {
                                    this.scroller.scrollTo({ bottom: 0, behavior: 'auto' })
                                }, 200)
                            }
                            else {
                                this.notificationMessages$.next([])
                            }
                        }
                    )
                }
            })
        ).subscribe())
    }

    getNotificationTypeString(type: NotificationType) {
        switch (type) {
            case NotificationType.Info:
                return 'Info'
            case NotificationType.Warning:
                return 'Warning'
            case NotificationType.Critical:
                return 'Critical'
        }
    }


    getPeriod(message: NotificationMessage) {
        return DateUtils.getPeriodLength(message.receivedDate, DateUtils.getUTCNow())
    }

    getIcon(message: NotificationMessage){
        switch(message.type){
            case NotificationType.Info:
                return 'info'
            case NotificationType.Warning:
                return 'warning'
            case NotificationType.Critical:
                return 'gpp_bad'
        }
    }

    hasUnreadMessageInGroup(group: MessageGroup) {
        return group.lastVisitedTs < group.lastMessage.receivedDateTs
    }

    onClickSelection() {
        this.selectedGroup = this.messageGroupsSelection.selectedOptions.selected[0]?.value
        this.fechingSub$.next(true)
    }

    onChangedCheckbox() {
        this.fechingSub$.next(true)
    }

    getSelectedNotificationTypes(): NotificationType[] {
        let selectedTypes: NotificationType[] = []
        if (this.checkedInfo) {
            selectedTypes.push(NotificationType.Info)
        }
        if (this.checkedWarn) {
            selectedTypes.push(NotificationType.Warning)
        }
        if (this.checkedCritical) {
            selectedTypes.push(NotificationType.Critical)
        }
        return selectedTypes
    }

    trackById(index: number, item: NotificationMessage) {
        return item.notificationBoxId
    }

    private fetchedMessages(subcriberId: string, selectedGroup: MessageGroup, selectedTypes: NotificationType[], postAction: (messages: NotificationMessage[]) => void) {
        const subcriber = this.notificationService
            .getMessages({
                subcriberId: subcriberId,
                messageGroupId: selectedGroup.id,
                lastFectchedTs: selectedGroup.lastMessage.receivedDateTs,
                selectedTypes: selectedTypes
            })
            .pipe(
                tap(res => {
                    if (!!postAction) {
                        postAction(res)
                    }

                    subcriber.unsubscribe()
                })
            ).subscribe()
    }
}
