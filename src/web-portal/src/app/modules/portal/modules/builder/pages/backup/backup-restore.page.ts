import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NGXLogger } from 'ngx-logger';
import { BackupsClient, Backup, PreviewRestoreModel } from 'services/portal.service';
import { DateUtils } from 'app/core/utils/date-util';

@Component({
    selector: 'let-backup-restore',
    templateUrl: './backup-restore.page.html',
    styleUrls: ['./backup-restore.page.scss']
})
export class BackupRestorePage implements OnInit {

    backup: Backup
    preview: PreviewRestoreModel
    isPreviewed = false
    btnOption = {
        active: false,
        text: 'Preview',
        buttonColor: 'primary',
        barColor: 'primary',
        raised: true,
        stroked: false,
        fab: false,
        mode: 'indeterminate',
        disabled: false,
    }

    constructor(
        private activatedRouter: ActivatedRoute,
        private router: Router,
        private logger: NGXLogger,
        private backupClient: BackupsClient
    ) { }

    ngOnInit(): void {
        this.backup = this.activatedRouter.snapshot.data.backup
    }

    getCreatedDate() {
        return DateUtils.toDateFormat(this.backup.createdDate, "MM/DD/YYYY HH:mm")
    }

    onPreview() {
        this.btnOption.active = true
        this.backupClient.previewBackup(this.backup.id).subscribe(
            res => {
                this.isPreviewed = true
                this.preview = res
                this.btnOption.active = false
            },
            err => {

            }
        )
    }
}
