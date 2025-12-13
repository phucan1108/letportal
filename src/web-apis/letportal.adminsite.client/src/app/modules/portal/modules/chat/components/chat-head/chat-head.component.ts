import { Component, OnInit, Output, EventEmitter, Input, ViewChild, ElementRef } from '@angular/core';
import { fadeInOnEnterAnimation, fadeOutOnLeaveAnimation, fadeInUpOnEnterAnimation, fadeOutDownOnLeaveAnimation, fadeInRightOnEnterAnimation, fadeInLeftOnEnterAnimation, fadeOutRightOnLeaveAnimation, headShakeAnimation } from 'angular-animations';
import { ChatOnlineUser } from '../../../../../../core/models/chat.model';
import { Observable, Subscription } from 'rxjs';
import { Store, Actions, ofActionCompleted, ofActionDispatched } from '@ngxs/store';
import { ActiveChatSearchBox, ClickedOnChatUser, ActiveDoubleChatRoom, IncomingMessageFromUnloadUser } from 'stores/chats/chats.actions';
import { ChatStateModel } from 'stores/chats/chats.state';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
    selector: 'let-chat-head',
    templateUrl: './chat-head.component.html',
    styleUrls: ['./chat-head.component.scss'],
    animations: [
        headShakeAnimation()
    ]
})
export class ChatHeadComponent implements OnInit {

    showChatBadge = false
    isShowSearchBox = false
    hasNewIncomeMessage = false
    currentUser: ChatOnlineUser
    sup: Subscription = new Subscription()

    @ViewChild('audio', { static: true })
    audio: ElementRef<HTMLAudioElement>
    constructor(
        private store: Store,
        private actions$: Actions
    ) { }

    ngOnInit(): void {         
        this.sup.add(this.actions$.pipe(
            ofActionCompleted(ActiveChatSearchBox)
        ).subscribe(
            res => {
                this.isShowSearchBox = true
            }
        ))

        this.sup.add(this.actions$.pipe(
            ofActionCompleted(ActiveDoubleChatRoom)
        ).subscribe(
            () => {
                this.isShowSearchBox = false
            }
        ))

        this.sup.add(this.actions$.pipe(            
            ofActionCompleted(IncomingMessageFromUnloadUser),
            debounceTime(1000)
        ).subscribe(
            () => {                
                if(!this.hasNewIncomeMessage){
                    this.hasNewIncomeMessage = true 
                    this.audio.nativeElement.play() 
                }              
            }
        ))
    }

    showSearchBox() {
        this.hasNewIncomeMessage = false
        this.store.dispatch(new ActiveChatSearchBox())
    }

    onSearchBoxClosed(){
        this.isShowSearchBox = false
    }

    clickedChatUser($event: ChatOnlineUser){        
        this.store.dispatch(new ClickedOnChatUser({
            inviee: $event
        }))
    }
}
