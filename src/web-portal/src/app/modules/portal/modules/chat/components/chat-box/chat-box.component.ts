import { Component, OnInit, Input, OnDestroy, Output, EventEmitter } from '@angular/core';
import { ChatRoom, RoomType, DoubleChatRoom, ChatOnlineUser } from '../../models/chat.model';
import { ChatService } from 'services/chat.service';
import { Subscription, Observable } from 'rxjs';
import { fadeInRightOnEnterAnimation, fadeOutRightOnLeaveAnimation } from 'angular-animations';

@Component({
    selector: 'let-chat-box',
    templateUrl: './chat-box.component.html',
    styleUrls: ['./chat-box.component.scss'],
    animations: [
        fadeInRightOnEnterAnimation({ duration: 500, translate: '30px' }),
        fadeOutRightOnLeaveAnimation({ duration: 500 ,translate: '30px' })
    ]
})
export class ChatBoxComponent implements OnInit, OnDestroy {
    showChatBadge = false
    @Input()
    chatRoom: ChatRoom

    @Input()
    hide$: Observable<boolean>

    @Output()
    onClickIcon: EventEmitter<boolean> = new EventEmitter()

    doubleChatRoom: DoubleChatRoom
    
    hasRoomAvatar = false
    roomShortName: string
    roomName: string
    isOnline = false
    isHideChatBox = false
    isReadyToDisplayBox = false

    offlineSup: Subscription
    onlineSup: Subscription
    hideSup: Subscription
    chatRoomSup: Subscription
    constructor(
        private chatService: ChatService
    ) { }

    ngOnInit(): void { 
        if(this.chatRoom.type === RoomType.Double){
            this.doubleChatRoom = <DoubleChatRoom>this.chatRoom
            this.hasRoomAvatar = this.doubleChatRoom.invitee.hasAvatar
            this.roomName = this.doubleChatRoom.invitee.fullName
            this.roomShortName = this.doubleChatRoom.invitee.shortName
            this.isOnline = this.doubleChatRoom.invitee.isOnline
        }

        this.offlineSup = this.chatService.offlineUser$.subscribe(res => {
            if(this.doubleChatRoom.invitee.userName === res.userName){
                this.isOnline = false
            }
        })

        this.onlineSup = this.chatService.onlineUser$.subscribe(res => {
            if(this.doubleChatRoom.invitee.userName === res.userName){
                this.isOnline = true
            }
        })

        this.hideSup = this.hide$.subscribe(a => {
            this.isHideChatBox = a
        })

        this.chatService.openDoubleChatRoom(this.doubleChatRoom.invitee)
        this.chatRoomSup = this.chatService.chatRoom$.subscribe(a => {
            if(a.type === RoomType.Double && a.participants.some(b => b.userName === this.doubleChatRoom.invitee.userName)){
                this.doubleChatRoom.chatRoomId = a.chatRoomId
                this.doubleChatRoom.chatSessions = a.chatSessions
                this.doubleChatRoom.currentSession = a.currentSession
                this.isReadyToDisplayBox = true                
            }
        })
    }

    
    ngOnDestroy(): void {
        this.offlineSup.unsubscribe()
        this.onlineSup.unsubscribe()
        this.hideSup.unsubscribe()
        this.chatRoomSup.unsubscribe()
    }

    showSearchBox(){
        this.isHideChatBox = !this.isHideChatBox
        this.onClickIcon.emit(!this.isHideChatBox)
    }

    onChatBoxClosed(){
        this.isHideChatBox = !this.isHideChatBox
    }
}
