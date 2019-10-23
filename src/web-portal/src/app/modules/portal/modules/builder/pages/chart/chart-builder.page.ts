import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { PageService } from 'services/page.service';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { DynamicListClient, ChartsClient, Chart, DatabaseOptions } from 'services/portal.service';
import { MatDialog } from '@angular/material';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ActivatedRoute, Router } from '@angular/router';
import { NGXLogger } from 'ngx-logger';
import { PortalValidators } from 'app/core/validators/portal.validators';
import { BehaviorSubject } from 'rxjs';
import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import { ChartOptions } from 'portal/modules/models/chart.extended.model';

@Component({
    selector: 'chart-builder',
    templateUrl: './chart-builder.page.html',
    styleUrls: ['./chart-builder.page.scss']
})
export class ChartBuilderPage implements OnInit {
    componentInfo: FormGroup
    edittingChart: Chart
    databaseOptions: DatabaseOptions
    shellOptions$: BehaviorSubject<Array<ExtendedShellOption>> = new BehaviorSubject([])
    shellOptions: Array<ExtendedShellOption> = []
    isEditMode = false
    isCanSubmit = false
    constructor(
        private chartsClient: ChartsClient,
        private pageService: PageService,
        private fb: FormBuilder,
        private dynamicListClient: DynamicListClient,
        private cd: ChangeDetectorRef,
        public dialog: MatDialog,
        private shortcutUtil: ShortcutUtil,
        private activatedRoute: ActivatedRoute,
        private router: Router,
        private logger: NGXLogger
    ) { }

    ngOnInit(): void {
        this.edittingChart = this.activatedRoute.snapshot.data.chart;
        if (this.edittingChart) {
            this.isEditMode = true
        }

        if (this.isEditMode) {
            this.componentInfo = this.fb.group({
                name: new FormControl({ value: this.edittingChart.name, disabled: true }, [Validators.required, Validators.maxLength(250)], [PortalValidators.chartUniqueName(this.chartsClient)]),
                displayName: [this.edittingChart.displayName, [Validators.required, Validators.maxLength(250)]]
            })
            this.shellOptions = this.edittingChart.options as ExtendedShellOption[]
            ChartOptions.combinedDefaultShellOptions(this.shellOptions)
            this.shellOptions$.next(this.shellOptions)
            this.isCanSubmit = true
        }
        else {
            this.componentInfo = this.fb.group({
                name: ['', [Validators.required, Validators.maxLength(250)], [PortalValidators.chartUniqueName(this.chartsClient)]],
                displayName: ['', [Validators.required, Validators.maxLength(250)]]
            })
            this.shellOptions = this.shellOptions.concat(ChartOptions.getDefaultShellOptionsForChart())
            this.shellOptions$.next(this.shellOptions)
        }
    }

    onValueChanges() {
        this.componentInfo.valueChanges.subscribe(newValue => {
            this.isCanSubmit = this.enableSubmit();
        })
        // Auto-generated name and url path
        this.componentInfo.get('displayName').valueChanges.subscribe(newValue => {
            if (newValue && !this.isEditMode) {
                // Apply this change to list name and url path
                const listNameValue = (<string>newValue).toLowerCase().replace(/\s/g, '-')
                this.componentInfo.get('name').setValue(listNameValue)
            }
        })
    }

    enableSubmit(){
        return this.componentInfo.valid
    }

    onSubmit() {

    }

    onCancel() {

    }

    onChangingOptions($event) {
        this.shellOptions = $event
        this.logger.debug('Options changed', this.shellOptions)
    }
}
