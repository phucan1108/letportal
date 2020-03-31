import { Injectable, InjectionToken, Optional, Inject } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, Observable, of, throwError, Subject } from 'rxjs';
import { HttpClient, HttpHeaders, HttpResponseBase, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { mergeMap, tap, catchError } from 'rxjs/operators';
import { ObjectUtils } from '../utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { SecurityService } from '../security/security.service';
import { ChatOnlineUser } from 'portal/modules/chat/models/chat.model';
export const CHAT_BASE_URL = new InjectionToken<string>('CHAT_BASE_URL');
@Injectable()
export class ChatService {
    private hubConnection: HubConnection;
    private baseUrl: string;
    private http: HttpClient;
    private connectionState$: BehaviorSubject<boolean> = new BehaviorSubject(false);
    onlineUsers$: BehaviorSubject<OnlineUser[]> = new BehaviorSubject([]);
    onlineUser$: Subject<OnlineUser> = new Subject()
    offlineUser$: Subject<OnlineUser> = new Subject()
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
    }

    public stop() {
        this.hubConnection.stop()
    }

    public getAllAvailableUsers(){
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

    public onlineUsers(){
        let url_ = this.baseUrl + "/api/chats/who-online"
        const subcription =  this.http.get<OnlineUser[]>(url_, {
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        }).pipe(
            tap(
                res => {
                    const availableUsers = this.onlineUsers$.value
                    if(ObjectUtils.isNotNull(res)){
                        res.forEach(a => {
                            const found = availableUsers.find(b => b.userName === a.userName && b.userName != this.security.getAuthUser().username)
                            if(ObjectUtils.isNotNull(found)){
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

    public openDoubleChatRoom(user: ChatOnlineUser){
        this.hubConnection
            .send('openDoubleChatRoom', user)
            .then(() => {

            })          
    }

    private listenOnlineUser(){
        this.hubConnection.on('online', (onlineUser: OnlineUser) => {
            this.logger.debug('User is online', onlineUser)
            let availableUsers = this.onlineUsers$.value
            let found = false
            availableUsers.forEach(a => {
                if(a.userName === onlineUser.userName){
                    a.isOnline = true
                    this.onlineUser$.next(a)
                    found = true
                    return false
                }
            })

            if(!found){
                availableUsers.push(onlineUser)
            }
            availableUsers = availableUsers.filter(a => a.userName !== this.security.getAuthUser().username)
            this.onlineUsers$.next(availableUsers)
        })
    }

    private listenOfflineUser(){
        this.hubConnection.on('offline', (userName: string) => {
            this.logger.debug('User is offline', userName)
            const availableUsers = this.onlineUsers$.value
            let found = false
            availableUsers.forEach(a => {
                if(a.userName === userName){
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

    private handleGetOnlineUsersError(error: HttpErrorResponse) {
        // 
        return throwError(error)
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