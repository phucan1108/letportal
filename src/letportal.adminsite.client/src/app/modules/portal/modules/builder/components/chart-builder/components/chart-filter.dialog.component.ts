import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { NGXLogger } from 'ngx-logger';
import { StaticResources } from 'portal/resources/static-resources';
import { BehaviorSubject } from 'rxjs';
import { ChartFilter, FilterType } from 'services/portal.service';
import { ChartFilterGridComponent } from './chart-filter.grid.component';

@Component({
    selector: 'let-chart-filter-dialog',
    templateUrl: './chart-filter.dialog.component.html'
})
export class ChartFilterDialogComponent implements OnInit {
    chartFilterFormGroup: FormGroup
    selectedChartFilter: ChartFilter

    _filterTypes: any[] = [] 
    _filterTypes$: BehaviorSubject<any[]> = new BehaviorSubject([])
    isEditMode = false
    isSmallDevice = false
    constructor(
        public dialogRef: MatDialogRef<ChartFilterGridComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef,
        private breakpointObserver: BreakpointObserver,
        private translate: TranslateService,
        private logger: NGXLogger
    ) {
        this.breakpointObserver.observe([
            Breakpoints.HandsetPortrait,
            Breakpoints.HandsetLandscape
        ]).subscribe(result => {
            if (result.matches) {
                this.isSmallDevice = true
                this.logger.debug('Small device on chart filter dialog', this.isSmallDevice)
            }
            else {
                this.isSmallDevice = false
                this.logger.debug('Small device on chart filter dialog', this.isSmallDevice)
            }
        });
    }

    ngOnInit(): void {
        this.selectedChartFilter = this.data
        this.logger.debug('Sent data chart filter', this.selectedChartFilter)
        this.isEditMode = this.selectedChartFilter.name ? true : false;

        StaticResources.chartFilterTypes()?.forEach(filter => {
            this._filterTypes.push({
                name: this.translate.instant(filter.name),
                value: filter.value
            })
        })

        this._filterTypes$.next(this._filterTypes)
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
