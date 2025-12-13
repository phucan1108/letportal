import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatMenuTrigger } from '@angular/material/menu';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { Actions, ofActionSuccessful, Store } from '@ngxs/store';
import { MessageGroup, NotificationMessage, NotificationType, OnlineSubcriber } from 'app/core/models/notification.model';
import { DateUtils } from 'app/core/utils/date-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { Subject, Subscription } from 'rxjs';
import { filter, takeUntil, tap } from 'rxjs/operators';
import { NotificationService } from 'services/notification.service';
import { ClickedOnMessageGroup, ClickedOnNotificationBox, ReceivedNewNotification, SubcribeToServer } from 'stores/notifications/notification.actions';
import { NOTIFICATION_STATE_TOKEN } from 'stores/notifications/notification.state';

@Component({
    selector: 'notification-box',
    templateUrl: './notification-box.component.html',
    styleUrls: ['./notification-box.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class NotificationBoxComponent implements OnInit, OnDestroy {
    hideBadge = true
    numberBadge = 0
    subcribed = false
    @ViewChild('menuTrigger', { static: true })
    menuTrigger: MatMenuTrigger
    sup: Subscription = new Subscription()

    receivedMessage$: Subject<NotificationMessage> = new Subject()
    snackBarOpened$: Subject<boolean> = new Subject()
    onlineSubcriber: OnlineSubcriber = {
        userId: '',
        userName: '',
        groups: [],
        roles: [],
        subcriberId: '',
        lastClickedTs: 0,
        lastUnreadMessages: 0
    }
    constructor(
        private logger: NGXLogger,
        private notificationService: NotificationService,
        private store: Store,
        private actions$: Actions,
        private cd: ChangeDetectorRef,
        private dialog: MatDialog,
        private _snackBar: MatSnackBar,
        private router: Router
    ) { }
    ngOnDestroy(): void {
        this.sup.unsubscribe()
    }

    ngOnInit(): void {
        this.sup.add(this.actions$.pipe(
            ofActionSuccessful(SubcribeToServer),
            tap(res => {
                this.onlineSubcriber = this.store.selectSnapshot(NOTIFICATION_STATE_TOKEN).onlineSubcriber
                if (!!this.onlineSubcriber) {
                    this.numberBadge = this.onlineSubcriber.lastUnreadMessages
                    if (this.numberBadge > 0) {
                        this.hideBadge = false
                    }
                }
                this.subcribed = true
                this.cd.markForCheck()
            })
        ).subscribe())

        this.sup.add(this.actions$.pipe(
            ofActionSuccessful(ClickedOnMessageGroup),
            tap(res => {
                this.onlineSubcriber = this.store.selectSnapshot(NOTIFICATION_STATE_TOKEN).onlineSubcriber
                this.cd.markForCheck()
            })
        ).subscribe())

        this.sup.add(this.actions$.pipe(            
            ofActionSuccessful(ReceivedNewNotification),
            tap(res => {
                this.receivedMessage$.next(this.store.selectSnapshot(NOTIFICATION_STATE_TOKEN).lastReceivedMessage)
            })
        ).subscribe())

        this.sup.add(this.receivedMessage$.pipe(
            takeUntil(this.snackBarOpened$.pipe(filter(opened => !opened))), // Wait until the opened has been close
            tap(
                message => {
                    this.openSnackbar(message.shortMessage)
                }
            )
        ).subscribe())
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
                return 'accent'
            case NotificationType.Critical:
                return 'warn'
        }
    }

    getPeriod(message: NotificationMessage) {
        if(!!message){
            return DateUtils.getPeriodLength(message.receivedDate, DateUtils.getUTCNow())
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

    clickedOnNotificationBox() {
        this.notificationService.clickedOnNotificationBox(this.onlineSubcriber.subcriberId, () => {
            this.numberBadge = 0
            this.hideBadge = true
            this.store.dispatch(new ClickedOnNotificationBox({
                lastClickedTs: DateUtils.getDotNetTicks(DateUtils.getUTCNow())
            }))
        })
    }

    openNotificationDialog() {
        this.router.navigateByUrl('/portal/notification/box')
    }

    clickOnGroup(group: MessageGroup) {
        this.router.navigateByUrl('/portal/notification/box/' + group.id)
    }

    private openSnackbar(message: string) {
        const snackBarRef = this._snackBar.open(message, 'Close')
        this.snackBarOpened$.next(true) // Prevent upcoming message
        snackBarRef.afterDismissed().subscribe(() => {
            this.snackBarOpened$.next(false)
        })
    }
}
