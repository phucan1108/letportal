import { Component, OnInit } from '@angular/core';
import { fadeInOnEnterAnimation, fadeOutOnLeaveAnimation, fadeInUpOnEnterAnimation, fadeOutDownOnLeaveAnimation } from 'angular-animations';

@Component({
    selector: 'let-chat-head',
    templateUrl: './chat-head.component.html',
    styleUrls: ['./chat-head.component.scss'],
    animations: [
        fadeInUpOnEnterAnimation({ duration: 500, translate: '30px' }),
        fadeOutDownOnLeaveAnimation({ duration: 500 ,translate: '30px' })
    ]
})
export class ChatHeadComponent implements OnInit {

    showChatBadge = true
    isShowSearchBox = false
    constructor() { }

    ngOnInit(): void { }

    showSearchBox() {
        this.isShowSearchBox = !this.isShowSearchBox
    }

    onSearchBoxClosed(){
        this.isShowSearchBox = !this.isShowSearchBox
    }
}
