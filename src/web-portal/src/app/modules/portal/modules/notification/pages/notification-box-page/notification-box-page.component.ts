import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';
import { AfterViewInit, Component, NgZone, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatButtonToggleGroup } from '@angular/material/button-toggle';
import { MatSelectionList } from '@angular/material/list';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Actions, ofActionSuccessful, Store } from '@ngxs/store';
import { MessageGroup, NotificationMessage, NotificationType, NOTIFICATION_TYPE_RES, OnlineSubcriber } from 'app/core/models/notification.model';
import { DateUtils } from 'app/core/utils/date-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { BehaviorSubject, combineLatest, Subject, Subscription, timer } from 'rxjs';
import { debounceTime, filter, map, pairwise, tap, throttleTime } from 'rxjs/operators';
import { NotificationService } from 'services/notification.service';
import { PageService } from 'services/page.service';
import { ClickedOnMessageGroup, SubcribeToServer } from 'stores/notifications/notification.actions';
import { NOTIFICATION_STATE_TOKEN } from 'stores/notifications/notification.state';

@Component({
    selector: 'notification-box-page',
    templateUrl: './notification-box-page.component.html',
    styleUrls: ['./notification-box-page.component.scss']
})
export class NotificationBoxPage implements OnInit, OnDestroy, AfterViewInit {

    @ViewChild('messageGroupsSelection', { static: true })
    private messageGroupsSelection: MatSelectionList
    @ViewChild('toggleGroups', { static: true })
    private toggleGroups: MatButtonToggleGroup
    @ViewChild('scroller', { static: true }) scroller: CdkVirtualScrollViewport
    formGroup: FormGroup

    messageGroup$: BehaviorSubject<MessageGroup[]> = new BehaviorSubject<MessageGroup[]>([])
    notificationMessages$: BehaviorSubject<NotificationMessage[]> = new BehaviorSubject<NotificationMessage[]>([])
    onlineSubcriber: OnlineSubcriber
    selectedGroup: MessageGroup

    disabled = false
    fechingSub$ = new Subject<boolean>()

    notificationTypes = NOTIFICATION_TYPE_RES
    selectedNotificationTypes: NotificationType[] = []
    loading = false
    // Use this var for counting the scroll top behavior, it is in px
    private messageWidthSpace = 70
    // When user scrolls to top, this is a number of messages which allows to call to fetch more messages
    // It counts from top to the current item (in scrolling)
    private minimumMessagesForFetching = 3

    sup: Subscription = new Subscription()

    private defaultMessageGroupId = ''
    constructor(
        private notificationService: NotificationService,
        private pageService: PageService,
        private translate: TranslateService,
        private logger: NGXLogger,
        private activatedRoute: ActivatedRoute,
        private router: Router,
        private fb: FormBuilder,
        private store: Store,
        private actions$: Actions,
        private ngZone: NgZone
    ) {
    }

    ngAfterViewInit(): void {

    }

    ngOnDestroy(): void {
        this.sup.unsubscribe()
    }

    ngOnInit(): void {
        this.formGroup = this.fb.group({
            groupSearch: ['', [Validators.required]]
        })

        this.sup.add(
            combineLatest([this.activatedRoute.paramMap, this.actions$.pipe(ofActionSuccessful(SubcribeToServer))])
                .subscribe(
                    pair => {
                        this.onlineSubcriber = this.store.selectSnapshot(NOTIFICATION_STATE_TOKEN).onlineSubcriber                        
                        this.messageGroup$.next(this.onlineSubcriber.groups)
                        if (ObjectUtils.isNotNull(pair[0].get('messageGroupId'))) {
                            this.defaultMessageGroupId = pair[0].get('messageGroupId')
                            setTimeout(() => {
                                let foundIndex = this.onlineSubcriber.groups.findIndex(a => a.id === this.defaultMessageGroupId)
                                this.messageGroupsSelection.options.get(foundIndex).selected = true
                                this.selectedGroup = this.onlineSubcriber.groups.find(a => a.id === this.defaultMessageGroupId)
                                this.fechingSub$.next(true)
                            }, 500)
                        }
                    }
                )
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
                                selectedTypes: this.selectedNotificationTypes
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
                        this.selectedNotificationTypes,
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

        this.sup.add(
            this.actions$.pipe(
                ofActionSuccessful(ClickedOnMessageGroup),
                tap(
                    res => {
                        this.onlineSubcriber = this.store.selectSnapshot(NOTIFICATION_STATE_TOKEN).onlineSubcriber                        
                        this.messageGroup$.next(this.onlineSubcriber.groups)
                    }
                )
            ).subscribe()
        )
    }

    getIcon(message: NotificationMessage) {
        switch (message.type) {
            case NotificationType.Info:
                return 'info'
            case NotificationType.Warning:
                return 'warning'
            case NotificationType.Critical:
                return 'gpp_bad'
        }
    }

    getColor(message: NotificationMessage) {
        switch (message.type) {
            case NotificationType.Info:
                return 'primary'
            case NotificationType.Warning:
                return 'warning'
            case NotificationType.Critical:
                return 'accent'
        }
    }

    hasUnreadMessageInGroup(group: MessageGroup) {
        if(!!group.lastMessage){
            return group.lastVisitedTs < group.lastMessage.receivedDateTs
        }
        return false        
    }

    hasLastMessage(group: MessageGroup){
        return !!group.lastMessage
    }

    noMessage(group: MessageGroup){
        return !ObjectUtils.isNotNull(group.lastMessage)
    }

    getPeriod(message: NotificationMessage) {
        return DateUtils.getPeriodLength(message.receivedDate, DateUtils.getUTCNow())
    }
    
    onClickSelection() {
        this.selectedGroup = ObjectUtils.clone(this.messageGroupsSelection.selectedOptions.selected[0]?.value)
        this.selectedGroup.lastVisitedTs = DateUtils.getDotNetTicks(DateUtils.getUTCNow())
        this.selectedGroup.numberOfUnreadMessages = 0
        this.fechingSub$.next(true)
        this.notificationService.clickedOnMessageGroup(this.selectedGroup, () => {
            this.store.dispatch(new ClickedOnMessageGroup({
                messageGroup: this.selectedGroup
            }))
        })
    }

    onSelectedChanges() {
        this.fechingSub$.next(true)
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
