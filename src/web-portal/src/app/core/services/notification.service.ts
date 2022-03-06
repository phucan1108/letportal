import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable, InjectionToken, Optional } from '@angular/core';
import { HttpTransportType, HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Actions, Store } from '@ngxs/store';
import { NGXLogger } from 'ngx-logger';
import { Observable } from 'rxjs';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { tap } from 'rxjs/operators';
import { ReceivedNewMessageGroup, ReceivedNewNotification, SubcribeToServer } from 'stores/notifications/notification.actions';
import { FetchedNotificationMessageRequest, MessageGroup, NotificationMessage, OnlineSubcriber } from '../models/notification.model';
import { SecurityService } from '../security/security.service';
export const NOTIFICATION_BASE_URL = new InjectionToken<string>('NOTIFICATION_BASE_URL');

// Important note: All SignarlR services must be registered in AppModule instead of CoreModule
// Because the HubConnection can't be shared in Lazy Loading (injector will be created when the module is loaded)
@Injectable()
export class NotificationService {
    private hubConnection: HubConnection
    private baseUrl: string
    private httpClient: HttpClient
    connectionState$: BehaviorSubject<boolean> = new BehaviorSubject(false)
    constructor(
        private actions$: Actions,
        private store: Store,
        private security: SecurityService,
        private logger: NGXLogger,
        @Inject(HttpClient) http: HttpClient,
        @Optional() @Inject(NOTIFICATION_BASE_URL) baseUrl?: string) {
        this.baseUrl = baseUrl ? baseUrl : 'https://localhost:44399'
        this.httpClient = http
    }

    public getMessageGroups(subcriberId: string): Observable<MessageGroup[]> {
        let url_ = this.baseUrl + "/api/notifications/message-groups/" + subcriberId
        return this.httpClient.get<MessageGroup[]>(url_)
    }

    public getMessages(fetchedRequest: FetchedNotificationMessageRequest): Observable<NotificationMessage[]> {
        let url_ = this.baseUrl + "/api/notifications/messages/fetch"
        return this.httpClient.post<NotificationMessage[]>(url_, fetchedRequest, {
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        })
    }

    public subcribe() {
        let url_ = this.baseUrl + "/api/notifications/subcribe"
        this.httpClient.post<OnlineSubcriber>(url_, null, {
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        }).pipe(
            tap(
                res => {
                    this.logger.info('Hit calling subcriber')
                    this.store.dispatch(new SubcribeToServer({
                        onlineSubcriber: {
                            userId: this.security.getAuthUser().userid,
                            userName: this.security.getAuthUser().username,
                            subcriberId: res.subcriberId,
                            lastClickedTs: res.lastClickedTs,
                            lastUnreadMessages: res.lastUnreadMessages,
                            groups: res.groups,
                            roles: this.security.getAuthUser().roles
                        }
                    }))

                    // Auto connect Hub
                    this.start()
                }
            )
        ).subscribe()
    }

    public clickedOnNotificationBox(subcriberId: string, postAction: () => void) {
        this.hubConnection.send('clickedOnNotificationbox', subcriberId).then(() => {
            if (postAction != null) {
                postAction()
            }
        })
    }

    public clickedOnMessageGroup(messageGroup: MessageGroup, postAction: () => void){
        this.hubConnection.send('clickedOnMessageGroup', messageGroup).then(() => {
            if (postAction != null) {
                postAction()
            }
        })
    }

    private start() {
        this.hubConnection = this.createHubConnection(this.baseUrl, this.security.getJwtToken())
        this.hubConnection.onreconnecting((err) => {
            this.connectionState$.next(false)
        })
        this.hubConnection.onreconnected(() => {
            this.connectionState$.next(true)
        })

        this.startHubConnection(this.hubConnection)

        // Register events
        this.listenPushMessage()
        this.listenNewMessageGroup()
    }

    private createHubConnection(baseUrl: string, jwtToken: string) {
        return new HubConnectionBuilder()
            .withUrl(baseUrl + '/notificationhub', { accessTokenFactory: () => jwtToken, transport: HttpTransportType.LongPolling | HttpTransportType.WebSockets })
            .withAutomaticReconnect()
            .build();
    }

    private startHubConnection(hubConnection: HubConnection) {
        hubConnection
            .start()
            .then(() => {
                this.connectionState$.next(true)
            })
            .catch(err => {
                this.connectionState$.next(false)
                // Try to reconnect hub chat after 2s
                setTimeout(function () { this.startHubConnection(hubConnection); }, 2000);
            })
    }

    private listenPushMessage() {
        this.hubConnection.on('push', (notificationMessage: NotificationMessage) => {
            this.store.dispatch(new ReceivedNewNotification({
                notificationMessage: notificationMessage
            }))
        })
    }

    private listenNewMessageGroup() {
        this.hubConnection.on('pushNewGroup', (messageGroup: MessageGroup) => {
            this.store.dispatch(new ReceivedNewMessageGroup({
                messageGroup: messageGroup
            }))
        })
    }
}