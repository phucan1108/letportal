import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { BackupBuilderPage } from '../../pages/backup/backup-builder.page';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NGXLogger } from 'ngx-logger';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { BackupsClient } from 'services/portal.service';
import { tap } from 'rxjs/operators';

@Component({
    selector: 'let-generate-code-dialog',
    templateUrl: './generatecode.dialog.html'
})
export class GenerateCodeDialog implements OnInit {
    generateCodeForm: FormGroup

    constructor(
        public dialogRef: MatDialogRef<BackupBuilderPage>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef,
        private translate: TranslateService,
        private backupClients: BackupsClient,
        private logger: NGXLogger
    ) { }

    ngOnInit(): void { 
        this.generateCodeForm = this.fb.group({
            fileName: ['', Validators.required],
            versionNumber: ['', Validators.required]   
        })
    }

    onSubmit(){
        if(this.generateCodeForm.valid){
            this.dialogRef.close({
                fileName: this.generateCodeForm.value.fileName,
                versionNumber: this.generateCodeForm.value.versionNumber
            })
        }
    }
}
