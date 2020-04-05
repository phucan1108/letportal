import { Injectable, InjectionToken, Optional, Inject } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, Observable, of, throwError, Subject } from 'rxjs';
import { HttpClient, HttpHeaders, HttpResponseBase, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { mergeMap, tap, catchError } from 'rxjs/operators';
import { ObjectUtils } from '../utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { SecurityService } from '../security/security.service';
import { ChatOnlineUser, ChatSession, ChatRoom, RoomType, Message, ExtendedMessage, DoubleChatRoom } from 'app/core/models/chat.model';
import { Store, Actions, ofActionDispatched, ofActionCompleted } from '@ngxs/store';
import { TakeUserOnline, FetchedNewDoubleChatRoom, FetchDoubleChatRoom, ReceivedMessage, LoadingMoreSession, LoadedMoreSession, AddedNewSession, ReceivedMessageFromAnotherDevice } from 'stores/chats/chats.actions';
import { ChatStateModel, CHAT_STATE_TOKEN } from 'stores/chats/chats.state';
import { on } from 'cluster';
export const CHAT_BASE_URL = new InjectionToken<string>('CHAT_BASE_URL');
@Injectable()
export class ChatService {
    private hubConnection: HubConnection
    private baseUrl: string
    private http: HttpClient

    connectionState$: BehaviorSubject<boolean> = new BehaviorSubject(false)
    onlineUsers$: BehaviorSubject<OnlineUser[]> = new BehaviorSubject([])
    onlineUser$: Subject<OnlineUser> = new Subject()
    offlineUser$: Subject<OnlineUser> = new Subject()

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
        this.onlineUsers$.next([])
        this.hubConnection.stop()
    }

    public getAllAvailableUsers() {
        let url_ = this.baseUrl + "/api/chats/all-users"
        const subscription = this.http.get<OnlineUser[]>(url_, {
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        }).pipe(
            tap(
                res => {
                    res = res.filter(a => a.userName !== this.security.getAuthUser().username)
                    this.onlineUsers()
                    this.onlineUsers$.next(res)
                    subscription.unsubscribe()
                }
            )
        ).subscribe()
    }

    public onlineUsers() {
        let url_ = this.baseUrl + "/api/chats/who-online"
        const subcription = this.http.get<OnlineUser[]>(url_, {
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        }).pipe(
            tap(
                res => {
                    const availableUsers = this.onlineUsers$.value
                    if (ObjectUtils.isNotNull(res)) {
                        res.forEach(a => {
                            const found = availableUsers.find(b => b.userName === a.userName)
                            if (ObjectUtils.isNotNull(found)) {
                                found.isOnline = true
                            }
                        })

                        this.onlineUsers$.next(availableUsers)
                    }
                    subcription.unsubscribe()
                }
            ),
            catchError(this.handleGetOnlineUsersError)
        ).subscribe()
    }

    public online() {
        let url_ = this.baseUrl + "/api/chats/online"
        this.http.post<OnlineUser>(url_, null, {
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
                    formattedMessage: message.formattedMessage
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
            const sender = this.onlineUsers$.value.find(a => a.userName === message.userName)
            this.store.dispatch(new ReceivedMessage({
                chatRoomId: chatRoomId,
                chatSessionId: chatSessionId,
                message: {
                    ...message,
                    chatSessionId: chatSessionId,
                    isReceived: true                   
                },
                sender: sender
            }))
        })
    }

    private listenLoadDoubleChatRoom() {
        this.hubConnection.on('loadDoubleChatRoom', (chatRoom: ChatRoom, chatSession: ChatSession, invitee: ChatOnlineUser, previousSession?: ChatSession) => {

            // We will combine all sessions into current sesstion but we will maintain previous session id
            if(ObjectUtils.isNotNull(chatSession.messages)){
                chatSession.messages.forEach(m => {
                    m.isReceived = m.userName !== this.security.getAuthUser().username
                })
            }
            else{
                chatSession.messages = []
            }
            if(ObjectUtils.isNotNull(previousSession)){
                previousSession.messages.forEach(m => {
                    m.isReceived = m.userName !== this.security.getAuthUser().username
                    chatSession.messages.unshift(m)
                })

                chatSession.previousSessionId = previousSession.previousSessionId
            }
            
            this.store.dispatch(new FetchedNewDoubleChatRoom({
                chatRoom: {
                    ...chatRoom,
                    invitee: invitee,
                    currentSession: chatSession,
                    chatSessions: []
                }
            }))
        })
    }

    private listenLoadPrevious() {
        this.hubConnection.on('addPreviousSession', (chatSession: ChatSession) => {
            chatSession.messages.forEach(m => {
                m.isReceived = m.userName !== this.security.getAuthUser().username
            })
            this.store.dispatch(new LoadedMoreSession({
                chatSession: chatSession
            }))
        })
    }

    private listenBoardCastSentMessage(){
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
                message: message
            }))
        })
    }

    private handleGetOnlineUsersError(error: HttpErrorResponse) {
        // 
        return throwError(error)
    }

    private listenOnlineUser() {
        this.hubConnection.on('online', (onlineUser: OnlineUser) => {
            this.logger.debug('User is online', onlineUser)
            let availableUsers = ObjectUtils.clone(this.onlineUsers$.value)
            let found = availableUsers.find(a => a.userName === onlineUser.userName)
            
            if (!found && this.security.getAuthUser().username != onlineUser.userName) {
                onlineUser.isOnline = true
                found = onlineUser
                availableUsers.push(found)
            }
            else{
                found.isOnline = true
            }
            
            this.onlineUser$.next(found)
            this.onlineUsers$.next(availableUsers)            
        })
    }

    private listenOfflineUser() {
        this.hubConnection.on('offline', (userName: string) => {
            this.logger.debug('User is offline', userName)
            const availableUsers = this.onlineUsers$.value
            let found = false
            availableUsers.forEach(a => {
                if (a.userName === userName) {
                    this.logger.debug('User is updated offline', a)
                    a.isOnline = false
                    this.offlineUser$.next(a)
                    found = true
                    return false
                }
            })

            this.onlineUsers$.next(availableUsers)
        })
    }

    private createHubConnection(baseUrl: string, jwtToken: string) {
        return new HubConnectionBuilder()
            .withUrl(baseUrl + '/chathub', { accessTokenFactory: () => jwtToken })
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