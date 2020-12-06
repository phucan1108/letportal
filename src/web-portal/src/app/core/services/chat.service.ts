import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable, InjectionToken, Optional } from '@angular/core';
import { HttpTransportType, HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Actions, ofActionCompleted, Store } from '@ngxs/store';
import { ChatOnlineUser, ChatRoom, ChatSession, ExtendedMessage, Message } from 'app/core/models/chat.model';
import { NGXLogger } from 'ngx-logger';
import { BehaviorSubject, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { AddedNewSession, FetchDoubleChatRoom, FetchedNewDoubleChatRoom, IncomingOfflineUser, IncomingOnlineUser, LoadedAllAvailableUsers, LoadedMoreSession, LoadingMoreSession, ReceivedMessage, ReceivedMessageFromAnotherDevice, TakeUserOnline } from 'stores/chats/chats.actions';
import { CHAT_STATE_TOKEN } from 'stores/chats/chats.state';
import { SecurityService } from '../security/security.service';
import { ObjectUtils } from '../utils/object-util';
export const CHAT_BASE_URL = new InjectionToken<string>('CHAT_BASE_URL');
@Injectable()
export class ChatService {
    private hubConnection: HubConnection
    private baseUrl: string
    private http: HttpClient

    connectionState$: BehaviorSubject<boolean> = new BehaviorSubject(false)

    constructor(
        private actions$: Actions,
        private store: Store,
        private security: SecurityService,
        private logger: NGXLogger,
        @Inject(HttpClient) http: HttpClient,
        @Optional() @Inject(CHAT_BASE_URL) baseUrl?: string) {
        this.baseUrl = baseUrl ? baseUrl : 'http://localhost:51622'
        this.http = http

        this.actions$.pipe(
            ofActionCompleted(FetchDoubleChatRoom)
        ).subscribe(
            () => {
                this.openDoubleChatRoom(this.store
                    .selectSnapshot(CHAT_STATE_TOKEN).invitingUser)
            }
        )

        this.actions$.pipe(
            ofActionCompleted(LoadingMoreSession)
        ).subscribe(
            () => {
                const activeChatSession = this.store.selectSnapshot(CHAT_STATE_TOKEN).activeChatSession
                this.loadMore(activeChatSession.previousSessionId)
            }
        )
    }

    public start() {
        this.hubConnection = this.createHubConnection(this.baseUrl, this.security.getJwtToken())
        this.hubConnection.onreconnecting((err) => {
            this.connectionState$.next(false)
        })
        this.hubConnection.onreconnected(() => {
            this.connectionState$.next(true)
        })

        this.startHubConnection(this.hubConnection)

        // Register all listening events
        this.listenOnlineUser()
        this.listenOfflineUser()
        this.listenLoadDoubleChatRoom()
        this.listenReceivedMesage()
        this.listenAddNewChatSession()
        this.listenLoadPrevious()
        this.listenBoardCastSentMessage()
    }

    public stop() {
        this.hubConnection.stop()
    }

    public getAllAvailableUsers() {
        let url_ = this.baseUrl + "/api/chats/all-users"
        const subscription = this.http.get<ChatOnlineUser[]>(url_, {
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        }).pipe(
            tap(
                res => {
                    res = res.filter(a => a.userName !== this.security.getAuthUser().username)
                    this.onlineUsers(res)
                    subscription.unsubscribe()
                }
            )
        ).subscribe()
    }

    public onlineUsers(allAvailableUsers: ChatOnlineUser[]) {
        let url_ = this.baseUrl + "/api/chats/who-online"
        const subcription = this.http.get<OnlineUser[]>(url_, {
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        }).pipe(
            tap(
                res => {
                    const availableUsers = allAvailableUsers
                    if (ObjectUtils.isNotNull(res)) {
                        res?.forEach(a => {
                            const found = availableUsers.find(b => b.userName === a.userName)
                            if (ObjectUtils.isNotNull(found)) {
                                found.isOnline = true
                            }
                        })

                        this.store.dispatch(new LoadedAllAvailableUsers({
                            availableUsers: availableUsers
                        }))
                    }
                    subcription.unsubscribe()
                }
            ),
            catchError(this.handleGetOnlineUsersError)
        ).subscribe()
    }

    public online() {
        let url_ = this.baseUrl + "/api/chats/online"
        this.http.post<ChatOnlineUser>(url_, null, {
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        }).pipe(
            tap(
                res => {
                    this.store.dispatch(new TakeUserOnline({
                        user: res
                    }))
                }
            )
        ).subscribe()
    }

    public openDoubleChatRoom(user: ChatOnlineUser) {
        this.hubConnection
            .send('openDoubleChatRoom', user)
            .then(() => {

            })
    }

    public sendMessage(
        chatRoomId: string,
        chatSessionId: string,
        receiver: string,
        message: Message,
        lastSentHashCode: string, postAction: () => void) {
        // const foundChatRoom = this.chatRooms.find(a => a.currentSession.sessionId === chatSessionId)
        // foundChatRoom.currentSession.messages.push(message)
        this.hubConnection
            .send('sendChatMessage', {
                chatRoomId: chatRoomId,
                chatSessionId: chatSessionId,
                receiver: receiver,
                message: {
                    userName: message.userName,
                    message: message.message,
                    formattedMessage: message.formattedMessage,
                    attachmentFiles: message.attachmentFiles
                },
                lastSentHashCode: lastSentHashCode
            })
            .then(() => {
                if (postAction) {
                    postAction()
                }
            })
    }

    public loadMore(previousChatSessionId: string,
        onSuccess: () => void = null,
        onError: (err: any) => void = null) {
        this.hubConnection
            .send('loadPrevious', previousChatSessionId)
            .then(onSuccess)
            .catch(onError)
    }
    private listenAddNewChatSession() {
        this.hubConnection.on('addNewChatSession', (chatSession: ChatSession) => {
            this.store.dispatch(new AddedNewSession({
                chatSession: chatSession
            }))
        })
    }

    private listenReceivedMesage() {
        this.hubConnection.on('receivedMessage', (chatRoomId: string, chatSessionId: string, message: Message) => {
            this.store.dispatch(new ReceivedMessage({
                chatRoomId: chatRoomId,
                chatSessionId: chatSessionId,
                message: {
                    ...message,
                    hasAttachmentFile: message.attachmentFiles && message.attachmentFiles.length > 0,
                    chatSessionId: chatSessionId,
                    isReceived: true,
                    renderTime: true
                }
            }))
        })
    }

    private listenLoadDoubleChatRoom() {
        this.hubConnection.on('loadDoubleChatRoom', (chatRoom: ChatRoom, chatSession: ChatSession, invitee: ChatOnlineUser, previousSession?: ChatSession) => {

            // We will combine all sessions into current sesstion but we will maintain previous session id
            if (ObjectUtils.isNotNull(chatSession.messages)) {
                let i = 0
                for (i = 0; i < chatSession.messages.length; i++) {
                    if(i === 0){
                        chatSession.messages[i].renderTime = true
                    }
                    else if(chatSession.messages[i - 1].userName !== chatSession.messages[i].userName){
                        chatSession.messages[i].renderTime = true
                    }
                    
                    chatSession.messages[i].isReceived = chatSession.messages[i].userName !== this.security.getAuthUser().username
                    chatSession.messages[i].hasAttachmentFile = chatSession.messages[i].attachmentFiles && chatSession.messages[i].attachmentFiles.length > 0
                }
            }
            else {
                chatSession.messages = []
            }
            if (ObjectUtils.isNotNull(previousSession)) {
                previousSession.messages?.forEach(m => {
                    m.isReceived = m.userName !== this.security.getAuthUser().username
                    m.hasAttachmentFile = m.attachmentFiles && m.attachmentFiles.length > 0
                })

                chatSession.messages = [
                    ...previousSession.messages,
                    ...chatSession.messages
                ]

                chatSession.previousSessionId = previousSession.previousSessionId
            }

            this.store.dispatch(
                new FetchedNewDoubleChatRoom({
                    chatRoom: {
                        ...chatRoom,
                        invitee: invitee,
                        currentSession: chatSession,
                        chatSessions: []
                    }
                })
            )
        })
    }

    private listenLoadPrevious() {
        this.hubConnection.on('addPreviousSession', (chatSession: ChatSession) => {
            chatSession.messages?.forEach(m => {
                m.isReceived = m.userName !== this.security.getAuthUser().username
                m.hasAttachmentFile = m.attachmentFiles && m.attachmentFiles.length > 0
            })
            this.store.dispatch(new LoadedMoreSession({
                chatSession: chatSession
            }))
        })
    }

    private listenBoardCastSentMessage() {
        this.hubConnection.on('boardcastSentMessage',
            (chatRoomId: string,
                chatSessionId: string,
                lastSentHashCode: string,
                message: ExtendedMessage) => {
                message.isReceived = false
                this.store.dispatch(new ReceivedMessageFromAnotherDevice({
                    chatRoomId: chatRoomId,
                    chatSessionId: chatSessionId,
                    lastHashCode: lastSentHashCode,
                    message: {
                        ...message,
                        hasAttachmentFile: message.attachmentFiles && message.attachmentFiles.length > 0
                    }
                }))
            })
    }

    private handleGetOnlineUsersError(error: HttpErrorResponse) {
        // 
        return throwError(error)
    }

    private listenOnlineUser() {
        this.hubConnection.on('online', (onlineUser: ChatOnlineUser) => {
            this.logger.debug('User is online', onlineUser)
            onlineUser.isOnline = true
            onlineUser.incomingMessages = 0
            if (onlineUser.userName !== this.security.getAuthUser().username) {
                this.store.dispatch(new IncomingOnlineUser({
                    onlineUser: onlineUser
                }))
            }
        })
    }

    private listenOfflineUser() {
        this.hubConnection.on('offline', (userName: string) => {
            this.store.dispatch(new IncomingOfflineUser({
                offlineUser: userName
            }))
        })
    }

    private createHubConnection(baseUrl: string, jwtToken: string) {
        return new HubConnectionBuilder()
            .withUrl(baseUrl + '/chathub', { accessTokenFactory: () => jwtToken, transport: HttpTransportType.LongPolling | HttpTransportType.WebSockets })
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
}

export interface OnlineUser {
    userName: string
    fullName: string
    avatar: string
    shortName: string
    hasAvatar: boolean
    isOnline: boolean
}