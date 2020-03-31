import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { fadeInOnEnterAnimation, fadeOutOnLeaveAnimation, fadeInUpOnEnterAnimation, fadeOutDownOnLeaveAnimation, fadeInRightOnEnterAnimation, fadeInLeftOnEnterAnimation, fadeOutRightOnLeaveAnimation } from 'angular-animations';
import { ChatOnlineUser } from '../../models/chat.model';
import { Observable, Subscription } from 'rxjs';

@Component({
    selector: 'let-chat-head',
    templateUrl: './chat-head.component.html',
    styleUrls: ['./chat-head.component.scss'],
    animations: [
        fadeInRightOnEnterAnimation({ duration: 500, translate: '30px' }),
        fadeOutRightOnLeaveAnimation({ duration: 500 ,translate: '30px' })
    ]
})
export class ChatHeadComponent implements OnInit {

    showChatBadge = false
    isShowSearchBox = false

    @Input()
    hide$: Observable<boolean>

    @Output()
    onClickChatUser: EventEmitter<ChatOnlineUser> = new EventEmitter()

    @Output()
    onClickIcon: EventEmitter<boolean> = new EventEmitter()

    hideSup: Subscription
    constructor() { }

    ngOnInit(): void { 
        this.hideSup = this.hide$.subscribe(res =>{
            this.isShowSearchBox = false
        })
    }

    showSearchBox() {
        this.isShowSearchBox = !this.isShowSearchBox
        this.onClickIcon.emit(this.isShowSearchBox)
    }

    onSearchBoxClosed(){
        this.isShowSearchBox = !this.isShowSearchBox
    }

    clickedChatUser($event: ChatOnlineUser){
        this.isShowSearchBox = false
        this.onClickChatUser.emit($event)        
    }
}
