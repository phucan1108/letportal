import { Component, Inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { MAT_SNACK_BAR_DATA, MatSnackBarRef } from '@angular/material';
import { timeout, delay } from 'rxjs/operators';
import { of } from 'rxjs';


@Component({
	selector: 'm-action-natification',
	templateUrl: './action-notification.component.html',
	changeDetection: ChangeDetectionStrategy.Default

})
export class ActionNotificationComponent implements OnInit {
	constructor(@Inject(MAT_SNACK_BAR_DATA) public data: any) { }

	ngOnInit() {
		if (!this.data.showUndoButton || (this.data.undoButtonDuration >= this.data.duration)) {
			return;
		}

		this.delayForUndoButton(this.data.undoButtonDuration).subscribe(() => {
			this.data.showUndoButton = false;
		});
	}

	delayForUndoButton(timeToDelay) {
		return of('').pipe(delay(timeToDelay));
	}

	public onDismissWithAction() { this.data.snackBar.dismiss(); }

  	public onDismiss() { this.data.snackBar.dismiss(); }
}
