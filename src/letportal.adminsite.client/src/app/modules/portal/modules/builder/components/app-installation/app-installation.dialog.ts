import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { AppInstallationPage } from '../../pages/app-installation/app-installation.page';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NGXLogger } from 'ngx-logger';
import { InstallWay, UnpackResponseModel, AppsClient } from 'services/portal.service';
import { tap } from 'rxjs/operators';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';

@Component({
    selector: 'let-app-install-dialog',
    templateUrl: './app-installation.dialog.html',
    styleUrls: ['./app-installation.dialog.scss']
})
export class AppInstallationDialog implements OnInit {
    installWayFormGroup: FormGroup

    unpackModel: UnpackResponseModel
    constructor(
        public dialogRef: MatDialogRef<AppInstallationPage>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef,
        private translate: TranslateService,
        private appsClient: AppsClient,
        private shortcut: ShortcutUtil,
        private logger: NGXLogger
    ) { }

    ngOnInit(): void { 
        this.unpackModel = this.data.unpack
        this.installWayFormGroup = this.fb.group({
            installWay: [InstallWay.Wipe]
        })
    }

    onSubmit(){
        this.appsClient.install({
            installWay: this.installWayFormGroup.controls.installWay.value,
            uploadFileId: this.unpackModel.uploadFileId
        }).pipe(
            tap(
                res => {
                    this.dialogRef.close(true)
                },
                err => {
                    this.dialogRef.close()
                    this.shortcut.toastMessage(this.translate.instant('common.somethingWentWrong'), ToastType.Error) 
                }
            )
        ).subscribe()
    }
}
