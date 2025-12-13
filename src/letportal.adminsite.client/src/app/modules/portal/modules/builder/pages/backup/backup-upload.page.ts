import { Component, OnInit, ChangeDetectorRef, ElementRef, ViewChild, ChangeDetectionStrategy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NGXLogger } from 'ngx-logger';
import { BackupsClient, UploadBackupResponseModel } from 'services/portal.service';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { Router } from '@angular/router';
import { PageService } from 'services/page.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'backup-upload',
    templateUrl: './backup-upload.page.html',
    styleUrls: ['./backup-upload.page.scss']
})
export class BackupUploadpage implements OnInit {
    @ViewChild('fileInput', { static: true }) fileInput: ElementRef
    form: FormGroup

    isMaximumSize = false
    isInvalidFileExtension = false

    selectedFile: File
    hasSelectedFile = false
    disabled = false

    progress;
    uploaded = false
    uploadFormName = 'upload'

    btnOption = {
        active: false,
        text: 'Upload',
        buttonColor: 'primary',
        barColor: 'primary',
        raised: true,
        stroked: false,
        fab: false,
        mode: 'indeterminate',
        disabled: true,
    }

    responseModel: UploadBackupResponseModel
    constructor(
        private pageService: PageService,
        private backupClient: BackupsClient,
        private shortcutUtil: ShortcutUtil,
        private translate: TranslateService,
        private logger: NGXLogger,
        private router: Router,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef
    ) { }

    ngOnInit(): void {
        this.pageService.init('backup-upload').subscribe()
        this.form = this.fb.group({
            upload: ['test', [Validators.required]]
        })
    }

    onFileChange($event) {
        if (this.disabled) {
            return
        }
        this.hasSelectedFile = true
        const latestFile: File = $event.target.files[$event.target.files.length - 1]
        this.logger.debug('Check file size', latestFile)
        this.isMaximumSize = this.isReachMaximumSize(latestFile.size)
        this.isInvalidFileExtension = this.isInvalidExtension(latestFile.name)
        this.logger.debug('Maximum size', this.isMaximumSize)
        this.logger.debug('Invalid file ext', this.isInvalidFileExtension)
        if (!this.isMaximumSize && !this.isInvalidFileExtension) {
            this.selectedFile = $event.target.files[0]
        }
        else {
            if (this.isMaximumSize) {
                this.form.get(this.uploadFormName).setErrors({ maximumsize: true })
            }
            else if (this.isInvalidFileExtension) {
                this.form.get(this.uploadFormName).setErrors({ fileextensions: true })
            }
        }
        this.form.get(this.uploadFormName).markAsTouched()
        this.fileInput.nativeElement.value = ''

        if(this.hasSelectedFile && this.form.valid){
            this.btnOption.disabled = false
        }
    }

    getFileSizeInMb(size: number) {
        return Math.round(size / (1024 * 1024))
    }

    getFileSizeInKb(size: number) {
        return Math.round(size / 1024)
    }
    isReachMaximumSize(fileSize: number) {
        const maxSizeValidator = {
            isActive: true,
            validatorOption: '16'
        }
        if (maxSizeValidator.isActive) {
            return parseInt(maxSizeValidator.validatorOption) < this.getFileSizeInMb(fileSize)
        }
        return false
    }

    isInvalidExtension(fileName: string) {
        const fileExtensionsValidator = {
            isActive: true,
            validatorOption: 'zip'
        }
        if (fileExtensionsValidator.isActive) {
            const fileExt = fileName.split('.')[1].toLowerCase()
            const splitted = fileExtensionsValidator.validatorOption.toLowerCase().split(';')
            return splitted.indexOf(fileExt) < 0
        }
        return false
    }

    isInvalid(validatorName: string): boolean {
        return this.form.get(this.uploadFormName).hasError(validatorName)
    }

    getErrorMessage(validatorName: string) {
        if (validatorName === 'required') {
            return 'Require a zip file'
        }
        else if (validatorName === 'maximumsize') {
            return 'Upload file must be less than 16Mb'
        }
        else if (validatorName === 'fileextensions') {
            return 'Upload file must be a zip file'
        }
        else {
            return ''
        }
    }

    onUpload() {
        this.btnOption.active = true
        this.backupClient.uploadBackupFile({
            data: this.selectedFile,
            fileName: this.selectedFile.name
        }).subscribe(
            res => {
                this.responseModel = res
                this.btnOption.active = false
                this.router.navigateByUrl('portal/builder/backup/restore/' + res.id)
            },
            err => {
                this.shortcutUtil.toastMessage(this.translate.instant('common.somethingWentWrong'), ToastType.Error)
                this.btnOption.active = false
            }
        )
    }

    onCancel(){
        this.router.navigateByUrl('portal/page/backup-management')
    }
}
