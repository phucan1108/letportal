import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Actions, ofActionSuccessful, Store } from '@ngxs/store';
import { MessageGroup, NotificationMessage, NotificationType, OnlineSubcriber } from 'app/core/models/notification.model';
import { DateUtils } from 'app/core/utils/date-util';
import { NGXLogger } from 'ngx-logger';
import { Subscription } from 'rxjs';
import { tap } from 'rxjs/operators';
import { NotificationService } from 'services/notification.service';
import { ClickedOnNotificationBox, SubcribeToServer } from 'stores/notifications/notification.actions';
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
    sup: Subscription = new Subscription()
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
        private router: Router
    ) { }
    ngOnDestroy(): void {
        this.sup.unsubscribe()
    }

    ngOnInit(): void {       
        this.sup.add(this.actions$.pipe(
          ofActionSuccessful(SubcribeToServer),
          tap(res=>{              
              this.onlineSubcriber = this.store.selectSnapshot(NOTIFICATION_STATE_TOKEN).onlineSubcriber
              this.logger.info('Online subcriber info', this.onlineSubcriber)
              if(!!this.onlineSubcriber){
                  this.numberBadge = this.onlineSubcriber.lastUnreadMessages
                  if(this.numberBadge > 0){
                      this.hideBadge = false
                  }
              }
              this.subcribed = true
              this.cd.markForCheck()
          }) 
        ).subscribe())
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

    getColor(message: NotificationMessage){
        switch(message.type){
            case NotificationType.Info:
                return 'primary'
            case NotificationType.Warning:
                return 'accent'
            case NotificationType.Critical:
                return 'warn'
        }
    }

    getPeriod(message: NotificationMessage){
        return DateUtils.getPeriodLength(message.receivedDate, DateUtils.getUTCNow())
    }

    hasUnreadMessageInGroup(group: MessageGroup){
        return group.lastVisitedTs < group.lastMessage.receivedDateTs
    }

    clickedOnNotificationBox(){
        this.notificationService.clickedOnNotificationBox(this.onlineSubcriber.subcriberId, () => {
            this.numberBadge = 0
            this.hideBadge = true
            this.store.dispatch(new ClickedOnNotificationBox({
                lastClickedTs: DateUtils.getDotNetTicks(DateUtils.getUTCNow())
            }))
        })
    }

    openNotificationDialog(){
        this.router.navigateByUrl('/portal/notification/box')
    }
}
