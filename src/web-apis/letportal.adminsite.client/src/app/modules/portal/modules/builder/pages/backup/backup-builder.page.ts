import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { SecurityService } from 'app/core/security/security.service';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import * as FileSaver from 'file-saver';
import { NGXLogger } from 'ngx-logger';
import { BehaviorSubject, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { PageService } from 'services/page.service';
import { AppsClient, BackupRequestModel, BackupsClient, ChartsClient, CompositeControlsClient, DatabasesClient, DynamicListClient, PagesClient, ShortEntityModel, StandardComponentClient } from 'services/portal.service';
import { GenerateCodeDialog } from '../../components/backup-builder/generatecode.dialog';


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
    notifiedApps: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])

    selectedDatabases: ShortEntityModel[]
    databases$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    searchDatabaseDebouncer: Subject<string> = new Subject<string>()
    notifiedDatabases: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])

    selectedStandards: ShortEntityModel[]
    standards$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    searchStandardDebouncer: Subject<string> = new Subject<string>()
    notifiedStandards: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])

    selectedTree: ShortEntityModel[]
    tree$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    searchTreeDebouncer: Subject<string> = new Subject<string>()
    notifiedTree: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])

    selectedArray: ShortEntityModel[]
    array$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    searchArrayDebouncer: Subject<string> = new Subject<string>()
    notifiedArray: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])

    selectedDynamicLists: ShortEntityModel[]
    dynamicLists$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    searchDynamicListDebouncer: Subject<string> = new Subject<string>()
    notifiedDynamicLists: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])

    selectedCharts: ShortEntityModel[]
    charts$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    searchChartDebouncer: Subject<string> = new Subject<string>()
    notifiedCharts: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])

    selectedPages: ShortEntityModel[]
    pages$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    searchPageDebouncer: Subject<string> = new Subject<string>()
    notifiedPages: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])

    selectedControls: ShortEntityModel[]
    controls$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    searchControlDebouncer: Subject<string> = new Subject<string>()
    notifiedControls: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])

    isCanSubmit = false
    isSubmitted = false
    isCreated = false
    downloadableUrl: string
    constructor(
        private appClient: AppsClient,
        private databaseClient: DatabasesClient,
        private standardClient: StandardComponentClient,
        private dynamicListClient: DynamicListClient,
        private compositeControlClient: CompositeControlsClient,
        private chartClient: ChartsClient,
        private pageClient: PagesClient,
        private backupClient: BackupsClient,
        private security: SecurityService,
        private pageService: PageService,
        private shortcutUtil: ShortcutUtil,
        private translate: TranslateService,
        private router: Router,
        private dialog: MatDialog,
        private logger: NGXLogger,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef
    ) { }

    ngOnInit(): void {
        this.pageService.init('backup-builder').subscribe()
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

        this.searchTreeDebouncer
            .pipe(
                debounceTime(500),
                distinctUntilChanged(),
                tap(
                    res => {
                        this.standardClient.getSortTreeStandards(res).subscribe(
                            res => {
                                this.tree$.next(res)
                            },
                            err => {

                            }
                        )
                    }
                )
            ).subscribe()

        this.searchArrayDebouncer
            .pipe(
                debounceTime(500),
                distinctUntilChanged(),
                tap(
                    res => {
                        this.standardClient.getSortArrayStandards(res).subscribe(
                            res => {
                                this.array$.next(res)
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

        this.searchControlDebouncer
            .pipe(
                debounceTime(500),
                distinctUntilChanged(),
                tap(
                    res => {
                        this.compositeControlClient.getShortControls(res).subscribe(
                            res => {
                                this.controls$.next(res)
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
        // 0.9.0: We will scan all components that are related to
        this.selectedApps.forEach(app => {
            this.standardClient.getSortStandards('').subscribe(res => {
                this.notifiedStandards.next(res.filter(a => a.appId === app.id))
            })
            this.standardClient.getSortArrayStandards('').subscribe(res => {
                this.notifiedArray.next(res.filter(a => a.appId === app.id))
            })
            this.standardClient.getSortTreeStandards('').subscribe(res => {
                this.notifiedTree.next(res.filter(a => a.appId === app.id))
            })
            this.chartClient.getShortCharts('').subscribe(res => {
                this.notifiedCharts.next(res.filter(a => a.appId === app.id))
            })
            this.pageClient.getShortPages('').subscribe(res => {
                this.notifiedPages.next(res.filter(a => a.appId === app.id))
            })
            this.dynamicListClient.getShortDynamicLists('').subscribe(res => {
                this.notifiedDynamicLists.next(res.filter(a => a.appId === app.id))
            })
            this.compositeControlClient.getShortControls('').subscribe(res => {
                this.notifiedControls.next(res.filter(a => a.appId === app.id))
            })
        })
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

    onSeachTreeChanged($event) {
        if ($event) {
            this.searchTreeDebouncer.next($event)
        }
        else {
            this.tree$.next([])
        }
    }

    onSelectTreeChanged($event) {
        this.selectedTree = $event
    }

    onSeachArrayChanged($event) {
        if ($event) {
            this.searchArrayDebouncer.next($event)
        }
        else {
            this.array$.next([])
        }
    }

    onSelectArrayChanged($event) {
        this.selectedArray = $event
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

    onSeachControlChanged($event) {
        if ($event) {
            this.searchControlDebouncer.next($event)
        }
        else {
            this.controls$.next([])
        }
    }

    onSelectControlChanged($event) {
        this.selectedControls = $event
    }

    onSubmit() {
        if (this.backupFormGroup.valid) {
            const formValues = this.backupFormGroup.value
            const requestModel: BackupRequestModel = {
                name: formValues.name,
                description: formValues.description,
                apps: this.selectedApps ? this.selectedApps.map(app => app.id) : [],
                databases: this.selectedDatabases ? this.selectedDatabases.map(db => db.id) : [],
                charts: this.selectedCharts ? this.selectedCharts.map(chart => chart.id) : [],
                standards: this.selectedStandards ? this.selectedStandards.map(standard => standard.id) : [],
                tree: this.selectedTree ? this.selectedTree.map(tree => tree.id) : [],
                array: this.selectedArray ? this.selectedArray.map(array => array.id) : [],
                dynamicLists: this.selectedDynamicLists ? this.selectedDynamicLists.map(dynamicList => dynamicList.id) : [],
                pages: this.selectedPages ? this.selectedPages.map(page => page.id) : [],
                compositeControls: this.selectedControls ? this.selectedControls.map(control => control.id) : [],
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
                    this.shortcutUtil.toastMessage(this.translate.instant('common.somethingWentWrong'), ToastType.Error)
                }
            )
        }
    }

    onGenerate() {
        const dialogRef = this.dialog.open(GenerateCodeDialog)
        dialogRef.afterClosed().subscribe(
            res => {
                if (!!res) {
                    this.backupClient.generateCode({
                        fileName: res.fileName,
                        versionNumber: res.versionNumber,
                        apps: this.selectedApps ? this.selectedApps.map(app => app.id) : [],
                        databases: this.selectedDatabases ? this.selectedDatabases.map(db => db.id) : [],
                        charts: this.selectedCharts ? this.selectedCharts.map(chart => chart.id) : [],
                        standards: this.selectedStandards ? this.selectedStandards.map(standard => standard.id) : [],
                        dynamicLists: this.selectedDynamicLists ? this.selectedDynamicLists.map(dynamicList => dynamicList.id) : [],
                        pages: this.selectedPages ? this.selectedPages.map(page => page.id) : [],
                        array: this.selectedArray ? this.selectedArray.map(array => array.id) : [],
                        tree: this.selectedTree ? this.selectedTree.map(tree => tree.id) : [],
                        compositeControls: this.selectedControls ? this.selectedControls.map(control => control.id) : []
                    }).pipe(
                        tap(res => {
                            FileSaver.saveAs(res.data, res.fileName)
                        })
                    ).subscribe()
                }
            }
        )
    }

    download() {
        window.location.href = this.downloadableUrl
    }

    refresh() {
        this.router.navigateByUrl('portal/builder/backup')
    }

    onCancel() {
        this.router.navigateByUrl('portal/page/backup-management')
    }
}
