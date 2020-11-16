import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { CdkTextareaAutosize } from '@angular/cdk/text-field';
import { AfterViewChecked, AfterViewInit, Component, ElementRef, EventEmitter, HostListener, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, FormGroupDirective, NgForm, Validators } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';
import { MatDialog } from '@angular/material/dialog';
import { Actions, ofActionSuccessful, Store } from '@ngxs/store';
import { DateUtils } from 'app/core/utils/date-util';
import { FormUtil } from 'app/core/utils/form-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import StringUtils from 'app/core/utils/string-util';
import { EmojiEvent } from 'app/modules/thirdparties/emoji-picker/misc/emoji-event';
import { environment } from 'environments/environment';
import { NGXLogger } from 'ngx-logger';
import { Observable, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, map, tap } from 'rxjs/operators';
import { ChatService } from 'services/chat.service';
import { DownloadableResponseFile, UploadFileService } from 'services/uploadfile.service';
import { AddedNewSession, LoadingMoreSession, SentMessage, ToggleOpenChatRoom } from 'stores/chats/chats.actions';
import { CHAT_STATE_TOKEN } from 'stores/chats/chats.state';
import { ChatOnlineUser, ChatSession, DoubleChatRoom, ExtendedMessage } from '../../../../../../core/models/chat.model';
import { EMOTION_SHORTCUTS } from '../../emotions/emotion.data';
import { VideoCallDialogComponent } from '../video-call-dialog/video-call-dialog.component';
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

    @Input()
    isUserOnline: boolean

    @Output()
    closed: EventEmitter<any> = new EventEmitter()
    @ViewChild('fileInput', { static: true }) fileInput: ElementRef
    @ViewChild('form', { static: false }) form: NgForm;
    @ViewChild('autosize', { static: false }) autosize: CdkTextareaAutosize
    errorMatcher = new CustomErrorStateMatcher();
    roomName: string
    roomShortName: string
    hasAvatar: boolean
    roomAvatar: string
    currentUser: ChatOnlineUser
    displayShowMore = false
    formGroup: FormGroup
    messages$: Observable<ExtendedMessage[]>
    currentChatSession: ChatSession
    sup: Subscription = new Subscription()
    heightChatContent = 270
    lastSentHashCode: string // Use this var for tracking who channel is sending last message
    connectionState = true
    lastHeight = 25 // Used for adjusting textarea height when shift+enter to add line breaker
    progress // Used for tracking an uploading progress
    hasUploading = false
    hasUploadingError = false
    toggled = false

    inDesktop = true
    constructor(
        private logger: NGXLogger,
        private actions$: Actions,
        private store: Store,
        private chatService: ChatService,
        private uploadFileService: UploadFileService,
        public dialog: MatDialog,
        private fb: FormBuilder,
        private breakpointObserver: BreakpointObserver
    ) { 
        this.breakpointObserver.observe([
            Breakpoints.Handset,
            Breakpoints.Tablet
        ]).subscribe(result => {
            if (result.matches){
                this.logger.debug('User in tablet or handset')
                this.inDesktop = false
            }
            else{
                this.inDesktop = true
            }
        })
    }
    ngAfterViewInit(): void {
    }
    ngOnDestroy(): void {
        this.sup.unsubscribe()
    }

    @ViewChild("messageContainer", { static: true }) messageContainer: ElementRef;
    @ViewChild("textInput", { static: false }) textInput: ElementRef
    @ViewChild("formFieldWarpper", { static: true }) formFieldWarpper: ElementRef
    @HostListener('window:keydown.enter', ['$event'])
    handleEnterPress(event: KeyboardEvent) {
        if(this.inDesktop){            
            this.send()
        }
    }
    onKeydown($event: KeyboardEvent) {
        if(this.inDesktop){      
            $event.preventDefault()
        }      
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
        this.roomAvatar = this.chatRoom.invitee.avatar
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
            map(state => state.activeChatSession.messages),
            tap(
                (messages) => {
                    setTimeout(() => {
                        this.scrollToBottom()
                    },300)
                }
            )
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
        this.hasUploadingError = false
        this.hasUploading = false
        FormUtil.triggerFormValidators(this.formGroup)
        if (this.formGroup.valid) {
            const formValues = this.formGroup.value
            const sentMessage: ExtendedMessage = {
                message: this.translateEmotionShortcuts(formValues.text),
                formattedMessage: formValues.text,
                attachmentFiles: [],
                timeStamp: 0,
                userName: this.currentUser.userName,
                isReceived: false,
                createdDate: new Date(),
                chatSessionId: this.currentChatSession.sessionId,
                hasAttachmentFile: false,
                renderTime: true
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
                    this.resetText()
                })
        }
    }

    isImageFile(fileType: string) {
        return ['jpg', 'jpeg', 'png', 'gif'].indexOf(fileType.toLowerCase()) >= 0
    }

    emotionPicked($event: EmojiEvent) {
        this.formGroup.controls.text.setValue(
            (this.formGroup.controls.text.value ? this.formGroup.controls.text.value : '') + $event.char)
    }

    popupVideoCall(){        
        let dialogRef = this.dialog.open(VideoCallDialogComponent, {
            disableClose: true,
            data: {
                invitee: this.chatRoom.invitee
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
            }
        })
    }

    openDownloadFileTab(downloadableUrl: string) {
        window.open(downloadableUrl, '_blank')
    }
    
    onFileChange($event) {
        const file: File = $event.target.files[0]
        let isValidFileTypes = !this.isInvalidExtension(file.name)
        let isValidFileSize = this.getFileSizeInMb(file.size) <= environment.chatOptions.maxFileSizeInMb

        if (!isValidFileSize) {
            this.formGroup.controls.text.setErrors({
                'maximum-file-size': true
            })
            this.formGroup.controls.text.markAsTouched()
        }
        else if (!isValidFileTypes) {
            this.formGroup.controls.text.setErrors({
                'invalid-file-type': true
            })
            this.formGroup.controls.text.markAsTouched()
        }
        else {
            this.hasUploading = true
            this.upload(file,
                (res) => {
                    this.hasUploading = false
                    const sentMessage: ExtendedMessage = {
                        chatSessionId: this.currentChatSession.sessionId,
                        attachmentFiles: [
                            {
                                downloadUrl: res.response.downloadableUrl,
                                fileType: res.fileName.split('.')[1],
                                fileName: res.fileName
                            }
                        ],
                        message: null,
                        formattedMessage: null,
                        timeStamp: 0,
                        userName: this.currentUser.userName,
                        isReceived: false,
                        hasAttachmentFile: true,
                        createdDate: new Date(),
                        renderTime: true
                    }
                    this.lastSentHashCode = StringUtils.b64EncodeUnicode(res.fileName + (new Date()).getUTCMilliseconds().toString())
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
                            this.resetText()
                        })
                },
                (err) => {
                    this.hasUploadingError = true
                    this.hasUploading = false
                },
                () => {
                    this.hasUploading = false
                })
        }

    }

    private upload(file: File,
        onSent?: (res: DownloadableResponseFile) => void, onError?: (err: any) => void, onComplete?: () => void) {
        const uploadSub = this.uploadFileService.uploadOne(file).pipe(
            map(res => <DownloadableResponseFile>{
                fileName: file.name,
                response: res
            }),
            tap(
                (res: DownloadableResponseFile) => {
                    if (onSent)
                        onSent(res)
                    uploadSub.unsubscribe()
                },
                err => {
                    this.logger.debug('Hit error on tap')
                    if (onError)
                        onError(err)
                    uploadSub.unsubscribe()
                },
                () => {
                    this.logger.debug('Hit complete')
                    if (onComplete)
                        onComplete()
                    uploadSub.unsubscribe()
                }
            )
        ).subscribe()
    }

    private getFileSizeInMb(size: number) {
        return Math.round(size / (1024 * 1024))
    }

    private isInvalidExtension(fileName: string) {
        const extSplitted = fileName.split('.')
        const fileExt = extSplitted[extSplitted.length - 1].toLowerCase()
        const splitted = environment.chatOptions.allowFileTypes.split(';')
        return splitted.indexOf(fileExt) < 0
    }

    private resetText() {

        this.formGroup.controls.text.setValue(null)
        this.form.resetForm()
        // Reset textarea
        this.autosize.reset()
        this.lastHeight = 18
        this.heightChatContent = 280
    }

    private scrollToBottom() {
        this.messageContainer.nativeElement.scrollTop = this.messageContainer.nativeElement.scrollHeight
    }

    private translateEmotionShortcuts(message: string) {
        EMOTION_SHORTCUTS?.forEach(key => {
            message = message.replace(key.key, key.unicode)
        })

        return message
    }
}
