import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PageDatasourceGridComponent } from './page-datasource.grid.component';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PageDatasource } from 'services/portal.service';

@Component({
    selector: 'let-page-datasource-dialog',
    templateUrl: './page-datasource.dialog.component.html',
    styleUrls: ['./page-datasource.dialog.component.scss']
})
export class PageDatasourceDialogComponent implements OnInit {

    currentDatasource: PageDatasource
    datasourceForm: FormGroup
    isEditMode = false

    constructor(
        public dialogRef: MatDialogRef<PageDatasourceGridComponent>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef,
    ) {
        this.currentDatasource = this.data.datasource
        this.isEditMode = !!this.currentDatasource.name
    }

    initDatasourceForm() {
        this.datasourceForm = this.fb.group({
            name: [this.currentDatasource.name, Validators.required],
            triggerCondition: [this.currentDatasource.triggerCondition, Validators.required],
            isActive: [this.currentDatasource.isActive]
        })
    }

    ngOnInit(): void { }

    onSubmit() {
        if(this.datasourceForm.valid){

        }
    }
}
