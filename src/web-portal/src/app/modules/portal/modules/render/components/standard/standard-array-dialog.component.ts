import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Store } from '@ngxs/store';
import { SimpleBoundControl } from 'app/core/context/bound-control';
import { OpenableBoundSection } from 'app/core/context/bound-section';
import { ExtendedPageSection, GroupControls } from 'app/core/models/extended.models';
import { DefaultControlOptions, PageRenderedControl } from 'app/core/models/page.model';
import { ObjectUtils } from 'app/core/utils/object-util';
import { MatProgressButtonOptions } from 'mat-progress-buttons';
import { NGXLogger } from 'ngx-logger';
import { PageService } from 'services/page.service';
import { ControlType } from 'services/portal.service';

@Component({
    selector: 'let-standard-array-dialog',
    templateUrl: './standard-array-dialog.component.html'
})
export class StandardArrayDialog implements OnInit {
    boundSection: OpenableBoundSection
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
        this.boundSection = this.data.boundSection as OpenableBoundSection

        // Change 0.9.0: we need to set bound control into each control
        this.controlsGroups.forEach(group => {
            if (!group.isLineBreaker) {
                group.controlsList.forEach(control => {
                    let foundBoundControl = this.boundSection.getOpenedSection().get(control.name)
                    if (ObjectUtils.isNotNull(foundBoundControl)) {
                        control.boundControl = foundBoundControl
                        control.boundControl.hide = control.defaultOptions.checkedHidden
                    }
                    else {
                        control.boundControl = new SimpleBoundControl(control.name, control.type, null)
                        control.boundControl.hide = control.defaultOptions.checkedHidden
                    }
                })
            }
        })
    }

    submit() {
        this.builderFormGroup.markAsTouched()
        this.submitBtnOptions.active = true
        // Prevent race condition on async and debounce
        setTimeout(() => {
            if (this.builderFormGroup.valid) {
                this.dialogRef.close(true);
            }
            this.submitBtnOptions.active = false
        }, 700)
    }

    getBoundControl = (control: PageRenderedControl<DefaultControlOptions>) => this.boundSection.getOpenedSection().get(control.name)
}
