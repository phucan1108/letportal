import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ChartFilter, FilterType } from 'services/portal.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { NGXLogger } from 'ngx-logger';
import { ChartFilterGridComponent } from './chart-filter.grid.component';
import { StaticResources } from 'portal/resources/static-resources';

@Component({
    selector: 'let-chart-filter-dialog',
    templateUrl: './chart-filter.dialog.component.html'
})
export class ChartFilterDialogComponent implements OnInit {
    chartFilterFormGroup: FormGroup
    selectedChartFilter: ChartFilter

    _filterTypes = StaticResources.chartFilterTypes()

    isEditMode = false
    isSmallDevice = false
    constructor(
        public dialogRef: MatDialogRef<ChartFilterGridComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef,
        private breakpointObserver: BreakpointObserver,
        private logger: NGXLogger
    ) {
        this.breakpointObserver.observe([
            Breakpoints.HandsetPortrait,
            Breakpoints.HandsetLandscape
        ]).subscribe(result => {
            if (result.matches) {
                this.isSmallDevice = true
                this.logger.debug('Small device', this.isSmallDevice)
            }
            else {
                this.isSmallDevice = false
                this.logger.debug('Small device', this.isSmallDevice)
            }
        });
    }

    ngOnInit(): void {
        this.selectedChartFilter = this.data
        this.logger.debug('Sent data chart filter', this.selectedChartFilter)
        this.isEditMode = this.selectedChartFilter.name ? true : false;

        this.createChartFilterForm()
    }

    createChartFilterForm() {
        this.chartFilterFormGroup = this.fb.group({
            name: [this.selectedChartFilter.name, [Validators.required, Validators.maxLength(250)]],
            displayName: [this.selectedChartFilter.displayName, [Validators.required, Validators.maxLength(250)]],
            type: [this.selectedChartFilter.type, Validators.required],
            rangeValue: [this.selectedChartFilter.rangeValue],
            defaultValue: [this.selectedChartFilter.defaultValue, Validators.maxLength(250)],
            allowDefaultValue: [this.selectedChartFilter.allowDefaultValue],
            isMultiple: [this.selectedChartFilter.isMultiple],
            isHidden: [this.selectedChartFilter.isHidden]
        })
    }

    enableRangeValue() {
        const currentType = this.chartFilterFormGroup.get('type').value as FilterType
        return currentType !== FilterType.None && currentType !== FilterType.Checkbox && currentType !== FilterType.Select
    }

    enableMultiple(){
        const currentType = this.chartFilterFormGroup.get('type').value as FilterType
        return currentType !== FilterType.None && currentType !== FilterType.Checkbox
    }

    onSubmit() {
        if (this.chartFilterFormGroup.valid) {
            this.dialogRef.close(this.combiningChartFilter())
        }
    }

    combiningChartFilter() {
        const formValues = this.chartFilterFormGroup.value
        const combiningChartFilter: ChartFilter = {
            name: formValues.name,
            displayName: formValues.displayName,
            type: formValues.type,
            datasourceOptions: this.selectedChartFilter.datasourceOptions,
            defaultValue: formValues.defaultValue,
            allowDefaultValue: formValues.allowDefaultValue,
            isHidden: formValues.isHidden,
            isMultiple: formValues.isMultiple,
            rangeValue: formValues.rangeValue
        }
        this.logger.debug('Combining Chart Filter', combiningChartFilter)
        return combiningChartFilter
    }
}
