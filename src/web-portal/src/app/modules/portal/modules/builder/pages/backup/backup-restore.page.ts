import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NGXLogger } from 'ngx-logger';
import { BackupsClient, Backup, PreviewRestoreModel } from 'services/portal.service';
import { DateUtils } from 'app/core/utils/date-util';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { MessageType, ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { SecurityService } from 'app/core/security/security.service';
import { PageService } from 'services/page.service';
import { TranslateService } from '@ngx-translate/core';

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
    title = 'Proceed Restore'
    description = 'Are you sure to proceed this backup point?'
    waiting = 'Loading...'
    constructor(
        private activatedRouter: ActivatedRoute,
        private router: Router,
        private pageService: PageService,
        private logger: NGXLogger,
        private backupClient: BackupsClient,
        private shortcutUtil: ShortcutUtil,
        private security: SecurityService,
        private translate: TranslateService
    ) { }

    ngOnInit(): void {
        this.pageService.init('backup-restore').subscribe()
        this.backup = this.activatedRouter.snapshot.data.backup
    }

    getCreatedDate() {
        return DateUtils.toDateFormat(this.backup.createdDate, 'MM/DD/YYYY HH:mm')
    }

    onPreview() {
        this.btnOption.active = true
        this.backupClient.previewBackup(this.backup.id).subscribe(
            res => {
                this.isPreviewed = true
                this.preview = res
                this.btnOption.active = false
                this.btnOption.disabled = true
            },
            err => {

            }
        )
    }

    onRestore(){
        const dialog = this.shortcutUtil.confirmationDialog(
            this.title, this.preview.totalChangedObjects === 0 ? 'No changed object, are you sure to restore this backup?' : this.description, this.waiting, MessageType.Custom, 'Restore')
        dialog.afterClosed().subscribe(
            res => {
                if(!res){
                    return
                }
                this.backupClient.restoreBackup(this.backup.id, {
                    id: this.backup.id,
                    requestor: this.security.getAuthUser().username
                }).subscribe(
                    res => {
                        this.shortcutUtil.toastMessage(this.translate.instant('common.restoreSuccessfully'), ToastType.Success)
                    }
                )
            }
        )
    }

    onCancel(){
        this.router.navigateByUrl('portal/page/backup-management')
    }
}
