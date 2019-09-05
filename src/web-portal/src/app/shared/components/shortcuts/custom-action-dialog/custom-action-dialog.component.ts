import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MessageType } from '../shortcut.models';

@Component({
	selector: 'm-custom-action-entity-dialog',
	templateUrl: './custom-action-dialog.component.html'
})
export class CustomActionEntityDialogComponent implements OnInit {
	viewLoading: boolean = false;

	messType: MessageType = MessageType.Create;
	saveButtonText = 'Create';
	constructor(
		public dialogRef: MatDialogRef<CustomActionEntityDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any
	) { }

	ngOnInit() {
		this.messType = this.data.messType;
		switch (this.messType) {
			case MessageType.Create:
				this.saveButtonText = 'Create'
				break;
			case MessageType.Update:
				this.saveButtonText = 'Update'
				break
			case MessageType.Delete:
				this.saveButtonText = 'Delete'
				break
			default:
				this.saveButtonText = 'Save'			
		}
	}

	onNoClick(): void {
		this.dialogRef.close();
	}

	onYesClick(): void {
		/* Server loading imitation. Remove this */
		this.viewLoading = true;
		setTimeout(() => {
			this.dialogRef.close(true); // Keep only this row
		}, 1000);
	}
}
