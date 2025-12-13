import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder } from '@angular/forms';
import { FilledParameterModel, ExecuteParamModel } from 'services/portal.service';

@Component({
    selector: 'let-params-dialog',
    templateUrl: './params-dialog.component.html'
})
export class ParamsDialogComponent implements OnInit {

    params: ExecuteParamModel[]

    constructor(
        public dialogRef: MatDialogRef<any>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef,
    ) { }

    ngOnInit(): void {
        this.params = this.data.params
    }

    onPolulating(){
        this.dialogRef.close(this.params)
    }
}
