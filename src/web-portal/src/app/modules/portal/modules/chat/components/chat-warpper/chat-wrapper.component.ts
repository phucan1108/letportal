import { Component, OnInit, OnDestroy } from '@angular/core';
import { ChatOnlineUser, ChatRoom, RoomType, DoubleChatRoom } from '../../../../../../core/models/chat.model';
import { ChatService } from 'services/chat.service';
import { NGXLogger } from 'ngx-logger';
import { Subject, Observable, Subscription } from 'rxjs';
import { ObjectUtils } from 'app/core/utils/object-util';
import { Store, Select, Actions, ofActionSuccessful, ofActionDispatched, ofActionCompleted } from '@ngxs/store';
import {  CHAT_STATE_TOKEN, ChatStateModel } from 'stores/chats/chats.state';
import { tap, filter } from 'rxjs/operators';
import { GotHubChatProblem, ActiveDoubleChatRoom } from 'stores/chats/chats.actions';

@Component({
    selector: 'let-chat-wrapper',
    templateUrl: './chat-wrapper.component.html',
    styleUrls: ['./chat-wrapper.component.scss']
})
export class ChatWrapperComponent implements OnInit, OnDestroy {
    isShowChatHead = true
    isShowChatBox = false

    chatRoom: ChatRoom
    @Select(CHAT_STATE_TOKEN)
    chatState$: Observable<ChatStateModel>
    sup: Subscription = new Subscription()
    constructor(
        private actions$: Actions,
        private chatService: ChatService,
        private store: Store,
        private logger: NGXLogger
    ) { }

    ngOnInit(): void {
        
        this.chatService.getAllAvailableUsers()
        this.sup.add(this.actions$.pipe(
            ofActionCompleted(GotHubChatProblem)
        ).subscribe(() => {
            // Display error about chat server's problem
        }))
        this.sup.add(
            this.actions$.pipe(
                ofActionSuccessful(ActiveDoubleChatRoom),
            ).subscribe(
                () => {
                    if(!this.isShowChatBox)
                        this.isShowChatBox = true
                }
            )
        )
        this.sup.add(this.chatState$.pipe(
            filter(state => ObjectUtils.isNotNull(state.currentUser)),
            tap(
                res => {
                    if(!this.isShowChatHead) this.isShowChatHead = true
                }
            )
        ).subscribe())
    }
    
    ngOnDestroy(): void {
        this.sup.unsubscribe()
    }
}
