import { Component, OnInit, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BehaviorSubject, Subject } from 'rxjs';
import { ChatOnlineUser } from '../../models/chat.model';
import { debounce, distinctUntilChanged, debounceTime, tap } from 'rxjs/operators';

@Component({
    selector: 'let-chat-search',
    templateUrl: './chat-search.component.html',
    styleUrls: ['./chat-search.component.scss']
})
export class ChatSearchComponent implements OnInit {
    @Output()
    closed: EventEmitter<any> = new EventEmitter<any>()
    
    allOnlineUsers: ChatOnlineUser[] = []
    onlineUsers$: BehaviorSubject<ChatOnlineUser[]> = new BehaviorSubject([])
    searchBoxForm: FormGroup
    isReadyRender: boolean = true
    constructor(
        private fb: FormBuilder,
        private cd: ChangeDetectorRef
    ) { }

    ngOnInit(): void { 
        this.searchBoxForm = this.fb.group({
            userFullName: ['', [ Validators.required, Validators.maxLength(250)]]
        });

        this.allOnlineUsers = [
            {
                fullName: 'Super Admin',
                avatar: '',
                userName: 'admin',
                hasAvatar: false,
                shortName: 'SA'
            },
            {
                fullName: 'Back Office',
                avatar: '',
                userName: 'backoffice',
                hasAvatar: true,
                shortName: 'BO'
            }
        ]
        this.onlineUsers$.next(this.allOnlineUsers)

        this.searchBoxForm.get('userFullName').valueChanges.pipe(
            debounceTime(500),
            distinctUntilChanged(),
            tap(
                newValue => {
                    this.isReadyRender = false
                    this.cd.markForCheck()
                    setTimeout(() =>{
                        const filters =  this.allOnlineUsers.filter(a => a.fullName.toLowerCase().indexOf(newValue.toLowerCase()) >= 0)
                        this.onlineUsers$.next(filters)
                        this.isReadyRender = true
                    },500)                    
                }
            )
        ).subscribe()
    }

    onClosed(){
        this.closed.emit(true)
    }
}
