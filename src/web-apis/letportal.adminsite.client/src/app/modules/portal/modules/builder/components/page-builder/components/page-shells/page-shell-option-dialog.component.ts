import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ShellOption } from 'services/portal.service';

@Component({
    selector: 'let-page-shell-option-dialog',
    templateUrl: './page-shell-option-dialog.component.html'
})
export class PageShellOptionDialogComponent implements OnInit {
    shellFormGroup: FormGroup;
    currentShellOption: ShellOption;
    constructor(
        public dialogRef: MatDialogRef<any>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef
    ) { }

    ngOnInit(): void {
        this.currentShellOption = this.data.shellOption

        this.shellFormGroup = this.fb.group({
            key: [this.currentShellOption.key, Validators.required],
            value: [this.currentShellOption.value, Validators.required]
        })
    }

    onSubmittingShellOption() {
        if (this.shellFormGroup.valid) {
            this.dialogRef.close(this.combiningShellOption())
        }
    }

    combiningShellOption(): ShellOption {
        const formValues = this.shellFormGroup.value
        return {
            key: formValues.key,
            value: formValues.value
        }
    }
}
