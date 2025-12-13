import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ChatOnlineUser } from 'app/core/models/chat.model';

@Component({
    selector: 'let-avatar',
    templateUrl: './avatar.component.html',
    styleUrls: ['./avatar.component.scss']
})
export class AvatarComponent implements OnInit {

    @Input()
    user: ChatOnlineUser

    @Input()
    trackOnline: boolean

    @Input()
    hasCounterNumber: boolean

    @Input()
    counterNumber: number

    @Input()
    cssClasses: string

    @Output()
    clicked: EventEmitter<any> = new EventEmitter()

    hasRoomAvatar = false
    constructor() { }

    ngOnInit(): void { 
        this.hasRoomAvatar = this.user.hasAvatar
        this.cssClasses += ' d-flex align-items-center justify-content-center'
    }

    onClicked(){
        this.clicked.emit(true)
    }
}
