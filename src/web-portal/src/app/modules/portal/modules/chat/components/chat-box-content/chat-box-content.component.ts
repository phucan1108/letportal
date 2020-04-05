import { Component, OnInit, Output, EventEmitter, Input, ViewChild, HostListener, ViewChildren, QueryList, ElementRef, AfterViewInit, AfterViewChecked, OnDestroy } from '@angular/core';
import { DoubleChatRoom, ChatRoom, RoomType, ExtendedMessage, ChatOnlineUser, ChatSession } from '../../../../../../core/models/chat.model';
import { FormBuilder, FormGroup, Validators, FormControl, NgForm, FormGroupDirective } from '@angular/forms';
import { ChatService } from 'services/chat.service';
import { Observable, BehaviorSubject, Subscription } from 'rxjs';
import { FormUtil } from 'app/core/utils/form-util';
import { ErrorStateMatcher } from '@angular/material';
import { DateUtils } from 'app/core/utils/date-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import { EmojiEvent } from 'ngx-emoji-picker';
import { CdkTextareaAutosize } from '@angular/cdk/text-field';
import { debounceTime, distinctUntilChanged, tap, filter, map } from 'rxjs/operators';
import { Store, Select, Actions, ofActionSuccessful } from '@ngxs/store';
import { SentMessage, ReceivedMessage, LoadingMoreSession, AddedNewSession, ReceivedMessageFromAnotherDevice, ToggleOpenChatRoom } from 'stores/chats/chats.actions';
import { CHAT_STATE_TOKEN, ChatStateModel } from 'stores/chats/chats.state';
import { NGXLogger } from 'ngx-logger';
import StringUtils from 'app/core/utils/string-util';
import { EMOTION_SHORTCUTS } from '../../emotions/emotion.data';

export class CustomErrorStateMatcher implements ErrorStateMatcher {
    isErrorState(control: FormControl, form: NgForm | FormGroupDirective | null) {
        return control && control.invalid && control.touched;
    }
}
@Component({
    selector: 'let-chat-box-content',
    templateUrl: './chat-box-content.component.html',
    styleUrls: ['./chat-box-content.component.scss']
})
export class ChatBoxContentComponent implements OnInit, OnDestroy, AfterViewInit, AfterViewChecked {

    @Input()
    chatRoom: DoubleChatRoom

    @Output()
    closed: EventEmitter<any> = new EventEmitter()

    @ViewChild('form', { static: false }) form: NgForm;
    @ViewChild('autosize', { static: false }) autosize: CdkTextareaAutosize
    errorMatcher = new CustomErrorStateMatcher();
    roomName: string
    roomShortName: string
    hasAvatar: boolean
    currentUser: ChatOnlineUser
    displayShowMore = false
    formGroup: FormGroup
    messages$: Observable<ExtendedMessage[]>
    currentChatSession: ChatSession   

    sup: Subscription = new Subscription()

    heightChatContent = 280
    lastSentHashCode: string // Use this var for tracking who channel is sending last message
    connectionState = true
    constructor(
        private logger: NGXLogger,
        private actions$: Actions,
        private store: Store,
        private chatService: ChatService,
        private fb: FormBuilder
    ) { }
    ngAfterViewInit(): void {
        this.scrollToBottom()
    }
    ngOnDestroy(): void {
        this.sup.unsubscribe()
    }

    @ViewChild("messageContainer", { static: true }) messageContainer: ElementRef;
    @ViewChild("textInput", { static: false }) textInput: ElementRef
    @ViewChild("formFieldWarpper", { static: true }) formFieldWarpper: ElementRef
    @HostListener('window:keydown.enter', ['$event'])
    handleEnterPress(event: KeyboardEvent) {
        this.send()
    }
    lastHeight = 18

    onKeydown($event: KeyboardEvent) {
        $event.preventDefault()
    }
    @HostListener("window:scroll", ['$event'])
    scrollChat($event) {
        if (this.displayShowMore && this.messageContainer.nativeElement.scrollTop === 0) {
            this.store.dispatch(new LoadingMoreSession())
            // Keep scroll down a little bit for adding more message
            this.messageContainer.nativeElement.scrollTop = 20
        }
    }

    ngAfterViewChecked(): void {

    }

    ngOnInit(): void {
        this.roomName = this.chatRoom.invitee.fullName
        this.roomShortName = this.chatRoom.invitee.shortName
        this.hasAvatar = this.chatRoom.invitee.hasAvatar
        this.formGroup = this.fb.group({
            text: [null, Validators.required]
        })
        this.sup.add(this.chatService.connectionState$.pipe(
            tap(
                connectionState => {
                    this.logger.debug('Current connection state', connectionState)
                    this.connectionState = connectionState
                }
            )
        ).subscribe())
        this.currentUser = this.store.selectSnapshot(CHAT_STATE_TOKEN).currentUser
        this.currentChatSession = this.store.selectSnapshot(CHAT_STATE_TOKEN).activeChatSession
        // Check chat room contains any session
        // Collect all for displaying messages        
        let messages: ExtendedMessage[] = []
        if (ObjectUtils.isNotNull(this.chatRoom.chatSessions)) {
            this.chatRoom.chatSessions.forEach(s => {
                if (ObjectUtils.isNotNull(s.messages)) {
                    s.messages.forEach(m => {
                        messages.push({
                            ...m,
                            isReceived: m.userName !== this.currentUser.userName,
                            chatSessionId: s.sessionId
                        })
                    })
                }
            })
        }

        if (ObjectUtils.isNotNull(this.currentChatSession.messages)) {
            this.currentChatSession.messages.forEach(m => {
                messages.push({
                    ...m,
                    isReceived: m.userName !== this.currentUser.userName,
                    chatSessionId: this.currentChatSession.sessionId
                })
            })
        }

        // Check we can load more session by getting most previous session
        this.displayShowMore = ObjectUtils.isNotNull(this.currentChatSession.previousSessionId)

        this.messages$ = this.store.select(CHAT_STATE_TOKEN).pipe(
            filter(state => state.activeChatSession.sessionId === this.currentChatSession.sessionId),
            tap(
                state => {
                    this.displayShowMore = ObjectUtils.isNotNull(state.activeChatSession.previousSessionId)
                    this.currentChatSession = {
                        ...state.activeChatSession
                    }
                }
            ),
            map(state => state.activeChatSession.messages)
        )

        this.sup.add(
            this.actions$.pipe(
                ofActionSuccessful(AddedNewSession)
            ).subscribe(
                () => {
                    this.currentChatSession = {
                        ...this.store.selectSnapshot(CHAT_STATE_TOKEN).activeChatSession
                    }
                }
            )
        )

        this.sup.add(
            this.actions$.pipe(
                ofActionSuccessful(ReceivedMessage)
            ).subscribe(
                () => {
                    setTimeout(() => {
                        this.scrollToBottom()
                    }, 300) 
                }
            )
        )

        this.sup.add(
            this.actions$.pipe(
                ofActionSuccessful(SentMessage)
            ).subscribe(
                () => {
                    setTimeout(() => {
                        this.scrollToBottom()
                    }, 300) 
                }
            )
        )

        this.sup.add(
            this.actions$.pipe(
                ofActionSuccessful(ReceivedMessageFromAnotherDevice)
            ).subscribe(
                () => {
                    setTimeout(() => {
                        this.scrollToBottom()
                    }, 300) 
                }
            )
        )

        this.sup.add(this.formGroup.controls.text.valueChanges.pipe(
            debounceTime(100),
            distinctUntilChanged(),
            tap(newValue => {
                if (this.lastHeight < this.textInput.nativeElement.offsetHeight) {
                    this.heightChatContent -= 18
                }
                else if (this.lastHeight > this.textInput.nativeElement.offsetHeight) {
                    this.heightChatContent += 18
                }

                this.lastHeight = this.textInput.nativeElement.offsetHeight
            })
        ).subscribe())
    }

    onClosed() {        
        this.store.dispatch(new ToggleOpenChatRoom(false))
        this.closed.emit()
    }

    translateTimeText(message: ExtendedMessage) {
        return DateUtils.toDateFormat(message.createdDate, 'ddd hh:mm A')
    }

    send() {
        FormUtil.triggerFormValidators(this.formGroup)
        if (this.formGroup.valid) {
            const formValues = this.formGroup.value
            const sentMessage: ExtendedMessage = {
                message: this.translateEmotionShortcuts(formValues.text),
                formattedMessage: formValues.text,
                fileUrls: [],
                timeStamp: 0,
                userName: this.currentUser.userName,
                isReceived: false,
                createdDate: new Date(),
                chatSessionId: this.currentChatSession.sessionId
            }
            sentMessage.formattedMessage = sentMessage.message

            this.lastSentHashCode = StringUtils.b64EncodeUnicode(sentMessage.message + (new Date()).getUTCMilliseconds().toString())
            this.chatService.sendMessage(
                this.chatRoom.chatRoomId,
                this.currentChatSession.sessionId,
                this.chatRoom.invitee.userName,
                sentMessage,
                this.lastSentHashCode,
                () => {
                    this.store.dispatch(new SentMessage({
                        chatRoomId: this.chatRoom.chatRoomId,
                        chatSessionId: this.currentChatSession.sessionId,
                        message: sentMessage,
                        receiver: this.chatRoom.invitee.userName,
                        lastSentHashCode: this.lastSentHashCode
                    }))
                    this.formGroup.controls.text.setValue(null)
                    this.form.resetForm()

                    // Reset textarea
                    this.autosize.reset()
                    this.lastHeight = 18
                    this.heightChatContent = 280
                })
        }
    }

    emotionPicked($event: EmojiEvent) {
        this.formGroup.controls.text.setValue(
            (this.formGroup.controls.text.value ? this.formGroup.controls.text.value : '') + $event.char)
    }

    private scrollToBottom(){
        this.messageContainer.nativeElement.scrollTop = this.messageContainer.nativeElement.scrollHeight
    }

    private translateEmotionShortcuts(message: string){
        EMOTION_SHORTCUTS.forEach(key => {
            message = message.replace(key.key, key.unicode)
        })

        return message
    }
}
