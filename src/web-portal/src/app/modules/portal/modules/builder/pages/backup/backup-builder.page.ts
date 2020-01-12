import { Component, OnInit, ChangeDetectorRef, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatTree, MatTreeFlattener, MatTreeFlatDataSource, MatInput } from '@angular/material';
import { ShortEntityModel, AppsClient, DatabasesClient, StandardComponentClient, DynamicListClient, ChartsClient, PagesClient, BackupsClient, BackupRequestModel } from 'services/portal.service';
import { BackupNode, SelectableBackupNode } from 'portal/modules/models/backup.extended.model';
import * as _ from 'lodash';
import { SelectionModel } from '@angular/cdk/collections';
import { FlatTreeControl } from '@angular/cdk/tree';
import { debounceTime, tap, distinctUntilChanged } from 'rxjs/operators';
import { Subject, BehaviorSubject } from 'rxjs';
import { ArrayUtils } from 'app/core/utils/array-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import { SecurityService } from 'app/core/security/security.service';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { Router } from '@angular/router';

@Component({
    selector: 'let-backup-builder',
    templateUrl: './backup-builder.page.html',
    styleUrls: ['./backup-builder.page.scss']
})
export class BackupBuilderPage implements OnInit {

    backupFormGroup: FormGroup
    selectedApps: ShortEntityModel[]
    apps$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    seachAppDebouncer: Subject<string> = new Subject<string>()

    selectedDatabases: ShortEntityModel[]
    databases$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    searchDatabaseDebouncer: Subject<string> = new Subject<string>()

    selectedStandards: ShortEntityModel[]
    standards$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    searchStandardDebouncer: Subject<string> = new Subject<string>()

    selectedDynamicLists: ShortEntityModel[]
    dynamicLists$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    searchDynamicListDebouncer: Subject<string> = new Subject<string>()

    selectedCharts: ShortEntityModel[]
    charts$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    searchChartDebouncer: Subject<string> = new Subject<string>()

    selectedPages: ShortEntityModel[]
    pages$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    searchPageDebouncer: Subject<string> = new Subject<string>()

    isCanSubmit = false
    isSubmitted = false
    isCreated = false
    downloadableUrl: string
    constructor(
        private appClient: AppsClient,
        private databaseClient: DatabasesClient,
        private standardClient: StandardComponentClient,
        private dynamicListClient: DynamicListClient,
        private chartClient: ChartsClient,
        private pageClient: PagesClient,
        private backupClient: BackupsClient,
        private security: SecurityService,
        private shortcutUtil: ShortcutUtil,
        private router: Router,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef
    ) { }

    ngOnInit(): void {
        this.backupFormGroup = this.fb.group({
            name: ['', [Validators.required, Validators.maxLength(250)]],
            description: ['', [Validators.required, Validators.maxLength(500)]]
        })
        this.seachAppDebouncer
            .pipe(
                debounceTime(500),
                distinctUntilChanged(),
                tap(
                    res => {
                        this.appClient.getShortApps(res).subscribe(
                            res => {
                                this.apps$.next(res)
                            },
                            err => {

                            }
                        )
                    }
                )
            ).subscribe()

        this.searchDatabaseDebouncer
            .pipe(
                debounceTime(500),
                distinctUntilChanged(),
                tap(
                    res => {
                        this.databaseClient.getShortDatabases(res).subscribe(
                            res => {
                                this.databases$.next(res)
                            },
                            err => {

                            }
                        )
                    }
                )
            ).subscribe()

        this.searchStandardDebouncer
            .pipe(
                debounceTime(500),
                distinctUntilChanged(),
                tap(
                    res => {
                        this.standardClient.getSortStandards(res).subscribe(
                            res => {
                                this.standards$.next(res)
                            },
                            err => {

                            }
                        )
                    }
                )
            ).subscribe()

        this.searchDynamicListDebouncer
            .pipe(
                debounceTime(500),
                distinctUntilChanged(),
                tap(
                    res => {
                        this.dynamicListClient.getShortDynamicLists(res).subscribe(
                            res => {
                                this.dynamicLists$.next(res)
                            },
                            err => {

                            }
                        )
                    }
                )
            ).subscribe()

        this.searchChartDebouncer
            .pipe(
                debounceTime(500),
                distinctUntilChanged(),
                tap(
                    res => {
                        this.chartClient.getShortCharts(res).subscribe(
                            res => {
                                this.charts$.next(res)
                            },
                            err => {

                            }
                        )
                    }
                )
            ).subscribe()

        this.searchPageDebouncer
            .pipe(
                debounceTime(500),
                distinctUntilChanged(),
                tap(
                    res => {
                        this.pageClient.getShortPages(res).subscribe(
                            res => {
                                this.pages$.next(res)
                            },
                            err => {

                            }
                        )
                    }
                )
            ).subscribe()
    }

    onSeachAppChanged($event) {
        if ($event) {
            this.seachAppDebouncer.next($event)
        }
        else {
            this.apps$.next([])
        }
    }

    onSelectAppChanged($event) {
        this.selectedApps = $event
    }

    onSeachDatabaseChanged($event) {
        if ($event) {
            this.searchDatabaseDebouncer.next($event)
        }
        else {
            this.databases$.next([])
        }
    }

    onSelectDatabaseChanged($event) {
        this.selectedDatabases = $event
    }

    onSeachStandardChanged($event) {
        if ($event) {
            this.searchStandardDebouncer.next($event)
        }
        else {
            this.standards$.next([])
        }
    }

    onSelectStandardChanged($event) {
        this.selectedStandards = $event
    }

    onSeachDynamicListChanged($event) {
        if ($event) {
            this.searchDynamicListDebouncer.next($event)
        }
        else {
            this.dynamicLists$.next([])
        }
    }

    onSelectDynamicListChanged($event) {
        this.selectedDynamicLists = $event
    }

    onSeachChartChanged($event) {
        if ($event) {
            this.searchChartDebouncer.next($event)
        }
        else {
            this.charts$.next([])
        }
    }

    onSelectChartChanged($event) {
        this.selectedCharts = $event
    }

    onSeachPageChanged($event) {
        if ($event) {
            this.searchPageDebouncer.next($event)
        }
        else {
            this.pages$.next([])
        }
    }

    onSelectPageChanged($event) {
        this.selectedPages = $event
    }

    onSubmit() {
        if (this.backupFormGroup.valid) {
            let formValues = this.backupFormGroup.value
            const requestModel: BackupRequestModel = {
                name: formValues.name,
                description: formValues.description,
                apps: this.selectedApps ? this.selectedApps.map(app => app.id) : [],
                databases: this.selectedDatabases ? this.selectedDatabases.map(db => db.id): [],
                charts: this.selectedCharts ? this.selectedCharts.map(chart => chart.id): [],
                standards: this.selectedStandards ? this.selectedStandards.map(standard => standard.id) : [],
                dynamicLists: this.selectedDynamicLists ? this.selectedDynamicLists.map(dynamicList => dynamicList.id) : [],
                pages: this.selectedPages ? this.selectedPages.map(page => page.id) : [],
                creator: this.security.getAuthUser().username
            }
            this.isCreated = false
            this.isSubmitted = true
            this.backupClient.create(requestModel).subscribe(
                res => {
                    this.isCreated = true
                    this.downloadableUrl = res.downloadableUrl
                },
                err => {
                    this.shortcutUtil.notifyMessage("Oops! Something went wrong, please try again", ToastType.Error)
                }
            )
        }
    }

    download(){
        window.location.href = this.downloadableUrl
    }

    refresh(){
        this.router.navigateByUrl('portal/builder/backup')
    }

    onCancel(){
        this.router.navigateByUrl('portal/page/backup-management')
    }
}
