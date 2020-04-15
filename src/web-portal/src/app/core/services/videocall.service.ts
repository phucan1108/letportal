import { Injectable, InjectionToken, Optional, Inject } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, Observable, of, throwError, Subject } from 'rxjs';
import { HttpClient, HttpHeaders, HttpResponseBase, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { NGXLogger } from 'ngx-logger';
import { SecurityService } from '../security/security.service';
import { ChatOnlineUser, ChatSession, ChatRoom, RoomType, Message, ExtendedMessage, DoubleChatRoom, ParticipantVideo, VideoRoomModel, RtcIceServer, VideoRtcSignal } from 'app/core/models/chat.model';
import { Store, Actions, ofActionDispatched, ofActionCompleted } from '@ngxs/store';
import { NotifyIncomingVideoCall, HandshakedVideoCall, ReceivedIceServer, DroppedCall, ForceDroppedCall, UserDeniedCall, UserDroppedCall, UserCancelledCall } from 'stores/chats/chats.actions';
export const VIDEO_BASE_URL = new InjectionToken<string>('VIDEO_BASE_URL');
import 'webrtc-adapter'
import { ObjectUtils } from '../utils/object-util';
@Injectable()
export class VideoCallService {
    private hubConnection: HubConnection
    private baseUrl: string
    private http: HttpClient
    private iceServer: RtcIceServer
    connectionState$: BehaviorSubject<boolean> = new BehaviorSubject(false)

    rtcSignalMessage$: Subject<VideoRtcSignal> = new Subject()

    constructor(
        private actions$: Actions,
        private store: Store,
        private security: SecurityService,
        private logger: NGXLogger,
        @Inject(HttpClient) http: HttpClient,
        @Optional() @Inject(VIDEO_BASE_URL) baseUrl?: string) {
        this.baseUrl = baseUrl ? baseUrl : 'http://localhost:51622'
        this.http = http
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
        this.listenSignalIncomingCall()
        this.listenHandshakedVideoCallRequest()
        this.listenReceivedException()
        this.listenReceivedICEServer()
        this.listenReceivedRtcSignal()
        this.listenReceivedDropCall()
        this.listenDeniedRequestCall()
        this.listenCancelCall()
    }

    public stop() {
        this.hubConnection.stop()
    }

    public signalingInvitee(invitee: ChatOnlineUser){
        this.hubConnection
            .send('signalingUser', invitee)
            .then(() => {

            })
    }

    public cancelCall(invitee: ChatOnlineUser){
        this.hubConnection
            .send('cancelCall', invitee)
            .then(() => {

            })
    }

    public answeringVideoCall(caller: ParticipantVideo){
        this.hubConnection
            .send('answeringVideoCall', caller)
            .then(() => {
                
            })
    }

    public denyCall(caller: ParticipantVideo){
        this.hubConnection
            .send('denyCall', caller)
            .then(() => {
                this.store.dispatch(new DroppedCall())
            })
    }
    
    public sendRtcSignal(message: VideoRtcSignal){
        this.hubConnection
            .send('sendRtcSignal', message)
            .then(() => {

            })
    }

    public droppedCall(roomId: string){
        this.hubConnection
            .send('dropCall', roomId)
            .then(() => {
                this.store.dispatch(new DroppedCall())
            })
    }

    private listenCancelCall(){
        this.hubConnection
            .on('cancelVideoCall', (caller: string) => {
                this.store.dispatch(new UserCancelledCall())
            })
    }

    private listenDeniedRequestCall(){
        this.hubConnection.on('deniedRequestCall', (receiver: string) => {
            this.store.dispatch(new UserDeniedCall())
        })
    }

    private listenReceivedException(){
        this.hubConnection.on('receivedException', (err: any) => {
            if(ObjectUtils.isNotNull(err.error)){
                this.store.dispatch(new ForceDroppedCall(err.error))
            }
        })
    }

    private listenSignalIncomingCall(){
        this.hubConnection.on('signalingIncomingCall', (caller: ParticipantVideo) => {
            this.store.dispatch(new NotifyIncomingVideoCall({
                caller: caller
            }))
        })
    }

    private listenHandshakedVideoCallRequest(){
        this.hubConnection
            .on('handshakedVideoCall', (videoRoom: VideoRoomModel) => {
                this.store.dispatch(new HandshakedVideoCall({
                    videoRoom: videoRoom
                }));
            })
    }

    private listenReceivedICEServer(){
        this.hubConnection
            .on('receivedIceServer', (iceServers: RtcIceServer[]) => {
                this.store.dispatch(new ReceivedIceServer({
                    iceServers: iceServers
                }))
            })
    }

    private listenReceivedRtcSignal(){
        this.hubConnection
            .on('receivedRtcSignal', (message: VideoRtcSignal) => {
                this.rtcSignalMessage$.next(message)
            })
    }

    private listenReceivedDropCall(){
        this.hubConnection
            .on('receivedDroppedCall', (roomId: string) => {
                this.store.dispatch(new UserDroppedCall())
            })
    }

    private createHubConnection(baseUrl: string, jwtToken: string) {
        return new HubConnectionBuilder()
            .withUrl(baseUrl + '/videohub', { accessTokenFactory: () => jwtToken })
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