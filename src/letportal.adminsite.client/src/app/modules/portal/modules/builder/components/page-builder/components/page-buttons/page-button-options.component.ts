import { Component, OnInit, Inject, ChangeDetectorRef, ViewChild } from '@angular/core';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PageButtonGridComponent } from './page-button-grid.component';
import { FormBuilder } from '@angular/forms';
import { ButtonOptions } from 'services/portal.service';
import { NGXLogger } from 'ngx-logger';
import { ButtonOptionsComponent } from 'portal/shared/button-options/buttonoptions.component';

@Component({
    selector: 'let-page-button-options',
    templateUrl: './page-button-options.component.html'
})
export class PageButtonOptionsDialogComponent implements OnInit {

    buttonOptions: ButtonOptions

    @ViewChild('buttonOptionsComponent', { static: true })buttonOptionsComponent: ButtonOptionsComponent
    constructor(public dialogRef: MatDialogRef<PageButtonGridComponent>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private logger: NGXLogger,
        private cd: ChangeDetectorRef) { }

    ngOnInit(): void {
        this.buttonOptions = this.data.buttonOptions
        this.logger.debug('Editting button options', this.buttonOptions)
    }

    save(){
        if(this.buttonOptionsComponent.valid()){
            this.dialogRef.close(this.buttonOptionsComponent.get())
        }
    }
}
