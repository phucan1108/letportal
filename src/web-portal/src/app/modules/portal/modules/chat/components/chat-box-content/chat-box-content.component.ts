import { Component, OnInit, Output, EventEmitter, Input, ViewChild, HostListener, ViewChildren, QueryList, ElementRef, AfterViewInit, AfterViewChecked } from '@angular/core';
import { DoubleChatRoom, ChatRoom, RoomType, ExtendedMessage, ChatOnlineUser } from '../../models/chat.model';
import { FormBuilder, FormGroup, Validators, FormControl, NgForm, FormGroupDirective } from '@angular/forms';
import { ChatService } from 'services/chat.service';
import { Observable, BehaviorSubject } from 'rxjs';
import { FormUtil } from 'app/core/utils/form-util';
import { ErrorStateMatcher } from '@angular/material';
import { DateUtils } from 'app/core/utils/date-util';
import { ObjectUtils } from 'app/core/utils/object-util';

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
export class ChatBoxContentComponent implements OnInit, AfterViewChecked {

    @Input()
    chatRoom: DoubleChatRoom

    @Output()
    closed: EventEmitter<any> = new EventEmitter()

    @ViewChild('form', { static: false }) form: NgForm;
    errorMatcher = new CustomErrorStateMatcher();
    roomName: string
    roomShortName: string
    hasAvatar: boolean

    currentUser: ChatOnlineUser

    formGroup: FormGroup
    messages$: BehaviorSubject<ExtendedMessage[]> = new BehaviorSubject([])

    constructor(
        private chatService: ChatService,
        private fb: FormBuilder
    ) { }

    @ViewChild("messageContainer", { static: true }) messageContainer: ElementRef;

    @HostListener('window:keydown.enter', ['$event'])
    handleEnterPress(event: KeyboardEvent) {
        this.send()
    }
    
    ngAfterViewChecked(): void {
        this.messageContainer.nativeElement.scrollTop = this.messageContainer.nativeElement.scrollHeight
    }

    ngOnInit(): void {
        this.roomName = this.chatRoom.invitee.fullName
        this.roomShortName = this.chatRoom.invitee.shortName
        this.hasAvatar = this.chatRoom.invitee.hasAvatar
        this.formGroup = this.fb.group({
            text: [null, Validators.required]
        })

        this.currentUser = this.chatService.currentUser
        // Check chat room contains any session
        // Collect all for displaying messages
        if (ObjectUtils.isNotNull(this.chatRoom.chatSessions)) {
            let messages: ExtendedMessage[] = []
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

            this.messages$.next(messages)
        }

        this.chatService.newMesage$.subscribe(newMessage => {
            if (newMessage.chatSessionId === this.chatRoom.currentSession.sessionId) {
                const messages = this.messages$.value
                messages.push(newMessage)
                this.messages$.next(messages)
            }
        })
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
                chatSessionId: this.chatRoom.currentSession.sessionId
            }
            this.chatService.sendMessage(
                this.chatRoom.currentSession.sessionId,
                this.chatRoom.invitee.userName,
                sentMessage,
                () => {
                    const messages = this.messages$.value
                    messages.push(sentMessage)
                    this.messages$.next(messages)
                    this.formGroup.controls.text.setValue(null)
                    this.form.resetForm()
                })
        }
    }
}
