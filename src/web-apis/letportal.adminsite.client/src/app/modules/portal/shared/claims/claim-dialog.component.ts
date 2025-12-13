import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClaimTableComponent } from './claim-table.component';
import { PortalClaim, ClaimValueType } from 'services/portal.service';
import { StaticResources } from 'portal/resources/static-resources';

@Component({
    selector: 'let-claim-dialog',
    templateUrl: './claim-dialog.component.html'
})
export class ClaimDialogComponent implements OnInit {

    claimFormGroup: FormGroup
    claim: PortalClaim
    _claimValueTypes = StaticResources.claimValueTypes()

    constructor(
        public dialogRef: MatDialogRef<ClaimTableComponent>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef
    ) { }

    ngOnInit(): void {
        this.claim = this.data.claim
        this.claimFormGroup = this.fb.group({
            name: [this.claim.name, Validators.required],
            displayName: [this.claim.displayName, Validators.required],
            valueType: [ClaimValueType.Boolean, Validators.required]
        })
    }

    onSubmit() {
        if(!this.claimFormGroup.invalid)
            this.dialogRef.close(this.combiningClaim())
    }

    combiningClaim(): PortalClaim {
        const formValues = this.claimFormGroup.value
        return {
            name: formValues.name,
            displayName: formValues.displayName,
            claimValueType: formValues.valueType
        }
    }
}
