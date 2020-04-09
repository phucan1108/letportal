import { Component, OnInit, ViewChild, ElementRef, Inject, OnDestroy } from '@angular/core';
import { ChatService } from 'services/chat.service';
import { NGXLogger } from 'ngx-logger';
import { VideoCallService } from 'services/videocall.service';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material';
import { PageDatasourceGridComponent } from 'portal/modules/builder/components/page-builder/components/page-datasources/page-datasource.grid.component';
import { ChatOnlineUser, VideoRoomModel, RtcIceServer, ParticipantVideo } from 'app/core/models/chat.model';
import { Store, Actions, ofActionSuccessful, ofActionCompleted } from '@ngxs/store';
import { Subscription } from 'rxjs';
import { HandshakedVideoCall, ReceivedIceServer, ForceDroppedCall, UserDeniedCall, DroppedCall, UserDroppedCall, UserCancelledCall } from 'stores/chats/chats.actions';
import { CHAT_STATE_TOKEN } from 'stores/chats/chats.state';
import 'webrtc-adapter'
import { async } from '@angular/core/testing';
import { ObjectUtils } from 'app/core/utils/object-util';
import { heartBeatAnimation } from 'angular-animations';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
@Component({
    selector: 'let-video-call-dialog',
    templateUrl: './video-call-dialog.component.html',
    styleUrls: ['./video-call-dialog.component.scss'],
    animations: [
        heartBeatAnimation({ anchor: 'heartBeat', direction: '<=>', duration: 1000 })
    ]
})
export class VideoCallDialogComponent implements OnInit, OnDestroy {
    @ViewChild('receivedVideo', { static: false })
    receivedVideo: ElementRef

    @ViewChild('localVideo', { static: false })
    localVideo: ElementRef

    invitee: ChatOnlineUser
    participant: ParticipantVideo
    roomName = ''
    videoCallState = VideoCallState
    currentCallState: VideoCallState

    isDialing = false
    animationInterval

    isRinging = false
    isWaitingDrop = false

    isHandle = false

    allowDisplayToastError = true

    handshakedRoom: VideoRoomModel
    sup: Subscription = new Subscription()
    iceServer: RtcIceServer
    currentRtcConnection: RTCPeerConnection

    constructor(
        public dialogRef: MatDialogRef<PageDatasourceGridComponent>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private videoService: VideoCallService,
        private action$: Actions,
        private store: Store,
        private logger: NGXLogger,
        private shortcutUtil: ShortcutUtil,
        private breakpointObserver: BreakpointObserver,
    ) {

        this.breakpointObserver.observe([
            Breakpoints.Handset,
            Breakpoints.Tablet
        ]).subscribe(result => {
            if (result.matches) {
                this.isHandle = true
            }
            else {
                this.isHandle = false
            }
        });
    }

    ngOnDestroy(): void {
        this.sup.unsubscribe()
    }

    ngOnInit(): void {
        this.invitee = this.data.invitee
        this.roomName = this.invitee.fullName
        if (!this.data.isRinging) {
            this.currentCallState = VideoCallState.Dialing
            this.animationInterval = setInterval(() => {
                this.isDialing = !this.isDialing
            }, 1000)
            // Perform calling
            this.call()
        }
        else {
            this.participant = this.data.participant
            this.currentCallState = VideoCallState.Ringing
            this.animationInterval = setInterval(() => {
                this.isRinging = !this.isRinging
            }, 1000)
        }
        this.sup.add(
            this.action$.pipe(
                ofActionSuccessful(HandshakedVideoCall)
            ).subscribe(
                () => {
                    clearInterval(this.animationInterval)
                    this.currentCallState = VideoCallState.Streaming
                    this.handshakedRoom = this.store.selectSnapshot(CHAT_STATE_TOKEN).handshakedVideoCall

                    if (!this.isWaitingDrop) {
                        this.startVideoStream()
                    }
                }
            )
        )

        this.sup.add(
            this.action$.pipe(
                ofActionSuccessful(ReceivedIceServer)
            ).subscribe(
                () => {
                    this.iceServer = this.store.selectSnapshot(CHAT_STATE_TOKEN).iceServer
                    this.initRtcConnect(
                        this.iceServer,
                        this.handshakedRoom.id,
                        this.handshakedRoom.participants.find(a => a.username === this.invitee.userName).connectionId
                    )
                }
            )
        )

        this.sup.add(
            this.action$.pipe(
                ofActionSuccessful(ForceDroppedCall)
            ).subscribe(
                () => {
                    if (this.allowDisplayToastError) {
                        const error = this.store.selectSnapshot(CHAT_STATE_TOKEN).callErrorCode
                        this.shortcutUtil.toastMessage(error.messageContent, ToastType.Error)
                        this.closed()
                    }

                }
            )
        )

        this.sup.add(
            this.action$.pipe(
                ofActionSuccessful(UserDeniedCall)
            ).subscribe(
                () => {
                    if (this.allowDisplayToastError) {
                        this.shortcutUtil.toastMessage('User rejected a call', ToastType.Warning)
                    }
                    this.store.dispatch(new DroppedCall())
                    this.dialogRef.close()
                })
        )

        this.sup.add(
            this.action$.pipe(
                ofActionCompleted(UserDroppedCall)
            ).subscribe(
                () => {
                    // Prevent some incoming exceptions, disable toast
                    this.allowDisplayToastError = false
                    this.shortcutUtil.toastMessage('User ended a call', ToastType.Warning)
                    this.currentRtcConnection.close()
                    this.store.dispatch(new DroppedCall())
                    this.dialogRef.close()
                }
            )
        )

        this.sup.add(
            this.action$.pipe(
                ofActionCompleted(UserCancelledCall)
            ).subscribe(
                () => {
                    // Caller is cancelled a call when he is dialing
                    // So we drop the call as well
                    if (this.currentRtcConnection) {
                        this.currentRtcConnection.close()
                    }                    
                    this.shortcutUtil.toastMessage('User ended a call', ToastType.Warning)
                    this.store.dispatch(new DroppedCall())
                    this.dialogRef.close()
                }
            )
        )

        this.sup.add(
            this.videoService.rtcSignalMessage$.subscribe(message => {
                const signalMessage: VideoRtSignal = JSON.parse(message.signalMessage)
                this.proceedVideoRtcSignal(message.connectionId, signalMessage)
            })
        )
    }

    call() {
        this.videoService.signalingInvitee(this.invitee)
    }

    answer() {
        this.videoService.answeringVideoCall(this.participant)
    }

    deny() {
        this.videoService.denyCall(this.participant)
        this.currentCallState = VideoCallState.Dropped
        this.dialogRef.close()
    }

    dropCall() {
        this.currentRtcConnection.close()
        this.videoService.droppedCall(this.handshakedRoom.id)
        this.currentCallState = VideoCallState.Dropped
        this.dialogRef.close()
    }

    closed() {
        switch (this.currentCallState) {
            case VideoCallState.Dialing:

                this.isWaitingDrop = true
                this.videoService.cancelCall(this.invitee)
                setTimeout(() => {
                    // Very rare case, but we should wait a handshake completely
                    // Then dropping the call, use the flat isWaitingDrop for preventing
                    if (this.currentCallState === VideoCallState.Dropped) {
                        this.dropCall()
                    }
                    // We accept a force closed here    
                    this.dialogRef.close()
                }, 500)
                break
            case VideoCallState.Ringing:
                this.deny()
                break
            case VideoCallState.Streaming:
                this.dropCall()
                break
            case VideoCallState.Dropped:
                this.dialogRef.close()
                break
        }
    }

    private proceedVideoRtcSignal(partnerConnectionId: string, message: VideoRtSignal) {
        switch (message.type) {
            case VideoRtcType.IceCandidate:
                this.addIceServer(message.candidate)
                break
            case VideoRtcType.Offer:
                this.proceedVideoOfferSignal(partnerConnectionId, message.spd)
                break
            case VideoRtcType.Answer:
                this.proceedVideoAnswerSignal(message.spd)
                break
        }
    }


    private addIceServer(candidate: RTCIceCandidate) {
        try {
            this.currentRtcConnection.addIceCandidate(candidate)
        }
        catch{

        }
    }

    private async proceedVideoOfferSignal(partnerConnectionId: string, sdp: RTCSessionDescription) {
        try {
            const connection = this.currentRtcConnection
            const desc = new RTCSessionDescription(sdp)

            await connection.setRemoteDescription(desc)
            const answer = await connection.createAnswer()
            await connection.setLocalDescription(answer)
            this.videoService.sendRtcSignal({
                connectionId: partnerConnectionId,
                roomId: this.handshakedRoom.id,
                signalMessage: JSON.stringify({
                    type: VideoRtcType.Answer,
                    spd: connection.localDescription
                })
            })
        }
        catch{

        }
    }


    private proceedVideoAnswerSignal(sdp: RTCSessionDescription) {
        try {
            this.currentRtcConnection.setRemoteDescription(sdp)
        }
        catch{

        }
    }

    private async startVideoStream() {
        const videoConstraint: MediaStreamConstraints = {
            video: true,
            audio: true
        }

        try {
            const webCam = await navigator.mediaDevices.getUserMedia(videoConstraint)            
            this.localVideo.nativeElement.srcObject = webCam
            webCam.getTracks().forEach((track: MediaStreamTrack) => {                
                this.currentRtcConnection.addTransceiver(track, { streams: [webCam] })
            })
        }
        catch (err) {
            this.logger.error('Streaming got a problem', err)
            this.handleStreamingError(err)
        }
    }

    private handleStreamingError(err: any) {
        switch (err.name) {
            case 'NotFoundError':
                this.shortcutUtil.toastMessage('We cannot detect the camera', ToastType.Error)
                break
            case 'SecurityError':
            case 'PermissionDeniedError':
                this.shortcutUtil.toastMessage('We cannot use the camera', ToastType.Error)
                break
            default:
                this.shortcutUtil.toastMessage('Something went wrong with the camera', ToastType.Error)
                break
        }
        // If there are no special, we force drop a call
        //this.dropCall()
    }

    private initRtcConnect(iceServer: RtcIceServer, roomId: string, inviteeConnectionId: string) {
        const connection = new RTCPeerConnection({ iceServers: [iceServer] })
        connection.onnegotiationneeded = async () => {
            try {
                const rtcDesc = await connection.createOffer()
                await connection.setLocalDescription(rtcDesc)
                this.videoService.sendRtcSignal({
                    connectionId: inviteeConnectionId,
                    roomId: roomId,
                    signalMessage: JSON.stringify({
                        type: VideoRtcType.Offer,
                        spd: rtcDesc
                    })
                })
            }
            catch{

            }
        }
        connection.oniceconnectionstatechange = () => {
            switch (connection.iceConnectionState) {
                case 'closed':
                case 'failed':
                case 'disconnected':
                    // Close the current video
                    break;
            }
        }
        connection.onsignalingstatechange = () => {
            switch (connection.signalingState) {
                case 'closed':
                    // Close the current video
                    break;
            }
        }
        connection.onicecandidate = async (event) => {
            if (event.candidate) {
                this.videoService.sendRtcSignal({
                    connectionId: inviteeConnectionId,
                    roomId: roomId,
                    signalMessage: JSON.stringify({
                        type: VideoRtcType.IceCandidate,
                        candidate: event.candidate
                    })
                })
            }
        }
        connection.onconnectionstatechange = (state) => {
            const states = {
                'iceConnectionState': connection.iceConnectionState,
                'iceGatheringState': connection.iceGatheringState,
                'connectionState': connection.connectionState,
                'signalingState': connection.signalingState
            }

            this.logger.debug('Video Connection State changed', states)
        }
        connection.ontrack = (event) => {
            // Received a stream from partner
            // Render it on receivedVideo
            this.receivedVideo.nativeElement.srcObject = event.streams[0]
        }

        this.currentRtcConnection = connection
    }
}

interface VideoRtSignal {
    type: VideoRtcType
    spd?: RTCSessionDescription
    candidate?: RTCIceCandidate
}

enum VideoRtcType {
    IceCandidate,
    Offer,
    Answer
}

enum VideoCallState {
    Dialing, // User is dialing
    Ringing, // User received a incoming call
    Streaming, // User is streaming video call
    Dropped // User dropped a call
}