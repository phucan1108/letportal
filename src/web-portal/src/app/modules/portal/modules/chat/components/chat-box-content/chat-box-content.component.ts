import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { DoubleChatRoom } from '../../models/chat.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'let-chat-box-content',
    templateUrl: './chat-box-content.component.html',
    styleUrls: ['./chat-box-content.component.scss']
})
export class ChatBoxContentComponent implements OnInit {

    @Input()
    chatRoom: DoubleChatRoom

    @Output()
    closed: EventEmitter<any> = new EventEmitter()

    roomName: string
    roomShortName: string
    hasAvatar: boolean

    formGroup: FormGroup
    constructor(
        private fb: FormBuilder
    ) { }

    ngOnInit(): void {
        this.roomName = this.chatRoom.invitee.fullName
        this.roomShortName = this.chatRoom.invitee.shortName
        this.hasAvatar = this.chatRoom.invitee.hasAvatar
        this.formGroup = this.fb.group({
            text: ['', Validators.required]
        })
    }

    onClosed(){
        this.closed.emit()
    }

    send(){
        
    }
}
