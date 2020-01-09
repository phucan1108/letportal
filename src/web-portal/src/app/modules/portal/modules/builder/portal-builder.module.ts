import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule, MatFormFieldModule, MatDatepickerModule, MatAutocompleteModule, MatListModule, MatSliderModule, MatCardModule, MatSelectModule, MatButtonModule, MatIconModule, MatNativeDateModule, MatSlideToggleModule, MatCheckboxModule, MatMenuModule, MatTabsModule, MatTooltipModule, MatSidenavModule, MatProgressBarModule, MatProgressSpinnerModule, MatSnackBarModule, MatTableModule, MatGridListModule, MatToolbarModule, MatExpansionModule, MatDividerModule, MatSortModule, MatStepperModule, MatChipsModule, MatPaginatorModule, MatDialogModule, MatRadioModule, MatTreeModule, MatIconRegistry, MAT_DIALOG_DATA } from '@angular/material';
import { NgJsonEditorModule } from 'ang-jsoneditor';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MomentumTableModule } from 'momentum-table';
import { ScrollDispatchModule } from '@angular/cdk/scrolling';
import { MatMomentDateModule, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from '@angular/material-moment-adapter';
import { QuillModule } from 'ngx-quill';
import { CommandModalComponent } from './components/dynamic-list/components/commands/command-dialog.component';
import { DynamicListDataSourceComponent } from './components/dynamic-list/components/dynamic-list.datasource.component';
import { DatasourceDialogComponent } from '../../shared/datasource/datasource.component';
import { BuilderDnDComponent } from './components/page-builder/components/page-builders/builder-dnd.component';
import { TextboxComponent } from '../controls/textbox.component';
import { SectionDialogComponent } from './components/page-builder/components/page-builders/section-dialog.component';
import { ControlsGridComponent } from './components/standard/controls/controls-grid.component';
import { ControlDialogComponent } from './components/standard/controls/control-dialog.component';
import { PageButtonGridComponent } from './components/page-builder/components/page-buttons/page-button-grid.component';
import { PageButtonDialogComponent } from './components/page-builder/components/page-buttons/page-button-dialog.component';
import { PageEventDialogComponent } from './components/page-builder/components/page-events/page-event-dialog.component';
import { PageEventGridComponent } from './components/page-builder/components/page-events/page-event-grid.component';
import { SuggestionHintComponent } from '../../shared/suggestion-hint/suggestion-hint.component';
import { PageShellOptionDialogComponent } from './components/page-builder/components/page-shells/page-shell-option-dialog.component';
import { MenuDialogComponent } from './components/menu/components/menu-dialog.component';
import { ClaimDialogComponent } from '../../shared/claims/claim-dialog.component';
import { PageInfoComponent } from '../../shared/pages/page-info.component';
import { ShellOptionComponent } from '../../shared/shelloptions/shelloption.component';
import { CommandGridComponent } from './components/dynamic-list/components/commands/command-grid.component';
import { DatabaseOptionComponent } from '../../shared/databasepopulation/database-opt.component';
import { FilterOptionComponent } from './components/dynamic-list/components/filters/filter-option.component';
import { ColumnGridComponent } from './components/dynamic-list/components/columns/column-grid.component';
import { ParamsDialogComponent } from '../../shared/databasepopulation/params-dialog.component';
import { DatasourceOptionsDialogComponent } from '../../shared/datasourceopts/datasourceopts.component';
import { ControlEventsDialogComponent } from './components/standard/controls/control-events.dialog.component';
import { PageDatasourceDialogComponent } from './components/page-builder/components/page-datasources/page-datasource.dialog.component';
import { CommandOptionsComponent } from '../../shared/button-options/commandoptions.component';
import { ShellOptionDialogComponent } from '../../shared/shelloptions/shelloption.dialog.component';
import { PageButtonRouteDialogComponent } from './components/page-builder/components/page-buttons/page-button-route.component';
import { ButtonOptionsComponent } from '../../shared/button-options/buttonoptions.component';
import { PageButtonOptionsDialogComponent } from './components/page-builder/components/page-buttons/page-button-options.component';
import { AsyncValidatorDialogComponent } from './components/standard/controls/control-async-validator.dialog.component';
import { PageBuilderPage } from './pages/page-builder/page-builder.page';
import { DynamicListBuilderPage } from './pages/dynamic-list/dynamic-list-builder.page';
import { MenuPage } from './pages/menu/menu.page';
import { ClaimTableComponent } from '../../shared/claims/claim-table.component';
import { MenuProfilesPage } from './pages/menu-profiles/menu-profiles.page';
import { RoleClaimsPage } from './pages/role-claims/role-claims.page';
import { StandardPagePage } from './pages/standard/standard.page';
import { ListDatasourceComponent } from './components/dynamic-list/components/datasource/list-datasource.component';
import { StandardPopulationComponent } from './components/standard/standard-population/standard-population.component';
import { StandardControlsListComponent } from './components/page-builder/components/page-builders/standard-controls-list.component';
import { HttpOptionsComponent } from '../../shared/httpoptions/httpoptions.component';
import { PageDatasourceGridComponent } from './components/page-builder/components/page-datasources/page-datasource.grid.component';
import { SectionTemplate } from '../render/components/standard/components/section-template.directive';
import { PageRouteComponent } from './components/page-builder/components/page-buttons/page-route.component';
import { DatabaseOptionsComponent } from '../../shared/databaseoptions/databaseoptions.component';
import { RouterModule } from '@angular/router';
import { PageBuilderResolve } from './resolve/page.builder.resolve';
import { MenuResolve } from './resolve/menu.resolve';
import { MenuProfilesResolve } from './resolve/menu-profiles.resolve';
import { RoleClaimsResolve } from './resolve/role-claims.resolve';
import { PortalBuilderRoutingModule } from './portal-builder-routing.module';
import { PortalBuilderPageComponent } from './portal-builder.component';
import { DynamicListBuilderResolve } from './resolve/dynamic-list.builder.resolve';
import { StandardResolve } from './resolve/standard.resolve';
import { HighlightModule } from 'ngx-highlightjs';
import { ChartBuilderPage } from './pages/chart/chart-builder.page';
import { ChartBuilderResolve } from './resolve/chart.builder.resolve';
import { ChartDatasourceComponent } from './components/chart-builder/components/chart-datasource.component';
import { ChartFilterGridComponent } from './components/chart-builder/components/chart-filter.grid.component';
import { ChartFilterDialogComponent } from './components/chart-builder/components/chart-filter.dialog.component';
import { BackupBuilderPage } from './pages/backup/backup-builder.page';
@NgModule({
	declarations: [
		PortalBuilderPageComponent,
		PageBuilderPage,
		DynamicListBuilderPage,
		CommandModalComponent,
		DynamicListDataSourceComponent,
		DatasourceDialogComponent,
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
		SectionTemplate,
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
		BackupBuilderPage
	],
	imports: [
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
		ScrollDispatchModule,
		MatMomentDateModule,
		QuillModule.forRoot({
			modules: {
				syntax: true,
				toolbar: [
					['bold', 'italic', 'underline', 'strike'],
					[{ 'header': [1, 2, 3, 4, 5, 6, false] }],
					[{ 'list': 'ordered' }, { 'list': 'bullet' }],
					[{ 'script': 'sub' }, { 'script': 'super' }],
					[{ 'indent': '-1' }, { 'indent': '+1' }],
					['link', 'image']
				]
			}
		}),
		HighlightModule
	],
	entryComponents: [
		CommandModalComponent,
		DynamicListDataSourceComponent,
		DatasourceDialogComponent,
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
		ChartFilterDialogComponent
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
		{ provide: MAT_DIALOG_DATA, useValue: [] },
		{ provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: true } }
	],
})
export class PortalBuilderModule { }