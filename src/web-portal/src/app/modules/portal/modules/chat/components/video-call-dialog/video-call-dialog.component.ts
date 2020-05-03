import { Component, OnInit, ViewChild, ElementRef, Inject, OnDestroy } from '@angular/core';
import { ChatService } from 'services/chat.service';
import { NGXLogger } from 'ngx-logger';
import { VideoCallService } from 'services/videocall.service';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
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
import { TranslateService } from '@ngx-translate/core';
@Component({
    selector: 'let-video-call-dialog',
    templateUrl: './video-call-dialog.component.html',
    styleUrls: ['./video-call-dialog.component.scss'],
    animations: [
        heartBeatAnimation({ anchor: 'heartBeat', direction: '<=>', duration: 1000 })
    ]
})
export class VideoCallDialogComponent implements OnInit, OnDestroy {

    invitee: ChatOnlineUser
    participant: ParticipantVideo
    roomName = ''
    videoCallState = VideoCallState
    currentCallState: VideoCallState

    isDialing = false
    animationInterval

    isRinging = false
    isWaitingDrop = false

    isHandset = false

    allowDisplayToastError = true
    @ViewChild('audio', { static: true })
    audio: ElementRef<HTMLAudioElement>
    @ViewChild('audioDrop', { static: true })
    audioDrop: ElementRef<HTMLAudioElement>

    maximumDialingTime = 5

    handshakedRoom: VideoRoomModel
    sup: Subscription = new Subscription()
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
        private translate: TranslateService
    ) {
        this.breakpointObserver.observe([
            Breakpoints.Handset,
            Breakpoints.Tablet
        ]).subscribe(result => {
            if (result.matches) {
                this.isHandset = true
            }
            else {
                this.isHandset = false
            }
        })
    }

    ngOnDestroy(): void {
        this.sup.unsubscribe()
    }

    ngOnInit(): void {
        this.invitee = this.data.invitee
        this.roomName = this.invitee.fullName
        if (!this.data.isRinging) {
            this.currentCallState = VideoCallState.Dialing
            this.audio.nativeElement.loop = true
            this.audio.nativeElement.play()
            this.animationInterval = setInterval(() => {
                this.isDialing = !this.isDialing
                // Try to resend request
                this.videoService.signalingInvitee(this.invitee)
                if (this.maximumDialingTime > 0) {
                    this.maximumDialingTime--
                }
                else if (this.maximumDialingTime === 0) {
                    this.shortcutUtil.toastMessage(this.translate.instant('chats.videoCall.messages.userUnreachable'), ToastType.Warning)
                    clearInterval(this.animationInterval)
                }
            }, 1000)
            // Perform calling
            this.call()
        }
        else {
            this.participant = this.data.participant
            this.currentCallState = VideoCallState.Ringing
            this.audio.nativeElement.loop = true
            this.audio.nativeElement.play()
            this.animationInterval = setInterval(() => {
                this.isRinging = !this.isRinging
            }, 1000)
        }
        this.sup.add(
            this.action$.pipe(
                ofActionSuccessful(HandshakedVideoCall)
            ).subscribe(
                () => {
                    this.audio.nativeElement.pause()
                    clearInterval(this.animationInterval)
                    this.currentCallState = VideoCallState.Streaming
                    this.handshakedRoom = this.store.selectSnapshot(CHAT_STATE_TOKEN).handshakedVideoCall
                }
            )
        )

        this.sup.add(
            this.action$.pipe(
                ofActionSuccessful(ForceDroppedCall)
            ).subscribe(
                () => {
                    clearInterval(this.animationInterval)
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
                    clearInterval(this.animationInterval)
                    if (this.allowDisplayToastError) {
                        this.shortcutUtil.toastMessage(this.translate.instant('chats.videoCall.messages.rejectedCall'), ToastType.Warning)
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
                    this.allowDisplayToastError = false
                    this.closed()
                }
            )
        )

        this.sup.add(
            this.action$.pipe(
                ofActionCompleted(DroppedCall)
            ).subscribe(
                () => {
                    this.closed()
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
                    this.shortcutUtil.toastMessage(this.translate.instant('chats.videoCall.messages.userEndCall'), ToastType.Warning)
                    this.store.dispatch(new DroppedCall())
                    this.dialogRef.close()
                }
            )
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
        this.currentCallState = VideoCallState.Dropped
        this.dialogRef.close()
    }

    closed() {
        this.audioDrop.nativeElement.play()
        setTimeout(() => {
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
        }, 200)

    }
}


enum VideoCallState {
    Dialing, // User is dialing
    Ringing, // User received a incoming call
    Streaming, // User is streaming video call
    Dropped // User dropped a call
}