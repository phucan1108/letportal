import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MessageType } from '../shortcut.models';

@Component({
	selector: 'let-custom-action-dialog',
	templateUrl: './custom-action-dialog.component.html'
})
export class ConfirmationDialogComponent implements OnInit {
	viewLoading = false;

	messType: MessageType = MessageType.Create;
	saveButtonText = 'Create';
	constructor(
		public dialogRef: MatDialogRef<ConfirmationDialogComponent>,
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
			case MessageType.Custom:
				this.saveButtonText = this.data.confirmText
				break
			default:
				this.saveButtonText = 'Save'
				break
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
