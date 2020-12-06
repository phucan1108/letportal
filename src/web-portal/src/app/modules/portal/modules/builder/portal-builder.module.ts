import { DragDropModule } from '@angular/cdk/drag-drop';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatMomentDateModule, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from '@angular/material-moment-adapter';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule, MatIconRegistry } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSliderModule } from '@angular/material/slider';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSortModule } from '@angular/material/sort';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTreeModule } from '@angular/material/tree';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { NgJsonEditorModule } from 'ang-jsoneditor';
import { CoreModule } from 'app/core/core.module';
import { MomentumTableModule } from 'app/modules/thirdparties/momentum-table/datatable/datatable';
import { MatProgressButtonsModule } from 'mat-progress-buttons';
import { HighlightModule } from 'ngx-highlightjs';
import { QuillModule } from 'ngx-quill';
import { ExecutionDatabaseStepComponent } from 'portal/shared/button-options/executiondatabase-step.component';
import { IconPickerSharedComponent } from 'portal/shared/icon-picker/icon-picker.component';
import { ButtonOptionsComponent } from '../../shared/button-options/buttonoptions.component';
import { CommandOptionsComponent } from '../../shared/button-options/commandoptions.component';
import { ClaimDialogComponent } from '../../shared/claims/claim-dialog.component';
import { ClaimTableComponent } from '../../shared/claims/claim-table.component';
import { DatabaseOptionsComponent } from '../../shared/databaseoptions/databaseoptions.component';
import { DatabaseOptionComponent } from '../../shared/databasepopulation/database-opt.component';
import { ParamsDialogComponent } from '../../shared/databasepopulation/params-dialog.component';
import { DatasourceOptionsDialogComponent } from '../../shared/datasourceopts/datasourceopts.component';
import { HttpOptionsComponent } from '../../shared/httpoptions/httpoptions.component';
import { PageInfoComponent } from '../../shared/pages/page-info.component';
import { ShellOptionComponent } from '../../shared/shelloptions/shelloption.component';
import { ShellOptionDialogComponent } from '../../shared/shelloptions/shelloption.dialog.component';
import { SuggestionHintComponent } from '../../shared/suggestion-hint/suggestion-hint.component';
import { TextboxComponent } from '../render/controls/textbox.component';
import { AppInstallationDialog } from './components/app-installation/app-installation.dialog';
import { BackupSelectionComponent } from './components/backup-builder/backup-selection.component';
import { GenerateCodeDialog } from './components/backup-builder/generatecode.dialog';
import { ChartDatasourceComponent } from './components/chart-builder/components/chart-datasource.component';
import { ChartFilterDialogComponent } from './components/chart-builder/components/chart-filter.dialog.component';
import { ChartFilterGridComponent } from './components/chart-builder/components/chart-filter.grid.component';
import { ColumnGridComponent } from './components/dynamic-list/components/columns/column-grid.component';
import { CommandModalComponent } from './components/dynamic-list/components/commands/command-dialog.component';
import { CommandGridComponent } from './components/dynamic-list/components/commands/command-grid.component';
import { ListDatasourceComponent } from './components/dynamic-list/components/datasource/list-datasource.component';
import { DynamicListDataSourceComponent } from './components/dynamic-list/components/dynamic-list.datasource.component';
import { FilterOptionComponent } from './components/dynamic-list/components/filters/filter-option.component';
import { MenuDialogComponent } from './components/menu/components/menu-dialog.component';
import { BuilderDnDComponent } from './components/page-builder/components/page-builders/builder-dnd.component';
import { SectionDialogComponent } from './components/page-builder/components/page-builders/section-dialog.component';
import { StandardControlsListComponent } from './components/page-builder/components/page-builders/standard-controls-list.component';
import { PageButtonDialogComponent } from './components/page-builder/components/page-buttons/page-button-dialog.component';
import { PageButtonGridComponent } from './components/page-builder/components/page-buttons/page-button-grid.component';
import { PageButtonOptionsDialogComponent } from './components/page-builder/components/page-buttons/page-button-options.component';
import { PageButtonRouteDialogComponent } from './components/page-builder/components/page-buttons/page-button-route.component';
import { PageRouteComponent } from './components/page-builder/components/page-buttons/page-route.component';
import { PageDatasourceDialogComponent } from './components/page-builder/components/page-datasources/page-datasource.dialog.component';
import { PageDatasourceGridComponent } from './components/page-builder/components/page-datasources/page-datasource.grid.component';
import { PageEventDialogComponent } from './components/page-builder/components/page-events/page-event-dialog.component';
import { PageEventGridComponent } from './components/page-builder/components/page-events/page-event-grid.component';
import { PageShellOptionDialogComponent } from './components/page-builder/components/page-shells/page-shell-option-dialog.component';
import { AsyncValidatorDialogComponent } from './components/standard/controls/control-async-validator.dialog.component';
import { ControlDialogComponent } from './components/standard/controls/control-dialog.component';
import { ControlEventsDialogComponent } from './components/standard/controls/control-events.dialog.component';
import { ControlsGridComponent } from './components/standard/controls/controls-grid.component';
import { StandardPopulationComponent } from './components/standard/standard-population/standard-population.component';
import { AppInstallationPage } from './pages/app-installation/app-installation.page';
import { AppPackagePage } from './pages/app-package/app-package.page';
import { BackupBuilderPage } from './pages/backup/backup-builder.page';
import { BackupRestorePage } from './pages/backup/backup-restore.page';
import { BackupUploadpage } from './pages/backup/backup-upload.page';
import { ChartBuilderPage } from './pages/chart/chart-builder.page';
import { CompositeBuilderPage } from './pages/composite-builder/composite-builder.page';
import { DynamicListBuilderPage } from './pages/dynamic-list/dynamic-list-builder.page';
import { LocalizationPage } from './pages/localization/localization.page';
import { MenuProfilesPage } from './pages/menu-profiles/menu-profiles.page';
import { MenuPage } from './pages/menu/menu.page';
import { PageBuilderPage } from './pages/page-builder/page-builder.page';
import { RoleClaimsPage } from './pages/role-claims/role-claims.page';
import { StandardPagePage } from './pages/standard/standard.page';
import { PortalBuilderRoutingModule } from './portal-builder-routing.module';
import { PortalBuilderPageComponent } from './portal-builder.component';
import { AppPackageResolve } from './resolve/app-package.resolve';
import { BackupResolve } from './resolve/backup.resolve';
import { ChartBuilderResolve } from './resolve/chart.builder.resolve';
import { CompositeControlResolve } from './resolve/composite-control.resolve';
import { DynamicListBuilderResolve } from './resolve/dynamic-list.builder.resolve';
import { LocaleResolve } from './resolve/locale.resolve';
import { MenuProfilesResolve } from './resolve/menu-profiles.resolve';
import { MenuResolve } from './resolve/menu.resolve';
import { PageBuilderResolve } from './resolve/page.builder.resolve';
import { RoleClaimsResolve } from './resolve/role-claims.resolve';
import { StandardResolve } from './resolve/standard.resolve';
@NgModule({
	declarations: [
		PortalBuilderPageComponent,
		PageBuilderPage,
		DynamicListBuilderPage,
		CommandModalComponent,
		DynamicListDataSourceComponent,
		BuilderDnDComponent,
		TextboxComponent,
		SectionDialogComponent,
		ControlsGridComponent,
		ControlDialogComponent,
		PageButtonGridComponent,
		PageButtonDialogComponent,
		PageEventDialogComponent,
		PageEventGridComponent,
		SuggestionHintComponent,
		PageShellOptionDialogComponent,
		MenuPage,
		MenuDialogComponent,
		ClaimTableComponent,
		ClaimDialogComponent,
		MenuProfilesPage,
		PageInfoComponent,
		ShellOptionComponent,
		CommandGridComponent,
		DatabaseOptionComponent,
		FilterOptionComponent,
		ColumnGridComponent,
		ParamsDialogComponent,
		RoleClaimsPage,
		StandardPagePage,
		ListDatasourceComponent,
		StandardPopulationComponent,
		DatasourceOptionsDialogComponent,
		StandardControlsListComponent,
		ControlEventsDialogComponent,
		HttpOptionsComponent,
		PageDatasourceGridComponent,
		PageDatasourceDialogComponent,
		CommandOptionsComponent,
		ShellOptionDialogComponent,
		PageButtonRouteDialogComponent,
		ButtonOptionsComponent,
		PageButtonOptionsDialogComponent,
		PageRouteComponent,
		AsyncValidatorDialogComponent,
		DatabaseOptionsComponent,
		ChartBuilderPage,
		ChartDatasourceComponent,
		ChartFilterGridComponent,
		ChartFilterDialogComponent,
		BackupBuilderPage,
		BackupSelectionComponent,
		BackupUploadpage,
		BackupRestorePage,
		GenerateCodeDialog,
		ExecutionDatabaseStepComponent,
		IconPickerSharedComponent,
		LocalizationPage,
		AppPackagePage,
		AppInstallationPage,
		AppInstallationDialog,
		CompositeBuilderPage
	],
	imports: [
		CoreModule.forChild(),
		PortalBuilderRoutingModule,
		CommonModule,
		FormsModule,
		ReactiveFormsModule,
		MatInputModule,
		MatFormFieldModule,
		MatDatepickerModule,
		MatAutocompleteModule,
		MatListModule,
		MatSliderModule,
		MatCardModule,
		MatSelectModule,
		MatButtonModule,
		MatIconModule,
		MatNativeDateModule,
		MatSlideToggleModule,
		MatCheckboxModule,
		MatMenuModule,
		MatTabsModule,
		MatTooltipModule,
		MatSidenavModule,
		MatProgressBarModule,
		MatProgressSpinnerModule,
		MatSnackBarModule,
		MatTableModule,
		MatGridListModule,
		MatToolbarModule,
		MatExpansionModule,
		MatDividerModule,
		MatSortModule,
		MatStepperModule,
		MatChipsModule,
		MatPaginatorModule,
		MatDialogModule,
		MatRadioModule,
		MatTreeModule,
		NgJsonEditorModule,
		DragDropModule,
		MomentumTableModule,
		ScrollingModule,
		MatMomentDateModule,
		MatProgressButtonsModule,
		TranslateModule,
		QuillModule.forRoot({
			modules: {
				syntax: true,
				toolbar: [
					['bold', 'italic', 'underline', 'strike'],
					[{ header: [1, 2, 3, 4, 5, 6, false] }],
					[{ list: 'ordered' }, { list: 'bullet' }],
					[{ script: 'sub' }, { script: 'super' }],
					[{ indent: '-1' }, { indent: '+1' }],
					['link', 'image']
				]
			}
		}),
		HighlightModule
	],
	entryComponents: [
		CommandModalComponent,
		DynamicListDataSourceComponent,
		BuilderDnDComponent,
		TextboxComponent,
		SectionDialogComponent,
		ControlsGridComponent,
		ControlDialogComponent,
		PageButtonGridComponent,
		PageButtonDialogComponent,
		PageEventDialogComponent,
		PageEventGridComponent,
		SuggestionHintComponent,
		PageShellOptionDialogComponent,
		MenuDialogComponent,
		ClaimDialogComponent,
		PageInfoComponent,
		ShellOptionComponent,
		CommandGridComponent,
		DatabaseOptionComponent,
		FilterOptionComponent,
		ColumnGridComponent,
		ParamsDialogComponent,
		DatasourceOptionsDialogComponent,
		ControlEventsDialogComponent,
		PageDatasourceDialogComponent,
		CommandOptionsComponent,
		ShellOptionDialogComponent,
		PageButtonRouteDialogComponent,
		ButtonOptionsComponent,
		PageButtonOptionsDialogComponent,
		AsyncValidatorDialogComponent,
		ChartDatasourceComponent,
		ChartFilterGridComponent,
		ChartFilterDialogComponent,
		BackupSelectionComponent,
		AppInstallationDialog
	],
	exports: [RouterModule],
	providers: [
		MatIconRegistry,
		DynamicListBuilderResolve,
		PageBuilderResolve,
		MenuResolve,
		MenuProfilesResolve,
		RoleClaimsResolve,
		StandardResolve,
		ChartBuilderResolve,
		BackupResolve,
		LocaleResolve,
		AppPackageResolve,
		CompositeControlResolve,
		{ provide: MAT_DIALOG_DATA, useValue: [] },
		{ provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: true } }
	],
})
export class PortalBuilderModule { }