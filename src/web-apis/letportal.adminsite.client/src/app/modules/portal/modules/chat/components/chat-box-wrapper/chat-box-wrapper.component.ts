import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { ChatRoom, ChatOnlineUser } from 'app/core/models/chat.model';
import { Store, Actions, ofActionCompleted, Select } from '@ngxs/store';
import { ChatStateModel, CHAT_STATE_TOKEN } from 'stores/chats/chats.state';
import { Subscription, Observable } from 'rxjs';
import { filter, map, tap } from 'rxjs/operators';
import { NGXLogger } from 'ngx-logger';

@Component({
    selector: 'let-chat-box-wrapper',
    templateUrl: './chat-box-wrapper.component.html'
})
export class ChatBoxWrapperComponent implements OnInit, OnDestroy {
    chatRooms: ChatRoom[] = []
    currentUser: ChatOnlineUser

    @Select(CHAT_STATE_TOKEN)
    chatState$: Observable<ChatStateModel>

    sup: Subscription = new Subscription()
    constructor(
        private logger: NGXLogger,
        private actions$: Actions,
        private store: Store
    ) { }
   
    ngOnInit(): void {
        this.currentUser = this.store.selectSnapshot(CHAT_STATE_TOKEN).currentUser

        this.sup.add(
            this.chatState$.pipe(
                filter(state => state.chatRooms.length > 0 && state.chatRooms.length !== this.chatRooms.length),
                map(state => state.chatRooms),
                tap(
                    chatRooms => {
                        this.chatRooms = chatRooms
                        this.logger.debug('Current available chat rooms', this.chatRooms)
                    }
                )
            ).subscribe()
        )
    }

    ngOnDestroy(): void {
        this.sup.unsubscribe()
    }

    trachById(index: number, element: ChatRoom): string{
        return element.chatRoomId
    }
}
