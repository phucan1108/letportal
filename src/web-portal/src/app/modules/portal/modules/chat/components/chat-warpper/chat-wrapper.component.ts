import { Component, OnInit } from '@angular/core';
import { ChatOnlineUser, ChatRoom, RoomType, DoubleChatRoom } from '../../models/chat.model';
import { ChatService } from 'services/chat.service';
import { NGXLogger } from 'ngx-logger';
import { Subject } from 'rxjs';

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

    clickedChatUser($event: ChatOnlineUser){
        this.logger.debug('Selected chat user', $event)  
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
        this.chatService.openDoubleChatRoom($event)     
    }

    onClickShowSearchBox(isShowSearchBox: boolean){
        if(isShowSearchBox){
            this.hideChatBox$.next(true)
        }
    }

    onClickShowChatBox(isShowChatBox: boolean){
        if(isShowChatBox){
            this.hideSearchBox$.next(true)
        }
    }
}
