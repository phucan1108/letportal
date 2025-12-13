import { Component, OnInit, Input, OnDestroy, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { ChatRoom, RoomType, DoubleChatRoom, ChatOnlineUser, ChatSession } from '../../../../../../core/models/chat.model';
import { ChatService } from 'services/chat.service';
import { Subscription, Observable } from 'rxjs';
import { fadeInRightOnEnterAnimation, fadeOutRightOnLeaveAnimation } from 'angular-animations';
import { Store, Actions, ofActionCompleted, ofActionSuccessful } from '@ngxs/store';
import { ChatStateModel, CHAT_STATE_TOKEN } from 'stores/chats/chats.state';
import { ObjectUtils } from 'app/core/utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { ActiveChatSearchBox, ActiveDoubleChatRoom, ClickedOnChatBox, NotifyNewIncomingMessage, ToggleOpenChatRoom, IncomingOnlineUser, IncomingOfflineUser } from 'stores/chats/chats.actions';
import { ChatBoxContentComponent } from '../chat-box-content/chat-box-content.component';
import { AvatarComponent } from '../avatar/avatar.component';
import { debounceTime } from 'rxjs/operators';

@Component({
    selector: 'let-chat-box',
    templateUrl: './chat-box.component.html',
    styleUrls: ['./chat-box.component.scss'],
    animations: [
    ]
})
export class ChatBoxComponent implements OnInit, OnDestroy {
    showChatBadge = false
    @Input()
    chatRoom: DoubleChatRoom

    @Input()
    index: number

    @ViewChild('avatar', { static: true })
    avatar: AvatarComponent
    @ViewChild('audio', { static: true })
    audio: ElementRef<HTMLAudioElement>    
    @ViewChild('chatBoxContent', { static: false})
    chatBoxContent: ChatBoxContentComponent
    currentUser: ChatOnlineUser
    invitee: ChatOnlineUser
    isActiveChat = false
    showChatBox = false

    sup: Subscription = new Subscription()
    overrideHeadClass = ''
    overrideDialogClass = ''

    counterIncomingMessage = 0
    constructor(
        private store: Store,
        private chatService: ChatService,
        private actions$: Actions,
        private logger: NGXLogger
    ) { }

    ngOnInit(): void {
        this.overrideHeadClass = this.getHeadBoxClass(this.index)
        this.overrideDialogClass = this.getChatBoxClass(this.index)
        this.currentUser = this.store.selectSnapshot(CHAT_STATE_TOKEN).currentUser
        const notifiedIncomingMessages = this.store.selectSnapshot(CHAT_STATE_TOKEN).notifiedChatRooms.filter(a => a === this.chatRoom.chatRoomId).length
        this.counterIncomingMessage = notifiedIncomingMessages
        if (this.chatRoom.type === RoomType.Double) {
            this.invitee = ObjectUtils.clone(this.chatRoom.invitee)
        }

        if (this.store.selectSnapshot(CHAT_STATE_TOKEN).activeChatSession.chatRoomId ===
            this.chatRoom.chatRoomId) {
            this.openChatbox()
        }

        this.sup.add(this.actions$.pipe(
            ofActionSuccessful(IncomingOnlineUser)
        ).subscribe(() => {
            const foundUser = this.store.selectSnapshot(CHAT_STATE_TOKEN).availableUsers
                .find(a => a.userName === this.chatRoom.invitee.userName)
            this.chatBoxContent.isUserOnline = foundUser.isOnline
            this.avatar.user.isOnline = foundUser.isOnline
        }))
        this.sup.add(this.actions$.pipe(
            ofActionSuccessful(IncomingOfflineUser)
        ).subscribe(() => {
            const foundUser = this.store.selectSnapshot(CHAT_STATE_TOKEN).availableUsers
                .find(a => a.userName === this.chatRoom.invitee.userName)
            this.chatBoxContent.isUserOnline = foundUser.isOnline
            this.avatar.user.isOnline = foundUser.isOnline
        }))

        this.sup.add(this.actions$.pipe(
            ofActionCompleted(ActiveChatSearchBox)
        ).subscribe(
            res => {
                if (this.showChatBox) {
                    this.showChatBox = false
                    this.store.dispatch(new ToggleOpenChatRoom(false))
                }
            }
        ))

        this.sup.add(
            this.actions$.pipe(
                ofActionSuccessful(ActiveDoubleChatRoom)
            ).subscribe(
                () => {
                    this.showChatBox = this.store.selectSnapshot(CHAT_STATE_TOKEN).activeChatSession.chatRoomId === this.chatRoom.chatRoomId
                    if (this.showChatBox)
                        this.openChatbox()
                }
            )
        )

        this.sup.add(
            this.actions$.pipe(
                ofActionSuccessful(NotifyNewIncomingMessage),
                debounceTime(1000)
            ).subscribe(
                () => {
                    const notifiedIncomingMessages = this.store.selectSnapshot(CHAT_STATE_TOKEN).notifiedChatRooms.filter(a => a === this.chatRoom.chatRoomId).length
                    this.counterIncomingMessage = notifiedIncomingMessages
                    this.audio.nativeElement.play()
                }
            )
        )
    }

    ngOnDestroy(): void {
        this.sup.unsubscribe()
    }

    showChatBoxContent() {
        if (!this.showChatBox) {
            this.counterIncomingMessage = 0
            this.store.dispatch(new ClickedOnChatBox({
                chatRoomId: this.chatRoom.chatRoomId
            }))
        }
    }

    onChatBoxClosed() {
        this.showChatBox = false
    }

    private openChatbox() {
        this.showChatBox = true
        this.store.dispatch(new ToggleOpenChatRoom(this.showChatBox))
    }

    private getHeadBoxClass(index: number) {
        return 'chat-box-head-' + index.toString() + ' chat-box-head'
    }
    private getChatBoxClass(index: number) {
        return `chat-box chat-box-${index.toString()} arrow-right p-3`
    }
}
