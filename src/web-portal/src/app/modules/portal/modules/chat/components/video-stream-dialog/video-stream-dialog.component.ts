import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { VideoCallService } from 'services/videocall.service';
import { Store, Actions, ofActionSuccessful, ofActionCompleted } from '@ngxs/store';
import { NGXLogger } from 'ngx-logger';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Subscription } from 'rxjs';
import { RtcIceServer, VideoRoomModel, ChatOnlineUser } from 'app/core/models/chat.model';
import { HandshakedVideoCall, ReceivedIceServer, UserDroppedCall, DroppedCall } from 'stores/chats/chats.actions';
import { CHAT_STATE_TOKEN } from 'stores/chats/chats.state';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { bounceInLeftOnEnterAnimation, bounceOutRightOnLeaveAnimation, bounceInRightOnEnterAnimation, slideInRightOnEnterAnimation, slideOutRightOnLeaveAnimation, slideOutLeftOnLeaveAnimation } from 'angular-animations';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'let-video-stream',
    templateUrl: './video-stream-dialog.component.html',
    styleUrls: ['./video-stream-dialog.component.scss'],
    animations: [
        slideInRightOnEnterAnimation({ anchor: 'enter' }),
        slideOutLeftOnLeaveAnimation({ anchor: 'leave' })
    ]
})
export class VideoStreamDialogComponent implements OnInit {

    @ViewChild('receivedVideo', { static: false })
    receivedVideo: ElementRef

    @ViewChild('localVideo', { static: false })
    localVideo: ElementRef
    
    @HostListener("window:resize", [])
    public onResize() {
      this.maxWidth = window.innerWidth
      this.maxHeight = window.innerHeight
    }
    maxWidth = window.innerWidth
    maxHeight = window.innerHeight

    turnOn = false
    // It will be changed by device such as Desktop, Tablet, Mobile    
    currentVideoConstraints: VideoContraints = {
        maxWidth: 800,
        minWidth: 640,
        maxHeight: 600,
        minHeight: 480,
        ratio: 1.333333333,
        idealWidth: 800,
        idealHeight: 600,
        muted: false,
        videoOn: true
    }
    receivedVideoConstraints: VideoContraints = {
        maxWidth: 800,
        minWidth: 640,
        maxHeight: 600,
        minHeight: 480,
        ratio: 1.333333333,
        idealWidth: 800,
        idealHeight: 600,
        muted: false,
        videoOn: true
    }
    handshakedRoom: VideoRoomModel
    sup: Subscription = new Subscription()
    invitee: ChatOnlineUser
    iceServers: RtcIceServer[] = []
    currentRtcConnection: RTCPeerConnection
    mediaStream: MediaStream
    currentDevice: DeviceDetect = DeviceDetect.Desktop
    constructor(
        private store: Store,
        private videoService: VideoCallService,
        private logger: NGXLogger,
        private breakpointObserver: BreakpointObserver,
        private actions$: Actions,
        private shortcutUtil: ShortcutUtil,
        private translate: TranslateService
    ) {
        this.breakpointObserver.observe([
            Breakpoints.Web
        ]).subscribe(result => {
            if (result.matches && this.currentDevice != DeviceDetect.Desktop) {
                this.currentDevice = DeviceDetect.Desktop
                this.applyCurrentVideoConstraints(this.getVideoConstraints(this.currentDevice))
                this.logger.debug('Is Desktop', this.currentVideoConstraints)
            }
        })

        this.breakpointObserver.observe([
            Breakpoints.TabletLandscape
        ]).subscribe(result => {
            if (result.matches && this.currentDevice != DeviceDetect.TabletLandscape) {
                this.currentDevice = DeviceDetect.TabletLandscape
                this.applyCurrentVideoConstraints(this.getVideoConstraints(this.currentDevice))
                this.logger.debug('Is Tablet Landscape', this.currentVideoConstraints)
            }
        });

        this.breakpointObserver.observe([
            Breakpoints.TabletPortrait
        ]).subscribe(result => {
            if (result.matches && this.currentDevice != DeviceDetect.TabletPortrait) {
                this.currentDevice = DeviceDetect.TabletPortrait
                this.applyCurrentVideoConstraints(this.getVideoConstraints(this.currentDevice))
                this.logger.debug('Is Tablet Portrait', this.currentVideoConstraints)
            }
        })

        this.breakpointObserver.observe([
            Breakpoints.HandsetLandscape
        ]).subscribe(result => {
            if (result.matches && this.currentDevice != DeviceDetect.MobileLandscape) {
                this.currentDevice = DeviceDetect.MobileLandscape
                this.applyCurrentVideoConstraints(this.getVideoConstraints(this.currentDevice))
                this.logger.debug('Is Mobile Landscape', this.currentVideoConstraints)
            }
        })

        this.breakpointObserver.observe([
            Breakpoints.HandsetPortrait
        ]).subscribe(result => {
            if (result.matches && this.currentDevice != DeviceDetect.MobilePortrait) {
                this.currentDevice = DeviceDetect.MobilePortrait
                this.applyCurrentVideoConstraints(this.getVideoConstraints(this.currentDevice))
                this.logger.debug('Is Mobile Portrait', this.currentVideoConstraints)
            }
        })
    }

    ngOnInit(): void {
        this.sup.add(
            this.videoService.rtcSignalMessage$.subscribe(async (message) => {
                const signalMessage: VideoRtSignal = JSON.parse(message.signalMessage)
                await this.proceedVideoRtcSignal(message.connectionId, signalMessage)
            })
        )
        this.sup.add(
            this.actions$.pipe(
                ofActionSuccessful(HandshakedVideoCall)
            ).subscribe(
                () => {
                    this.turnOn = true
                    this.handshakedRoom = this.store.selectSnapshot(CHAT_STATE_TOKEN).handshakedVideoCall                    
                    const allUsers = this.store.selectSnapshot(CHAT_STATE_TOKEN).availableUsers
                    const currentUser = this.store.selectSnapshot(CHAT_STATE_TOKEN).currentUser
                    this.invitee = allUsers.find(c => c.userName === this.handshakedRoom.participants.find(a => a.username !== currentUser.userName).username)
                    this.startVideoStream()
                }
            )
        )

        this.sup.add(
            this.actions$.pipe(
                ofActionSuccessful(ReceivedIceServer)
            ).subscribe(
                () => {
                    this.iceServers = this.store.selectSnapshot(CHAT_STATE_TOKEN).iceServers
                    this.initRtcConnect(
                        this.iceServers,
                        this.handshakedRoom.id,
                        this.handshakedRoom.participants.find(a => a.username === this.invitee.userName).connectionId
                    )
                }
            )
        )


        this.sup.add(
            this.actions$.pipe(
                ofActionCompleted(UserDroppedCall)
            ).subscribe(
                () => {
                    this.shortcutUtil.toastMessage(this.translate.instant('chats.videoCall.messages.userEndCall'), ToastType.Warning)
                    this.dropAll()
                }
            )
        )
    }

    dropped() {
        this.notifyDrop()
        this.dropAll()
    }

    muted() {
        this.currentVideoConstraints.muted = !this.currentVideoConstraints.muted
        this.mediaStream.getAudioTracks()?.forEach(track => {
            track.enabled = !this.currentVideoConstraints.muted
        })
    }

    cameraOn() {
        this.currentVideoConstraints.videoOn = !this.currentVideoConstraints.videoOn
        this.mediaStream.getVideoTracks()?.forEach(track => {
            track.enabled = this.currentVideoConstraints.videoOn
        })
    }

    private dropAll() {
        try {
            this.currentRtcConnection.close()
            this.mediaStream.getTracks()?.forEach(track => track.stop())
            this.mediaStream = null
        }
        catch (err) {
            this.logger.error('Error when turning off a camera', err)
        }
        this.turnOn = false
        this.handshakedRoom = null
        this.iceServers = null
        this.invitee = null
        this.currentVideoConstraints.muted = false
        this.currentVideoConstraints.videoOn = true
    }

    private notifyDrop() {
        this.videoService.droppedCall(this.handshakedRoom.id)
    }

    private async proceedVideoRtcSignal(partnerConnectionId: string, message: VideoRtSignal) {
        switch (message.type) {
            case VideoRtcType.IceCandidate:
                this.addIceServer(message.candidate)
                break
            case VideoRtcType.Offer:
                // In case we got an offer, current user need to do two steps
                // 1. Set RemoteRtc by sent sdp
                // 2. Send back answer signal to let caller apply RemoteRtc as well
                await this.proceedVideoOfferSignal(partnerConnectionId, message.spd)
                break
            case VideoRtcType.Answer:
                this.proceedVideoAnswerSignal(message.spd)
                break
        }
    }

    private applyCurrentVideoConstraints(videoConstraint: VideoContraints) {
        this.currentVideoConstraints = videoConstraint
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
            const rtcSignal: VideoRtSignal = {
                type: VideoRtcType.Answer,
                spd: connection.localDescription,
                videoConstraints: this.currentVideoConstraints
            }
            this.videoService.sendRtcSignal({
                connectionId: partnerConnectionId,
                roomId: this.handshakedRoom.id,
                signalMessage: JSON.stringify(rtcSignal)
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
        // For full options: https://developer.mozilla.org/en-US/docs/Web/API/MediaDevices/getUserMedia
        const videoConstraint: MediaStreamConstraints = {
            video: {
                width: {
                    max: this.currentVideoConstraints.maxWidth,
                    min: this.currentVideoConstraints.minWidth,
                    ideal: this.currentVideoConstraints.idealWidth
                },
                height: {
                    max: this.currentVideoConstraints.maxHeight,
                    min: this.currentVideoConstraints.minHeight,
                    ideal: this.currentVideoConstraints.idealHeight
                },
                aspectRatio: {
                    min: 1.333333333,
                    max: 1.777777778,
                    ideal: 1.333333333
                },
                frameRate: {
                    min: 30,
                    max: 60,
                    ideal: 60
                }
            },
            audio: true
        }

        try {            
            this.mediaStream = await navigator.mediaDevices.getUserMedia(videoConstraint)
            this.localVideo.nativeElement.srcObject = this.mediaStream            
            this.mediaStream.getTracks()?.forEach((track: MediaStreamTrack) => {           
                this.currentRtcConnection.addTransceiver(track, { streams: [this.mediaStream] })
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
                this.shortcutUtil.toastMessage(this.translate.instant('chats.videoCall.messages.cannotDetectCamera'), ToastType.Error)                
                this.dropped()
                break
            case 'SecurityError':
            case 'PermissionDeniedError':
                this.shortcutUtil.toastMessage(this.translate.instant('chats.videoCall.messages.cannotUseCamera'), ToastType.Error)                
                this.dropped()
                break
            default:
                this.shortcutUtil.toastMessage(this.translate.instant('chats.videoCall.messages.cameraError'), ToastType.Error)                
                this.dropped()
                break
        }
    }

    makingOffer = false // Flag to avoid race condition
    private initRtcConnect(iceServers: RtcIceServer[], roomId: string, inviteeConnectionId: string) {
        const connection = new RTCPeerConnection({ iceServers: iceServers })
        connection.onnegotiationneeded = async () => {
            // P2P will send offer to partner for making Negotiation
            // If it failed, force drop a call
            try {
                if (this.makingOffer) {
                    return
                }
                this.makingOffer = true
                const rtcDesc = await connection.createOffer()
                await connection.setLocalDescription(rtcDesc)

                // If we use pure RTC, we should use onmessage to make perfect negotiation
                // However, we combine with SignalR to exchange message perfectly
                // So we don't want to use this technique here for pushing security edge to RTC
                // https://developer.mozilla.org/en-US/docs/Web/API/WebRTC_API/Perfect_negotiation

                // Send RtCSignal to partner for letting them create their RtcConnection
                // And send back an answer message
                const rtcSignal: VideoRtSignal = {
                    type: VideoRtcType.Offer,
                    spd: connection.localDescription,
                    videoConstraints: this.currentVideoConstraints
                }
                this.videoService.sendRtcSignal({
                    connectionId: inviteeConnectionId,
                    roomId: roomId,
                    signalMessage: JSON.stringify(rtcSignal)
                })
            }
            catch (err) {
                this.logger.error('Negotiaion Error', err)
            }
            finally {
                this.makingOffer = false
            }
        }
        connection.oniceconnectionstatechange = () => {
            // Support for changed TUN server, we can implement reconnect logic here
            // By default, we force drop a call
            switch (connection.iceConnectionState) {
                case 'closed':
                case 'failed':
                case 'disconnected':
                    this.logger.error('ICE connection has been closed')
                    // Close the current video
                    break;
            }
        }
        connection.onsignalingstatechange = () => {
            switch (connection.signalingState) {
                case 'closed':
                    // Close the current video
                    this.logger.error('Partner connection has been closed')
                    this.dropped()
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

    private getVideoConstraints(device: DeviceDetect): VideoContraints {
        switch (device) {
            case DeviceDetect.Desktop:
                return {
                    maxWidth: 800,
                    minWidth: 640,
                    maxHeight: 600,
                    minHeight: 480,
                    ratio: 1.333333333,
                    idealWidth: 800,
                    idealHeight: 600,
                    muted: false,
                    videoOn: true
                }
            case DeviceDetect.TabletLandscape:
                return {
                    maxWidth: 800,
                    minWidth: 640,
                    maxHeight: 600,
                    minHeight: 480,
                    ratio: 1.333333333,
                    idealWidth: 800,
                    idealHeight: 600,
                    muted: false,
                    videoOn: true
                }
            case DeviceDetect.TabletPortrait:
                return {
                    maxWidth: 800,
                    minWidth: 640,
                    maxHeight: 600,
                    minHeight: 480,
                    ratio: 1.333333333,
                    idealWidth: 800,
                    idealHeight: 600,
                    muted: false,
                    videoOn: true
                }
            case DeviceDetect.MobileLandscape:
                // Support 16:9 -> 640x360 to 1280x720
                return {
                    maxWidth:1280,
                    minWidth: 640,
                    maxHeight: 720,        
                    minHeight: 360,
                    ratio: 1.777777778,        
                    idealWidth: 1280,
                    idealHeight: 720,
                    muted: false,
                    videoOn: true
                }
            case DeviceDetect.MobilePortrait:
                // Support 16:9 -> 640x360
                return {
                    maxWidth:1280,
                    minWidth: 640,
                    maxHeight: 720,        
                    minHeight: 360,
                    ratio: 1.777777778,        
                    idealWidth: 1280,
                    idealHeight: 720,
                    muted: false,
                    videoOn: true
                }
        }
    }

    setStreamStyle() {
        switch (this.currentDevice) {
            case DeviceDetect.Desktop:
                return {
                    'width': '1024px',
                    'height': '768px'
                }
            case DeviceDetect.TabletLandscape:
                return {
                    'width': this.maxWidth.toString() + 'px',
                    'height': this.maxHeight.toString() + 'px'
                }
            case DeviceDetect.TabletPortrait:
                return {
                    'width': this.maxWidth.toString() + 'px',
                    'height': this.maxHeight.toString() + 'px'
                }
            case DeviceDetect.MobileLandscape:
                return {
                    'width': this.maxWidth.toString() + 'px',
                    'height': this.maxHeight.toString() + 'px'
                }
            case DeviceDetect.MobilePortrait:
                return {
                    'width': this.maxWidth.toString() + 'px',
                    'height': this.maxHeight.toString() + 'px'
                }
        }
    }

    setLocalStreamStyle() {
        switch (this.currentDevice) {
            case DeviceDetect.Desktop:
                return {
                    'width': '200px',
                    'height': '150px'
                }
            case DeviceDetect.TabletLandscape:
                return {
                    'width': '200px',
                    'height': '150px'
                }
            case DeviceDetect.TabletPortrait:
                return {
                    'width': '200px',
                    'height': '150px'
                }
            case DeviceDetect.MobileLandscape:
                return {
                    'width': '200px',
                    'height': '112px'
                }
            case DeviceDetect.MobilePortrait:
                return {
                    'width': '200px',
                    'height': '112px'
                }
        }
    }
}

interface VideoRtSignal {
    type: VideoRtcType
    spd?: RTCSessionDescription
    candidate?: RTCIceCandidate
    videoConstraints?: VideoContraints // Aware partner video constraints
}

enum VideoRtcType {
    IceCandidate,
    Offer,
    Answer
}

interface VideoContraints {
    minWidth: number
    maxWidth: number
    minHeight: number
    maxHeight: number
    idealWidth: number
    idealHeight: number
    ratio: number
    muted: boolean
    videoOn: boolean
}

enum DeviceDetect {
    Desktop,
    TabletLandscape,
    TabletPortrait,
    MobileLandscape,
    MobilePortrait
}