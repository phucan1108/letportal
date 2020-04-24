import { Component, OnInit, ChangeDetectorRef, Inject } from '@angular/core';
import { GroupControls, ExtendedPageSection } from 'app/core/models/extended.models';
import { PageRenderedControl, DefaultControlOptions } from 'app/core/models/page.model';
import { FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NGXLogger } from 'ngx-logger';
import { PageService } from 'services/page.service';
import { Store } from '@ngxs/store';
import { ControlType } from 'services/portal.service';
import { MatProgressButtonOptions } from 'mat-progress-buttons';
import { ObjectUtils } from 'app/core/utils/object-util';

@Component({
    selector: 'let-standard-array-dialog',
    templateUrl: './standard-array-dialog.component.html'
})
export class StandardArrayDialog implements OnInit {
    builderFormGroup: FormGroup
    controlsGroups: Array<GroupControls>
    controlType = ControlType
    section: ExtendedPageSection
    _labelClass = 'col-lg-2 col-form-label'
    _boundedClass = 'col-lg-4'
    isEdit = false
    submitBtnOptions: MatProgressButtonOptions = {
        active: false,
        text: 'Save',
        buttonColor: 'primary',
        barColor: 'primary',
        raised: false,
        stroked: false,
        fab: false,
        mode: 'indeterminate',
        disabled: false,
    }
    constructor(
        private store: Store,  
        private pageService: PageService,
        public dialogRef: MatDialogRef<any>,
        private logger: NGXLogger,
        private cd: ChangeDetectorRef,
        @Inject(MAT_DIALOG_DATA) public data: any
    ) { 
        
    }

    ngOnInit(): void { 
        this.controlsGroups = this.data.controlsGroups
        this.builderFormGroup = this.data.formGroup
        this.section = this.data.section
        this.isEdit = this.data.isEdit
    }

    submit(){
        this.builderFormGroup.markAsTouched()
        this.submitBtnOptions.active = true
        // Prevent race condition on async and debounce
        setTimeout(() => {
            if(this.builderFormGroup.valid){
                this.dialogRef.close(true);
            }
            this.submitBtnOptions.active = false
        },700)        
    }
}
