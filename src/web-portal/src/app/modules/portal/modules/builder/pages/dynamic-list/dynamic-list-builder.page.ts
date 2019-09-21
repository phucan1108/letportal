import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import * as _ from 'lodash'
import { MatDialog } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { StaticResources } from 'portal/resources/static-resources';
import { CommandGridComponent } from '../../components/dynamic-list/components/commands/command-grid.component';
import { SecurityService } from 'app/core/security/security.service';
import { NGXLogger } from 'ngx-logger';
import { PagesClient, DynamicListClient, DynamicList, ShellOption, PortalClaim, CommandButtonInList, ColumndDef, Parameter, DynamicListSourceType, DatabaseOptions } from 'services/portal.service';
import { PageService } from 'services/page.service';
import { PortalValidators } from 'app/core/validators/portal.validators';
import { ExtendedFilterField, ListOptions } from 'portal/modules/models/dynamiclist.extended.model';
import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';

@Component({
    selector: 'dynamic-list-builder',
    templateUrl: './dynamic-list-builder.page.html',
    styleUrls: ['./dynamic-list-builder.page.scss']
})
export class DynamicListBuilderPage implements OnInit {
    //#region New changes
    componentInfo: FormGroup
    shellOptions$: BehaviorSubject<Array<ExtendedShellOption>> = new BehaviorSubject([])
    shellOptions: Array<ExtendedShellOption> = []
    claims: Array<PortalClaim> = []
    filterOptions: Array<ExtendedFilterField> = []
    commandsInList: Array<CommandButtonInList> = [];
    columnDefs: Array<ColumndDef> = [];
    heading = 'Dynamic List Builder'
    parameters: Array<Parameter> = []
    databaseOptions: DatabaseOptions
    //#endregion

    isSubmitted: boolean = false

    edittingDynamicList: DynamicList;
    isEditMode = false;
    _paramSourceTypes = StaticResources.paramSourceTypes()

    isCanSubmit = false;

    @ViewChild(CommandGridComponent) commandGrid: CommandGridComponent

    constructor(
        private pageService: PageService,
        private fb: FormBuilder,
        private dynamicListClient: DynamicListClient,
        private cd: ChangeDetectorRef,
        public dialog: MatDialog,
        private shortcutUtil: ShortcutUtil,
        private activatedRoute: ActivatedRoute,
        private router: Router,
        private pagesClient: PagesClient,
        private security: SecurityService,
        private logger: NGXLogger
    ) {
    }

    ngOnInit(): void {
        this.pageService.init('dynamic-list-builder').subscribe()
        this.edittingDynamicList = this.activatedRoute.snapshot.data.dynamicList;
        this.isEditMode = this.edittingDynamicList ? true : false;

        this.initialBuilderForm()
    }

    enableSubmit() {
        return true
    }

    initialBuilderForm() {
        if (this.isEditMode) {
            this.initialEditBuilderForm()            
            this.databaseOptions = this.edittingDynamicList.listDatasource.databaseConnectionOptions
        }
        else {
            this.createDynamicListForm()            
            this.databaseOptions = {
                databaseConnectionId: '',
                entityName: '',
                query: ''
            }
        }
    }

    initialEditBuilderForm() {
        this.componentInfo = this.fb.group({
            name: new FormControl( { value: this.edittingDynamicList.name, disabled: true }, [Validators.required, Validators.maxLength(250)], [PortalValidators.dynamicListUniqueName(this.dynamicListClient)]),
            displayName: [this.edittingDynamicList.displayName, [Validators.required, Validators.maxLength(250)]]
        })

        
        this.filterOptions = this.edittingDynamicList.filtersList ? this.edittingDynamicList.filtersList.filterFields as Array<ExtendedFilterField> : []
        this.columnDefs = this.edittingDynamicList.columnsList ? this.edittingDynamicList.columnsList.columndDefs : []
        this.commandsInList = _.cloneDeep(this.edittingDynamicList.commandsList.commandButtonsInList)
        this.shellOptions = this.edittingDynamicList.options as ExtendedShellOption[]

        ListOptions.combinedDefaultShellOptions(this.shellOptions)
        this.shellOptions$.next(this.shellOptions)
        this.onValueChanges()
        this.isCanSubmit = true
    }

    createDynamicListForm() {
        this.componentInfo = this.fb.group({
            name: ['', [Validators.required, Validators.maxLength(250)], [PortalValidators.dynamicListUniqueName(this.dynamicListClient)]],
            displayName: ['', [Validators.required, Validators.maxLength(250)]]
        })
        this.shellOptions = this.shellOptions.concat(ListOptions.getDefaultShellOptionsForList())
        this.shellOptions$.next(this.shellOptions)
        this.onValueChanges()
    }

    onCancelClick() {
        this.router.navigateByUrl('/portal/page/dynamic-list-management')
    }

    onSubmitDynamicListForm() {
        if (this.enableSubmit()) {
            this.isSubmitted = true;
            const submittingDynamicList = this.combineAllDatasToDynamicList()
            if (this.isEditMode) {
                this.dynamicListClient.update(this.edittingDynamicList.id, submittingDynamicList)
                    .subscribe(rep => {
                        this.shortcutUtil.notifyMessage("Update successfully!", ToastType.Success)
                    },
                        err => {
                            this.shortcutUtil.notifyMessage("Oops, Something went wrong, please try again!", ToastType.Error)
                        })
            }
            else {
                this.dynamicListClient.create(submittingDynamicList).subscribe(rep => {
                    this.router.navigateByUrl('/portal/dynamic-list/builder/')
                    this.shortcutUtil.notifyMessage("Save successfully!", ToastType.Success)
                },
                    err => {
                        this.shortcutUtil.notifyMessage("Oops, Something went wrong, please try again!", ToastType.Error)
                    })
            }
        }
    }

    databaseOptionsChanged($event){
        this.databaseOptions = $event
        this.logger.debug('database options changed', this.databaseOptions)
    }

    combineAllDatasToDynamicList(): DynamicList {
        const pageInfoValues = this.componentInfo.value;
        return {
            name: this.isEditMode ? this.edittingDynamicList.name : pageInfoValues.name,
            displayName: pageInfoValues.displayName,
            listDatasource: {
                sourceType: DynamicListSourceType.Database,
                databaseConnectionOptions: this.databaseOptions
            },
            columnsList: {
                columndDefs: this.columnDefs
            },
            commandsList: {
                commandButtonsInList: this.commandsInList
            },
            filtersList: {
                filterFields: this.filterOptions
            },
            options: this.shellOptions,
            paramsList: {
                parameters: this.parameters
            }
        }
        return null
    }
    /** SELECTION */

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

    afterSelectingEntity($event) {
        this.filterOptions = $event.filters
        this.columnDefs = $event.columnDefs
        this.commandsInList = $event.commands
        this.commandGrid.setCommands($event.commands)
        this.commandGrid.refreshCommandTable()
        this.cd.markForCheck()
    }

    afterPopulatingQuery($event) {
        this.filterOptions = $event.filters
        this.columnDefs = $event.columnDefs
        this.parameters = $event.parameters
        this.commandGrid.refreshCommandTable()
        this.cd.markForCheck()
    }

    commandsChanged($event) {
        this.commandsInList = $event
        this.logger.debug('Builder new commands', this.commandsInList)
    }

    columnDefsChanged($event) {
        this.columnDefs = $event
        this.logger.debug('Builder new column defs', this.columnDefs)
    }

    filtersChanged($event) {
        this.filterOptions = $event
        this.logger.debug('Builder new filters', this.filterOptions)
    }

    onChangingOptions($event){
        this.shellOptions = $event
        this.logger.debug('Options changed', this.shellOptions)
    }
}
