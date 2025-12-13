import { Component, OnInit, Output, EventEmitter, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BehaviorSubject, Subject, Subscription, Observable, of } from 'rxjs';
import { ChatOnlineUser } from '../../../../../../core/models/chat.model';
import { debounce, distinctUntilChanged, debounceTime, tap, map, filter } from 'rxjs/operators';
import { ChatService } from 'services/chat.service';
import { NGXLogger } from 'ngx-logger';
import { Store, Actions } from '@ngxs/store';
import { ClickedOnChatUser } from 'stores/chats/chats.actions';
import { CHAT_STATE_TOKEN, ChatStateModel } from 'stores/chats/chats.state';
import { ObjectUtils } from 'app/core/utils/object-util';

@Component({
    selector: 'let-chat-search',
    templateUrl: './chat-search.component.html',
    styleUrls: ['./chat-search.component.scss']
})
export class ChatSearchComponent implements OnInit, OnDestroy {
    @Output()
    closed: EventEmitter<any> = new EventEmitter()

    onlineUsers$: BehaviorSubject<ChatOnlineUser[]> = new BehaviorSubject([])
    searchBoxForm: FormGroup
    isReadyRender: boolean = true
    sup: Subscription = new Subscription()
    connectionState = true
    filterWord: string
    constructor(
        private store: Store,
        private actions$: Actions,
        private logger: NGXLogger,
        private chatService: ChatService,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef
    ) { }
    
    ngOnInit(): void { 
        this.searchBoxForm = this.fb.group({
            userFullName: ['', [ Validators.required, Validators.maxLength(250)]]
        });
        this.sup.add(this.chatService.connectionState$.pipe(
            tap(
                connectionState => {
                    this.logger.debug('Current connection state', connectionState)
                    this.connectionState = connectionState
                }
            )
        ).subscribe())
        this.sup.add(this.store.select(CHAT_STATE_TOKEN).pipe(
            filter(state => state.availableUsers && state.availableUsers.length > 0),
            map((state: ChatStateModel) => ObjectUtils.clone(state.availableUsers).sort((u1: ChatOnlineUser,u2: ChatOnlineUser)=> u2.incomingMessages - u1.incomingMessages)),
            tap(
                (users: ChatOnlineUser[]) => {
                    this.onlineUsers$.next(users)
                }
            )
        ).subscribe())

        this.sup.add(this.searchBoxForm.get('userFullName').valueChanges.pipe(
            debounceTime(500),
            distinctUntilChanged(),
            tap(
                newValue => {
                    this.isReadyRender = false
                    this.cd.markForCheck()
                    setTimeout(() =>{
                        const filters =  this.onlineUsers$.value.filter(a => a.fullName.toLowerCase().indexOf(newValue.toLowerCase()) >= 0)
                        this.onlineUsers$.next(filters)
                        this.isReadyRender = true
                    },500)                    
                }
            )
        ).subscribe())
    }

    ngOnDestroy(): void {
       this.sup.unsubscribe()
    }

    onClosed(){
        this.closed.emit(true)
    }

    selectedUser(user: ChatOnlineUser){
        this.store.dispatch(new ClickedOnChatUser({
            inviee: user
        }))
    }
}
