import { Component, OnInit, ChangeDetectorRef, ViewChild } from '@angular/core';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import { MatDialog, MatTable } from '@angular/material';
import { ActivatedRoute, Router, Route } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { StepperSelectionEvent, STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { ExtendedPageSection, ExtendedPageControl } from '../../../../core/models/extended.models';
import { Guid } from 'guid-typescript'
import { Store } from '@ngxs/store';
import { filter, tap, map } from 'rxjs/operators';
import { ShortcutUtil } from 'app/shared/components/shortcuts/shortcut-util';
import { PageBuilderStateModel } from 'stores/pages/pagebuilder.state';
import * as _ from 'lodash';
import { MessageType, ToastType } from 'app/shared/components/shortcuts/shortcut.models';
import { Constants } from 'portal/resources/constants';
import { ShellContants } from 'app/core/shell/shell.contants';
import { ArrayUtils } from 'app/core/utils/array-util';
import { SessionService } from 'services/session.service';
import { NGXLogger } from 'ngx-logger';
import { PortalStandardClaims } from 'app/core/security/portalClaims';
import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import { SecurityService } from 'app/core/security/security.service';
import { PortalClaim, DatabaseConnection, EntitySchema, DatabasesClient, EntitySchemasClient, RouteType, Page, PagesClient, ControlType, PageEvent, SectionContructionType } from 'services/portal.service';
import { NextToWorkflowAction, NextToRouteAction, UpdateShellOptions, InitCreatePageBuilderAction, NextToPageBuilderAction, GeneratePageActionCommandsAction, GeneratePageBuilderInfoAction, GeneratePageEventsAction, InitEditPageBuilderAction, UpdatePageInfoAction, UpdatePageClaims, EditPageAction, CreatePageAction, NextToDatasourceAction, GatherAllChanges } from 'stores/pages/pagebuilder.actions';
import { PageService } from 'services/page.service';
import { PortalValidators } from 'app/core/validators/portal.validators';
import { ObjectUtils } from 'app/core/utils/object-util';
@Component({
    selector: 'let-page-builder',
    templateUrl: './page-builder.page.html',
    providers: [{
        provide: STEPPER_GLOBAL_OPTIONS, useValue: { showError: true }
    }]
})
export class PageBuilderPageComponent implements OnInit {
    @ViewChild('table') table: MatTable<ExtendedShellOption>;
    loading$: BehaviorSubject<boolean> = new BehaviorSubject(false);

    //#region New changes
    heading = 'Page Builder'
    pageInfoFormGroup: FormGroup
    shellOptions$: BehaviorSubject<Array<ExtendedShellOption>> = new BehaviorSubject([])
    shellOptions: Array<ExtendedShellOption> = []
    claims: Array<PortalClaim> = []
    //#endregion

    pageState$: Observable<PageBuilderStateModel>
    page: Page

    tempBuilderForm: FormGroup

    databaseConnections: Observable<Array<DatabaseConnection>>;
    entities: BehaviorSubject<Array<EntitySchema>> = new BehaviorSubject([]);
    shallowedEntitySchemas: Array<EntitySchema>;

    formSections: Array<ExtendedPageSection>

    isEditMode = false
    editId = ''

    //#region Constants
    _formTypes = [
        { name: 'Entity', value: 0 },
        { name: 'Workflow', value: 1 }
    ]

    //#endregion

    constructor(
        private pageService: PageService,
        private fb: FormBuilder,
        private databaseClient: DatabasesClient,
        private entityClient: EntitySchemasClient,
        private cd: ChangeDetectorRef,
        public dialog: MatDialog,
        private shortcutUtil: ShortcutUtil,
        private activatedRoute: ActivatedRoute,
        private store: Store,
        private session: SessionService,
        private security: SecurityService,
        private pagesClient: PagesClient,
        private router: Router,
        private logger: NGXLogger) {
    }

    ngOnInit(): void {
        this.pageService.init('page-builder').subscribe()
        this.page = this.activatedRoute.snapshot.data.page
        if (this.page) {
            this.isEditMode = true
        }
        else {
            this.store.dispatch(new InitCreatePageBuilderAction())
        }
        this.tempBuilderForm = this.fb.group([])
        this.initialDynamicForm()
        this.initialInputDynamicForm()
        this.onValueChanges()
        this.databaseConnections = this.databaseClient.getAll()
    }

    nextToBuilder() {
        this.store.dispatch(new NextToPageBuilderAction())
    }

    nextToDatasource() {
        this.store.dispatch(new NextToDatasourceAction())
    }

    nextToWorkflow() {
        this.store.dispatch(new NextToWorkflowAction())
    }

    nextToRoute() {
        this.store.dispatch(new NextToRouteAction())
    }

    //#region Angular Form methods    
    initialDynamicForm() {
        this.store
            .select(state => state.pagebuilder)
            .pipe(
                filter(result =>
                    result.filterState
                    && result.filterState !== GeneratePageActionCommandsAction
                    && result.filterState !== GeneratePageBuilderInfoAction
                    && result.filterState !== GeneratePageEventsAction
                    && result.filter !== InitEditPageBuilderAction),
                tap(result => {
                    this.logger.debug('Current state', result)
                    this.page = result.processPage
                })
            ).subscribe()
    }

    initialInputDynamicForm() {
        if (!this.isEditMode) {
            this.pageInfoFormGroup = this.fb.group({
                name: ['', [Validators.required, Validators.maxLength(250)], [PortalValidators.pageUniqueName(this.pagesClient)]],
                displayName: ['', [Validators.required, Validators.maxLength(250)]],
                urlPath: ['', Validators.required]
            })

            // Init some must have dynamic form part
            this.claims.push(PortalStandardClaims.AllowAccess)
        }
        else {
            this.pageInfoFormGroup = this.fb.group({
                name: new FormControl({ value: this.page.name, disabled: true}),
                displayName: [this.page.displayName, [Validators.required, Validators.maxLength(250)]],
                urlPath: [this.page.urlPath, Validators.required]
            })

            _.forEach(this.page.shellOptions, shellOpt => {
                this.shellOptions.push({
                    id: Guid.create().toString(),
                    key: shellOpt.key,
                    value: shellOpt.value,
                    allowDelete: true,
                    description: ''
                })
            })

            this.shellOptions$.next(this.shellOptions)

            this.claims = this.page.claims ? ObjectUtils.clone(this.page.claims) : []

            this.store.dispatch([
                new InitEditPageBuilderAction(this.page)
            ])
        }
    }

    onValueChanges() {
        // Auto-generated name and url path
        this.pageInfoFormGroup.get('displayName').valueChanges.subscribe(newValue => {
            if(!this.isEditMode){
            // Apply this change to list name and url path
            const formNameValue = (<string>newValue).toLowerCase().replace(/\s/g, '-')
            this.pageInfoFormGroup.get('name').setValue(formNameValue)
            this.pageInfoFormGroup.get('urlPath').setValue(Constants.PREFIX_FORM_URL + formNameValue)

            this.pageInfoFormGroup.get('name').markAsTouched()
            }
        })
    }

    onChangingOptions($event) {
        this.shellOptions = $event
    }

    isInHiddenListField(fieldName: string): boolean {
        return _.indexOf(['_id'], fieldName) >= 0
    }
    //#endregion

    //#region Events

    onSubmitDynamicFormBuilder() {
        if (this.pageInfoFormGroup.valid) {
            this.notifyDynamicFormInfoChange()
        }
    }

    onStepChange(event) {
        this.cd.markForCheck()
    }

    notifyDynamicFormInfoChange() {
        let formValues = this.pageInfoFormGroup.value
        this.store.dispatch([
            new UpdatePageInfoAction(
                this.isEditMode ? this.page.id : Guid.create().toString(),
                this.isEditMode ? this.page.name : formValues.name,
                this.session.getCurrentApp().id,
                formValues.displayName,
                formValues.urlPath,
                formValues.canSave),
            new UpdateShellOptions(
                this.shellOptions
            ),
            new UpdatePageClaims(
                this.claims
            )
        ])
    }

    saveChanges() {
        if (this.pageInfoFormGroup.valid) {
            const _title = "Save changes"
            const _description = "Are you sure to save all changes?"
            const _waitDesciption = "Waiting..."
            const dialogRef = this.shortcutUtil.actionEntityElement(_title, _description, _waitDesciption, MessageType.Delete);
            dialogRef.afterClosed().subscribe(res => {
                if (!res) {
                    return;
                }

                this.notifyDynamicFormInfoChange();
                this.store.dispatch(new GatherAllChanges())
                setTimeout(() => {
                    if (this.isEditMode) {
                        this.store.dispatch(new EditPageAction()).subscribe(
                            result => {
                                this.shortcutUtil.notifyMessage("Update Dynamic Form Successfully", ToastType.Success)
                            }
                        );
                    }
                    else {
                        this.store.dispatch(new CreatePageAction()).subscribe(
                            result => {
                                this.shortcutUtil.notifyMessage("Create Dynamic Form Successfully", ToastType.Success)
                            }
                        );
                    }
                }, 1000)
            });

        }
    }

    //#endregion
}
