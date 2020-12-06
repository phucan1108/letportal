import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ShellOption } from 'services/portal.service';
import { FormUtil } from 'app/core/utils/form-util';
import { ExtendedShellOption } from './extened.shell.model';
 
import { Guid } from 'guid-typescript';

@Component({
    selector: 'let-shelloption-dialog',
    templateUrl: './shelloption.dialog.component.html'
})
export class ShellOptionDialogComponent implements OnInit {

    shellOptForm: FormGroup

    availableShellOptions: ExtendedShellOption[] = []

    constructor(public dialogRef: MatDialogRef<any>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef) { }

    ngOnInit(): void {
        this.availableShellOptions = this.data.shellOptions
        this.shellOptForm = this.fb.group({
            key: ['', [Validators.required, Validators.maxLength(100), FormUtil.isExist(this.getKeys(), '')]],
            value: ['', [Validators.required, Validators.maxLength(250)]]
        })
    }

    save() {
        if (this.shellOptForm.valid) {
            const formValues = this.shellOptForm.value
            const combine: ExtendedShellOption = {
                key: formValues.key,
                value: formValues.value,
                id: Guid.create().toString(),
                allowDelete: true,
                description: ''
            }

            this.dialogRef.close(combine)
        }
        else{
            FormUtil.triggerFormValidators(this.shellOptForm)
        }
    }

    private getKeys(): string[]{
        const keys: string[] = []
        this.availableShellOptions?.forEach(opt => {
            keys.push(opt.key)
        })

        return keys
    }
}
