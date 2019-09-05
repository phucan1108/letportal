import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material';
import { Menu, AvailableUrl, AppsClient } from 'services/portal.service';
import { ExtendedMenu } from '../models/extended.model';
import { Observable } from 'rxjs';
import * as _ from 'lodash';

@Component({
    selector: 'let-menu-dialog',
    templateUrl: './menu-dialog.component.html'
})
export class MenuDialogComponent implements OnInit {

    menuFormGroup: FormGroup

    menu: ExtendedMenu
    isEditMode = false
    appId = ''
    availableUrls: Array<AvailableUrl>;

    constructor(
        public dialogRef: MatDialogRef<any>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef,
        private appClient: AppsClient
    ) {
        this.menu = this.data.menu
        this.isEditMode = this.data.isEditMode
        this.appId = this.data.appId

        this.menuFormGroup = this.fb.group({
            displayName: [this.menu.displayName, Validators.required],
            hide: [this.menu.hide],
            url: [this.menu.url, Validators.required],
            availableUrl: ['']
        })

        this.menuFormGroup.get('availableUrl').valueChanges.subscribe(newValue => {
            if (newValue) {
                this.menuFormGroup.get('url').setValue(newValue)
                this.menuFormGroup.get('displayName').setValue(_.find(this.availableUrls, url => url.url === newValue).name)
            }
        })

        this.appClient.getAvailableUrls(this.appId).subscribe(result => {
            this.availableUrls = result
        })
    }

    ngOnInit(): void { }

    onSubmit() {
        if (!this.menuFormGroup.invalid) {
            this.dialogRef.close(this.combiningMenu())
        }
    }

    private combiningMenu(): ExtendedMenu {
        let formValues = this.menuFormGroup.value
        return {
            id: this.menu.id,
            displayName: formValues.displayName,
            icon: this.menu.icon,
            menuPath: this.menu.menuPath,
            parentId: this.menu.parentId,
            url: formValues.url,
            subMenus: this.menu.subMenus,
            order: this.menu.order,
            hide: formValues.hide
        }
    }
}
