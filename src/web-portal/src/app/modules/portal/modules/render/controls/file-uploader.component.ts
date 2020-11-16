import { Component, OnInit, Input, ElementRef, ViewChild, AfterContentChecked, ChangeDetectorRef, AfterViewInit } from '@angular/core';
import { FormGroup, NgForm } from '@angular/forms';
import { PageRenderedControl, DefaultControlOptions } from 'app/core/models/page.model';
import { ExtendedControlValidator } from 'app/core/models/extended.models';
 
import { NGXLogger } from 'ngx-logger';
import { FilesClient, ValidatorType, ResponseUploadFile } from 'services/portal.service';
import { UploadFileService, DownloadableResponseFile } from 'services/uploadfile.service';
import { forkJoin } from 'rxjs';

@Component({
    selector: 'let-file-uploader',
    templateUrl: './file-uploader.component.html',
    styleUrls: ['./file-uploader.component.scss']
})
export class FileUploaderComponent implements OnInit, AfterContentChecked {
    @ViewChild('fileInput', { static: true }) fileInput: ElementRef
    @Input()
    form: FormGroup

    @Input()
    formControlKey: string

    @Input()
    tooltip: string

    @Input()
    control: PageRenderedControl<DefaultControlOptions>

    @Input()
    validators: Array<ExtendedControlValidator> = []

    @Input()
    multiple = false

    isMaximumSize = false
    isInvalidFileExtension = false

    selectedFiles: Set<File> = new Set()
    hasSelectedFile = false

    uploadedFiles: DownloadableResponseFile[] = []
    disabled = false

    progress;
    uploaded = false
    constructor(
        private cd: ChangeDetectorRef,
        private uploadFileService: UploadFileService,
        private logger: NGXLogger) { }
    ngAfterContentChecked(): void {
        setTimeout(() => {            
            if(this.isMaximumSize || this.isInvalidFileExtension){
                if (this.isMaximumSize) {
                    this.form.get(this.formControlKey).setErrors({ maximumsize: true })
                }
                
                if (this.isInvalidFileExtension) {
                    this.form.get(this.formControlKey).setErrors({ fileextensions: true })                
                }                
            }
        },500)
    }
    ngAfterViewInit(): void {
       
    }

    ngOnInit(
    ): void {
        this.disabled = this.control.defaultOptions.checkDisabled
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
            if (this.multiple) {
                for (const key in $event.target.files) {
                    if (!isNaN(parseInt(key))) {
                        this.selectedFiles.add($event.target.files[key]);
                    }
                }
            }
            else {
                this.selectedFiles = new Set()
                this.selectedFiles.add($event.target.files[0])
            }
            this.form.get(this.formControlKey).setErrors(null)
        }
        this.form.get(this.formControlKey).markAsTouched()
        this.fileInput.nativeElement.value = ''
    }

    getFileSizeInMb(size: number) {
        return Math.round(size / (1024 * 1024))
    }

    getFileSizeInKb(size: number) {
        return Math.round(size / 1024)
    }

    upload(file: File) {
        const uploadingFiles = new Set<File>()
        uploadingFiles.add(file)

        this.progress = this.uploadFileService.upload(uploadingFiles)
        const uploadingProgressObserables = []
        for (const key in this.progress) {
            uploadingProgressObserables.push(this.progress[key].completed);
        }

        forkJoin(uploadingProgressObserables).subscribe((responseFiles: any[]) => {
            if (!this.multiple) {
                this.logger.debug('current control options', this.control.defaultOptions)
                this.form.get(this.formControlKey).setValue(this.control.defaultOptions.allowfileurl ? responseFiles[0].response.downloadableUrl : responseFiles[0].response.fileId)
            }

            this.uploadedFiles = responseFiles
        });
    }

    canDownloadableFile(file: File) {
        const found = this.uploadedFiles.find(a => a.fileName === file.name)
        return !!found
    }

    getDownloadableLink(file: File) {
        const found = this.uploadedFiles.find(a => a.fileName === file.name)
        return !!found ? found.response.downloadableUrl : ''
    }

    remove(file: File) {
        this.selectedFiles.delete(file)
        const index = this.uploadedFiles.findIndex(a => a.fileName === file.name)
        if (index > -1) {
            this.uploadedFiles.splice(index, 1)
        }

        if (this.selectedFiles.size === 0) {
            this.form.get(this.formControlKey).setValue(null)
        }
    }

    isReachMaximumSize(fileSize: number) {
        const maxSizeValidator = this.control.validators.find(a => a.validatorType === ValidatorType.FileMaximumSize)
        if (maxSizeValidator.isActive) {
            return parseInt(maxSizeValidator.validatorOption) < this.getFileSizeInMb(fileSize)
        }
        return false
    }

    isInvalidExtension(fileName: string) {
        const fileExtensionsValidator = this.control.validators.find(a => a.validatorType === ValidatorType.FileExtensions)
        if (fileExtensionsValidator.isActive) {
            const extSplitted = fileName.split('.')
            const fileExt = extSplitted[extSplitted.length - 1].toLowerCase()
            const splitted = fileExtensionsValidator.validatorOption.toLowerCase().split(';')
            return splitted.indexOf(fileExt) < 0
        }
        return false
    }

    isInvalid(validatorName: string): boolean {
        return this.form.get(this.control.name).hasError(validatorName)
    }

    getErrorMessage(validatorName: string) {
        return this.validators.find(validator => validator.validatorName === validatorName).validatorErrorMessage
    }
}

