import { Component, OnInit, Inject, ChangeDetectorRef, ViewChild, Input } from '@angular/core';
import { PageButtonGridComponent } from './page-button-grid.component';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PageButton } from 'services/portal.service';
import { StaticResources } from 'portal/resources/static-resources';
import { Observable, BehaviorSubject } from 'rxjs';
 
import { CommandOptionsComponent } from 'portal/shared/button-options/commandoptions.component';
import { startWith, map } from 'rxjs/operators';
import { ExtendedPageSection } from 'app/core/models/extended.models';
import { NGXLogger } from 'ngx-logger';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';

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
    sections: any[] = []
    @ViewChild('actionOptions', { static: false }) actionOptions: CommandOptionsComponent
    constructor(
        public dialogRef: MatDialogRef<PageButtonGridComponent>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private logger: NGXLogger,
        private cd: ChangeDetectorRef) { }

    ngOnInit(
    ): void {
        this.pageButton = this.data.command
        this.sections = this.data.sections
        this.logger.debug('Passing sections', this.sections)
        this.isEditMode = this.pageButton.name ? true : false
        this.initialCommandForm()
    }

    initialCommandForm() {
        this.actionCommandForm = this.fb.group({
            name: [this.pageButton.name, Validators.required],
            icon: [this.pageButton.icon],
            color: [this.pageButton.color, Validators.required],
            allowHidden: [this.pageButton.allowHidden],
            isRequiredValidation: [this.pageButton.isRequiredValidation],
            placeSectionId: [this.pageButton.placeSectionId]
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
        const formValues = this.actionCommandForm.value
        return {
            id: this.pageButton.id,
            name: formValues.name,
            color: formValues.color,
            allowHidden: formValues.allowHidden,
            icon: formValues.icon,
            isRequiredValidation: formValues.isRequiredValidation,
            buttonOptions: this.pageButton.buttonOptions,
            placeSectionId: formValues.placeSectionId
        }
    }

    private _filterIcon(choosingIconValue: string): Array<string> {
        const filterValue = choosingIconValue.toLowerCase()

        return this._icons.filter(op => op.toLowerCase().includes(filterValue))
    }
}
