import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AvailableUrl, AppsClient } from 'services/portal.service';
 
import { ExtendedMenu } from 'portal/modules/models/menu.model';

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
    currentLevel = 0

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
        this.currentLevel = this.data.level

        this.menuFormGroup = this.fb.group({
            displayName: [this.menu.displayName, Validators.required],
            hide: [this.menu.hide],
            url: [this.menu.url],
            availableUrl: [''],
            icon: [this.menu.icon, Validators.required]
        })

        this.menuFormGroup.get('availableUrl').valueChanges.subscribe(newValue => {
            if (newValue) {
                this.menuFormGroup.get('url').setValue(newValue)
                this.menuFormGroup.get('displayName').setValue(this.availableUrls.find(url => url.url === newValue).name)
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
        const formValues = this.menuFormGroup.value
        return {
            id: this.menu.id,
            displayName: formValues.displayName,
            icon: formValues.icon,
            menuPath: this.menu.menuPath,
            parentId: this.menu.parentId,
            url: formValues.url,
            subMenus: this.menu.subMenus,
            order: this.menu.order,
            hide: formValues.hide,
            level: this.menu.level
        }
    }
}
