import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { PageService } from 'services/page.service';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { DynamicListClient, ChartsClient, Chart, SharedDatabaseOptions, ChartType, ChartFilter, PageSectionLayoutType, App, AppsClient } from 'services/portal.service';
import { MatDialog } from '@angular/material/dialog';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ActivatedRoute, Router } from '@angular/router';
import { NGXLogger } from 'ngx-logger';
import { PortalValidators } from 'app/core/validators/portal.validators';
import { BehaviorSubject, Observable } from 'rxjs';
import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import { ChartOptions } from 'portal/modules/models/chart.extended.model';
import { StaticResources } from 'portal/resources/static-resources';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'chart-builder',
    templateUrl: './chart-builder.page.html',
    styleUrls: ['./chart-builder.page.scss']
})
export class ChartBuilderPage implements OnInit {
    componentInfo: FormGroup
    edittingChart: Chart
    databaseOptions: SharedDatabaseOptions
    _chartTypes = StaticResources.chartTypes()
    shellOptions$: BehaviorSubject<Array<ExtendedShellOption>> = new BehaviorSubject([])
    shellOptions: Array<ExtendedShellOption> = []
    chartFilters: ChartFilter[] = []
    isEditMode = false
    isCanSubmit = false
    _layoutTypes = StaticResources.sectionLayoutTypes()
    apps$: Observable<App[]>
    constructor(
        private chartsClient: ChartsClient,
        private appsClient: AppsClient,
        private pageService: PageService,
        private fb: FormBuilder,
        private translate: TranslateService,
        private cd: ChangeDetectorRef,
        public dialog: MatDialog,
        private shortcutUtil: ShortcutUtil,
        private activatedRoute: ActivatedRoute,
        private router: Router,
        private logger: NGXLogger
    ) { }

    ngOnInit(): void {
        this.pageService.init('chart-builder').subscribe()
        this.edittingChart = this.activatedRoute.snapshot.data.chart;
        this.apps$ = this.appsClient.getAll()
        if (this.edittingChart) {
            this.isEditMode = true
        }

        if (this.isEditMode) {
            this.componentInfo = this.fb.group({
                name: new FormControl({ value: this.edittingChart.name, disabled: true }, [Validators.required, Validators.maxLength(250)], [PortalValidators.chartUniqueName(this.chartsClient)]),
                displayName: [this.edittingChart.displayName, [Validators.required, Validators.maxLength(250)]],
                app: [this.edittingChart.appId, [Validators.required]],
                chartTitle: [this.edittingChart.definitions.chartTitle, [Validators.required, Validators.maxLength(250)]],
                chartType: [this.edittingChart.definitions.chartType, Validators.required],
                layoutType: [this.edittingChart.layoutType.toString(), Validators.required],
                mappingProjection: [this.edittingChart.definitions.mappingProjection, Validators.required]
            })
            this.shellOptions = this.edittingChart.options as ExtendedShellOption[]
            ChartOptions.combinedDefaultShellOptions(this.shellOptions)
            this.shellOptions$.next(this.shellOptions)
            this.isCanSubmit = true
            this.databaseOptions = this.edittingChart.databaseOptions
            this.chartFilters = this.edittingChart.chartFilters
        }
        else {
            this.componentInfo = this.fb.group({
                name: ['', [Validators.required, Validators.maxLength(250)], [PortalValidators.chartUniqueName(this.chartsClient)]],
                displayName: ['', [Validators.required, Validators.maxLength(250)]],
                app: [this.edittingChart.appId, [Validators.required]],
                chartTitle: ['', [Validators.required, Validators.maxLength(250)]],
                chartType: [ChartType.VerticalBarChart, Validators.required],
                layoutType: [PageSectionLayoutType.OneColumn.toString(), Validators.required],
                mappingProjection: ['name=name;value=value;group=group', Validators.required]
            })
            this.shellOptions = this.shellOptions.concat(ChartOptions.getDefaultShellOptionsForChart())
            this.shellOptions$.next(this.shellOptions)
            this.databaseOptions = {
                databaseConnectionId: '',
                entityName: '',
                query: ''
            }
        }

        this.onValueChanges();
    }

    onValueChanges() {
        this.componentInfo.valueChanges.subscribe(newValue => {
            this.isCanSubmit = this.enableSubmit();
        })
        // Auto-generated name
        this.componentInfo.get('displayName').valueChanges.subscribe(newValue => {
            if (newValue && !this.isEditMode) {
                // Apply this change to name
                const listNameValue = (newValue as string).toLowerCase().replace(/\s/g, '')
                this.componentInfo.get('name').setValue(listNameValue)

                this.componentInfo.get('chartTitle').setValue(newValue)
            }
        })
    }

    enableSubmit() {
        return this.componentInfo.valid
    }

    onSubmit() {
        if (this.componentInfo.valid && this.isCanSubmit) {
            const formValues = this.componentInfo.value
            const combiningChart: Chart = {
                name: formValues.name,
                displayName: formValues.displayName,
                appId: formValues.app,
                definitions: {
                    chartTitle: formValues.chartTitle,
                    chartType: formValues.chartType,
                    mappingProjection: formValues.mappingProjection
                },
                layoutType: parseInt(formValues.layoutType),
                chartFilters: this.chartFilters,
                options: this.shellOptions,
                databaseOptions: this.databaseOptions
            }

            if (this.isEditMode) {
                combiningChart.id = this.edittingChart.id
                combiningChart.name = this.edittingChart.name
                combiningChart.allowOverrideOptions = this.edittingChart.allowOverrideOptions
                combiningChart.allowPassingDatasource = this.edittingChart.allowPassingDatasource
                combiningChart.datasourceName = this.edittingChart.datasourceName
                combiningChart.timeSpan = this.edittingChart.timeSpan
                this.chartsClient.update(this.edittingChart.id, combiningChart).subscribe(rep => {
                    this.shortcutUtil.toastMessage(this.translate.instant('common.updateSuccessfully'), ToastType.Success)
                },
                    err => {
                        this.shortcutUtil.toastMessage(this.translate.instant('common.somethingWentWrong'), ToastType.Error)
                    })
            }
            else {
                this.chartsClient.create(combiningChart).subscribe(rep => {
                    this.router.navigateByUrl('/portal/page/charts-management')
                    this.shortcutUtil.toastMessage(this.translate.instant('common.saveSuccessfully'), ToastType.Success)
                },
                    err => {
                        this.shortcutUtil.toastMessage(this.translate.instant('common.somethingWentWrong'), ToastType.Error)
                    })
            }
        }
    }

    onCancel() {
        this.router.navigateByUrl('/portal/page/charts-management')
    }

    onChangingOptions($event) {
        this.shellOptions = $event
        this.logger.debug('Options changed', this.shellOptions)
    }

    afterSelectingEntity($event) {
        this.logger.debug('After selecting entity Chart filters:', $event)
    }

    afterPopulatingQuery($event) {
        this.logger.debug('After populating entity Chart filters:', $event)
        this.chartFilters = $event.chartFilters
    }

    databaseOptionsChanged($event) {
        this.logger.debug('After database change Chart filters:', $event)
        this.databaseOptions = $event
    }

    chartFiltersChanged($event: ChartFilter[]) {
        this.chartFilters = $event
    }
}
