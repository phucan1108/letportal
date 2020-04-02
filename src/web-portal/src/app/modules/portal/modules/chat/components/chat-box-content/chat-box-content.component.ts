import { Component, OnInit, Output, EventEmitter, Input, ViewChild, HostListener, ViewChildren, QueryList, ElementRef, AfterViewInit, AfterViewChecked, OnDestroy } from '@angular/core';
import { DoubleChatRoom, ChatRoom, RoomType, ExtendedMessage, ChatOnlineUser, ChatSession } from '../../models/chat.model';
import { FormBuilder, FormGroup, Validators, FormControl, NgForm, FormGroupDirective } from '@angular/forms';
import { ChatService } from 'services/chat.service';
import { Observable, BehaviorSubject, Subscription } from 'rxjs';
import { FormUtil } from 'app/core/utils/form-util';
import { ErrorStateMatcher } from '@angular/material';
import { DateUtils } from 'app/core/utils/date-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import { EmojiEvent } from 'ngx-emoji-picker';
import { CdkTextareaAutosize } from '@angular/cdk/text-field';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';

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
    messages$: BehaviorSubject<ExtendedMessage[]> = new BehaviorSubject([])
    currentChatSession: ChatSession
    lastChatSession: ChatSession
    sup: Subscription = new Subscription()

    heightChatContent = 280

    constructor(
        private chatService: ChatService,
        private fb: FormBuilder
    ) { }
    ngAfterViewInit(): void {
        // Make sure scroll bottom when init
        this.messageContainer.nativeElement.scrollTop = this.messageContainer.nativeElement.scrollHeight
    }
    ngOnDestroy(): void {
        this.sup.unsubscribe()
    }

    @ViewChild("messageContainer", { static: true }) messageContainer: ElementRef;
    @ViewChild("textInput", { static: false }) textInput: ElementRef
    @ViewChild("formFieldWarpper", {static: true}) formFieldWarpper: ElementRef
    @HostListener('window:keydown.enter', ['$event'])
    handleEnterPress(event: KeyboardEvent) {
        this.send()
    }
    lastHeight = 18

    onKeydown($event: KeyboardEvent){
        $event.preventDefault()
    }
    @HostListener("window:scroll", ['$event'])
    scrollChat($event) {
        if (this.displayShowMore && this.messageContainer.nativeElement.scrollTop === 0) {
            this.chatService.loadMore(this.lastChatSession.previousSessionId)
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

        this.currentChatSession = this.chatRoom.currentSession
        this.lastChatSession = this.currentChatSession
        this.currentUser = this.chatService.currentUser
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
        this.messages$.next(messages)

        // Check we can load more session by getting most previous session
        if (this.chatRoom.chatSessions.length > 0) {
            const mostPreSession = this.chatRoom.chatSessions[this.chatRoom.chatSessions.length - 1]
            this.displayShowMore = ObjectUtils.isNotNull(mostPreSession.previousSessionId)
            this.lastChatSession = mostPreSession
        }

        this.sup.add(this.chatService.newMesage$.subscribe(newMessage => {
            if (newMessage.chatSessionId === this.currentChatSession.sessionId) {
                const messages = this.messages$.value
                messages.push(newMessage)
                this.messages$.next(messages)
            }
        }))

        this.sup.add(this.chatService.addNewSession$.subscribe(newSession => {
            if (newSession.previousSessionId === this.currentChatSession.sessionId) {
                this.currentChatSession = newSession
            }
        }))

        this.sup.add(this.chatService.loadPreviousSession$.subscribe(preSession => {
            if (ObjectUtils.isNotNull(preSession) && ObjectUtils.isNotNull(preSession.messages)) {
                let messages = this.messages$.value
                preSession.messages.reverse().forEach(m => {
                    // Push on first queue
                    messages.unshift({
                        ...m,
                        isReceived: m.userName !== this.currentUser.userName,
                        chatSessionId: preSession.sessionId
                    })
                })

                this.messages$.next(messages)
                this.lastChatSession = preSession
                this.displayShowMore = ObjectUtils.isNotNull(preSession.previousSessionId)
            }
        }))

        this.formGroup.controls.text.valueChanges.pipe(
            debounceTime(100),
            distinctUntilChanged(),
            tap(newValue => {
                if(this.lastHeight < this.textInput.nativeElement.offsetHeight){
                    this.heightChatContent -= 18
                }
                else if(this.lastHeight > this.textInput.nativeElement.offsetHeight){
                    this.heightChatContent += 18
                }

                this.lastHeight = this.textInput.nativeElement.offsetHeight
            })
        ).subscribe()
    }

    onClosed() {
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
                message: formValues.text,
                formattedMessage: formValues.text,
                fileUrls: [],
                timeStamp: 0,
                userName: this.chatService.currentUser.userName,
                isReceived: false,
                createdDate: new Date(),
                chatSessionId: this.currentChatSession.sessionId
            }
            this.chatService.sendMessage(
                this.currentChatSession.sessionId,
                this.chatRoom.invitee.userName,
                sentMessage,
                () => {

                    const messages = this.messages$.value
                    messages.push(sentMessage)
                    this.messages$.next(messages)
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
}
