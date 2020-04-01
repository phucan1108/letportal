import { Injectable, InjectionToken, Optional, Inject } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, Observable, of, throwError, Subject } from 'rxjs';
import { HttpClient, HttpHeaders, HttpResponseBase, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { mergeMap, tap, catchError } from 'rxjs/operators';
import { ObjectUtils } from '../utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { SecurityService } from '../security/security.service';
import { ChatOnlineUser, ChatSession, ChatRoom, RoomType, Message, ExtendedMessage } from 'portal/modules/chat/models/chat.model';
export const CHAT_BASE_URL = new InjectionToken<string>('CHAT_BASE_URL');
@Injectable()
export class ChatService {
    private hubConnection: HubConnection;
    private baseUrl: string;
    private http: HttpClient;
    private connectionState$: BehaviorSubject<boolean> = new BehaviorSubject(false);

    private maxChatRoom: number = 5
    chatRooms: ChatRoom[] = []
    onlineUsers$: BehaviorSubject<OnlineUser[]> = new BehaviorSubject([]);
    onlineUser$: Subject<OnlineUser> = new Subject()
    offlineUser$: Subject<OnlineUser> = new Subject()

    chatRoom$: Subject<ChatRoom> = new Subject()
    newMesage$: Subject<ExtendedMessage> = new Subject()
    currentUser: ChatOnlineUser
    constructor(
        private security: SecurityService,
        private logger: NGXLogger,
        @Inject(HttpClient) http: HttpClient,
        @Optional() @Inject(CHAT_BASE_URL) baseUrl?: string) {
        this.baseUrl = baseUrl ? baseUrl : 'http://localhost:51622'
        this.http = http
    }

    public start() {
        this.hubConnection = this.createHubConnection(this.baseUrl, this.security.getJwtToken())
        this.startHubConnection(this.hubConnection)

        // Register all listening events
        this.listenOnlineUser()
        this.listenOfflineUser()
        this.listenLoadDoubleChatRoom()
        this.listenReceivedDoubleChatRoom()
    }

    public stop() {
        this.chatRooms = []
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
                            const found = availableUsers.find(b => b.userName === a.userName && b.userName != this.security.getAuthUser().username)
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
                    this.currentUser = res
                }
            )
        ).subscribe()
    }

    public openDoubleChatRoom(user: ChatOnlineUser) {
        const foundChatRoom = this.chatRooms.find(a => 
            a.type === RoomType.Double 
            && a.participants.some(b => b.userName === user.userName))
        if (ObjectUtils.isNotNull(foundChatRoom)) {
            this.chatRoom$.next(foundChatRoom)
        }
        else {
            this.hubConnection
                .send('openDoubleChatRoom', user)
                .then(() => {

                })
        }
    }

    public sendMessage(chatSessionId: string, receiver: string, message: Message, postAction: () => void){
        const foundChatRoom = this.chatRooms.find(a => a.currentSession.sessionId === chatSessionId)
        foundChatRoom.currentSession.messages.push(message)
        this.hubConnection
            .send('sendChatMessage', {
                chatSessionId: chatSessionId,
                receiver: receiver,
                message: {
                    userName: message.userName,
                    message: message.message,
                    formattedMessage: message.formattedMessage
                }
            })
            .then(() => {
                if(postAction){
                    postAction()
                }
            })
    }

    private listenReceivedDoubleChatRoom(){
        this.hubConnection.on('receivedMessage', (chatSessionId: string, message: Message) => {
            // Add to current chat room for reload
            const foundChatRoom = this.chatRooms.find(a => a.currentSession.sessionId === chatSessionId)
            foundChatRoom.currentSession.messages.push(message)
            this.newMesage$.next({
                ...message,
                chatSessionId: chatSessionId,
                isReceived: true
            })
        })
    }

    private listenLoadDoubleChatRoom() {
        this.hubConnection.on('loadDoubleChatRoom', (chatRoom: ChatRoom, chatSession: ChatSession, invitee: ChatOnlineUser, previousSession?: ChatSession) => {
            const foundRoom = this.chatRooms.find(a => a.chatRoomId === chatRoom.chatRoomId)
            if (ObjectUtils.isNotNull(foundRoom)) {
                foundRoom.chatSessions.push(chatSession)
                foundRoom.currentSession = chatSession
                this.chatRoom$.next(foundRoom)
            }
            else {
                // Maintain maximum chat room
                if (this.chatRooms.length === this.maxChatRoom) {
                    this.chatRooms.shift()
                }
                chatRoom.chatSessions = []
                if (ObjectUtils.isNotNull(previousSession)) {
                    chatRoom.chatSessions.push(previousSession)
                }
                chatRoom.chatSessions.push(chatSession)
                chatRoom.currentSession = chatSession
                this.chatRooms.push(chatRoom)


                this.chatRoom$.next(chatRoom)
            }
        })
    }

    private handleGetOnlineUsersError(error: HttpErrorResponse) {
        // 
        return throwError(error)
    }

    private listenOnlineUser() {
        this.hubConnection.on('online', (onlineUser: OnlineUser) => {
            this.logger.debug('User is online', onlineUser)
            let availableUsers = this.onlineUsers$.value
            let found = false
            availableUsers.forEach(a => {
                if (a.userName === onlineUser.userName) {
                    a.isOnline = true
                    this.onlineUser$.next(a)
                    found = true
                    return false
                }
            })

            if (!found) {
                availableUsers.push(onlineUser)
            }
            availableUsers = availableUsers.filter(a => a.userName !== this.security.getAuthUser().username)
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