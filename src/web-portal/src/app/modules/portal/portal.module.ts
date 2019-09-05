import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import {
	MatIconRegistry,
	MatIcon,
	MatInputModule,
	MatDatepickerModule,
	MatFormFieldModule,
	MatAutocompleteModule,
	MatSliderModule,
	MatListModule,
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
	MatGridListModule,
	MatTableModule,
	MatExpansionModule,
	MatToolbarModule,
	MatSortModule,
	MatDividerModule,
	MatStepperModule,
	MatChipsModule,
	MatPaginatorModule,
	MatDialogModule,
	MatRadioModule,
	MatDialog,
	MatTreeModule,
	MAT_DIALOG_DATA
} from '@angular/material';
import { MatMomentDateModule, MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from "@angular/material-moment-adapter";
import { DragDropModule } from '@angular/cdk/drag-drop'

import { DynamicListBuilderComponent } from './dynamic-list/builder/dynamic-list-builder.component';
import { PortalPageComponent } from './portal-page.component';
import { NgJsonEditorModule } from 'ang-jsoneditor';
import { CommandModalComponent } from './dynamic-list/builder/components/commands/command-dialog.component';
import { DynamicListRouterResolver } from './dynamic-list/resolver/dynamic-list.router.resolve';
import { DynamicListDataSourceComponent } from './dynamic-list/builder/components/dynamic-list.datasource.component';
import { DatasourceDialogComponent } from './shared/datasource/datasource.component';
import { DynamicListBuilderResolver } from './dynamic-list/resolver/dynimic-list.builder.resolve';
import { SuggestionHintComponent } from './shared/suggestion-hint/suggestion-hint.component';
import { DynamicListRenderComponent } from './dynamic-list/render/dynamic-list.render.component';
import { DynamicListGridComponent } from './dynamic-list/render/components/dynamic-list.grid.component';
import { DynamicListFiltersComponent } from './dynamic-list/render/components/dynamic-list.filters.component';
import { DynamicListCommandComponent } from './dynamic-list/render/components/dynamic-list.command.component';
import { PageBuilderPageComponent } from './pages/builder/page-builder.page';
import { BuilderDnDComponent } from './pages/builder/components/page-builders/builder-dnd.component';
import { TextboxComponent } from './pages/controls/textbox.component';
import { DividedColumnsSectionComponent } from './pages/render/components/divided-columns-section.component';
import { SectionDialogComponent } from './pages/builder/components/page-builders/section-dialog.component';
import { ControlsGridComponent } from './standard/controls/controls-grid.component';
import { ControlDialogComponent } from './standard/controls/control-dialog.component';
import { JsonEditorCustomComponent } from './pages/controls/json-editor.component';
import { PageButtonGridComponent } from './pages/builder/components/page-buttons/page-button-grid.component';
import { PageButtonDialogComponent } from './pages/builder/components/page-buttons/page-button-dialog.component';
import { PageEventDialogComponent } from './pages/builder/components/page-events/page-event-dialog.component';
import { PageEventGridComponent } from './pages/builder/components/page-events/page-event-grid.component';
import { ScrollDispatchModule } from '@angular/cdk/scrolling';
import { PageRenderComponent } from './pages/render/page-render.page';
import { PageRenderResolver } from './pages/resolver/page.render.resolve';
import { PageBuilderResolver } from './pages/resolver/page.builder.resolve';
import { ActionCommandsSectionComponent } from './pages/render/components/action-commands-section.component';
import { GeneralControlComponent } from './pages/controls/general-control.component';
import { MenuComponent } from './menu/menu.component';
import { MenuRouterResolver } from './menu/resolve/menu.router.resolve';
import { MenuDialogComponent } from './menu/components/menu-dialog.component';
import { ClaimTableComponent } from './shared/claims/claim-table.component';
import { ClaimDialogComponent } from './shared/claims/claim-dialog.component';
import { MenuProfilesComponent } from './menu-profiles/menu-profiles.component';
import { MenuProfilesRouterResolver } from './menu-profiles/resolve/menu-profiles.router.resolve';
import { PageInfoComponent } from './shared/pages/page-info.component';
import { ShellOptionComponent } from './shared/shelloptions/shelloption.component';
import { CommandGridComponent } from './dynamic-list/builder/components/commands/command-grid.component';
import { DatabaseOptionComponent } from './shared/databasepopulation/database-opt.component';
import { FilterOptionComponent } from './dynamic-list/builder/components/filters/filter-option.component';
import { ColumnGridComponent } from './dynamic-list/builder/components/columns/column-grid.component';
import { ParamsDialogComponent } from './shared/databasepopulation/params-dialog.component';
import { DynamicListDataDialogComponent } from './dynamic-list/render/components/dynamic-list-data-dialog.component';
import { AdvancedFilterDialogComponent } from './dynamic-list/render/components/advancedfilter-dialog.component';
import { RoleClaimsComponent } from './role-claims/role-claims.component';
import { RoleClaimsRouterResolver } from './role-claims/resolve/role-claims.router.resolve';
import { PortalRoutingModule } from './portal-routing.module';
import { PageShellOptionDialogComponent } from './pages/builder/components/page-shells/page-shell-option-dialog.component';
import { StandardPageComponent } from './standard/standard.component';
import { StandardResolver } from './standard/resolver/standard.resolve';
import { ListDatasourceComponent } from './dynamic-list/builder/components/datasource/list-datasource.component';
import { StandardPopulationComponent } from './standard/standard-population/standard-population.component';
import { MomentumTableModule } from 'momentum-table';
import { DatasourceOptionsDialogComponent } from './shared/datasourceopts/datasourceopts.component';
import { StandardControlsListComponent } from './pages/builder/components/page-builders/standard-controls-list.component';
import { HttpOptionsComponent } from './shared/httpoptions/httpoptions.component';
import { ControlEventsDialogComponent } from './standard/controls/control-events.dialog.component';
import { PageDatasourceGridComponent } from './pages/builder/components/page-datasources/page-datasource.grid.component';
import { PageDatasourceDialogComponent } from './pages/builder/components/page-datasources/page-datasource.dialog.component';
import { SectionTemplate } from './pages/render/components/section-template.directive';
import { PageRenderSectionWrapperComponent } from './pages/render/components/page-render-section-wrapper.component';
import { PageRenderBuilderComponent } from './pages/render/components/page-render-builder.component';
import { PageService } from 'services/page.service';
import { CommandOptionsComponent } from './shared/button-options/commandoptions.component';
import { IconPickerComponent } from './pages/controls/icon-picker.component';
import { ShellOptionDialogComponent } from './shared/shelloptions/shelloption.dialog.component';
import { PageButtonRouteDialogComponent } from './pages/builder/components/page-buttons/page-button-route.component';
import { ButtonOptionsComponent } from './shared/button-options/buttonoptions.component';
import { PageButtonOptionsDialogComponent } from './pages/builder/components/page-buttons/page-button-options.component';
import { PageRouteComponent } from './pages/builder/components/page-buttons/page-route.component';
import { QuillModule } from 'ngx-quill'
import { FileUploaderComponent } from './pages/controls/file-uploader.component';
import { UploadFileService } from 'services/uploadfile.service';
import { AsyncValidatorDialogComponent } from './standard/controls/control-async-validator.dialog.component';
import { DatabaseOptionsComponent } from './shared/databaseoptions/databaseoptions.component';

@NgModule({
	imports: [
		CommonModule,
		PortalRoutingModule,
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
					[{ 'indent': '-1'}, { 'indent': '+1' }],
					['link', 'image']    
				]
			}
		})
	],
	entryComponents: [
		CommandModalComponent,
		DynamicListGridComponent,
		DynamicListFiltersComponent,
		DynamicListCommandComponent,
		DynamicListDataSourceComponent,
		DatasourceDialogComponent,
		BuilderDnDComponent,
		TextboxComponent,
		DividedColumnsSectionComponent,
		SectionDialogComponent,
		ControlsGridComponent,
		ControlDialogComponent,
		JsonEditorCustomComponent,
		PageButtonGridComponent,
		PageButtonDialogComponent,
		PageEventDialogComponent,
		PageEventGridComponent,
		SuggestionHintComponent,
		PageShellOptionDialogComponent,
		ActionCommandsSectionComponent,
		GeneralControlComponent,
		MenuDialogComponent,
		ClaimDialogComponent,
		PageInfoComponent,
		ShellOptionComponent,
		CommandGridComponent,
		DatabaseOptionComponent,
		FilterOptionComponent,
		ColumnGridComponent,
		ParamsDialogComponent,
		DynamicListDataDialogComponent,
		AdvancedFilterDialogComponent,
		DatasourceOptionsDialogComponent,
		ControlEventsDialogComponent,
		PageDatasourceDialogComponent,
		CommandOptionsComponent,
		IconPickerComponent,
		ShellOptionDialogComponent,
		PageButtonRouteDialogComponent,
		ButtonOptionsComponent,
		PageButtonOptionsDialogComponent,
		AsyncValidatorDialogComponent
	],
	exports: [RouterModule],
	providers: [
		MatIconRegistry,
		DynamicListRouterResolver,
		DynamicListBuilderResolver,
		PageRenderResolver,
		PageBuilderResolver,
		MenuRouterResolver,
		MenuProfilesRouterResolver,
		RoleClaimsRouterResolver,
		StandardResolver,
		{ provide: MAT_DIALOG_DATA, useValue: [] },
		{ provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: true } },
		PageService,
		UploadFileService
	],
	declarations: [
		PortalPageComponent,
		PageBuilderPageComponent,
		DynamicListBuilderComponent,
		CommandModalComponent,
		DynamicListRenderComponent,
		DynamicListGridComponent,
		DynamicListFiltersComponent,
		DynamicListCommandComponent,
		DynamicListDataSourceComponent,
		DatasourceDialogComponent,
		BuilderDnDComponent,
		TextboxComponent,
		DividedColumnsSectionComponent,
		SectionDialogComponent,
		ControlsGridComponent,
		ControlDialogComponent,
		JsonEditorCustomComponent,
		PageButtonGridComponent,
		PageButtonDialogComponent,
		PageEventDialogComponent,
		PageEventGridComponent,
		SuggestionHintComponent,
		PageRenderComponent,
		PageShellOptionDialogComponent,
		ActionCommandsSectionComponent,
		GeneralControlComponent,
		MenuComponent,
		MenuDialogComponent,
		ClaimTableComponent,
		ClaimDialogComponent,
		MenuProfilesComponent,
		PageInfoComponent,
		ShellOptionComponent,
		CommandGridComponent,
		DatabaseOptionComponent,
		FilterOptionComponent,
		ColumnGridComponent,
		ParamsDialogComponent,
		DynamicListDataDialogComponent,
		AdvancedFilterDialogComponent,
		RoleClaimsComponent,
		StandardPageComponent,
		ListDatasourceComponent,
		StandardPopulationComponent,
		DatasourceOptionsDialogComponent,
		StandardControlsListComponent,
		ControlEventsDialogComponent,
		HttpOptionsComponent,
		PageDatasourceGridComponent,
		PageDatasourceDialogComponent,
		SectionTemplate,
		PageRenderSectionWrapperComponent,
		PageRenderBuilderComponent,
		CommandOptionsComponent,
		IconPickerComponent,
		ShellOptionDialogComponent,
		PageButtonRouteDialogComponent,
		ButtonOptionsComponent,
		PageButtonOptionsDialogComponent,
		PageRouteComponent,
		FileUploaderComponent,
		AsyncValidatorDialogComponent,
		DatabaseOptionsComponent
	]
})
export class PortalModule { }
