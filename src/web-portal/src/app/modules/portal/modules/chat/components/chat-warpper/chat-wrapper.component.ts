import { Component, OnInit } from '@angular/core';
import { ChatOnlineUser, ChatRoom, RoomType, DoubleChatRoom } from '../../models/chat.model';
import { ChatService } from 'services/chat.service';
import { NGXLogger } from 'ngx-logger';
import { Subject } from 'rxjs';
import { ObjectUtils } from 'app/core/utils/object-util';

@Component({
    selector: 'let-chat-wrapper',
    templateUrl: './chat-wrapper.component.html',
    styleUrls: ['./chat-wrapper.component.scss']
})
export class ChatWrapperComponent implements OnInit {
    isShowChatHead = true
    isShowChatBox = false

    chatRoom: ChatRoom
    hideChatBox$: Subject<boolean> = new Subject()
    hideSearchBox$: Subject<boolean> = new Subject()
    constructor(
        private chatService: ChatService,
        private logger: NGXLogger
    ) { }

    ngOnInit(): void { }

    clickedChatUser($event: ChatOnlineUser) {
        this.logger.debug('Selected chat user', $event)
        // Check this room is existed or not
        const found = this.chatService.chatRooms.find(a => a.type === RoomType.Double && a.participants.some(b => b.userName === $event.userName))
        if (ObjectUtils.isNotNull(found)) {
            const doubleChatRoom: DoubleChatRoom = {
                ...found,
                invitee: $event
            }
            this.chatRoom = doubleChatRoom
            this.isShowChatBox = true
            this.hideChatBox$.next(false)
        }
        else{
            const doubleChatRoom: DoubleChatRoom = {
                chatRoomId: '',
                chatSessions: [],
                currentSession: null,
                roomName: $event.fullName,
                participants: [
                    $event,
                    this.chatService.currentUser
                ],
                type: RoomType.Double,
                invitee: $event
            }
            this.chatRoom = doubleChatRoom
            this.isShowChatBox = true
        }        
    }

    onClickShowSearchBox(isShowSearchBox: boolean) {
        if (isShowSearchBox) {
            this.hideChatBox$.next(true)
        }
    }

    onClickShowChatBox(isShowChatBox: boolean) {
        if (isShowChatBox) {
            this.hideSearchBox$.next(true)
        }
    }
}
