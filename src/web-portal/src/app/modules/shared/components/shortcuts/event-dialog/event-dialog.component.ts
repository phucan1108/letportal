import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MessageType, EventDialogType } from '../shortcut.models';

@Component({
	selector: 'let-event-dialog',
    templateUrl: './event-dialog.component.html',
    styleUrls: ['./event-dialog.component.scss']
})
export class EventDialogComponent implements OnInit {
	viewLoading = false;

    eventType: EventDialogType = EventDialogType.Info;
    eventHeader = ''
    eventContent = ''
    icon = 'info'
    class = ''
	constructor(
		public dialogRef: MatDialogRef<EventDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any
	) { }

	ngOnInit() {
        this.eventType = this.data.eventType
        this.eventContent = this.data.eventContent
        this.eventHeader = this.data.eventHeader

        switch(this.eventType){
            case EventDialogType.Info:
                this.icon = 'info'
                break
            case EventDialogType.Error:
                this.icon = 'error'
                this.class = 'error'
                break
            case EventDialogType.Success:
                this.icon = 'stars'
                this.class = 'success'
                break
            case EventDialogType.Warning:
                this.icon = 'warning'
                this.class = 'warn'
                break
        }
    }
    
}
