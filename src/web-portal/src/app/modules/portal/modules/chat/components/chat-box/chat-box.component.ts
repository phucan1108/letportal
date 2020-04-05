import { Component, OnInit, Input, OnDestroy, Output, EventEmitter, ViewChild } from '@angular/core';
import { ChatRoom, RoomType, DoubleChatRoom, ChatOnlineUser, ChatSession } from '../../../../../../core/models/chat.model';
import { ChatService } from 'services/chat.service';
import { Subscription, Observable } from 'rxjs';
import { fadeInRightOnEnterAnimation, fadeOutRightOnLeaveAnimation } from 'angular-animations';
import { Store, Actions, ofActionCompleted, ofActionSuccessful } from '@ngxs/store';
import { ChatStateModel, CHAT_STATE_TOKEN } from 'stores/chats/chats.state';
import { ObjectUtils } from 'app/core/utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { ActiveChatSearchBox, ActiveDoubleChatRoom, ClickedOnChatBox, NotifyNewIncomingMessage, ToggleOpenChatRoom } from 'stores/chats/chats.actions';
import { ChatBoxContentComponent } from '../chat-box-content/chat-box-content.component';

@Component({
    selector: 'let-chat-box',
    templateUrl: './chat-box.component.html',
    styleUrls: ['./chat-box.component.scss'],
    animations: [
        fadeInRightOnEnterAnimation({ duration: 500, translate: '30px' }),
        fadeOutRightOnLeaveAnimation({ duration: 500, translate: '30px' })
    ]
})
export class ChatBoxComponent implements OnInit, OnDestroy {
    showChatBadge = false
    @Input()
    chatRoom: DoubleChatRoom

    @Input()
    index: number

    currentUser: ChatOnlineUser

    isActiveChat = false

    hasRoomAvatar = false
    roomShortName: string
    roomName: string
    isOnline = false
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
            this.hasRoomAvatar = this.chatRoom.invitee.hasAvatar
            this.roomName = this.chatRoom.invitee.fullName
            this.roomShortName = this.chatRoom.invitee.shortName
            this.isOnline = this.chatRoom.invitee.isOnline
        }

        if (this.store.selectSnapshot(CHAT_STATE_TOKEN).activeChatSession.chatRoomId ===
            this.chatRoom.chatRoomId) {
            this.openChatbox()
        }

        this.sup.add(this.chatService.offlineUser$.subscribe(res => {
            if (this.chatRoom.invitee.userName === res.userName) {
                this.isOnline = false
            }
        }))

        this.sup.add(this.chatService.onlineUser$.subscribe(res => {
            if (this.chatRoom.invitee.userName === res.userName) {
                this.isOnline = true
            }
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
                    this.logger.debug('This box is actived ' + this.roomName, this.store.selectSnapshot(CHAT_STATE_TOKEN).activeChatSession.chatRoomId === this.chatRoom.chatRoomId)
                    this.showChatBox = this.store.selectSnapshot(CHAT_STATE_TOKEN).activeChatSession.chatRoomId === this.chatRoom.chatRoomId
                    if (this.showChatBox)
                        this.openChatbox()
                }
            )
        )

        this.sup.add(
            this.actions$.pipe(
                ofActionSuccessful(NotifyNewIncomingMessage),
            ).subscribe(
                () => {
                    const notifiedIncomingMessages = this.store.selectSnapshot(CHAT_STATE_TOKEN).notifiedChatRooms.filter(a => a === this.chatRoom.chatRoomId).length
                    this.counterIncomingMessage = notifiedIncomingMessages
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
        return 'chat-box-head-' + index.toString()
    }
    private getChatBoxClass(index: number) {
        return `chat-box chat-box-${index.toString()} arrow-right p-3`
    }
}
