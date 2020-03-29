import { Injectable, InjectionToken, Optional, Inject } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { HttpClient, HttpHeaders, HttpResponseBase, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { mergeMap, tap, catchError } from 'rxjs/operators';
export const CHAT_BASE_URL = new InjectionToken<string>('CHAT_BASE_URL');
@Injectable()
export class ChatService {
    private hubConnection: HubConnection;
    private baseUrl: string;
    private http: HttpClient;
    connectionState$: BehaviorSubject<boolean> = new BehaviorSubject(false);
    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(CHAT_BASE_URL) baseUrl?: string) {
        this.baseUrl = baseUrl ? baseUrl : 'http://localhost:51622'
        this.hubConnection = this.createHubConnection(this.baseUrl)
        this.http = http
    }

    public start() {
        this.startHubConnection(this.hubConnection)
    }

    public stop() {
        this.hubConnection.stop()
    }

    public onlineUsers(): Observable<OnlineUser[]> {
        let url_ = this.baseUrl + "/api/chats/who-online"
        return this.http.get<OnlineUser[]>(url_, {
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        }).pipe(
            catchError(this.handleGetOnlineUsersError)
        )
    }

    public online() {
        let url_ = this.baseUrl + "/api/chats/online"
        this.http.post(url_, null, {
            observe: "response",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        }).pipe(
            tap(
                (res: HttpResponseBase) => {
                    const status = res.status
                    if (status === 200 || status === 204) {

                    }
                }
            )
        ).subscribe()
    }

    public offline(){
        let url_ = this.baseUrl + "/api/chats/offline"
        this.http.post(url_, null, {
            observe: "response",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        }).pipe(
            tap(
                (res: HttpResponseBase) => {
                    const status = res.status
                    if (status === 200 || status === 204) {

                    }
                }
            )
        ).subscribe()
    }

    private handleGetOnlineUsersError(error: HttpErrorResponse) {
        // 
        return throwError(error)
    }

    private createHubConnection(baseUrl: string) {
        return new HubConnectionBuilder()
            .withUrl(baseUrl + '/chathub')
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
}