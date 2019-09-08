import { Component, OnInit, Inject, ChangeDetectorRef, ViewChild } from '@angular/core';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA, MatTable } from '@angular/material';
import { PageButtonGridComponent } from './page-button-grid.component';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PageButton } from 'services/portal.service';
import { StaticResources } from 'portal/resources/static-resources';
import { Observable } from 'rxjs';
import * as _ from 'lodash';
import { CommandOptionsComponent } from 'portal/shared/button-options/commandoptions.component';
import { startWith, map } from 'rxjs/operators';

@Component({
    selector: 'let-page-button-dialog',
    templateUrl: './page-button-dialog.component.html',
    styleUrls: ['./page-button-dialog.component.scss']
})
export class PageButtonDialogComponent implements OnInit {
    iconFilterOptions: Observable<string[]>;
    _icons = StaticResources.iconsList()

    actionCommandForm: FormGroup

    pageButton: PageButton

    _colors = StaticResources.colorButtons()
    isEditMode = false

    @ViewChild('actionOptions') actionOptions: CommandOptionsComponent

    constructor(
        public dialogRef: MatDialogRef<PageButtonGridComponent>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef) { }

    ngOnInit(
    ): void {
        this.pageButton = this.data.command
        this.isEditMode = this.pageButton.name ? true : false
        this.initialCommandForm()
    }

    initialCommandForm() {
        this.actionCommandForm = this.fb.group({
            name: [this.pageButton.name, Validators.required],
            icon: [this.pageButton.icon, Validators.required],
            color: [this.pageButton.color, Validators.required],
            allowHidden: [this.pageButton.allowHidden],
            isRequiredValidation: [this.pageButton.isRequiredValidation]
        })

        this.iconFilterOptions = this.actionCommandForm.get('icon').valueChanges.pipe(
            startWith(''),
            map(value => this._filterIcon(value))
        )
    }

    onSubmittingCommand() {
        if (this.actionCommandForm.valid) {
            this.dialogRef.close(this.combiningCommand())
        }
    }

    combiningCommand(): PageButton {
        let formValues = this.actionCommandForm.value
        return {
            id: this.pageButton.id,
            name: formValues.name,
            color: formValues.color,
            allowHidden: formValues.allowHidden,
            isRequiredValidation: formValues.isRequiredValidation,
            buttonOptions: this.pageButton.buttonOptions
        }
    }

    private _filterIcon(choosingIconValue: string): Array<string> {
        const filterValue = choosingIconValue.toLowerCase()

        return this._icons.filter(op => op.toLowerCase().includes(filterValue))
    }
}
